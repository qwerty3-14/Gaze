using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectGaze.Entities.Projectiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Entities.Ships
{
    public class Illusioner : Ship
    {
        public const int IllHealth = 4;
        public List<Illusion> illusions = new List<Illusion>();
        public Illusioner(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Illusioner;
            ShipStats.GetStatsFor(ShipID.Illusioner, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 1;

            mass = 8;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(7, -4),
                new Vector2(7, 4),
                new Vector2(-5, 4),
                new Vector2(-5, -4)
            });

            incorpreal = -1;
        }
        int shotCooldown;
        public override void Shoot()
        {
            if (!holdingSpecial && energy >= 2 && shotCooldown <= 0)
            {
                shotCooldown = 30;
                AssetManager.PlaySound(SoundID.Pew);
                energy -=2;
                Projectile pelt = new IllBolt(position, velocity + Functions.PolarVector(6.5f, rotation), team);
                pelt.rotation = rotation;
                for (int i = 0; i < illusions.Count; i++)
                {
                    illusions[i].Shoot();
                }
            }
        }
        bool holdingSpecial = false;
        public override void Special()
        {
            if (energy >= 6 && !holdingSpecial)
            {
                energy -= 6;
                for (int i = 0; i < illusions.Count; i++)
                {
                    illusions[i].Kill();
                }
                illusions.Clear();
                holdingSpecial = true;
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[4], pos, null, null, new Vector2(5.5f, 4.5f), rotation, Vector2.One, Color.White, 0, 0);
        }
        int counter;
        
        public override void LocalUpdate()
        {
            ShipStats.GetStatsFor(ShipID.Illusioner, out _, out _, out _, out acceleration, out _, out turnSpeed);
            if (!Controls.controlSpecial[team] && holdingSpecial)
            {
                holdingSpecial = false;
                Vector2[] positions = new Vector2[5];
                positions[0] = position;
                for (int i = 0; i < 4; i++)
                {
                    positions[i + 1] = position + Functions.PolarVector(50, (i / 4f) * 2f * (float)Math.PI);
                }
                int posIndex = 0;
                if (Controls.controlShoot[team])
                {
                    posIndex = 2;
                }
                if (Controls.controlRight[team])
                {
                    posIndex = 1;
                }
                if (Controls.controlLeft[team])
                {
                    posIndex = 3;
                }
                if (Controls.controlThrust[team])
                {
                    posIndex = 4;
                }
                position = positions[posIndex];
                for (int d = 0; d < 14; d++)
                {
                    float r = velocity.ToRotation() + (float)Math.PI * 2 * (float)Main.random.NextDouble() * (Main.random.Next(2) == 0 ? -1 : 1);
                    new Particle(position + Functions.PolarVector((float)Main.random.NextDouble() * 5f + 2, r), 30, Color.White, Functions.PolarVector((float)Main.random.NextDouble() * 0.2f + 0.3f, r));
                }
                AssetManager.PlaySound(SoundID.CreateIllusion);
                for (int k = 0; k < 4; k++)
                {
                    int i = k;
                    if (i >= posIndex)
                    {
                        i++;
                    }
                    illusions.Add(new Illusion(this, positions[i] - position, team));
                    for (int d = 0; d < 14; d++)
                    {
                        float r = velocity.ToRotation() + (float)Math.PI * 2 * (float)Main.random.NextDouble() * (Main.random.Next(2) == 0 ? -1 : 1);
                        new Particle(positions[i] + Functions.PolarVector((float)Main.random.NextDouble() * 5f + 2, r), 30, Color.White, Functions.PolarVector((float)Main.random.NextDouble() * 0.2f + 0.3f, r));
                    }
                }
            }
            else if (holdingSpecial)
            {
                acceleration = 0;
                turnSpeed = 0;
            }
            for (int i = 0; i < illusions.Count; i++)
            {
                if (!Main.entities.Contains(illusions[i]))
                {
                    illusions.Remove(illusions[i]);
                }
            }

            ExtraHealthBoxes.Clear();
            ExtraHealths.Clear();
            for (int i = 0; i < illusions.Count; i++)
            {
                ExtraHealthBoxes.Add(IllHealth);
                ExtraHealths.Add(illusions[i].health);
                if (illusions[i].StunTime <= 0)
                {
                    illusions[i].position = position + illusions[i].offset;
                    illusions[i].rotation = rotation;
                    illusions[i].velocity = velocity;
                    illusions[i].position -= velocity;
                }
            }
            if (shotCooldown > 0)
            {
                shotCooldown--;
            }
            if (thrusting)
            {
                counter++;
                if (counter % 5 == 0)
                {
                    new Particle(position + Functions.PolarVector(-5, rotation), 10, Color.Orange);
                    for (int i = 0; i < illusions.Count; i++)
                    {
                        new Particle(illusions[i].position + Functions.PolarVector(-5, rotation), 10, Color.Orange);
                    }
                }
            }

        }

        bool AI_ToMaxAngle = false;
        public override void AI()
        {
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            List<Projectile> enemyProjectiles = EnemyProjectiles();
            bool AI_Dodging = false;
            /*
            if(AI_ImpendingBeamCollision(20))
            {
                AI_Kite(6.5f, (6.5f * 30));
                AI_Dodging = true;
            }
            */
            if (holdingSpecial)
            {
                int posIndex = Main.random.Next(5);
                if (enemyShip != null)
                {
                    Vector2[] positions = new Vector2[5];
                    positions[0] = position;
                    float furthest = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        positions[i + 1] = position + Functions.PolarVector(50, (i / 4f) * 2f * (float)Math.PI);
                    }
                    for (int k = 0; k < 5; k++)
                    {
                        float dist = (positions[k] - Functions.screenLoopAdjust(positions[k], enemyShip.position)).Length();
                        if(AI_ToMaxAngle)
                        {
                            dist = Functions.AngularDifference((positions[k] - Functions.screenLoopAdjust(positions[k], enemyShip.position)).ToRotation(), enemyShip.rotation);
                        }
                        if(dist > furthest)
                        {
                            furthest = dist;
                            posIndex = k;
                        }
                    }
                }
                switch (posIndex)
                {
                    case 1:
                        Controls.controlRight[team] = true;
                        break;
                    case 2:
                        Controls.controlShoot[team] = true;
                        break;
                    case 3:
                        Controls.controlLeft[team] = true;
                        break;
                    case 4:
                        Controls.controlThrust[team] = true;
                        break;
                }
            }
            else
            {
                AI_ToMaxAngle = false;
                
                if (!AI_Dodging && enemyShip != null && AI_ImpendingCollision(enemyShip, 60))
                {
                    if (AI_ImpendingCollision(enemyShip, 10) && energy >= 6)
                    {
                        Controls.controlSpecial[team] = true;
                    }
                    else
                    {

                        Controls.controlThrust[team] = true;
                        AI_Dodge(enemyShip);
                    }
                    AI_Dodging = true;
                }
                if (!AI_Dodging && AI_ImpendingBeamCollision(10))
                {
                    if (energy >= 6)
                    {
                        Controls.controlSpecial[team] = true;
                        if(enemyShip is Strafer || enemyShip is Lancer)
                        {
                            AI_ToMaxAngle = true;
                        }
                        AI_Dodging = true;
                    }
                }
                if (!AI_Dodging)
                {
                    for (int i = 0; i < enemyProjectiles.Count; i++)
                    {
                        if (AI_ImpendingCollision(enemyProjectiles[i], Math.Min(10, enemyProjectiles[i].lifeTime)))
                        {
                            if (energy >= 6)
                            {
                                Controls.controlSpecial[team] = true;
                            }
                            else
                            {
                                Controls.controlThrust[team] = true;
                                AI_Dodge(enemyProjectiles[i]);
                            }
                            AI_Dodging = true;
                        }

                    }
                }
                if (!AI_Dodging && enemyShip != null)
                {
                    Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                    float toward = (enemyPos - position).ToRotation();

                    if ((enemyPos - position).Length() > (6.5f * 30))
                    {
                        Controls.controlThrust[team] = true;
                    }
                    if (illusions.Count > 1 && energy >= 8 && (enemyPos - position).Length() > (6.5f * 15))
                    {
                        float aimAt = Functions.PredictiveAim(position, 6.5f, enemyPos, enemyShip.velocity - velocity);
                        if (!float.IsNaN(aimAt))
                        {
                            if (AI_TurnToward(aimAt) && (enemyPos - position).Length() < (6.5f * 30))
                            {
                                Controls.controlShoot[team] = true;
                            }
                        }
                        else
                        {
                            AI_TurnToward(toward);
                        }
                    }
                    else
                    {
                        if (energy >= 8)
                        {
                            AI_Kite(6.5f, (6.5f * 30));
                        }
                        else
                        {
                            if(enemyShip is Lancer)
                            {
                                AI_AvoidFront(20);
                            }
                            else
                            {
                                AI_Retreat(enemyPos);
                            }
                        }
                    }
                }
                if (illusions.Count < 2 && energy >= 6)
                {
                    Controls.controlSpecial[team] = true;
                }
            }
        }
    }
    public class Illusion : Entity
    {
        Illusioner parent;
        public Vector2 offset;
        public Illusion(Illusioner parent, Vector2 offset, int team = 0)
        {
            this.team = team;
            this.parent = parent;
            health = Illusioner.IllHealth;
            //5.5 4.5
            shape = new Polygon(new Vector2[]
            {
                new Vector2(7, -4),
                new Vector2(7, 4),
                new Vector2(-5, 4),
                new Vector2(-5, -4)
            });
            this.offset = offset;
            incorpreal = -1;
        }
        public override void LocalUpdate()
        {
            if(!Main.entities.Contains(parent))
            {
                Kill();
            }
        }
        public void Shoot()
        {
            Projectile pelt = new IllBoltFAKE(position, velocity + Functions.PolarVector(6.5f, rotation), team);
            pelt.rotation = rotation;
        }
        public override void OnKill()
        {
            for (int i = 0; i < 14; i++)
            {
                float r = velocity.ToRotation() + (float)Math.PI * 2 * (float)Main.random.NextDouble() * (Main.random.Next(2) == 0 ? -1 : 1);
                new Particle(position + Functions.PolarVector((float)Main.random.NextDouble() * 5f + 2, r), 30, Color.White, Functions.PolarVector((float)Main.random.NextDouble() * 0.2f + 0.3f, r));
            }
            AssetManager.PlaySound(SoundID.IllusionDown);
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[4], pos, null, null, new Vector2(5.5f, 4.5f), rotation, Vector2.One, Color.White, 0, 0);
            //Hitbox().Draw(spriteBatch, Color.Green);
        }
    }
}
