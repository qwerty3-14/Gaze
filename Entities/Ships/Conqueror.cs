using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GazeOGL.Entities.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities.Ships
{
    public class Conqueror : Ship
    {
        public Conqueror(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Conqueror;
            ShipStats.GetStatsFor(ShipID.Conqueror, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;
            energyRate = 1;

            mass = 45;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(15, 3),
                new Vector2(-5, 13),
                new Vector2(-14, 11),
                new Vector2(-17, 0),
                new Vector2(-14, -11),
                new Vector2(-5, -13)
                ,new Vector2(15, -3),
            });
        }
        int shotCooldown = 0;
        public override void Shoot()
        {
            if (shotCooldown <= 0 && energy >= 6)
            {
                AssetManager.PlaySound(SoundID.MissileLaunch);
                energy -= 6;
                shotCooldown = 20;
                AimAssist(3.5f, 15);
                Projectile m = new Missile(position + Functions.PolarVector(15, rotation), Functions.PolarVector(3.5f, rotation) + velocity, team);
                m.rotation = rotation;
            }

        }
        int specialCooldown;
        public override void Special()
        {

            Entity enemy = GetEnemy();
            if (enemy != null)
            {
                if (energy >= 12 && specialCooldown <= 0)
                {
                    energy -= 12;
                    Vector2 enemyPos = enemy.position;
                    Vector2 myPos = position;
                    float enemyRotation = enemy.rotation;
                    float myRotation = rotation;

                    position = enemyPos;
                    enemy.position = myPos;
                    rotation = enemyRotation;
                    enemy.rotation = myRotation;
                    velocity = enemy.velocity = Vector2.Zero;
                    AssetManager.PlaySound(SoundID.Warp);
                    specialCooldown = 30;
                    for (int i = 0; i < 8; i++)
                    {
                        float dir = (float)Main.random.NextDouble() * 2f * (float)Math.PI;
                        new Particle(position + Functions.PolarVector(20, dir), 5, Main.WarpPink, Functions.PolarVector(-4, dir));
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        float dir = (float)Main.random.NextDouble() * 2f * (float)Math.PI;
                        new Particle(enemy.position + Functions.PolarVector(20, dir), 5, Main.WarpPink, Functions.PolarVector(-4, dir));
                    }
                    if (enemy is Ship)
                    {
                        //((Ship)enemy).energy = 0;
                    }
                    List<Projectile> enemyProjectiles = EnemyProjectiles();
                    for (int i = 0; i < enemyProjectiles.Count; i++)
                    {
                        if(enemyProjectiles[i].invulnerable)
                        {
                            enemyProjectiles[i].Kill();
                        }
                        else
                        {
                            enemyProjectiles[i].team = team;
                        }
                    }

                }
            }
        }
        int counter;
        int hitDrawTime;
        public override void LocalUpdate()
        {
            shotCooldown--;
            specialCooldown--;
            counter++;
            if (thrusting)
            {
                if (counter % 6 == 0)
                {
                    for(int i =0; i < engineCount; i++)
                    {
                        new Particle(position + Functions.PolarVector(engineRadius * (float)Math.Sin(((float)counter * engineRotateSpeed) + (i / (float)engineCount) * (float)Math.PI * 2f), rotation + (float)Math.PI / 2) + Functions.PolarVector(-17, rotation), 14, Color.Orange);
                    }
                }
            }
            if(hitDrawTime >0)
            {
                hitDrawTime--;
            }

        }
        public override void HitByProjectile(int damage)
        {
            if (EMPTime <= 0 && StunTime <= 0)
            {
                energy += damage * 2;
                if (energy > energyCapacity)
                {
                    energy = energyCapacity;
                }
                hitDrawTime += damage * 30;
            }
        }
        float engineRotateSpeed = 0.03f;
        int engineCount = 3;
        float engineRadius = 9f;
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.extraEntities[11], pos, null, Color.White, rotation, new Vector2(17.5f, 13.5f), Vector2.One, SpriteEffects.None, 0f);
            
            for(int i =0; i < engineCount; i++)
            {
                if((float)Math.Cos(((float)counter * engineRotateSpeed) + (i / (float)engineCount) * (float)Math.PI * 2f) > 0)
                {
                    spriteBatch.Draw(AssetManager.extraEntities[12], pos, null, Color.White, rotation, new Vector2(17.5f, 2.5f + engineRadius * (float)Math.Sin(((float)counter * engineRotateSpeed) + (i / (float)engineCount) * (float)Math.PI * 2f)), Vector2.One, SpriteEffects.None, 0f);
                }
            }
            for(int i =0; i < engineCount; i++)
            {
                if((float)Math.Cos(((float)counter * engineRotateSpeed) + (i / (float)engineCount) * (float)Math.PI * 2f) <= 0)
                {
                    spriteBatch.Draw(AssetManager.extraEntities[12], pos, null, Color.White, rotation, new Vector2(17.5f, 2.5f + engineRadius * (float)Math.Sin(((float)counter * engineRotateSpeed) + (i / (float)engineCount) * (float)Math.PI * 2f)), Vector2.One, SpriteEffects.None, 0f);
                }
            }
            if(hitDrawTime > 0)
            {
                spriteBatch.Draw(AssetManager.extraEntities[10], pos, null, Color.White, rotation, new Vector2(17.5f, 13.5f), Vector2.One, SpriteEffects.None, 0f);
            }
        }
        public override void AI()
        {
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            if (enemyShip != null)
            {
                AI_Kite(4.5f, 1000, 15);
            }
            int incomingDamage = 0;
            List<Projectile> enemyProjectiles = EnemyProjectiles();
            if (! (enemyShip is Illusion || enemyShip is Illusioner))
            {
                //loops through enemy projectiles to calculate incomingDamage
                for (int i = 0; i < enemyProjectiles.Count; i++)
                {
                    if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(Math.Min(20, enemyProjectiles[i].lifeTime), enemyProjectiles[i].lifeTime)))
                    {
                        if(enemyProjectiles[i] is Kugelblitz)
                        {
                            incomingDamage += 12;
                        }
                        else if (enemyProjectiles[i] is Mine)
                        {
                            incomingDamage += 4;
                        }
                        else if (enemyProjectiles[i] is Tripwire)
                        {
                            incomingDamage += 12;
                        }
                        else if (enemyProjectiles[i] is EMP)
                        {
                            incomingDamage += 12;
                        }
                        else
                        {
                            incomingDamage += enemyProjectiles[i].damage;
                        }
                    }
                }

                if (incomingDamage >= 2 && energy >= 12)
                {
                    AI_cSpecial();
                }
            }
            if (energy >= 18)
            {
                AI_cShoot();
            }
        }
    }
}
