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
    public class Palladin : Ship
    {
        const int AuraMax = 170;
        public const float Range = 3.7f * 30;
        public Palladin(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Palladin;
            ShipStats.GetStatsFor(ShipID.Palladin, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed, true);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 3;

            shape = new Polygon(new Vector2[]
            {
                new Vector2(12, -2),
                new Vector2(12, 2),
                new Vector2(-4, 8),
                new Vector2(-15, 8),
                new Vector2(-15, -8),
                new Vector2(-4, -8),
            });
            mass = 30;
            DrawOnTop = true;
        }

        int shotCooldown = 0;
        int shotIndex = 0;
        Vector2[] gunOffsets = new Vector2[]
        {
            new Vector2(11, -4),
            new Vector2(11, 4),
            new Vector2(6, -7),
            new Vector2(6, 7),
        };
        public override void Shoot()
        {
            if(energy >= 2 && shotCooldown <= 0)
            {
                energy-=2;
                shotCooldown = 16;
                PewPew();
            }
        }
        void PewPew()
        {
            for (int i = 0; i < 1; i++)
            {
                AssetManager.PlaySound(SoundID.SmallExplosion);
                Projectile p = new PalShell(position + Functions.PolarVector(gunOffsets[shotIndex].X, rotation) + Functions.PolarVector(gunOffsets[shotIndex].Y, rotation + (float)Math.PI / 2), Functions.PolarVector(3.7f, rotation) + velocity, team);
                p.rotation = rotation;
                shotIndex++;
                if (shotIndex > 3)
                {
                    shotIndex = 0;
                }
            }
        }
        float auraRadius = 0;
        int auraTime = 0;
        public override void Special()
        {
            if(auraTime <=0 && energy >= 1)
            {
                energy--;
                auraTime = 20;
                if (auraRadius == 0)
                {
                    AssetManager.PlaySound(SoundID.CreateIllusion, -0.3f);
                }
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            if(auraRadius  > 0)
            {
                Circle c = new Circle(pos, auraRadius);
                c.Draw(spriteBatch, new Color(0, 20, 80, 80));
            }
            spriteBatch.Draw(AssetManager.ships[6], pos, null, Color.White, rotation, new Vector2(16.5f, 11f), Vector2.One, SpriteEffects.None, 0f);
        }
        void SlowingAura(Circle area)
        {
            for (int i = 0; i < Arena.entities.Count; i++)
            {
                if (Arena.entities[i].team != team)
                {
                    Shape[] col = Arena.entities[i].AllHitboxes();
                    for (int k = 0; k < col.Length; k++)
                    {
                        if (col[k].Colliding(area))
                        {
                            Arena.entities[i].SlowTime = 2;
                            break;
                        }
                    }
                }
            }
        }
        int counter;
        public override void LocalUpdate()
        {
            if (auraTime > 0)
            {
                auraTime--;
                if(auraRadius < AuraMax)
                {
                    auraRadius += 6;
                }
                if(auraRadius > AuraMax)
                {
                    auraRadius = AuraMax;
                }
            }
            else
            {
                if(auraRadius == AuraMax)
                {
                    AssetManager.PlaySound(SoundID.IllusionDown, -.3f);
                }
                if (auraRadius > 0)
                {
                    auraRadius -= 6;
                }
                if (auraRadius < 0)
                {
                    auraRadius = 0;
                }
            }
            if(auraRadius > 0)
            {
                SlowingAura(new Circle(position, auraRadius));
                for(int i =0; i < 5; i++)
                {
                    if(Main.random.Next(AuraMax) < auraRadius)
                    {
                        float dir = (float)Main.random.NextDouble() * 2f * (float)Math.PI;
                        new Particle(position + Functions.PolarVector((float)Main.random.NextDouble() * auraRadius, dir), 1, Color.LightBlue);
                    }
                }
            }
            if (shotCooldown > 0)
            {
                shotCooldown--;
                if (shotCooldown == 4 || shotCooldown == 8 || shotCooldown == 12)
                {
                    PewPew();
                }
            }
            if (thrusting)
            {
                counter++;
                if (counter % 5 == 0)
                {
                    new Particle(position 
                        + Functions.PolarVector(-7, rotation + (float)Math.PI / 2) 
                        + Functions.PolarVector(-13, rotation), 
                        14, Color.Orange);
                    new Particle(position
                        + Functions.PolarVector(7, rotation + (float)Math.PI / 2)
                        + Functions.PolarVector(-13, rotation),
                        14, Color.Orange);
                    new Particle(position
                        + Functions.PolarVector(-2, rotation + (float)Math.PI / 2)
                        + Functions.PolarVector(-14, rotation),
                        14, Color.Orange); 
                    new Particle(position
                        + Functions.PolarVector(2, rotation + (float)Math.PI / 2)
                        + Functions.PolarVector(-14, rotation),
                        14, Color.Orange);
                }
            }


        }
        bool AI_ShootingProj = false;
        bool AI_BatteryHitZero = false;
        public override void AI()
        {
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            AI_ShootingProj = false;
            Circle auraArea = new Circle(position, AuraMax);
            if (AI_CollidingWithEnemy(auraArea))
            {
                AI_cSpecial();
            }
            List<Projectile> enemyProjectiles = EnemyProjectiles();
            for (int i = 0; i < enemyProjectiles.Count; i++)
            {
                if (enemyProjectiles[i].health == 1 && energy > 6)
                {
                    if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(30, enemyProjectiles[i].lifeTime)))
                    {
                        AI_cSpecial();
                        AI_ShootingProj = true;
                        Vector2 projVel = enemyProjectiles[i].velocity * (enemyProjectiles[i].SlowTime > 0 ? 0.5f : 1);
                        float aimAt = Functions.PredictiveAim(position, 3, enemyProjectiles[i].position, projVel - velocity);
                        if (!float.IsNaN(aimAt))
                        {
                            if (AI_TurnToward(aimAt))
                            {
                                AI_cShoot();
                            }
                        }
                    }
                }
                else if(enemyProjectiles[i].damage > 2 || AI_BatteryHitZero || enemyProjectiles[i] is Tripwire)
                {
                    if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(30, enemyProjectiles[i].lifeTime)))
                    {
                        AI_cSpecial();
                        AI_ShootingProj = true;
                        AI_cThrust();
                        AI_Dodge(enemyProjectiles[i]);
                    }
                }
            }
            if (enemyShip != null && !AI_ShootingProj)
            {
                bool attemptBackstab = false;
                if(enemyShip is Trebeche)
                {
                    attemptBackstab = true;
                }
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                float toward = (enemyPos - position).ToRotation();
                Vector2 enemyVel = enemyShip.velocity * (enemyShip.SlowTime > 0 ? 0.5f : 1);
                if(AI_BatteryHitZero)
                {
                    AI_Kite(6.7f, 3.7f * 30);
                    if(energy == energyCapacity)
                    {
                        AI_BatteryHitZero = false;
                    }
                }
                else
                {
                    if ((enemyPos - position).Length() < Range)
                    {
                        
                        if(attemptBackstab && Functions.AngularDifference((position - enemyPos).ToRotation(), enemyShip.rotation) < (float)Math.PI/2)
                        {
                            AI_AvoidFront(10);
                        }
                        else
                        {
                            float aimAt = Functions.PredictiveAim(position, 3.7f, enemyPos, enemyVel - velocity);
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
                    }
                    else
                    {
                        AI_cThrust();
                        AI_TurnToward(toward);
                    }
                    if(energy == 0 && !(enemyShip is Illusioner) && !(enemyShip is Illusion))
                    {
                        AI_BatteryHitZero = true;
                    }
                }
            }
        }
    }
}
