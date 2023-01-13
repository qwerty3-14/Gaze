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
    public class Brute : Ship
    {
        int range = 50;
        public Brute(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Brute;
            ShipStats.GetStatsFor(ShipID.Brute, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed, true);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;
            //5.5 7.5
            energyRate = 1;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(5, 5),
                new Vector2(5, -5),
                new Vector2(-5, -5),
                new Vector2(-5, 5),
            });
            mass = 8;
        }
        int shotCooldown = 0;
        public override void Shoot()
        {
            if (shotCooldown <= 0 && (energy >= 6 || specialCooldown > 0))
            {

                if (specialCooldown <= 0)
                {
                    energy -= 6;
                }
                shotCooldown = specialCooldown > 0 ? 10 : 30;
                for (int i = 0; i < 12 * bonusShots; i++)
                {
                    Vector2 shotPs = position + Functions.PolarVector(3, rotation) + Functions.PolarVector(Main.random.Next(-4, 5), rotation + (float)Math.PI / 2);
                    Vector2 vel = velocity + Functions.PolarVector((float)Main.random.NextDouble() * 2.5f + 1.5f, rotation + (float)Main.random.NextDouble() * (float)Math.PI / 4 - (float)Math.PI / 8);
                    new HunterPelt(shotPs, vel, team)
                    {
                        lifeTime = range,
                        rotation = (vel - velocity).ToRotation()
                    };
                }
                velocity = Functions.PolarVector(-3.5f * bonusShots, rotation);

                bonusShots = 1;
            }
        }
        int bonusShots = 1;
        int specialCooldown = 0;
        public override void Special()
        {
            if (specialCooldown <= 0 && health > 2)
            {
                health -= 2;
                energy += 4;
                bonusShots++;
                specialCooldown = 6;
            }
        }
        int counter;
        public override void LocalUpdate()
        {
            if(shotCooldown > 0)
            {
                shotCooldown--;
            }
            if (specialCooldown > 0)
            {
                specialCooldown--;
            }
            if (thrusting)
            {
                counter++;
                if (counter % 4 == 0)
                {
                    new Particle(position + Functions.PolarVector(4, rotation + (float)Math.PI / 2) + Functions.PolarVector(-5, rotation), 7, Color.Orange);
                    new Particle(position + Functions.PolarVector(-4, rotation + (float)Math.PI / 2) + Functions.PolarVector(-5, rotation), 7, Color.Orange);
                }
            }
        }

        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[16], pos, null, Color.White, rotation, new Vector2(5.5f, 7.5f), Vector2.One, SpriteEffects.None, 0f);
        }
        float recentEnemyRot = 0;
        public override void AI()
        {
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            List<Projectile> enemyProjectiles = EnemyProjectiles();
            bool AI_Dodging = false;

            if (enemyShip == null || ((Functions.screenLoopAdjust(position, enemyShip.position) - position).Length() > 2.5f * range))
            {
                for (int i = 0; i < enemyProjectiles.Count(); i++)
                {
                    if (AI_ImpendingCollision(enemyProjectiles[i], 60, out int expectedTIme))
                    {
                        if(enemyProjectiles[i] is Kugelblitz)
                        {
                            if(AI_TurnToward(recentEnemyRot + 3f * (float)Math.PI/8f))
                            {
                                AI_cShoot();
                            }
                        }
                        else
                        {
                            AI_cThrust();
                            AI_Dodge(enemyProjectiles[i]);
                            if (expectedTIme < 10)
                            {
                                AI_cShoot();
                            }

                        }
                        AI_Dodging = true;
                    }
                }
            }
            if (!AI_Dodging && enemyShip != null)
            {
                recentEnemyRot = enemyShip.rotation;
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                AI_cThrust();
                if ((enemyPos - position).Length() > 2.5f * range)
                {
                    
                    AI_TurnToward((enemyPos - position).ToRotation());
                   
                }
                else
                {
                    float aimAt = Functions.PredictiveAim(position, 3f, enemyPos, enemyShip.velocity - velocity);

                    if (!float.IsNaN(aimAt))
                    {
                        if (AI_TurnToward(aimAt))
                        {
                            if((bonusShots * 12 < enemyShip.health || energy < 6) && health > 2)
                            {
                                AI_cSpecial();
                            }
                            else
                            {
                                AI_cShoot();
                            }
                        }
                    }
                    else
                    {
                        AI_TurnToward((enemyPos - position).ToRotation());
                    }
                }
                
            }
        }
        
    }
}
