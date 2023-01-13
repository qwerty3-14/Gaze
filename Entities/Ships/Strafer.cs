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
    class Strafer : Ship
    {
        StraferTurret[] turrets;
        Beam[] beams = new Beam[4];
        public const int BeamRange = 50;
        public Strafer(Vector2 position, int team = 0) : base(position, team)
        {
            mass = 4;
            type = ShipID.Strafer;
            ShipStats.GetStatsFor(ShipID.Strafer, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;
            energyRate = 1;
            shape = new Polygon(new Vector2[]
             {
                new Vector2(7, 0),
                new Vector2(-4, 5),
                new Vector2(-7, 5),
                new Vector2(-3, 0),
                new Vector2(-7, -5),
                new Vector2(-4, -5),
             });
            turrets = new StraferTurret[] 
            {
                new StraferTurret(this, new Vector2(2, -2)),
                new StraferTurret(this, new Vector2(-1, -3), -(float)Math.PI / 8f),
                new StraferTurret(this, new Vector2(2, 2)),
                new StraferTurret(this, new Vector2(-1, 3), (float)Math.PI / 8f)
            };
        }
        int counter;
        int shotCooldown;
        public override void Shoot()
        {
            if(energy >= 3 && shotCooldown <= 0)
            {
                AssetManager.PlaySound(SoundID.Pew2);
                energy -= 3;
                shotCooldown = 20;
                for(int i =0; i < 4; i++)
                {
                    beams[i] = new Beam(this, Color.DarkCyan, BeamRange, 10, -1, 1, 0.8f);
                }
            }
        }
        int EMPCooldown;
        public override void Special()
        {
            if (energy >= 8 && EMPCooldown <= 0)
            {
                EMPCooldown = 60;
                AssetManager.PlaySound(SoundID.Pew);
                energy -=6;
                AimAssist(6f, 2);
                Projectile pelt = new EMP(position + Functions.PolarVector(2, rotation), velocity + Functions.PolarVector(6, rotation), team);
                pelt.rotation = rotation;
            }
        }
        public override void LocalUpdate()
        {
            if (turrets.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    turrets[i].UpdateRelativePosition();
                    int g = 1;
                    if (i < 2)
                    {
                        g = -1;
                    }
                    bool front = i % 2 == 0;
                    Entity target = null;
                    if (front)
                    {
                        //prioritise enemy ship
                        AggressiveTargetting(turrets[i].AbsolutePosition(), 150, out target, 1, delegate (Entity possibleTarget)
                        {
                            Vector2 posPosition = Functions.screenLoopAdjust(turrets[i].AbsolutePosition(), possibleTarget.position);
                            float towardPos = (posPosition - turrets[i].AbsolutePosition()).ToRotation();
                            return Functions.AngularDifference(towardPos, g * (float)Math.PI / 3f + rotation) < (float)Math.PI / 3f;
                        });

                    }
                    else
                    {
                        //prioritise projectiles
                        DefensiveTargetting(turrets[i].AbsolutePosition(), 150, out target, 1, delegate (Entity possibleTarget)
                        {
                            Vector2 posPosition = Functions.screenLoopAdjust(turrets[i].AbsolutePosition(), possibleTarget.position);
                            float towardPos = (posPosition - turrets[i].AbsolutePosition()).ToRotation();
                            return Functions.AngularDifference(towardPos, g * (float)Math.PI / 3f + rotation) < (float)Math.PI / 3f;
                        });
                    }
                    if (target != null)
                    {
                        Vector2 enemyPos = Functions.screenLoopAdjust(turrets[i].AbsolutePosition(), target.position);
                        turrets[i].AimAt(enemyPos);
                    }
                    else
                    {
                        turrets[i].AimHome();
                    }
                }
            }
            if (shotCooldown > 0)
            {
                shotCooldown--;
            }
            if (EMPCooldown > 0)
            {
                EMPCooldown--;
            }
            if (thrusting)
            {
                counter++;
                if (counter % 2 == 0)
                {
                    new Particle(position + Functions.PolarVector(0, rotation + (float)Math.PI / 2) + Functions.PolarVector(-3, rotation), 14, Color.Orange);
                }
            }
            for(int i = 0; i < 4; i++)
            {
                if(beams[i] != null && turrets[i] != null)
                {
                    if (beams[i].Update(turrets[i].AbsoluteShootPosition(), turrets[i].AbsoluteRotation()))
                    {
                        beams[i].ProcessCollision();
                    }
                    else
                    {
                        beams[i] = null;
                    }
                }
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[2], pos, null, Color.White, rotation, new Vector2(7.5f, 5.5f), Vector2.One, SpriteEffects.None, 0f);
            for (int i = 0; i < 4; i++)
            {
                if (beams[i] != null)
                {
                    beams[i].Draw(spriteBatch);
                }
            }
            for (int i = 0; i < turrets.Length; i++)
            {
                turrets[i].Draw(spriteBatch, pos);
            }
        }
        bool AI_CheckForEMP()
        {
            foreach(Entity entity in Arena.entities)
            {
                if(entity.team == team && entity is EMP)
                {
                    return true;
                }
            }
            return false;
        }
        bool AI_Dodging = false;
        public override void AI()
        {
            AI_Dodging = false;
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            AI_cThrust();
            Polygon hitArea = new Polygon(new Vector2[]
                    {
                    position + Functions.PolarVector(BeamRange + 10, rotation),
                    position + Functions.PolarVector(BeamRange + 10, rotation + 2*(float)Math.PI/3f),
                    position + Functions.PolarVector(BeamRange + 10, rotation - 2*(float)Math.PI/3f),
                    });
            if (enemyShip != null)
            {
                List<Projectile> enemyProjectiles = EnemyProjectiles();
                if(AI_ImpendingCollision(enemyShip, 60))
                {
                    AI_Dodge(enemyShip);
                    AI_Dodging = true;
                }
                if (!AI_Dodging)
                {
                    for (int i = 0; i < enemyProjectiles.Count; i++)
                    {
                        if (enemyProjectiles[i].health == 1)
                        {
                            if (AI_CollidingWithEntitiy(enemyProjectiles[i], hitArea))
                            {
                                AI_cShoot();
                            }
                        }
                        else if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(60, enemyProjectiles[i].lifeTime)))
                        {
                            AI_Dodge(enemyProjectiles[i]);
                            AI_Dodging = true;
                        }
                    }
                }
                if (!AI_Dodging)
                {

                    Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                    if (Functions.AngularDifference((enemyPos - position).ToRotation(), rotation) > (float)Math.PI / 4f && SlowTime <= 0)
                    {

                        Vector2[] offsets = Functions.OffsetsForDrawing();
                        Vector2 newEnemyPos = enemyPos;
                        float minAngle = Functions.AngularDifference((newEnemyPos - position).ToRotation(), rotation);
                        for (int i = 1; i < offsets.Length; i++)
                        {
                            float angle = Functions.AngularDifference(((enemyPos + offsets[i]) - position).ToRotation(), rotation);
                            if (angle < minAngle)
                            {
                                minAngle = angle;
                                newEnemyPos = enemyPos + offsets[i];
                            }
                        }

                        float aimAt = (newEnemyPos - position).ToRotation();
                        AI_TurnToward(aimAt);
                    }
                    else
                    {
                        if(SlowTime >0)
                        {
                            AI_cThrust(true);
                        }
                        if((enemyShip is Ship) && ((((Ship)enemyShip).energy > 0) || ((Ship)enemyShip).EMPTime < 30))
                        {
                            float aimAt = Functions.PredictiveAimWithOffset(position, 6, enemyPos, enemyShip.velocity - velocity, 2);
                            if (!float.IsNaN(aimAt))
                            {
                                if (AI_TurnToward(aimAt) && energy == energyCapacity && !AI_CheckForEMP())
                                {
                                    AI_cSpecial();
                                }
                            }
                        }
                        else
                        {
                            float aimAt = (enemyPos - position).ToRotation() + (enemyShip is Eagle ? 0 : (float)Math.PI/16f);
                            AI_TurnToward(aimAt);
                            /*
                            Vector2 rightSpot = enemyPos + Functions.PolarVector(40, aimAt + (float)Math.PI / 2);
                            Vector2 leftSpot = enemyPos + Functions.PolarVector(40, aimAt - (float)Math.PI / 2);
                            Vector2 flyTo = leftSpot;
                            if (Functions.AngularDifference((rightSpot - position).ToRotation(), rotation) < Functions.AngularDifference((leftSpot - position).ToRotation(), rotation))
                            {
                                flyTo = rightSpot;
                            }
                            float targetRot = (flyTo - position).ToRotation();
                            AI_TurnToward(targetRot);
                            */
                        }
                    }
                }
                if (AI_CollidingWithEnemy(hitArea))
                {
                    AI_cShoot();
                }
            }
        }
    }
    class StraferTurret : Turret
    {
        public StraferTurret(Entity parent, Vector2 anchorAt, float homeRotation = 0) : base(parent, anchorAt, homeRotation)
        {
            origin = new Vector2(1.5f, 1.5f);
            texture = AssetManager.turrets[0];
            turretLength = 2.5f; 
            rotSpeed = (float)Math.PI / 15f;
        }
    }
}
