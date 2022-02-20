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
    public class Buccaneer : Ship
    {
        Sword sword;
        Grapple grapple;
        public Buccaneer(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Buccaneer;
            ShipStats.GetStatsFor(ShipID.Buccaneer, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 1;

            mass = 16;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(4, 2),
                new Vector2(4, -2),
                new Vector2(-5.5f, -8f),
                new Vector2(-5.5f, 8f),
            });

            sword = new Sword(this, Vector2.Zero, (float)Math.PI / 2 - (float)Math.PI / 16f);

        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {

            if (grapple != null && !grapple.dontDrawChain)
            {
                Vector2 fakePos = grapple.position;
                float length = (Functions.screenLoopAdjust(position, fakePos) - position).Length();
                float rot = (Functions.screenLoopAdjust(position, fakePos) - position).ToRotation();
                for (float l = 0; l < length; l += 2)
                {
                    spriteBatch.Draw(AssetManager.projectiles[16], pos + Functions.PolarVector(l, rot), null, null, new Vector2(0, 1f), rot, Vector2.One, Color.White, 0, 0);
                }


            }
            if (sword != null)
            {
                sword.Draw(spriteBatch, pos);
            }
            spriteBatch.Draw(AssetManager.ships[11], pos, null, null, new Vector2(5.5f, 8f), rotation, Vector2.One, Color.White, 0, 0);
        }
        int shotCooldown = 0;
        public override void Shoot()
        {
            if(shotCooldown <= 0 && energy > 0)
            {
                energy--;
                shotCooldown = 20;
                sword.Fire();
            }
        }
        bool justLaunched = false;
        public override void Special()
        {
            if (grapple == null)
            {
                if (energy > 2)
                {
                    justLaunched = true;
                    energy -= 2;
                    grapple = new Grapple(this, position, velocity + Functions.PolarVector(8f, rotation), team);
                }
            }
            else if(!justLaunched)
            {
                grapple.Pull();
            }
        }
        int counter = 0;
        public override void LocalUpdate()
        {
            if(!Controls.controlSpecial[team])
            {
                justLaunched = false;
            }
            if(grapple != null)
            {
                grapple.lifeTime = 2;
                if (!Main.entities.Contains(grapple))
                {
                    grapple = null;
                }
            }
            if(shotCooldown > 0)
            {
                shotCooldown--;
                Polygon projClearArea = new Polygon(new Vector2[]
                {
                    position,
                    position + Functions.PolarVector(28, sword.AbsoluteRotation() - (float)Math.PI / 4f),
                    position + Functions.PolarVector(28, sword.AbsoluteRotation() + (float)Math.PI / 4f),
                });

                List<Projectile> enemyProjectiles = EnemyProjectiles();
                for (int i = 0; i < enemyProjectiles.Count(); i++)
                {
                    if(AI_CollidingWithEntitiy(enemyProjectiles[i], projClearArea) && enemyProjectiles[i].health <= 4 && enemyProjectiles[i].health >= 0)
                    {
                        enemyProjectiles[i].Kill();
                    }
                }
            }
            counter++;
            if (thrusting)
            {
                if (counter % 14 == 0)
                {
                    new Particle(position + Functions.PolarVector(-5, rotation) + Functions.PolarVector(-4.5f, rotation + (float)Math.PI / 2f), 10, Color.Orange);
                    new Particle(position + Functions.PolarVector(-5, rotation) + Functions.PolarVector(4.5f, rotation + (float)Math.PI / 2f), 10, Color.Orange);
                }
            }
            if(sword != null)
            {
                sword.UpdateRelativePosition();
                sword.AimHome();
            }
        }
        bool AI_Dodging = false;
        bool batteryDied = false;
        public override void AI()
        {
            AI_Dodging = false;
            AI_ResetControls();
            if(energy == 0)
            {
                batteryDied = true;
            }
            Entity enemyShip = GetEnemy();
            if (grapple != null)
            {
                if (grapple.IsStuck())
                {
                    enemyShip = grapple.StuckTo();
                }
            }
            Polygon swordArea = new Polygon(new Vector2[]
            {
                    position,
                    position + Functions.PolarVector(28, rotation + (float)Math.PI / 2 - (float)Math.PI / 16f),
                    position + Functions.PolarVector(28, rotation),
                    position + Functions.PolarVector(28, rotation + -(float)Math.PI / 2 + (float)Math.PI / 16f),
            });
            
            List <Projectile> enemyProjectiles = EnemyProjectiles();
            for (int i = 0; i < enemyProjectiles.Count(); i++)
            {
                if( enemyProjectiles[i].health <= 4 && enemyProjectiles[i].health >= 0)
                {
                    if (AI_CollidingWithEntitiy(enemyProjectiles[i], swordArea))
                    {
                        Controls.controlShoot[team] = true;
                    }
                }
                else if(!AI_Dodging)
                {
                    if(AI_ImpendingCollision(enemyProjectiles[i], 60))
                    {
                        AI_Dodging = true;
                        AI_Dodge(enemyProjectiles[i]);
                        Controls.controlThrust[team] = true;
                    }
                }
            }
            if(!AI_Dodging)
            {
                if (enemyShip != null)
                {
                    Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                    float dist = (enemyPos - position).Length();
                    if (dist < 70)
                    {
                        AI_TurnToward((enemyPos - position).ToRotation());
                        for (int i = 0; i < Main.entities.Count; i++)
                        {
                            if (Main.entities[i].team != team && !(Main.entities[i] is Projectile))
                            {
                                if (AI_CollidingWithEntitiy(Main.entities[i], swordArea))
                                {
                                    Controls.controlShoot[team] = true;
                                }
                            }
                            AI_DefaultFlight(enemyShip, enemyPos);
                            if (!batteryDied)
                            {
                                Controls.controlThrust[team] = false;
                            }
                        }
                    }
                    else
                    {
                        if (grapple != null)
                        {
                            if (grapple.IsFlying())
                            {
                                if (dist < (Functions.screenLoopAdjust(position, grapple.position) - position).Length())
                                {
                                    Controls.controlSpecial[team] = true;
                                }
                            }
                            AI_DefaultFlight(enemyShip, enemyPos);
                        }
                        else
                        {
                            if (batteryDied)
                            {
                                AI_DefaultFlight(enemyShip, enemyPos);
                            }
                            else
                            {
                                float aimToward = Functions.PredictiveAim(position, 8, enemyPos, enemyShip.velocity - velocity);
                                if (!float.IsNaN(aimToward))
                                {
                                    if (AI_TurnToward(aimToward))
                                    {
                                        Controls.controlSpecial[team] = true;
                                    }
                                }
                                else
                                {
                                    AI_DefaultFlight(enemyShip, enemyPos);
                                }
                            }
                        }
                    }
                }
            }
        }
        void AI_DefaultFlight(Entity enemyShip, Vector2 enemyPos)
        {
            if (batteryDied)
            {
                float toward = (enemyPos - position).ToRotation();
                if (velocity.Length() < (maxSpeed * 0.2f) - 0.1f || Functions.AngularDifference(velocity.ToRotation(), (position - enemyPos).ToRotation()) > 3 * (float)Math.PI / 4f)
                {
                    AI_Retreat(enemyPos);
                }
                else
                {
                    if(DefensiveTargetting(position, 100, out Entity threat, 4))
                    {
                        AI_TurnToward((Functions.screenLoopAdjust(position, threat.position) - position).ToRotation());
                    }
                }
                if (energy == energyCapacity)
                {
                    batteryDied = false;
                }
            }
            else
            {
                if (grapple != null)
                {
                    if (grapple.IsStuck())
                    {
                        Controls.controlSpecial[team] = true;
                    }
                }
                float s = (float)maxSpeed * 0.2f;
                float aimToward = Functions.PredictiveAim(position, s, enemyPos, enemyShip.velocity - velocity);
                if (float.IsNaN(aimToward))
                {
                    aimToward = (enemyPos - position).ToRotation();
                }
                AI_TurnToward(aimToward);
                Controls.controlThrust[team] = true;
            }

        }
    }
    public class Sword : Turret
    {
        public Sword(Entity parent, Vector2 anchorAt, float homeRotation = 0) : base(parent, anchorAt, homeRotation)
        {

            this.parent = parent;
            origin = new Vector2(2.5f, 4f);
            texture = AssetManager.turrets[3];
            turretLength = 7f;
            rotSpeed = (float)Math.PI / 18f;
        }
        int timer = 0;
        Beam beam;
        bool rightSide = true;
        public override void Update()
        {
            timer++;
            if (beam != null)
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
            timer = 0;
            beam = new Beam(parent, Color.White, 20, 12, -1, 4);
            beam.Update(AbsoluteShootPosition(), AbsoluteRotation());
            AssetManager.PlaySound(SoundID.Pew2);
            if(rightSide)
            {
                homeRotation = -(float)Math.PI / 2 + (float)Math.PI / 16f;
                rightSide = false;
            }
            else
            {
                homeRotation = (float)Math.PI / 2 - (float)Math.PI / 16f;
                rightSide = true;
            }

        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 Pos)
        {
            spriteBatch.Draw(texture, Pos + relativePosition, null, Color.White, AbsoluteRotation(), origin, new Vector2(1, 1), SpriteEffects.None, 0);
            

        }
    }
}
