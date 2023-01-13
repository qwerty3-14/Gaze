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
    public class Eagle : Ship
    {
        public Eagle(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Eagle;
            ShipStats.GetStatsFor(ShipID.Eagle, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed, true);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 1;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(5, 0),
                new Vector2(3, 4),
                new Vector2(0, 11),
                new Vector2(-5, 11),
                new Vector2(-6, 9),
                new Vector2(-7, 2),
                new Vector2(-7, -2),
                new Vector2(-6, -9),
                new Vector2(-5, -11),
                new Vector2(0, -11),
                new Vector2(3, -4),
            });
            mass = 8;
            instaAcc = true;
        }
        public override void Shoot()
        {
            if(shotCooldown <=0 && energy > 2)
            {
                Entity enemyShip = GetEnemy();
                if(enemyShip != null)
                {
                    energy -= 2;
                    shotCooldown = 24;
                    Vector2 shotPosLeft = position + Functions.PolarVector(6, rotation - (float)Math.PI / 2f);
                    Vector2 shotPosRight = position + Functions.PolarVector(6, rotation + (float)Math.PI / 2f);
                    Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                    Vector2 shootFrom = position;
                    if((enemyPos - shotPosLeft).Length() < (enemyPos - shotPosRight).Length())
                    {
                        shootFrom = shotPosLeft;
                        frame = 2;
                    }
                    else
                    {
                        shootFrom = shotPosRight;
                        frame = 1;
                    }
                    float shotSpeed = 4.6f;
                    float aimAt = Functions.PredictiveAim(shootFrom, shotSpeed, enemyPos, enemyShip.velocity - velocity);
                    if(float.IsNaN(aimAt))
                    {
                        aimAt = (enemyPos - shootFrom).ToRotation();
                    }
                    Projectile p = new MarkOfStatehood(shootFrom, velocity + Functions.PolarVector(shotSpeed, aimAt), team);
                    p.rotation = p.velocity.ToRotation();
                }
            }
        }
        public Eaglet[] eaglets = new Eaglet[2];
        public const int EagletHealth = 4;
        public override void Special()
        {
            int eagletsGone = 0;
            for (int i = 0; i < eaglets.Length; i++)
            {
                if (eaglets[i] == null)
                {
                    eagletsGone++;
                }
            }
            if (shotCooldown <= 0 && energy == energyCapacity && eagletsGone > 0)
            {
                for (int i = 0; i < eaglets.Length; i++)
                {
                    if (eaglets[i] == null)
                    {
                        AssetManager.PlaySound(SoundID.CallMilitia);
                        energy = 0;
                        shotCooldown = 30;
                        frame = 3;
                        eaglets[i] = new Eaglet(this, position + Functions.PolarVector(10, rotation), team, rotation);
                        break;
                    }
                }
            }
        }
        int shotCooldown = 0;
        public override void LocalUpdate()
        {
            if (shotCooldown > 0)
            {
                shotCooldown--;
            }
            if(shotCooldown < 10)
            {
                frame = 0;
            }
            ExtraHealthBoxes.Clear();
            ExtraHealths.Clear();
            for (int i = 0; i < eaglets.Length; i++)
            {
                if (eaglets[i] != null && !Arena.entities.Contains(eaglets[i]))
                {
                    eaglets[i] = null;
                    break;
                }
                else if (eaglets[i] != null)
                {
                    ExtraHealthBoxes.Add(EagletHealth);
                    ExtraHealths.Add(eaglets[i].health);
                }
            }
        }
        int frame = 0;
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.extraEntities[1], pos, new Rectangle(0, frame * 23, 13, 23), Color.White, rotation, new Vector2(7.5f, 11.5f), Vector2.One, SpriteEffects.None, 0f);
            //spriteBatch.Draw(AssetManager.extraEntities[1], pos, null, new Rectangle(0, frame * 23, 13, 23), new Vector2(7.5f, 11.5f), rotation, Vector2.One, Color.White, 0, 0);
        }
        bool AI_Dodging = false;
        float aoeDodgeAlert = 0;
        
        float recentEnemyRot = 0;
        public override void AI()
        {
            AI_Dodging = false;
            AI_ResetControls();
            List<Projectile> enemyProj = EnemyProjectiles();
            for(int i =0; i < enemyProj.Count; i++)
            {
                if(AI_ImpendingCollision(enemyProj[i], 45) || (enemyProj[i] is Kugelblitz && aoeDodgeAlert > 0))
                {
                    if(enemyProj[i] is Kugelblitz)
                    {
                        AI_TurnToward(recentEnemyRot + 3f * (float)Math.PI/8f);
                    }
                    else
                    {
                        AI_Dodge(enemyProj[i]);
                    }
                    AI_cThrust();
                    aoeDodgeAlert = 60;
                    AI_Dodging = true;
                    break;
                }
            }
            aoeDodgeAlert--;
            Entity enemyShip = GetEnemy();
            if (enemyShip != null)
            {
                
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                recentEnemyRot = enemyShip.rotation;
                if (!AI_Dodging)
                {
                    int eagletsGone =0;
                    for(int i = 0; i < eaglets.Length; i++)
                    {
                        if(eaglets[i] == null)
                        {
                            eagletsGone++;
                        }
                    }
                    if (eagletsGone > 0)
                    {
                        AI_cSpecial();
                        AI_Retreat(enemyPos);
                    }
                    else if(enemyShip is Ship && ((Ship)enemyShip).GetTurnSpeed() * (enemyPos - position).Length() > maxSpeed * 0.2f)
                    {
                        AI_AimAtEnemy(maxSpeed * 0.2f);
                        AI_cThrust();
                        AI_cShoot(true);
                    }
                    
                    else
                    {
                        AI_AvoidFront(20);
                    }
                    
                }
                if ((enemyPos - position).Length() < 180)
                {
                    AI_cShoot();
                }
            }

            //Console.WriteLine(Controls.controlLeft[team] + ", " + Controls.controlRight[team]);
        }
    }
    public class Eaglet : Entity
    {
        public Eagle parent;
        public Eaglet(Eagle parent, Vector2 position, int team = 0, float rotation = 0) 
        {
            health = Eagle.EagletHealth;
            this.position = position;
            this.parent = parent;
            this.team = team;
            this.rotation = rotation;
            shape = new Polygon(new Vector2[]
             {
                new Vector2(2f, -2.5f),
                new Vector2(2f, 2.5f),
                new Vector2(-2f, 2.5f),
                new Vector2(-2f, -2.5f)
             });
            incorpreal = -1;
            mass = 2;
        }
        float maxSpeed = 3f;
        float turnSpeed = (float)Math.PI / 30;
        float acceleration = .2f;
        Vector2 flyTo;
        float orientation = 0f;
        int repositionCooldown;
        int shotCooldown;
        public override void LocalUpdate()
        {
            if(!Arena.entities.Contains(parent))
            {
                Kill();
            }
            if (repositionCooldown <= 0)
            {
                orientation = (float)Main.random.NextDouble() * 2f * (float)Math.PI;
                repositionCooldown = 60;
            }
            repositionCooldown--;
            shotCooldown--;
            flyTo = parent.position + Functions.PolarVector(40, orientation);
            flyTo = Functions.screenLoopAdjust(position, flyTo);
            rotation.SlowRotation(Functions.ToRotation(flyTo - position), turnSpeed);
            velocity += Functions.PolarVector(acceleration, rotation);
            if (velocity.Length() > maxSpeed)
            {
                velocity.Normalize();
                velocity *= maxSpeed;
            }
            Entity enemy = GetEnemy();
            if (enemy != null)
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemy.position);
                float speed = 4f;
                float AimAt = Functions.PredictiveAim(position, speed, enemyPos, enemy.velocity - velocity);
                if (!float.IsNaN(AimAt) && shotCooldown <= 0 && StunTime <= 0)
                {
                    if ((enemyPos - position).Length() < speed * 40)
                    {
                        shotCooldown = 60;
                        HunterPelt p = new HunterPelt(position, velocity + Functions.PolarVector(speed, AimAt), team);
                        p.rotation = p.velocity.ToRotation();
                        p.health = 1;
                    }
                }
            }
        }

        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.extraEntities[2], pos, null, Color.White, rotation, new Vector2(2f, 2.5f), Vector2.One, SpriteEffects.None, 0f);
        }
    }

}
