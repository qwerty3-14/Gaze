using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectGaze.Entities.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Entities.Ships
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
                Projectile m = new Missile(position + Functions.PolarVector(15, rotation), Functions.PolarVector(3.2f, rotation) + velocity, team);
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
        public override void LocalUpdate()
        {
            shotCooldown--;
            specialCooldown--;
            if (thrusting)
            {
                counter++;
                if (counter % 6 == 0)
                {
                    new Particle(position + Functions.PolarVector(-10, rotation + (float)Math.PI / 2) + Functions.PolarVector(-14, rotation), 14, Color.Orange);
                    new Particle(position + Functions.PolarVector(10, rotation + (float)Math.PI / 2) + Functions.PolarVector(-14, rotation), 14, Color.Orange);
                    new Particle(position + Functions.PolarVector(0, rotation + (float)Math.PI / 2) + Functions.PolarVector(-17, rotation), 14, Color.Orange);
                }
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
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[1], pos, null, null, new Vector2(17.5f, 13.5f), rotation, Vector2.One, Color.White, 0, 0);
        }
        public override void AI()
        {

            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            if (enemyShip != null)
            {
                AI_Kite(3.2f, 1000);
            }
            int incomingDamage = 0;
            List<Projectile> enemyProjectiles = EnemyProjectiles();
            if (enemyShip is Illusion || enemyShip is Illusioner)
            {

            }
            else
            {
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
                        else
                        {
                            incomingDamage += enemyProjectiles[i].damage;
                        }
                    }
                }
                if (incomingDamage >= 2 && energy >= 12)
                {
                    Controls.controlSpecial[team] = true;
                }
            }
            if (energy >= 18)
            {
                Controls.controlShoot[team] = true;
            }
        }
    }
}
