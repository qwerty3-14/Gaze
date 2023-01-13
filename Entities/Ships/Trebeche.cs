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
    public class Trebeche : Ship
    {
        TrebuchetTurret turret;
        Beam turretBeam;
        public Trebeche(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Trebeche;
            ShipStats.GetStatsFor(ShipID.Trebeche, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 1;
            breaks = 0.999f;
            mass = 8;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(8, 0),
                new Vector2(3, -8),
                new Vector2(-8, -8),
                new Vector2(-8, 8),
                new Vector2(3, 8),
            });
            turret = new TrebuchetTurret(this, new Vector2(5.5f, 0), 0);

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
            spriteBatch.Draw(AssetManager.ships[10], pos, null, Color.White, rotation, new Vector2(9f, 8.5f), Vector2.One, SpriteEffects.None, 0f);
            if(shieldTime > 0)
            {
                spriteBatch.Draw(AssetManager.extraEntities[3], pos, null, Color.White, rotation, new Vector2(9f, 8.5f), Vector2.One, SpriteEffects.None, 0f);
            }
            if (turretBeam != null)
            {
                turretBeam.Draw(spriteBatch);
            }
            turret.Draw(spriteBatch, pos);
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
            if(energy >= 4 && specialCooldown <= 0)
            {
                specialCooldown = 45;
                energy -= 4;
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
                velocity += Functions.PolarVector(damageBlocked * 1.2f, hitDirection);
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
            if ((!(npcShoot || (team < 2 && Controls.controlShoot[team])) || range > 400) && kug != null)
            {
                kug.Release(range);
                //kug.velocity = velocity;
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

            turret.UpdateRelativePosition();
            if(DefensiveTargetting(turret.AbsolutePosition(), 40, out Entity target, 1, delegate (Entity possibleTarget)
            {
                Vector2 posPosition = Functions.screenLoopAdjust(turret.AbsolutePosition(), possibleTarget.position);
                float towardPos = (posPosition - turret.AbsolutePosition()).ToRotation();
                return Functions.AngularDifference(towardPos, rotation) < (float)Math.PI / 4f;
            }))
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(turret.AbsolutePosition(), target.position);
                if(turret.AimAt(enemyPos) && turretBeam == null)
                {
                    turretBeam = new Beam(this, Color.Gray, 35, 30, -1, 1, 0.8f);
                    AssetManager.PlaySound(SoundID.Pew, 0.6f);
                }
            }
            else
            {
                turret.AimHome();
            }
            if(turretBeam != null && turret != null)
            {
                if (turretBeam.Update(turret.AbsoluteShootPosition(), turret.AbsoluteRotation()))
                {
                    turretBeam.ProcessCollision();
                }
                else
                {
                    turretBeam = null;
                }
            }
        }
        public override void AI()
        {

            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            bool AI_Dodging = false;
            List<Projectile> enemyProj = EnemyProjectiles();
            
            if(kug != null)
            {
                AI_cShoot();
            }
            else
            {
                for(int i =0; i < enemyProj.Count; i++)
                {
                    if(enemyProj[i].velocity == Vector2.Zero || (enemyProj[i] is PsuedostableVacum))
                    {
                        if(AI_ImpendingCollision(enemyProj[i], 60))
                        {
                            AI_Dodge(enemyProj[i]);

                            AI_Dodging = true;
                            AI_cThrust();
                        }
                    }

                }
            }
            if (enemyShip != null && !AI_Dodging)
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                if (velocity.Length() < (float)maxSpeed * 0.2f * 0.9f)
                {
                    AI_Retreat(enemyPos);
                }
                else
                {
                    

                    if (npcShoot || (team < 2 && Controls.controlShoot[team]))
                    {
                        float aimAt = Functions.PredictiveAim(position, range / Kugelblitz.KugelTime, enemyPos, enemyShip.velocity);
                        if (float.IsNaN(aimAt))
                        {
                            aimAt = Functions.PredictiveAim(position, 6, enemyPos, enemyShip.velocity);
                        }
                        if (!float.IsNaN(aimAt))
                        {
                            if (AI_TurnToward(aimAt))
                            {
                                //AI_cShoot();
                            }
                        }


                        Vector2 landingPos = position + Functions.PolarVector(-2, rotation) + Functions.PolarVector(range, rotation);
                        Vector2 altEnemyPos = Functions.screenLoopAdjust(landingPos, enemyShip.position);
                        if ((altEnemyPos - position).Length() < (Functions.PolarVector(range, rotation) + (Vector2.Zero - enemyShip.velocity) * Kugelblitz.KugelTime).Length() )
                        {

                            AI_cShoot(true);
                        }
                    }
                    else if(enemyShip is Ship)
                    {
                        float sensitivity = (float)Math.PI/2f;
                        ShipStats.GetWeaponStats(((Ship)enemyShip).type, out _, out int enemyRange);
                        if(enemyShip is Ship && enemyRange <= 300)
                        {
                            sensitivity = 2f * (float)Math.PI;
                        }
                        AI_Kite(6, 600, 0, sensitivity);
                    }
                }

                if (AI_ImpendingBeamCollision(5))
                {
                    AI_cSpecial();
                }
                List<Projectile> enemyProjectiles = EnemyProjectiles();
                for (int i = 0; i < enemyProjectiles.Count(); i++)
                {
                    if (AI_ImpendingCollision(enemyProjectiles[i], 5) && enemyProjectiles[i].damage > 0)
                    {
                        AI_cSpecial();
                    }
                }
            }
        }
    }
    public class TrebuchetTurret : Turret
    {
        public TrebuchetTurret(Entity parent, Vector2 anchorAt, float homeRotation = 0) : base(parent, anchorAt, homeRotation)
        {
            origin = new Vector2(1.5f, 1.5f);
            texture = AssetManager.turrets[5];
            turretLength = 3.5f; 
            rotSpeed = (float)Math.PI / 30f;
        }
    }
}
