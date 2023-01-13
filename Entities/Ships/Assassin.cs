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
    public class Assassin : Ship
    {
        public const int BeamRange = 30;
        Beam[] beams = null;
        public Assassin(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Assassin;
            ShipStats.GetStatsFor(ShipID.Assassin, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed, true);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(6, -5),
                new Vector2(6, 5),
                new Vector2(-6, 7),
                new Vector2(-6, -7)
            });
            mass = 4;
            noHurtOnCollision = true;
        }
        public override void Shoot()
        {
            if(beams == null && energy >= 3)
            {
                AssetManager.PlaySound(SoundID.Beam);
                float locRange = (new Vector2(BeamRange, 5)).Length();
                energy -= 3;
                beams = new Beam[]
                {
                    new Beam(this, Color.Crimson, locRange, 15, -1, 2, 1.1f),
                    new Beam(this, Color.Crimson, locRange, 15, -1, 2, 1.1f)
                };
            }
        }
        public TwistedWormhole wormhole;
        bool pressedSpecial = false;
        public override void Special()
        {
            if (wormhole != null)
            {
                if (wormhole.lifeTime < 2)
                {
                    wormhole.lifeTime++;
                }
            }
            else if (energy >= 6 && !pressedSpecial)
            {
                energy -= 6;
                wormhole = new TwistedWormhole(position, Functions.PolarVector(5f, rotation) + velocity, team, this);
            }
            pressedSpecial = true;
        }
        public override void LocalUpdate()
        {
            if(thrusting && instaAcc)
            {
                instaAcc = false;
            }
            if(!(npcSpecial || (team < 2 && Controls.controlSpecial[team])))
            {
                pressedSpecial = false;
            }
            if (wormhole != null && wormhole.lifeTime == 1)
            {
                for (int i = 0; i < 8; i++)
                {
                    float dir = (float)Main.random.NextDouble() * 2f * (float)Math.PI;
                    new Particle(position, 5, Main.WarpPink, Functions.PolarVector(-4, dir));
                }
                position = wormhole.position;
                velocity = Vector2.Zero;
                if (GetEnemy() != null)
                {
                    float distToEnemy = (Functions.screenLoopAdjust(position, GetEnemy().position) - position).Length();
                    float wormDistToEnemy = (Functions.screenLoopAdjust(wormhole.position, GetEnemy().position) - wormhole.position).Length();
                    rotation = (Functions.screenLoopAdjust(position, GetEnemy().position) - position).ToRotation();
                    if (wormDistToEnemy > distToEnemy)
                    {
                        rotation += (float)Math.PI;
                    }
                }
                AssetManager.PlaySound(SoundID.Warp);
                for (int i = 0; i < 8; i++)
                {
                    float dir = (float)Main.random.NextDouble() * 2f * (float)Math.PI;
                    new Particle(position + Functions.PolarVector(20, dir), 5, Main.WarpPink, Functions.PolarVector(-4, dir));
                }
                instaAcc = true;
            }
            if(wormhole != null && !Arena.entities.Contains(wormhole))
            {
                wormhole = null;
            }
            if (thrusting)
            {
                new Particle(position + Functions.PolarVector(-3, rotation), 10, Color.Red);
            }
            if (beams != null)
            {
                float length = 7;
                Vector2 aimAt = position + Functions.PolarVector(length + BeamRange, rotation);
                for (int i = 0; i < 2; i++)
                {
                    if (beams[i] != null)
                    {
                        Vector2 start = position + Functions.PolarVector(length, rotation) + Functions.PolarVector(5 * (i == 0 ? 1 : -1), (float)Math.PI / 2f + rotation);
                        if (beams[i].Update(start, (aimAt - start).ToRotation()))
                        {
                            beams[i].ProcessCollision();
                        }
                        else
                        {
                            beams[i] = null;
                        }
                    }
                }
                if(beams[0] == null && beams[1] == null)
                {
                    beams = null;
                }
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[5], pos, null, Color.White, rotation, new Vector2(6.5f, 8.5f), Vector2.One, SpriteEffects.None, 0f);
            if(beams != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (beams[i] != null)
                    {
                        beams[i].Draw(spriteBatch);
                    }
                }
            }
        }
        public override void AI()
        {
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            List<Projectile> enemyProjectiles = EnemyProjectiles();
            bool AI_Dodging = false;
            if(wormhole != null)
            {
                AI_cSpecial();

                if (!AI_Dodging && AI_ImpendingBeamCollision(10))
                {
                    AI_cSpecial(true);
                    AI_Dodging = true;

                }
            }

            float length = 7;
            Polygon hitArea = new Polygon(new Vector2[]
            {
                position + Functions.PolarVector(length + BeamRange, rotation),
                position + Functions.PolarVector(length, rotation) + Functions.PolarVector(5, (float)Math.PI / 2f + rotation),
                position + Functions.PolarVector(length, rotation) + Functions.PolarVector(-5, (float)Math.PI / 2f + rotation)
            });
            if(AI_CollidingWithEnemy(hitArea))
            {
                AI_cShoot();
            }
            if (enemyShip == null || (enemyShip != null && (Functions.screenLoopAdjust(position, enemyShip.position) - position).Length() > 100))
            {
                for (int i = 0; i < enemyProjectiles.Count(); i++)
                {
                    if (energy >= 3 && (enemyProjectiles[i].health == 1 || enemyProjectiles[i].health == 2) && AI_CollidingWithEntitiy(enemyProjectiles[i], hitArea))
                    {
                        AI_cShoot();
                    }
                    else if (AI_ImpendingCollision(enemyProjectiles[i], 5) && wormhole != null)
                    {
                        AI_cSpecial(true);
                        AI_Dodging = true;
                    }
                    else if (AI_ImpendingCollision(enemyProjectiles[i], 30))
                    {
                        AI_cThrust();
                        AI_Dodge(enemyProjectiles[i]);
                        AI_Dodging = true;
                    }
                }
            }
            if (!AI_Dodging && enemyShip != null)
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                float toward = (enemyPos - position).ToRotation();
                if ((enemyPos - position).Length() > 200 && wormhole == null)
                {
                    if (energy >= 18)
                    {
                        float aimAt = Functions.PredictiveAim(position, 5f, enemyPos, enemyShip.velocity - velocity);
                        if (!float.IsNaN(aimAt))
                        {
                            if (AI_TurnToward(aimAt))
                            {
                                AI_cSpecial();
                            }
                        }
                        else
                        {
                            AI_cSpecial(true);
                        }
                    }
                }
                else
                {
                    AI_cThrust();

                    if (energy > 6)
                    {
                        if (Functions.AngularDifference((enemyPos - position).ToRotation(), enemyShip.rotation) < (float)Math.PI / 4f)
                        {
                            AI_TurnToward(toward);
                        }
                        else
                        {
                            if ((enemyPos - position).Length() < 100)
                            {
                                AI_TurnToward(toward);
                            }
                            else
                            {
                                AI_AvoidFront(80);
                            }
                            if (wormhole == null && !(enemyShip is Eagle))
                            {
                                AI_cSpecial();
                            }
                            
                        }
                        if (!(enemyShip is Strafer) && wormhole != null && ((Functions.AngularDifference((Functions.screenLoopAdjust(wormhole.position, enemyShip.position) - wormhole.position).ToRotation(), enemyShip.rotation) < (float)Math.PI / 4f)
                                || ((Functions.screenLoopAdjust(wormhole.position, enemyShip.position) - wormhole.position).Length() < BeamRange * 5 && (Functions.screenLoopAdjust(wormhole.position, enemyShip.position) - wormhole.position).Length() < (Functions.screenLoopAdjust(wormhole.position + wormhole.velocity, enemyShip.position) - (wormhole.position + wormhole.velocity)).Length())))
                        {
                            AI_cSpecial(true);
                        }
                    }
                    else
                    {
                        AI_TurnToward((position - enemyPos).ToRotation());
                    }
                }
            }
        }
    }
}
