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
    public class Missionary : Ship
    {
        MissionaryTurret[] turrets = new MissionaryTurret[2];
        const int energyCost = 5;
        public const float vel = 4.8f;
        public const float range = 240;
        public Missionary(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Missionary;
            ShipStats.GetStatsFor(type, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 2;

            mass = 32;
            //11, 20.5
            shape = new Polygon(new Vector2[]
            {
                new Vector2(12, 5),
                new Vector2(2.5f, 20),
                new Vector2(-6f, 20),
                new Vector2(-10f, 17.5f),
                new Vector2(-10f, -17.5f),
                new Vector2(-6f, -20),
                new Vector2(2.5f, -20),
                new Vector2(12, -5),
            });
            turrets = new MissionaryTurret[]
            {
                new MissionaryTurret(this, new Vector2(10, 15.5f)),
                new MissionaryTurret(this, new Vector2(10, -15.5f)),
            };
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            for(int i =0; i <2; i++)
            {
                turrets[i].Draw(spriteBatch, pos);
            }
            spriteBatch.Draw(AssetManager.ships[12], pos, null, Color.White, rotation, new Vector2(9f, 20.5f), Vector2.One, SpriteEffects.None, 0f);
        }
        int shotIndex = 0;
        int shotCooldown = 0;
        public override void Shoot()
        {
            if(energy >= energyCost && shotCooldown <= 0)
            {
                shotCooldown = 8;
                energy -= energyCost;
                turrets[shotIndex].Fire();
                shotIndex = shotIndex == 1 ? 0 : 1;
            }
        }
        public Projectile wave;
        public override void Special()
        {
            if(energy == energyCapacity)
            {
                energy = 0;
                velocity = Functions.PolarVector((float)maxSpeed * 0.2f - (float)acceleration * 0.2f * (1f / 50f), rotation);
                
                if(wave != null)
                {
                    wave.Kill();
                }
                
                wave = new PsuedostableVacum(position + Functions.PolarVector(26, rotation), velocity, this, team);
                wave.rotation = rotation;
                AssetManager.PlaySound(SoundID.MissileLaunch, 0.8f);
            }
        }
        int counter;
        public override void LocalUpdate()
        {
            if(shotCooldown > 0)
            {
                shotCooldown--;
            }
            if (StunTime <= 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    turrets[i].UpdateRelativePosition();
                    if (AggressiveTargetting(turrets[i].AbsolutePosition(), 300, out Entity target, 2, delegate (Entity possibleTarget)
                    {
                        Vector2 posPosition = Functions.screenLoopAdjust(turrets[i].AbsolutePosition(), possibleTarget.position);
                        float towardPos = (posPosition - turrets[i].AbsolutePosition()).ToRotation();
                        return Functions.AngularDifference(towardPos, rotation) < 1f * ((float)Math.PI / 2f);
                    }))
                    {
                        float aimAt = Functions.PredictiveAimWithOffset(turrets[i].AbsolutePosition(), vel, Functions.screenLoopAdjust(turrets[i].AbsolutePosition(), target.position), target.velocity - velocity, 5f);
                        if (!float.IsNaN(aimAt))
                        {
                            turrets[i].AimAt(aimAt);
                        }
                    }
                    else
                    {
                        turrets[i].AimHome();
                    }

                }
            }
            if (thrusting)
            {
                counter++;
                if (counter % 3 == 0)
                {
                    new Particle(position + Functions.PolarVector(-10.5f - (counter % 2), rotation + (float)Math.PI / 2) + Functions.PolarVector(-7, rotation), 6, Color.Orange);
                    new Particle(position + Functions.PolarVector(10.5f + (counter % 2), rotation + (float)Math.PI / 2) + Functions.PolarVector(-7, rotation), 6, Color.Orange);
                }
            }
        }
        bool AI_ShootingProj;
        bool AI_BatteryDied;
        public override void AI()
        {
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            AI_ShootingProj = false;
            List<Projectile> enemyProjectiles = EnemyProjectiles();
            if(energy == energyCapacity)
            {
                AI_BatteryDied = false;
            }
            if (enemyShip != null && energy == energyCapacity && (wave == null || wave.velocity != velocity))
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                float aimAt = Functions.PredictiveAim(position, (float)maxSpeed * 0.2f, enemyPos, enemyShip.velocity);
                if (!float.IsNaN(aimAt))
                {
                    if (AI_TurnToward(aimAt))
                    {
                        AI_cSpecial();
                    }
                }
                else
                {
                    if (AI_TurnToward((enemyPos - position).ToRotation()))
                    {
                        AI_cSpecial();
                    }
                }
            }
            else
            {
                for (int i = 0; i < enemyProjectiles.Count; i++)
                {
                    if (enemyProjectiles[i].health <= 2 && enemyProjectiles[i].health >= 0 && energy >= energyCost)
                    {

                    }
                    else
                    {
                        if (AI_ImpendingCollision(enemyProjectiles[i], 60, out int expectedTime))
                        {
                            if (enemyProjectiles[i].health == -1)
                            {
                                AI_ShootingProj = true;
                                AI_Dodge(enemyProjectiles[i], false);
                                AI_cThrust();
                            }
                            else if (AI_ImpendingCollisionAlly(wave, enemyProjectiles[i], 60, out int waveTime))
                            {
                                if (expectedTime < waveTime)
                                {
                                    AI_ShootingProj = true;
                                    AI_Dodge(enemyProjectiles[i], false);
                                    AI_cThrust();
                                }
                            }
                            else
                            {

                                AI_ShootingProj = true;
                                AI_Dodge(enemyProjectiles[i], false);
                                AI_cThrust();
                            }
                        }
                    }
                }
                if (enemyShip != null && !AI_ShootingProj)
                {
                    Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                    if ((enemyPos - position).Length() < range - 40)
                    {
                        if (AI_TurnToward((enemyPos - position).ToRotation()))
                        {
                            AI_cShoot();
                        }
                    }
                    else
                    {
                        if ( ClosestProjectile(position, range - 40, out Entity target, 2, delegate (Entity possibleTarget) { return AI_ImpendingCollision(possibleTarget, 60); }))
                        {
                            Vector2 projPos = Functions.screenLoopAdjust(position, target.position);
                            if ((projPos - position).Length() < range - 40)
                            {
                                AI_ShootingProj = true;
                                if (AI_TurnToward((projPos - position).ToRotation()))
                                {
                                    AI_cShoot();
                                }
                            }
                        }
                        if (!AI_ShootingProj)
                        {
                            if ((position - enemyPos).Length() < (position + velocity - enemyPos).Length() || wave == null || wave.velocity != velocity)
                            {
                                if (enemyShip is Apocalypse || AI_BatteryDied)
                                {
                                    AI_Kite(6, range - 40);
                                }
                                else
                                {

                                    if (energy == 0)
                                    {
                                        AI_BatteryDied = true;
                                    }
                                    float aimAt = Functions.PredictiveAim(position, (float)maxSpeed * 0.2f, enemyPos, enemyShip.velocity - velocity);
                                    if (!float.IsNaN(aimAt))
                                    {
                                        if (AI_TurnToward(aimAt))
                                        {
                                            AI_cThrust();
                                        }
                                    }
                                }
                                
                            }

                        }

                    }
                }
            }

        }
    }
    class MissionaryTurret : Turret
    {
        public MissionaryTurret(Entity parent, Vector2 anchorAt, float homeRotation = 0) : base(parent, anchorAt, homeRotation)
        {
            origin = new Vector2(3f, 3f);
            texture = AssetManager.turrets[4];
            turretLength = 5f;
            rotSpeed = (float)Math.PI / 90f;
        }
        public override void Fire()
        {
            Projectile proj = new Inq(AbsoluteShootPosition(), Functions.PolarVector(Missionary.vel, AbsoluteRotation()) + parent.velocity, parent.team);
            proj.rotation = AbsoluteRotation();
            AssetManager.PlaySound(SoundID.Pew, -0.5f);
        }
    }
}
