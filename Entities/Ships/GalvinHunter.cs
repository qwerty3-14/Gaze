using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GazeOGL.Entities.Projectiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities.Ships
{
    class Hunter : Ship
    {
        public Hunter(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Hunter;
            ShipStats.GetStatsFor(ShipID.Hunter, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;
            energyRate = 2;
            shape = new Polygon(new Vector2[] 
            {
                new Vector2(14, 6),
                new Vector2(14, -6),
                new Vector2(-9, -6),
                new Vector2(-9, 6)
            });
        }
        int shotCooldown = 0;
        public override void Shoot()
        {
            if (energy > 0 && shotCooldown <= 0)
            {
                shotCooldown = 6;
                AssetManager.PlaySound(SoundID.Pew);
                energy--;
                Projectile pelt = new HunterPelt(position + Functions.PolarVector(Main.random.Next(-3, 4), rotation + (float)Math.PI / 2), velocity + Functions.PolarVector(3, rotation), team);
                pelt.rotation = rotation;
            }
        }
        int trapCooldown;
        public override void Special()
        {
            Entity enemy = GetEnemy();
            if (enemy != null && trapCooldown <= 0 && energy >= 10 && enemy.StunTime == 0)
            {
                AssetManager.PlaySound(SoundID.Warp);

                energy -= 10;
                trapCooldown = 40;
                Vector2 Pos = enemy.position + Functions.PolarVector(50, enemy.rotation);
                if (enemy.velocity.Length() * 60 > 50)
                {
                    Pos = enemy.position + enemy.velocity * 60;
                }
                
                Projectile wire = new Tripwire(Pos, Vector2.Zero, team);
                wire.rotation = (enemy.position - wire.position).ToRotation() + (float)Math.PI/2f;
                for (int i = 0; i < 8; i++)
                {
                    float dir = (float)Main.random.NextDouble() * 2f * (float)Math.PI;
                    new Particle(Pos + Functions.PolarVector(20, dir), 5, Main.WarpPink, Functions.PolarVector(-4, dir));
                }
                for (int i = 0; i < 5; i++)
                {
                    new Particle(position + Functions.PolarVector(-2f, rotation), 5, Main.WarpPink, Functions.PolarVector(4, (rotation + (float)Math.PI / 2 - (float)Math.PI / 8 + (float)Main.random.NextDouble() * (float)Math.PI / 4) ) + velocity);
                }
            }
        }
        int counter;
        public override void LocalUpdate()
        {
            if(shotCooldown > 0)
            {
                shotCooldown--;
            }
            if (trapCooldown > 0)
            {
                trapCooldown--;
            }
            if (thrusting)
            {
                counter++;
                if (counter % 5 == 0)
                {
                    new Particle(position + Functions.PolarVector(-3, rotation + (float)Math.PI / 2) + Functions.PolarVector(-7, rotation), 14, Color.Orange);
                    new Particle(position + Functions.PolarVector(3, rotation + (float)Math.PI / 2) + Functions.PolarVector(-7, rotation), 14, Color.Orange);
                }
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[0], pos, null, Color.White, rotation, new Vector2(7.5f, 5.5f), Vector2.One, SpriteEffects.None, 0f);
        }
        bool AI_ShootingProj = false;
        bool rush = true;
        public override void AI()
        {
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            AI_ShootingProj = false;
            List<Projectile> enemyProjectiles = EnemyProjectiles();
            for (int i = 0; i < enemyProjectiles.Count; i++)
            {
                float towardP = (Functions.screenLoopAdjust(position, enemyProjectiles[i].position) - position).ToRotation();
                if (enemyProjectiles[i] is Mine)
                {
                    if ((Functions.screenLoopAdjust(position, enemyProjectiles[i].position) - position).Length() < 2.2f * 50)
                    {
                        AI_ShootingProj = true;
                        float aimAt = Functions.PredictiveAim(position, 3, enemyProjectiles[i].position, enemyProjectiles[i].velocity - velocity);
                        if (!float.IsNaN(aimAt))
                        {
                            if (AI_TurnToward(aimAt))
                            {
                                AI_cShoot();
                            }
                        }
                    }
                }
                else if (enemyProjectiles[i].health == 1 && Functions.AngularDifference(towardP, rotation) < (float)Math.PI/4f)
                {
                    if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(60, enemyProjectiles[i].lifeTime)))
                    {
                        AI_ShootingProj = true;
                        float aimAt = Functions.PredictiveAim(position, 3, enemyProjectiles[i].position, enemyProjectiles[i].velocity - velocity);
                        if (!float.IsNaN(aimAt))
                        {
                            if (AI_TurnToward(aimAt))
                            {
                                AI_cShoot();
                            }
                        }
                    }
                }
                else  if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(60, enemyProjectiles[i].lifeTime)))
                {
                    AI_ShootingProj = true;
                    AI_Dodge(enemyProjectiles[i]);
                    AI_cThrust();
                }
            }
            if (enemyShip != null && !AI_ShootingProj)
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                float toward = (enemyPos - position).ToRotation();
                if ((enemyPos - position).Length() < 3 * 50)
                {
                    if ((enemyPos - position).Length() > 2.2f * 50)
                    {
                        AI_cThrust();
                    }
                    float aimAt = Functions.PredictiveAim(position, 3, enemyPos, enemyShip.velocity - velocity);
                    if (!float.IsNaN(aimAt))
                    {
                        if (AI_TurnToward(aimAt))
                        {
                            AI_cShoot();
                        }
                    }
                    else
                    {
                        AI_TurnToward(toward);
                    }
                }
                else
                {
                    if (rush)
                    {
                        AI_cThrust();
                        AI_TurnToward(toward);
                        if (enemyShip.StunTime == 0 && (enemyPos - position).Length() > 120 && ((enemyPos + enemyShip.velocity) - position).Length() > (enemyPos - position).Length())
                        {
                            AI_cSpecial();
                        }
                        if(energy < 2)
                        {
                            rush = false;
                        }
                    }
                    else
                    {
                        AI_Kite(4, 3 * 50);
                        if (energy == energyCapacity)
                        {
                            AI_cSpecial();
                            rush = true;
                        }
                    }

                }
                Vector2 trapPosition = enemyPos + Functions.PolarVector(40, enemyShip.rotation);
                if (enemyShip.velocity.Length() > 0.2f)
                {
                    trapPosition = enemyPos + enemyShip.velocity * 60;
                }
                if ((trapPosition - position).Length() < 3 * 30 && enemyShip.StunTime == 0)
                {
                    AI_cSpecial();
                }
                if(enemyShip is Conqueror && ((Conqueror)enemyShip).energy >= 8)
                {
                    AI_cSpecial(true);
                }
            }
        }
        
    }
}
