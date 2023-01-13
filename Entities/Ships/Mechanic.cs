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
    public class Mechanic : Ship
    {
        public Mechanic(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Mechanic;
            ShipStats.GetStatsFor(type, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed, true);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 2;
            //6.5 5.5
            shape = new Polygon(new Vector2[]
            {
                new Vector2(4, 1),
                new Vector2(-1, 5),
                new Vector2(-6, 5),
                new Vector2(-6, -5),
                new Vector2(-1, -5),
                new Vector2(4, -1),
            });
            mass = 6;
        }
        public int specialCountdown = 0;
        public override void Special()
        {
            if(energy == energyCapacity)
            {
                energy = 0;
                specialCountdown = 90;
            }
        }
        int counter = 0;
        public override void LocalUpdate()
        {
            ShipStats.GetStatsFor(type, out _, out _, out _, out acceleration, out _, out turnSpeed);
            if (shotCooldown > 0)
            {
                shotCooldown--;
            }
            if(specialCountdown > 0)
            {
                acceleration = 0;
                turnSpeed = 0;
                specialCountdown--;
                if(specialCountdown == 0)
                {
                    health += 4;
                    if(health > healthMax)
                    {
                        health = healthMax;
                    }
                }
            }
            if (thrusting)
            {
                counter++;
                if (counter % 5 == 0)
                {
                    new Particle(position + Functions.PolarVector(-5, rotation), 10, Color.Orange);
                }
            }

        }
        int shotCooldown = 0;
        public override void Shoot()
        {
            if (energy > 2 && shotCooldown <= 0)
            {
                shotCooldown = 15;
                AssetManager.PlaySound(SoundID.Pew);
                energy-=2;
                Projectile pelt = new MechBolt(position + Functions.PolarVector(3, rotation), velocity + Functions.PolarVector(4.5f, rotation + ((float)Math.PI /8f) * ((float)Main.random.NextDouble()) - ((float)Math.PI / 16f)), team);
                pelt.rotation = rotation;
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[14], pos, null, Color.White, rotation, new Vector2(6.5f, 5.5f), Vector2.One, SpriteEffects.None, 0f);
            if (specialCountdown > 0)
            {
                spriteBatch.Draw(AssetManager.extraEntities[4], pos + Functions.PolarVector(16 * (float)Math.Abs((float)(specialCountdown - 45) / 45f) - 10, rotation), null, Color.White, rotation, new Vector2(1.5f, 7.5f), Vector2.One, SpriteEffects.None, 0f);
                //spriteBatch.Draw(AssetManager.extraEntities[4], pos + Functions.PolarVector(16 * (float)Math.Abs((float)(specialCountdown - 45) / 45f) - 10, rotation), null, null, new Vector2(1.5f, 7.5f), rotation, Vector2.One, Color.White, 0, 0);
            }
        }
        bool AI_BatteryHitZero = false;
        public override void AI()
        {
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            bool AI_ShootingProj = false;

            List<Projectile> enemyProjectiles = EnemyProjectiles();
            int incomingDamage = 0;
            for (int i = 0; i < enemyProjectiles.Count; i++)
            {
                if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(60, enemyProjectiles[i].lifeTime)))
                {
                    incomingDamage += enemyProjectiles[i].damage;
                    if (enemyProjectiles[i] is Tripwire)
                    {
                        incomingDamage += 10;
                        break;
                    }
                    if (incomingDamage > health - 4)
                    {
                        break;
                    }
                    //AI_Dodge(enemyProjectiles[i]);
                }
            }

            if (incomingDamage > health - 4 || enemyShip == null || (Functions.screenLoopAdjust(position, enemyShip.position) - position).Length() > 240)
            {
                for (int i = 0; i < enemyProjectiles.Count; i++)
                {
                    if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(60, enemyProjectiles[i].lifeTime), out int ET))
                    {
                        AI_ShootingProj = true;
                        AI_cThrust();
                        if(enemyProjectiles[i].health == 1 || enemyProjectiles[i].health == 2 && energy > 2 && ET > 1.3f * Functions.AngularDifference((Functions.screenLoopAdjust(position, enemyProjectiles[i].position) - position).ToRotation(), rotation) / GetTurnSpeed())
                        {
                            Vector2 projPos = Functions.screenLoopAdjust(position, enemyProjectiles[i].position);
                            float aimAt = Functions.PredictiveAimWithOffset(position, 4.5f, projPos, enemyProjectiles[i].velocity - velocity, 3f);
                            if (!float.IsNaN(aimAt))
                            {
                                if (AI_TurnToward(aimAt))
                                {
                                    AI_cShoot();
                                }
                            }
                            else
                            {
                                AI_Dodge(enemyProjectiles[i]);
                            }
                        }
                        else
                        {
                            AI_Dodge(enemyProjectiles[i]);
                        }
                        break;
                    }
                }
            }
            /*if (enemyShip != null && !AI_ShootingProj)
            {
                if(AI_ImpendingCollision(enemyShip, 60))
                {
                    AI_ShootingProj = true;
                    AI_cThrust();
                    AI_Dodge(enemyShip);
                }
            }*/
            if (enemyShip != null && !AI_ShootingProj)
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                float toward = (enemyPos - position).ToRotation();
                Vector2 enemyVel = enemyShip.velocity;
                if (AI_BatteryHitZero)
                {
                    AI_Kite(6.7f, 4.5f * 30);
                    if (energy == energyCapacity)              
                    {
                        if(health < healthMax)
                        {
                            //Debug.WriteLine("hmm");
                            for (int i = 0; i < enemyProjectiles.Count; i++)
                            {
                                if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(60, enemyProjectiles[i].lifeTime)))
                                {
                                    AI_ShootingProj = true;
                                    AI_cThrust();
                                    AI_Dodge(enemyProjectiles[i]);
                                }
                            }
                            if (!AI_ShootingProj && specialCountdown == 0 && ((enemyPos - position).Length() > 180 || enemyShip is Trebeche))
                            {
                                AI_cSpecial();
                            }
                        }
                        else
                        {
                            AI_BatteryHitZero = false;
                        }
                    }
                }
                else
                {
                    /*
                    if ((enemyPos - position).Length() < 4.5f * 22)
                    {
                        AI_Kite(4.5f, 4.5f * 30);
                    }
                    else*/
                    if (AggressiveTargetting(position, 4.5f * 38, out Entity target))
                    {
                        Vector2 targetPos = Functions.screenLoopAdjust(position, target.position);
                        float aimAt = Functions.PredictiveAimWithOffset(position, 4.5f, targetPos, enemyVel - velocity, 3f);
                        if (!float.IsNaN(aimAt))
                        {
                            if (AI_TurnToward(aimAt))
                            {
                                AI_cShoot();
                            }
                        }
                        else
                        {
                            AI_TurnToward((targetPos - position).ToRotation());
                        }
                    }
                    else
                    {
                        if (health < healthMax)
                        {
                            AI_BatteryHitZero = true;
                        }
                        AI_cThrust();
                        if (enemyShip is Strafer)
                        {
                            //AI_TurnToward((enemyPos + Functions.PolarVector(4.5f * 10, enemyShip.rotation + (float)Math.PI / 2f) - position).ToRotation());
                            AI_Kite(4.5f, 4.5f * 30, 3f);
                        }
                        else
                        {
                            AI_TurnToward(toward);
                        }
                    }
                    if (energy == 0 || health <= 4)
                    {
                        AI_BatteryHitZero = true;
                    }
                }
            }
        }
    }
}
