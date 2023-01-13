using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GazeOGL;
using GazeOGL.Entities.Projectiles;
using GazeOGL.Entities.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities.Ships
{
    public class Frigate : Ship
    {
        public Frigate(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Frigate;
            ShipStats.GetStatsFor(type, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 1;

            mass = 20;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(10, 2),
                new Vector2(5, 6),
                new Vector2(1, 6),
                new Vector2(-12, 6),
                new Vector2(-15, 3),
                new Vector2(-15, -3),
                new Vector2(-12, -6),
                new Vector2(1, -6),
                new Vector2(5, -6),
                new Vector2(10, -2),
            });
        }
        Capsule capsule;
        bool pressedShoot = false;
        public override void Shoot()
        {
            if (capsule != null)
            {
                if (capsule.lifeTime < 2)
                {
                    capsule.lifeTime++;
                }
                if(!Arena.entities.Contains(capsule))
                {
                    capsule = null;
                }
            }
            else if (energy >= 8 && !pressedShoot)
            {
                AssetManager.PlaySound(SoundID.IllusionDown, -0.5f);
                energy -= 8;
                capsule = new Capsule(position, Functions.PolarVector(5f, rotation) + velocity, team);
                capsule.rotation = rotation;
            }
            pressedShoot = true;
        }
        Emitter emitter;
        int specialCooldown = 0;
        public override void Special()
        {
            if (energy >= 8 && specialCooldown <= 0)
            {
                if(emitter !=null)
                {
                    emitter.Kill();
                }
                specialCooldown = 120;
                AssetManager.PlaySound(SoundID.SmallExplosion, 0.4f);
                emitter = new Emitter(position, velocity + Functions.PolarVector(-5f, rotation), team);
                energy -= 12;
            }
        }
        int counter;
        public override void LocalUpdate()
        {
            ExtraHealthBoxes = new List<int>();
            ExtraHealths = new List<int>();
            if(emitter!=null)
            {
                if (!Arena.entities.Contains(emitter))
                {
                    emitter = null;
                }
                else
                {
                    ExtraHealthBoxes.Add(6);
                    ExtraHealths.Add(emitter.health);
                }
            }
            if (!(npcShoot || (team < 2 && Controls.controlShoot[team])))
            {
                pressedShoot = false;
            }
            if(specialCooldown > 0)
            {
                specialCooldown--;
            }
            if (capsule != null && !Arena.entities.Contains(capsule))
            {
                capsule = null;
            }
            if (thrusting)
            {
                counter++;
                if (counter % 8 == 0)
                {
                    new Particle(position + Functions.PolarVector(1, rotation + (float)Math.PI / 2) + Functions.PolarVector(-14, rotation), 14, Color.Orange);
                }
                if (counter % 8 == 4)
                {
                    new Particle(position + Functions.PolarVector(-1, rotation + (float)Math.PI / 2) + Functions.PolarVector(-14, rotation), 14, Color.Orange);
                }
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[15], pos, null, Color.White, rotation, new Vector2(15.5f, 6.5f), Vector2.One, SpriteEffects.None, 0f);
        }
        public override void AI()
        {
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            bool AI_Dodging = false;
            List<Projectile> enemyProj = EnemyProjectiles();
            for(int i = 0; i < enemyProj.Count; i++)
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
            if (enemyShip != null && !AI_Dodging)
            {
                {
                    if ((enemyShip is Ship && ((Ship)enemyShip).maxSpeed < maxSpeed))
                    {
                        if (emitter == null && (Functions.screenLoopAdjust(position, enemyShip.position) - position).Length() < 180)
                        {
                            AI_cSpecial();
                        }
                        else
                        {
                            AI_Kite(7, 800);
                        }
                    }
                    else
                    {
                        if(emitter != null)
                        {
                            Vector2 emitterPos = Functions.screenLoopAdjust(position, emitter.position);
                            Vector2 diff = emitterPos - position;
                            if(diff.Length() > LingeringExplosion.radiusBig && Functions.AngularDifference(velocity.ToRotation(), diff.ToRotation()) > (float)Math.PI / 8f)
                            {
                                if(AI_TurnToward(diff.ToRotation()))
                                {
                                    AI_cThrust();
                                }
                            }
                            else if (AI_AimAtEnemy(5))
                            {
                                AI_cShoot();
                            }
                        }
                        else 
                        {
                            AI_cSpecial();
                        }
                    }
                    if (pressedShoot)
                    {
                        AI_cShoot(true);
                    }
                }
                if (capsule != null)
                {
                    AI_cShoot();
                    Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                    Vector2 capsulePos = Functions.screenLoopAdjust(position, capsule.position);
                    if ((enemyPos - position).Length() < (capsulePos - position).Length())
                    {
                        AI_cShoot(true);
                    }
                    List<Projectile> enemyProjectiles = EnemyProjectiles();
                    Circle circle = new Circle(capsulePos, LingeringExplosion.radius);
                    for (int i = 0; i < enemyProjectiles.Count; i++)
                    {
                        if (enemyProjectiles[i].health == 1 && AI_CollidingWithEntitiy(enemyProjectiles[i], circle))
                        {
                            AI_cShoot(true);
                            break;
                        }
                    }
                }
            }
        }
    }
    public class Emitter : Entity
    {
        LingeringExplosion lingeringExplosion;
        public Emitter(Vector2 position, Vector2 velocity, int team)
        {
            this.team = team;
            health = 6;
            this.velocity = velocity;
            this.position = position;
            shape = new Polygon(new Vector2[]
             {
                new Vector2(4, 1),
                new Vector2(1, 4),
                new Vector2(-1, 4),
                new Vector2(-4, 1),
                new Vector2(-4, -1),
                new Vector2(-1, -4),
                new Vector2(1, -4),
                new Vector2(4, -1),
             });
            mass = 10;
            lingeringExplosion = new LingeringExplosion(position, Vector2.Zero, team, true);
        }
        int lifeTimeCounter = 0;
        public override void LocalUpdate()
        {
            rotation += (float)Math.PI / 30f;
            velocity *= 0.92f;
            lifeTimeCounter++;
            if (lifeTimeCounter == 60 * 2)
            {
                AssetManager.PlaySound(SoundID.MissileLaunch, -0.5f);
            }
            if (lifeTimeCounter > 60 * 10)
            {
                Kill();
            }
            lingeringExplosion.position = position;
        }
        public override void OnKill()
        {
            if(lingeringExplosion != null)
            {
                lingeringExplosion.Kill();
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.extraEntities[6], pos, null, Color.White, rotation, new Vector2(4f, 4f), Vector2.One, SpriteEffects.None, 0f);
        }
    }
}
