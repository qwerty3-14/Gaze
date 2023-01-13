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
    public class Lancer : Ship
    {
        Beam beam;
        public const float BeamRange = 150;
        public Lancer(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Lancer;
            ShipStats.GetStatsFor(type, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 4;

            mass = 8;
            //14, 6
            shape = new Polygon(new Vector2[]
            {
                new Vector2(8, 2),
                new Vector2(-14, 6),
                new Vector2(-14, -6),
                new Vector2(8, -2),
            });
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            if (beam != null)
            {
                beam.Draw(spriteBatch);
            }
            spriteBatch.Draw(AssetManager.ships[13], pos, null, Color.White, rotation, new Vector2(14f, 6f), Vector2.One, SpriteEffects.None, 0f);
        }
        public int beamCharge = 0;
        public override void Shoot()
        {
            if (beam == null)
            {
                if (beamCharge == 0 && energy >= 4)
                {
                    energy -= 4;
                    beamCharge++;
                }
                else
                {
                    EMPTime++; 
                    if (beamCharge >= 30 * 8)
                    {
                        beamCharge = 30 * 8;
                    }
                    if (beamCharge % 3 == 0)
                    {
                        new Particle(position + Functions.PolarVector(11, rotation), 10, getBeamColor(), velocity + Functions.PolarVector(.75f, rotation + (float)Main.random.NextDouble() * (float)Math.PI / 2f - (float)Math.PI / 4f));
                    }
                    beamCharge++;
                }
            }
            base.Shoot();
        }
        int dashCooldown = 0;
        public override void Special()
        {
            if (energy >= 4 && dashCooldown <= 0)
            {
                AssetManager.PlaySound(SoundID.TwinFang);
                dashCooldown = 30;
                energy -= 4;
                velocity = Functions.PolarVector(7f, rotation);
                Vector2 ringCenter = position + Functions.PolarVector(-5f, rotation);
                for (int i = 0; i < 20; i++)
                {
                    float r = 2f * (float)Math.PI * ((float)i / 20f);
                    float x = (float)Math.Cos(r) * 2.5f;
                    float y = (float)Math.Sin(r) * 5.5f;
                    new Particle(ringCenter + Functions.PolarVector(x, rotation) + Functions.PolarVector(y, rotation + (float)Math.PI / 2), 30, Color.Cyan);
                }
                for (int i = 0; i < 12; i++)
                {
                    float r = 2f * (float)Math.PI * ((float)i / 12f);
                    float x = (float)Math.Cos(r) * 1.25f * 1.5f;
                    float y = (float)Math.Sin(r) * 2.75f * 1.5f;
                    new Particle(ringCenter + Functions.PolarVector(x, rotation) + Functions.PolarVector(y, rotation + (float)Math.PI / 2), 30, Color.Cyan, Functions.PolarVector(-0.25f, rotation));
                }
            }
        }
        int counter = 0;
        Color getBeamColor()
        {
            int damage = (int)((float)beamCharge / 30);
            Color beamColor = Color.White;
            switch (damage)
            {
                case 1:
                    beamColor = Color.Red;
                    break;
                case 2:
                    beamColor = Color.Orange;
                    break;
                case 3:
                    beamColor = Color.Yellow;
                    break;
                case 4:
                    beamColor = Color.Green;
                    break;
                case 5:
                    beamColor = Color.Blue;
                    break;
                case 6:
                    beamColor = Color.Indigo;
                    break;
                case 7:
                    beamColor = Color.Violet;
                    break;
                case 8:
                    beamColor = Color.HotPink;
                    break;
            }
            return beamColor;
        }
        public override void LocalUpdate()
        {
            //Debug.WriteLine(beamCharge);
            if (!(npcShoot || (team < 2 &&Controls.controlShoot[team])) && beamCharge > 30 && beam == null)
            {
                int damage = (int)((float)beamCharge / 30);

                beam = new Beam(this, getBeamColor(),BeamRange, 20, -1, damage, Math.Max(1, (float)damage / 3f));
                AssetManager.PlaySound(SoundID.Pew2);
                beamCharge = 0;
            }
            else if (!(npcShoot || (team < 2 &&Controls.controlShoot[team])))
            {
                beamCharge = 0;
            }
            if (beam != null)
            {
                if (beam.Update(position + Functions.PolarVector(8, rotation), rotation))
                {
                    beam.ProcessCollision();
                }
                else
                {
                    beam = null;
                }
            }
            if (dashCooldown > 0)
            {
                dashCooldown--;
            }
            if (thrusting)
            {
                counter++;
                if (counter % 2 == 0)
                {
                    new Particle(position + Functions.PolarVector(0, rotation + (float)Math.PI / 2) + Functions.PolarVector(-12, rotation), 14, Color.Orange);
                }
            }
        }
        bool AI_Dodging = false;
        int enemyHealthBeforeBeam = 0;
        public override void AI()
        {
            AI_ResetControls();
            AI_Dodging = false;
            Entity enemyShip = GetEnemy();
            List<Projectile> enemyProj = EnemyProjectiles();
            bool cancelShoot = false;
            for (int i = 0; i < enemyProj.Count; i++)
            {
                if (AI_ImpendingCollision(enemyProj[i], 30, out int ET))
                {
                    if (beam != null && enemyProj[i].health >= 0 && enemyProj[i].health <= beam.damage)
                    {
                        AI_TurnToward((Functions.screenLoopAdjust(position, enemyProj[i].position) - position).ToRotation());
                    }
                    else if (beam == null && enemyProj[i].health >= 0 && beamCharge >= 30 * enemyProj[i].health && ET > 1.3f * Functions.AngularDifference((Functions.screenLoopAdjust(position, enemyProj[i].position) - position).ToRotation(), rotation) / GetTurnSpeed())
                    {
                        AI_cShoot(true);
                        cancelShoot = true;
                    }    
                    else
                    {
                        if (energy >= 4 && ET < 10)
                        {
                            LancerDodge(enemyProj[i]);

                        }
                        else
                        {
                            AI_cThrust();
                            AI_Dodge(enemyProj[i]);
                        }
                    }
                    AI_Dodging = true;
                    break;
                }
            }
            if (enemyShip != null)
            {
                int damageNeeded = Math.Min(enemyShip.health, 7);
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                if (beam != null && enemyHealthBeforeBeam == enemyShip.health)
                {
                    if(!AI_Dodging)
                    {
                        AI_TurnToward((enemyPos - position).ToRotation());
                    }    
                }
                else
                {
                    //AI_TurnToward((enemyPos - position).ToRotation());

                    //collisionLine = (enemyPos - position).ToRotation();
                    //new Particle(enemyPos, 3);
                    if ((!(npcShoot || (team < 2 &&Controls.controlShoot[team])) && energy == energyCapacity) || beamCharge > 0)
                    {
                        AI_cShoot();
                    }
                    if (energy == 0 || cancelShoot)
                    {
                        AI_cShoot(true);
                        enemyHealthBeforeBeam = enemyShip.health;
                    }
                    if (!AI_Dodging)
                    {
                        if (beamCharge >= 30 * damageNeeded || beam != null)
                        {

                            if ((enemyPos - position).Length() < BeamRange - 20)
                            {
                                if (AI_TurnToward((enemyPos - position).ToRotation()))
                                {
                                    AI_cShoot(true);
                                    enemyHealthBeforeBeam = enemyShip.health;
                                }
                            }
                            else 
                            {
                                if (AI_TurnToward((enemyPos - position).ToRotation()))
                                {
                                    if (energy > 4)
                                    {
                                        AI_cSpecial();
                                    }
                                    else if (dashCooldown <= 0)
                                    {
                                        AI_cThrust();
                                    }
                                }
                            }
                        }
                    }
                    if (!(npcShoot || (team < 2 &&Controls.controlShoot[team])) && (enemyPos - position).Length() < 300)
                    {
                        AI_Retreat(enemyPos);
                    }
                }
            }
            
        }
        void LancerDodge(Entity entity)
        {
            Vector2 entVel = entity.velocity * (entity.SlowTime > 0 ? 0.5f : 1f);
            float turnToRight = (entVel - velocity).ToRotation() + (float)Math.PI / 2;
            float turnToLeft = (entVel - velocity).ToRotation() - (float)Math.PI / 2;

            if (Functions.AngularDifference(rotation, turnToRight) < Functions.AngularDifference(rotation, turnToLeft))
            {
                if (AI_TurnToward(turnToRight))
                {
                    AI_cSpecial();
                }
            }
            else
            {
                if (AI_TurnToward(turnToLeft))
                {
                    AI_cSpecial();
                }
            }
        }
    }
}
