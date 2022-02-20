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
    public class Trebeche : Ship
    {
        public Trebeche(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Trebeche;
            ShipStats.GetStatsFor(ShipID.Trebeche, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 1;

            mass = 8;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(8, 0),
                new Vector2(3, -8),
                new Vector2(-8, -8),
                new Vector2(-8, 8),
                new Vector2(3, 8),
            });

        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            
            if(kug != null)
            {
                Vector2 kugPos = kug.position;
                new Line(kugPos + Functions.PolarVector(12, kug.rotation + (float)Math.PI / 2), kugPos + Functions.PolarVector(range, kug.rotation)).Draw(spriteBatch, Color.Red);
                new Line(kugPos + Functions.PolarVector(12, kug.rotation - (float)Math.PI / 2), kugPos + Functions.PolarVector(range, kug.rotation)).Draw(spriteBatch, Color.Red);

                float a = (counter % 10 > 4) ? 0.5f : 0.3f;
                Color beamColor = new Color((226 / 255f) * a, (209 / 255f) * a, (24 / 255f) * a, 1f * a);
                new Line(kugPos + Functions.PolarVector(12, kug.rotation - (float)Math.PI / 2), position + Functions.PolarVector(4, rotation) + Functions.PolarVector(8, rotation - (float)Math.PI / 2)).Draw(spriteBatch, beamColor);
                new Line(kugPos + Functions.PolarVector(12, kug.rotation - (float)Math.PI / 2), position + Functions.PolarVector(-7, rotation) + Functions.PolarVector(8, rotation - (float)Math.PI / 2)).Draw(spriteBatch, beamColor);
                new Line(kugPos + Functions.PolarVector(12, kug.rotation + (float)Math.PI / 2), position + Functions.PolarVector(4, rotation) + Functions.PolarVector(8, rotation + (float)Math.PI / 2)).Draw(spriteBatch, beamColor);
                new Line(kugPos + Functions.PolarVector(12, kug.rotation + (float)Math.PI / 2), position + Functions.PolarVector(-7, rotation) + Functions.PolarVector(8, rotation + (float)Math.PI / 2)).Draw(spriteBatch, beamColor);
            }
            spriteBatch.Draw(AssetManager.ships[10], pos, null, null, new Vector2(9f, 8.5f), rotation, Vector2.One, Color.White, 0, 0);
            if(shieldTime > 0)
            {
                spriteBatch.Draw(AssetManager.extraEntities[3], pos, null, null, new Vector2(9f, 8.5f), rotation, Vector2.One, Color.White, 0, 0);
            }
        }
        Kugelblitz kug;
        float range = 40;
        public override void Shoot()
        {
            if (energy >= 12)
            {
                if (kug == null)
                {
                    kug = new Kugelblitz(position, Vector2.Zero, team);
                    range = 40;
                    energy -= 12;
                }
            }
        }
        int counter;
        int specialCooldown = 0;
        int shieldTime = 0;
        public override void Special()
        {
            if(energy >= 8 && specialCooldown <= 0)
            {
                specialCooldown = 60;
                energy -= 8;
                shieldTime = 30;
                AssetManager.PlaySound(SoundID.Shield);
            }
        }
        public override void ModifyHitByProjectile(ref int damage, float hitDirection)
        {
            if (shieldTime > 0)
            {
                int damageBlocked = damage / 2;
                if (damage % 2 == 1)
                {
                    damageBlocked += Main.random.Next(2) == 0 ? 1 : 0;
                }
                damage -= damageBlocked;
                velocity += Functions.PolarVector(damageBlocked * 2f, hitDirection);
            }
        }
        public override void LocalUpdate()
        {
            if(shieldTime > 0)
            {
                shieldTime--;
            }
            if(specialCooldown > 0)
            {
                specialCooldown--;
            }
            if ((!Controls.controlShoot[team] || range > 400) && kug != null)
            {
                kug.Release(range);
                kug.velocity = velocity;
                kug = null;
            }
            else if( kug != null)
            {
                range += 6f;
                kug.lifeTime = Kugelblitz.KugelTime;
                kug.rotation = rotation ;
                kug.position = position + Functions.PolarVector(-2, rotation);
            }
            counter++;
            if (thrusting)
            {
                if (counter % 7 == 0)
                {
                    new Particle(position + Functions.PolarVector(-3, rotation), 10, Color.Orange);
                }
            }
        }
        public override void AI()
        {

            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            if(kug != null)
            {
                Controls.controlShoot[team] = true;
            }
            if (enemyShip != null)
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                if (velocity.Length() < (float)maxSpeed * 0.2f * 0.9f)
                {
                    AI_Retreat(enemyPos);
                }
                else
                {
                    

                    if (Controls.controlShoot[team])
                    {
                        float aimAt = Functions.PredictiveAim(position, range / Kugelblitz.KugelTime, enemyPos, enemyShip.velocity - velocity);
                        if (float.IsNaN(aimAt))
                        {
                            aimAt = Functions.PredictiveAim(position, 6, enemyPos, enemyShip.velocity - velocity);
                        }
                        if (!float.IsNaN(aimAt))
                        {
                            if (AI_TurnToward(aimAt))
                            {
                                //Controls.controlShoot[team] = true;
                            }
                        }


                        Vector2 landingPos = position + Functions.PolarVector(-2, rotation) + Functions.PolarVector(range, rotation);
                        Vector2 altEnemyPos = Functions.screenLoopAdjust(landingPos, enemyShip.position);
                        if ((altEnemyPos - position).Length() < (Functions.PolarVector(range, rotation) + (velocity - enemyShip.velocity) * Kugelblitz.KugelTime).Length() )
                        {

                            Controls.controlShoot[team] = false;
                        }
                    }
                    else
                    {
                        if (AI_TurnToward((enemyPos - position).ToRotation()))
                        {
                            Controls.controlShoot[team] = true;
                        }
                    }
                }

                if (AI_ImpendingBeamCollision(5))
                {
                    Controls.controlSpecial[team] = true;
                }
                List<Projectile> enemyProjectiles = EnemyProjectiles();
                for (int i = 0; i < enemyProjectiles.Count(); i++)
                {
                    if (AI_ImpendingCollision(enemyProjectiles[i], 5) && enemyProjectiles[i].damage > 0)
                    {
                        Controls.controlSpecial[team] = true;
                    }
                }
            }
        }
    }
}
