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
    public class Apocalypse : Ship
    {
        public ApocalypseTurret turret;
        public const int TurretRange = 80;
        float shotOffset = 12;
        public Apocalypse(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Apocalypse;
            ShipStats.GetStatsFor(ShipID.Apocalypse, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed, true);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 1;

            shape = new Polygon(new Vector2[]
            {
                new Vector2(8, 5),
                new Vector2(0, 9),
                new Vector2(-7, 9),
                new Vector2(-7, -9),
                new Vector2(0, -9),
                new Vector2(8, -5),
            });
            mass = 50;
            turret = new ApocalypseTurret(this, Vector2.Zero, 0);
        }
        int shotCooldown = 0;
        public override void Shoot()
        {
            if (shotCooldown <= 0 && energy >= 3)
            {
                energy -= 3;
                shotCooldown = 15;
                Blotch b = new Blotch(position + Functions.PolarVector(shotOffset, rotation), velocity + Functions.PolarVector(2.5f, rotation), team);
                b.rotation = rotation;
                AssetManager.PlaySound(SoundID.SmallExplosion);
            }
        }
        int specialCooldown = 0;
        public override void Special()
        {
            if (specialCooldown <= 0 && energy >= 8)
            {
                energy -= 8;
                specialCooldown = 60;
                Mine b = new Mine(position + Functions.PolarVector(-4, rotation), velocity + Functions.PolarVector(-1f, rotation), team);
                b.rotation = rotation;
                AssetManager.PlaySound(SoundID.SmallExplosion);
            }
        }
        int eyeTime = 0;
        public override void HitByProjectile(int damage)
        {
            eyeTime = 45;
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[9], pos, null, Color.White, rotation, new Vector2(7.5f, 13.5f), Vector2.One, SpriteEffects.None, 0f);
            if(eyeTime > 0)
            {
                spriteBatch.Draw(AssetManager.extraEntities[7], pos, null, Color.White, rotation, new Vector2(7.5f, 13.5f), Vector2.One, SpriteEffects.None, 0f);
            }
            turret.Draw(spriteBatch, pos);
        }
        int counter;
        public override void LocalUpdate()
        {
            if (specialCooldown > 0)
            {
                specialCooldown--;
            }
            if (shotCooldown > 0)
            {
                shotCooldown--;
            }
            if (eyeTime > 0)
            {
                eyeTime--;
            }
            if (thrusting)
            {
                counter++;
                if (counter % 5 == 0)
                {
                    new Particle(position
                        + Functions.PolarVector(4, rotation + (float)Math.PI / 2)
                        + Functions.PolarVector(-6, rotation),
                        10, Color.Orange);
                    new Particle(position
                        + Functions.PolarVector(-4, rotation + (float)Math.PI / 2)
                        + Functions.PolarVector(-6, rotation),
                        10, Color.Orange);
                }
            }
            if (StunTime <= 0)
            {
                turret.UpdateRelativePosition();
                if (DefensiveTargetting(position, 900, out Entity target, 4))
                {
                    Vector2 enemyPos = Functions.screenLoopAdjust(position, target.position);
                    if (turret.AimAt(enemyPos))
                    {
                        if ((enemyPos - position).Length() < TurretRange)
                        {
                            turret.Fire();
                        }
                    }
                }
                else
                {
                    turret.AimHome();
                }
            }
        }
        
        public override void AI()
        {
            bool AI_Dodging = false;
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            List<Projectile> enemyProj = EnemyProjectiles();

            Polygon ShotArea = new Polygon(new Vector2[]
            {
                    position + Functions.PolarVector(5.5f, rotation + (float)Math.PI/2),
                    position + Functions.PolarVector(-5.5f, rotation + (float)Math.PI/2),
                    position + Functions.PolarVector(-5.5f, rotation + (float)Math.PI/2) + Functions.PolarVector(20, rotation),
                    position + Functions.PolarVector(5.5f, rotation + (float)Math.PI/2) + Functions.PolarVector(20, rotation),
            });
            for(int i =0; i < enemyProj.Count; i++)
            {
                if (enemyProj[i].health >= 0 && enemyProj[i].health <=2 && AI_CollidingWithEntitiy(enemyProj[i], ShotArea))
                {
                    AI_cShoot();
                }
                else if(enemyProj[i].velocity == Vector2.Zero || (enemyProj[i] is PsuedostableVacum))
                {
                    if(AI_ImpendingCollision(enemyProj[i], 60))
                    {
                        AI_Dodge(enemyProj[i]);

                        AI_Dodging = true;
                        AI_cThrust();
                    }
                }
            }
            if (enemyShip != null && !AI_Dodging)
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                if (AggressiveTargetting(position, 300, out Entity target, 2, delegate (Entity possibleTarget) { return !(possibleTarget is Missile); }))
                {
                    Vector2 targetPos = Functions.screenLoopAdjust(position, target.position);
                    float aimAt = Functions.PredictiveAimWithOffset(position, 2.5f, targetPos, enemyShip.velocity - velocity, shotOffset);

                    if (!float.IsNaN(aimAt))
                    {
                        if (AI_TurnToward(aimAt))
                        {
                            if(target is Ship || target is Illusion || energy == energyCapacity)
                            {
                                AI_cShoot();
                            }
                        }
                    }
                    else
                    {
                        AI_TurnToward((targetPos - position).ToRotation());
                    }
                }
                else
                {
                    AI_TurnToward((enemyPos - position).ToRotation());
                    AI_cThrust();
                }
                if(energy == energyCapacity)
                {
                    AI_cSpecial();
                }
            }
        }
    }
    public class ApocalypseTurret : Turret
    {
        public ApocalypseTurret(Apocalypse parent, Vector2 anchorAt, float homeRotation = 0) : base(parent, anchorAt, homeRotation)
        {
            this.parent = parent;
            origin = new Vector2(4.5f, 5.5f);
            texture = AssetManager.turrets[2];
            turretLength = 9f;
            rotSpeed = (float)Math.PI / 210f;
        }
        int timer = 0;
        Beam beam;
        public override void Update()
        {
            timer++;
            if(timer > 90 && timer % 2 == 0)
            {
                new Particle(AbsoluteShootPosition(), 10, Color.Red, parent.velocity + Functions.PolarVector(.75f, AbsoluteRotation() + (float)Main.random.NextDouble() * (float)Math.PI / 2f - (float)Math.PI / 4f) );
            }
            if(beam != null)
            {
                if (beam.Update(AbsoluteShootPosition(), AbsoluteRotation()))
                {
                    beam.ProcessCollision();
                }
                else
                {
                    beam = null;
                }
            }
        }
        public override void Fire()
        {
            if(timer > 90)
            {
                timer = 0;
                beam = new Beam(parent, Color.Red, Apocalypse.TurretRange, 10, -1, 4);
                beam.Update(AbsoluteShootPosition(), AbsoluteRotation());
                AssetManager.PlaySound(SoundID.Pew2);
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 Pos)
        {
            spriteBatch.Draw(texture, Pos + relativePosition, null, Color.White, AbsoluteRotation(), origin, new Vector2(1, 1), SpriteEffects.None, 0);
            if(beam != null)
            {
                beam.Draw(spriteBatch);
            }
        }
    }
}
