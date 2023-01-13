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
    public class Escort : Ship
    {
        public bool attached = true;
        public Platform platform;
        public const float BeamRange = 160;
        public Escort(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Escort;
            ShipStats.GetStatsFor(ShipID.Escort, out _, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed, true);
            healthMax = 5;
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(4, 0),
                new Vector2(-5, 5),
                new Vector2(-5, -5)
            });
            platform = new Platform(this, team);
            mass = 6;
        }
        bool pressedSpecial = false;
        int cooldown;
        Beam beam;
        public override void Shoot()
        {
            if (attached && platform != null)
            {
                if (cooldown <= 0 && energy >= 8 && beam == null)
                {
                    BeamBeam();
                    energy -= 8;
                    cooldown = 30;
                }
            }
            else
            {
                if (cooldown <= 0 && energy >= 1)
                {
                    PewPew();
                    energy -= 1;
                    cooldown = 30;
                }
            }
        }
        void PewPew()
        {
            Projectile proj = new GreenPulse(position, Functions.PolarVector(4, rotation) + velocity, team);
            proj.rotation = rotation;
            AssetManager.PlaySound(SoundID.Pew);
        }
        void BeamBeam()
        {
            AssetManager.PlaySound(SoundID.Beam, -.5f);
            beam = new Beam(this, Color.Green, BeamRange, 4, -1, 2, 2);
        }
        public override void Special()
        {
            if (!pressedSpecial)
            {
                if (attached)
                {
                    rotation += (float)Math.PI;
                    platform.velocity = this.velocity;
                    velocity += Functions.PolarVector(1.6f, rotation);
                    attached = false;
                    energy = 0;
                    
                }
                else
                {
                    if (platform != null)
                    {

                        Functions.ProximityExplosion(new Circle(platform.position, 50), platform.health, team);
                        platform.Kill();
                        platform = null;
                    }
                    else if (energy >= energyCapacity)
                    {
                        attached = true;
                        platform = new Platform(this, team);
                        energy = 0;
                    }
                }
            }
            pressedSpecial = true;
        }
        int counter = 0;
        public override void LocalUpdate()
        {
            if(cooldown > 0)
            {
                cooldown--;
                if(cooldown == 22 || cooldown == 14)
                {
                    if(attached)
                    {
                        BeamBeam();
                    } 
                    else
                    {
                        PewPew();
                    }
                }
            }
            if(!(npcSpecial || (team < 2 && Controls.controlSpecial[team])))
            {
                pressedSpecial = false;
            }
            if(platform != null && !Arena.entities.Contains(platform))
            {
                platform = null;
                attached = false;
                energy = 0;
            }
            mass = attached ? 36 : 6;
            ExtraHealthBoxes.Clear();
            ExtraHealths.Clear();
            if(platform != null)
            {
                ExtraHealthBoxes.Add(12);
                ExtraHealths.Add(platform.health);
            }
            if (attached && platform != null)
            {
                platform.position = position;
                platform.rotation = rotation;
                StunTime = platform.StunTime;
                ShipStats.GetStatsFor(ShipID.Escort, out _, out _, out _, out acceleration, out _, out turnSpeed, true);
                invulnerable = true; 
                platform.velocity = velocity;
                platform.position -= velocity;
                if(beam!=null)
                {
                    if(beam.Update(position+ Functions.PolarVector(10, rotation), rotation))
                    {
                        beam.ProcessCollision();
                    }
                    else
                    {
                        beam = null;
                    }
                }
            }
            else
            {
                beam = null;
                ShipStats.GetStatsFor(ShipID.Escort, out _, out _, out _, out acceleration, out _, out turnSpeed, false);
                invulnerable = false;
            }
            if (thrusting)
            {
                counter++;
                if (counter % 8 == 0)
                {
                    new Particle(position + Functions.PolarVector(0, rotation + (float)Math.PI / 2) + Functions.PolarVector(-3, rotation), 20, Color.Orange);
                }
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            if(beam != null)
            {
                beam.Draw(spriteBatch);
            }
            spriteBatch.Draw(AssetManager.ships[3], pos, null, Color.White, rotation, new Vector2(5.5f, 5.5f), Vector2.One, SpriteEffects.None, 0f);
        }
        bool AI_ShootingProj = false;
        public override void AI()
        {
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            if (enemyShip != null)
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                float toward = (enemyPos - position).ToRotation();
                if (attached)
                {
                    AI_cThrust();
                    if (enemyShip is Escort && team == 0)
                    {
                         AI_cThrust(true);
                    }
                    if ((enemyPos - position).Length() < BeamRange - 30)
                    {
                         AI_cThrust(true);
                    }
                    if ((enemyPos - position).Length() < BeamRange)
                    {
                        if (AI_TurnToward(toward))
                        {
                            AI_cShoot();
                        }
                    }
                    else
                    {
                        AI_TurnToward(toward);
                    }
                    int incomingDamage = 0;
                    List<Projectile> enemyProjectiles = EnemyProjectiles();
                    Line beamZone = new Line(position, position + Functions.PolarVector(BeamRange, rotation));
                    
                    for (int i = 0; i < enemyProjectiles.Count; i++)
                    {
                        if(enemyProjectiles[i].health == 2 || enemyProjectiles[i].health >= 0)
                        {
                            Shape[] hit = enemyProjectiles[i].AllHitboxes();
                            for (int k = 0; k < hit.Length; k++)
                            {
                                if (hit[k].Colliding(beamZone))
                                {
                                    AI_cShoot();
                                    break;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < enemyProjectiles.Count; i++)
                    {
                        if (AI_ImpendingCollisionAlly(platform, enemyProjectiles[i], Math.Min(Math.Min(20, enemyProjectiles[i].lifeTime), enemyProjectiles[i].lifeTime), out int frames))
                        {
                            float turnAmt = frames * (SlowTime > 0 ? 0.5f : 1f) *(turnSpeed / 15f * 2f * (float)Math.PI / 60f);
                            if (turnAmt > (float)Math.PI / 4f)
                            {
                                turnAmt = (float)Math.PI / 4f;
                            }
                            Polygon beamArea = new Polygon(new Vector2[]
                            {
                                Vector2.Zero,
                                new Vector2(Escort.BeamRange, Escort.BeamRange *(float)Math.Tan(turnAmt)),
                                new Vector2(Escort.BeamRange, Escort.BeamRange *(float)Math.Tan(-turnAmt)),
                            });
                            beamArea.Rotate(Vector2.Zero, rotation);
                            beamArea.Move(position);
                            bool canHit = false;
                            Shape[] projHitboxes = enemyProjectiles[i].AllHitboxes();
                            for (int j = 0; j < frames; j++)
                            {
                                for (int h = 0; h < 9; h++)
                                {
                                    if (projHitboxes[h].Colliding(beamArea))
                                    {
                                        canHit = true;
                                    }
                                    projHitboxes[h].Move(enemyProjectiles[i].velocity);
                                }
                                beamArea.Move(velocity);
                            }
                            if(enemyProjectiles[i].health == 2 && canHit)
                            {
                                AI_ShootingProj = true;
                                AI_TurnToward((Functions.screenLoopAdjust(position, enemyProjectiles[i].position) - position).ToRotation());
                                 AI_cThrust(true);
                            }
                            else
                            {
                                incomingDamage += enemyProjectiles[i].damage;
                                if(enemyProjectiles[i] is Tripwire)
                                {
                                    incomingDamage += 12;
                                }
                            }
                        }
                    }
                    if (incomingDamage >= platform.health)
                    {
                        AI_cSpecial();
                    }
                    if(enemyShip is Assassin)
                    {
                        if(AI_ImpendingBeamCollision(10))
                        {
                            AI_cSpecial();

                        }
                    }
                }
                else
                {
                    if(platform != null && !pressedSpecial)
                    {
                        Circle prox = new Circle(platform.position, 50);
                        if(AI_CollidingWithEnemy(prox))
                        {
                            AI_cSpecial();
                        }
                    }
                    AI_ShootingProj = false;
                    List<Projectile> enemyProjectiles = EnemyProjectiles();
                    for (int i = 0; i < enemyProjectiles.Count; i++)
                    {
                        Vector2 projPos = Functions.screenLoopAdjust(position, enemyProjectiles[i].position);
                        if(enemyProjectiles[i] is Mine )
                        {
                            if ((projPos - position).Length() < 140)
                            {
                                AI_ShootingProj = true;
                                float aimAt = Functions.PredictiveAim(position, 4, projPos, enemyProjectiles[i].velocity - velocity);
                                if (!float.IsNaN(aimAt))
                                {
                                    if (AI_TurnToward(aimAt))
                                    {
                                        AI_cShoot();
                                    }
                                }
                            }
                        }
                        else if (enemyProjectiles[i].health == 1)
                        {
                            //if (AI_CollidingWithEntitiy(enemyProjectiles[i], pHitArea))
                            if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(60, enemyProjectiles[i].lifeTime)))
                            {
                                AI_ShootingProj = true;
                                float aimAt = Functions.PredictiveAim(position, 4, projPos, enemyProjectiles[i].velocity - velocity);
                                if (!float.IsNaN(aimAt))
                                {
                                    if (AI_TurnToward(aimAt))
                                    {
                                        AI_cShoot();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(60, enemyProjectiles[i].lifeTime)))
                            {
                                AI_ShootingProj = true;
                                AI_cThrust();
                                AI_Dodge(enemyProjectiles[i]);
                            }
                        }
                    }
                    if (!AI_ShootingProj)
                    {
                        if(enemyShip is Trooper)
                        {
                            AI_AvoidFront(300);

                            if (energy == energyCapacity && !pressedSpecial)
                            {
                                if (AI_TurnToward((enemyPos - position).ToRotation()))
                                {
                                    AI_cThrust();
                                    if (Functions.AngularDifference(velocity.ToRotation(), (enemyPos - position).ToRotation()) < (float)Math.PI / 2f && velocity.Length() > 7 * 0.2f)
                                    {
                                        AI_cSpecial();
                                    }
                                }
                            }
                        }
                        else
                        {
                            AI_Kite(4, 140);
                            if (energy == energyCapacity && !pressedSpecial)
                            {
                                AI_cSpecial();
                            }
                        }
                    }
                }
            }
        }

    }
    public class Platform : Entity
    {
        EscortTurret[] turrets = new EscortTurret[2];
        public Escort parent;
        public Platform(Escort parent, int team)
        {
            this.parent = parent;
            mass = 30;
            health = 12;
            this.team = team;
            shape = new Polygon(new Vector2[]
             {
                new Vector2(9, -3),
                new Vector2(9, 3),
                new Vector2(-10, 11),
                 new Vector2(-10, -11),
             });
            turrets[0] = new EscortTurret(this, new Vector2(-5, 6));
            turrets[1] = new EscortTurret(this, new Vector2(-5, -6));
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.extraEntities[0], pos, null, Color.White, rotation, new Vector2(10.5f, 11.5f), Vector2.One, SpriteEffects.None, 0f);

            for (int i = 0; i < turrets.Length; i++)
            {
                turrets[i].Draw(spriteBatch, pos);
            }
        }
        public override void OnKill()
        {
            AssetManager.PlaySound(SoundID.Death);
            Debris.DebrisSet.CreatePlatformDebris(this);
        }
        float turretRange = 30 * 4.5f;
        public override void LocalUpdate()
        {
            Projectile alreadyTargetted = null;
            for (int i = 0; i < turrets.Length; i++)
            {
                turrets[i].UpdateRelativePosition();
                if (StunTime <= 0)
                {
                    if (DefensiveTargetting(turrets[i].AbsolutePosition(), turretRange, out Entity target, 1, delegate(Entity possibleTarget) { return possibleTarget != alreadyTargetted;}))
                    {
                        float aimAt = Functions.PredictiveAimWithOffset(turrets[i].AbsolutePosition(), 4.5f, Functions.screenLoopAdjust(turrets[i].AbsolutePosition(), target.position), target.velocity - velocity, 10f);
                        if (!float.IsNaN(aimAt))
                        {
                            if (turrets[i].AimAt(aimAt))
                            {
                                turrets[i].Fire();
                            }
                        }
                        if(target is Projectile)
                        {
                            alreadyTargetted = (Projectile)target;
                        }
                    }
                    else
                    {
                        turrets[i].AimHome();
                    }
                }
            }
        }
    }
    public class EscortTurret : Turret
    {
        public EscortTurret(Entity parent, Vector2 anchorAt, float homeRotation = 0) : base(parent, anchorAt, homeRotation)
        {
            origin = new Vector2(4.5f, 4.5f);
            texture = AssetManager.turrets[1];
            turretLength = 10f;
            rotSpeed = (float)Math.PI / 30f;
        }
        int shotCounter = 0;
        public override void Fire()
        {
            if(shotCounter <= 0)
            {
                Projectile proj = new GreenPulse(AbsoluteShootPosition(), Functions.PolarVector(5.5f, AbsoluteRotation()) + parent.velocity, parent.team);
                proj.rotation = AbsoluteRotation();
                AssetManager.PlaySound(SoundID.Pew);
                shotCounter = 24;
            }
        }
        public override void Update()
        {
            if(shotCounter > 0)
            {
                shotCounter--;
            }
        }
    }
}
