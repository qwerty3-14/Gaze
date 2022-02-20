using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectGaze.Entities.Projectiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Entities.Ships
{
    class Trooper : Ship
    {
        public Trooper(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Trooper;
            ShipStats.GetStatsFor(ShipID.Trooper, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed, true);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 2;

            shape = new Polygon(new Vector2[]
            {
                new Vector2(9, 2),
                new Vector2(4, 4),
                new Vector2(2, 9),
                new Vector2(-2, 9),
                new Vector2(-2, -9),
                new Vector2(2, -9),
                new Vector2(4, -4),
                new Vector2(9, -2),
            });
            mass = 11;
        }
        int counter = 0;
        public override void LocalUpdate()
        {
            if (shotCooldown > 0)
            {
                shotCooldown--;
            }
            if (missileCooldown > 0)
            {
                missileCooldown--;
            }
            if (thrusting)
            {
                counter++;
                if (counter % 2 == 0)
                {
                    new Particle(position + Functions.PolarVector(-1, rotation), 6, Color.Lime);
                }
            }
        }
        int shotCooldown;
        public override void Shoot()
        {
            if (energy >= 6 && shotCooldown <= 0)
            {
                shotCooldown = 30;
                AssetManager.PlaySound(SoundID.Pew2);
                energy -= 6;
                Projectile pelt = new TrooperWave(position + Functions.PolarVector(10, rotation), velocity + Functions.PolarVector(5.2f, rotation), team);
                pelt.rotation = rotation;
            }
        }
        int missileCooldown = 0;
        int shotIndex = 0;
        Vector2[] gunOffsets = new Vector2[]
        {
            new Vector2(3, -4),
            new Vector2(3, 4),
        };
        public override void Special()
        {
            if (energy >= 2 && missileCooldown <= 0)
            {
                energy -= 2;
                missileCooldown = 5;
                PewPew();
            }
        }
        void PewPew()
        {
            AssetManager.PlaySound(SoundID.MissileLaunch);
            Vector2 shootFrom = position + Functions.PolarVector(gunOffsets[shotIndex].X, rotation) + Functions.PolarVector(gunOffsets[shotIndex].Y, rotation + (float)Math.PI / 2);
            float rot = (shootFrom - (position + Functions.PolarVector(-2, rotation))).ToRotation();
            Projectile p = new MicroMissile(shootFrom, Functions.PolarVector(3f, rot) + velocity, team);
            p.rotation = rot;
            shotIndex++;
            if (shotIndex > 1)
            {
                shotIndex = 0;
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[7], pos, null, null, new Vector2(3.5f, 9.5f), rotation, Vector2.One, Color.White, 0, 0);
        }
        bool AI_Dodging = false;
        public override void AI()
        {
            AI_Dodging = false;
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            Circle microMissileRange = new Circle(position, 100);
            int missileRequests = 0;
            List<Projectile> enemyProjectiles = EnemyProjectiles();
            for (int i = 0; i < enemyProjectiles.Count; i++)
            {
                if (enemyProjectiles[i].health == 1 && energy >= 2)
                {
                    if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(30, enemyProjectiles[i].lifeTime)))
                    {
                        missileRequests++;
                    }
                }
                else
                {
                    if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(60, enemyProjectiles[i].lifeTime)))
                    {
                        AI_Dodging = true;
                        Controls.controlThrust[team] = true;
                        AI_Dodge(enemyProjectiles[i]);
                    }
                }
            }
            if (missileRequests > 0)
            {
                int currentMissiles = 0;
                for (int i = 0; i < Main.entities.Count; i++)
                {
                    if (Main.entities[i].team == team && Main.entities[i] is MicroMissile)
                    {
                        currentMissiles++;
                    }
                }
                if (currentMissiles < missileRequests)
                {
                    Controls.controlSpecial[team] = true;
                }
            }
            if (enemyShip != null)
            {
                
                if(AI_CollidingWithEnemy(microMissileRange))
                {
                    Controls.controlSpecial[team] = true;
                }
                
                if (!AI_Dodging)
                {
                    Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                    if ((enemyPos - position).Length() > 120 * 5.2f)
                    {
                        float toward = (enemyPos - position).ToRotation();
                        if(AI_TurnToward(toward))
                        {
                            Controls.controlThrust[team] = true;
                        }
                    }
                    else
                    {
                        //Debug.WriteLine("velocity: " + velocity.Length());
                        //Debug.WriteLine("required: " + (maxSpeed * .2f - acceleration * 0.2f * (1f / 50f)));
                        if (velocity.Length() < maxSpeed * .2f - acceleration * 0.2f * (1f / 50f))
                        {
                            //Debug.WriteLine("Run!");
                            AI_Retreat(enemyPos);
                        }
                        else
                        {
                            //Debug.WriteLine("Aim!");
                            if (AI_AimAtEnemy(5.2f))
                            {
                                if (energy == energyCapacity && (enemyPos - position).Length() < 120 * 5.2f)
                                {
                                    Controls.controlShoot[team] = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                }

            }
        }
    }
}
