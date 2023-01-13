using Microsoft.Xna.Framework;
using GazeOGL.Entities.Projectiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities.Ships
{
    public abstract class Ship : Entity
    {
        public const float MAXSHIPSPEED = 6f;
        public int energyCapacity;
        public int energy;
        public int energyGen;
        public int healthMax;
        public int maxSpeed;
        protected int acceleration;
        public int turnSpeed;
        protected bool thrusting;
        public ShipID type;
        protected int energyRate = 1; 
        public List<int> ExtraHealthBoxes = new List<int>();
        public List<int> ExtraHealths = new List<int>();
        public int EMPTime = 0;
        public bool instaAcc = false;
        protected bool onlyCapSpeedOnThrust = false;
        protected float breaks = 0.95f;

        public bool npcThrust = false;
        public bool npcLeft = false;
        public bool npcRight = false;
        public bool npcDown = false;
        public bool npcShoot = false;
        public bool npcSpecial = false;
        
        public Ship(Vector2 position, int team = 0)
        {
            this.position = position;
            this.team = team;
        }

        protected float GetAcc()
        {
            return (float)acceleration * 0.2f * (1f / 30f);
        }
        public virtual void Thrust(bool reverse = false)
        {
            thrusting = true;
            float s = (float)maxSpeed * 0.2f;
            float a = GetAcc();
            if(reverse)
            {
                a *= -1;
            }
            if (instaAcc)
            {
                velocity = Functions.PolarVector(s, rotation);
            }
            else
            {
                velocity += Functions.PolarVector(a, rotation);
                if (velocity.Length() > s)
                {
                    velocity += Functions.PolarVector(-GetAcc(), Functions.ToRotation(velocity));
                    if (velocity.Length() < s)
                    {
                        velocity.Normalize();
                        velocity *= s;
                    }
                }
            }
        }
        public float GetTurnSpeed()
        {
            return (SlowTime > 0 ? 0.5f : 1) * (((float)turnSpeed / 15f) * 2f * (float)Math.PI) / 60f;
        }
        public virtual void Left()
        {
            rotation -= GetTurnSpeed();
        }
        public virtual void Right()
        {
            rotation += GetTurnSpeed();
        }
        public virtual void Shoot()
        {

        }
        public virtual void Special()
        {

        }
        int energyCounter = 0;
        public override void ModerateUpdate()
        {
            thrusting = false;
            if (EMPTime > 0)
            {
                EMPTime--;
            }
            else
            {
                energyCounter++;
                if (energyCounter % ((60 / energyGen) * energyRate) == 0 && energy < energyCapacity && StunTime <= 0)
                {
                    energy += energyRate;
                }
                if (energy > energyCapacity)
                {
                    energy = energyCapacity;
                }
            }
            if(velocity.Length() > MAXSHIPSPEED)
            {
                velocity.Normalize();
                velocity *= MAXSHIPSPEED;
            }
            else if(!onlyCapSpeedOnThrust && velocity.Length() > (float)maxSpeed * 0.2f)
            {
                velocity *= breaks;
            }
        }
        static Ship Place(ShipID shipID, int team, Vector2 position)
        {
            switch (shipID)
            {
                case ShipID.Hunter:
                    return new Hunter(position, team);
                case ShipID.Conqueror:
                    return new Conqueror(position, team);
                case ShipID.Strafer:
                    return new Strafer(position, team);
                case ShipID.Escort:
                    return new Escort(position, team);
                case ShipID.Illusioner:
                    return new Illusioner(position, team);
                case ShipID.Assassin:
                    return new Assassin(position, team);
                case ShipID.Palladin:
                    return new Palladin(position, team);
                case ShipID.Trooper:
                    return new Trooper(position, team);
                case ShipID.Eagle:
                    return new Eagle(position, team);
                case ShipID.Apocalypse:
                    return new Apocalypse(position, team);
                case ShipID.Trebeche:
                    return new Trebeche(position, team);
                case ShipID.Buccaneer:
                    return new Buccaneer(position, team);
                case ShipID.Missionary:
                    return new Missionary(position, team);
                case ShipID.Lancer:
                    return new Lancer(position, team);
                case ShipID.Mechanic:
                    return new Mechanic(position, team);
                case ShipID.Frigate:
                    return new Frigate(position, team);
                case ShipID.Brute:
                    return new Brute(position, team);
                case ShipID.Surge:
                    return new Surge(position, team);
            }
            return null;
        }
        public static Ship Spawn(ShipID shipID, int team)
        {
            Vector2 position = Vector2.One * 100 + Vector2.One * team * 200;
            //position.X += 400 * Main.random.Next(2);
            //position.Y += 400 * Main.random.Next(2);
            Ship enemy = Arena.ships[team == 0 ? 1 : 0];
            
            if(shipID == ShipID.Count)
            {
                shipID = (ShipID)Main.random.Next((int)ShipID.Count);
            }
            if(team >= 2)
            {
                position = (Camera.screenPosition + Camera.CameraWorldSize * Vector2.One) + (Arena.boundrySize / 2 * Vector2.One);
                /*
                position = Arena.boundrySize * 0.5f * Vector2.One;
                float radius = Arena.boundrySize / 8;
                if(Arena.npcsSpawned % 10 == 9)
                {
                    ShipStats.GetTitlesFor(shipID, out _, out string shipName);
                    Console.WriteLine("The "+shipName+ " gang is here!!");
                    Arena.npcs.Add(Place(shipID, team, position + Functions.PolarVector(radius, 2f * (float)Math.PI / 3f)));
                    Arena.npcs[Arena.npcs.Count - 1].rotation = 2f * (float)Math.PI / 3f;
                    Arena.npcs.Add(Place(shipID, team, position + Functions.PolarVector(radius, -2f * (float)Math.PI / 3f)));
                    Arena.npcs[Arena.npcs.Count - 1].rotation = -2f * (float)Math.PI / 3f;
                    position += Vector2.UnitX * radius;
                }
                */
            }
            else if (enemy != null)
            {
                position = enemy.position + Vector2.One * (Arena.boundrySize * 0.4f);
                //position.X += Main.random.Next(-10, 11);
                //position.Y += Main.random.Next(-10, 11);
            }
            switch (shipID)
            {
                case ShipID.Hunter:
                    return new Hunter(position, team);
                case ShipID.Conqueror:
                    return new Conqueror(position, team);
                case ShipID.Strafer:
                    return new Strafer(position, team);
                case ShipID.Escort:
                    return new Escort(position, team);
                case ShipID.Illusioner:
                    return new Illusioner(position, team);
                case ShipID.Assassin:
                    return new Assassin(position, team);
                case ShipID.Palladin:
                    return new Palladin(position, team);
                case ShipID.Trooper:
                    return new Trooper(position, team);
                case ShipID.Eagle:
                    return new Eagle(position, team);
                case ShipID.Apocalypse:
                    return new Apocalypse(position, team);
                case ShipID.Trebeche:
                    return new Trebeche(position, team);
                case ShipID.Buccaneer:
                    return new Buccaneer(position, team);
                case ShipID.Missionary:
                    return new Missionary(position, team);
                case ShipID.Lancer:
                    return new Lancer(position, team);
                case ShipID.Mechanic:
                    return new Mechanic(position, team);
                case ShipID.Frigate:
                    return new Frigate(position, team);
                case ShipID.Brute:
                    return new Brute(position, team);
                case ShipID.Surge:
                    return new Surge(position, team);
            }
            return null;
        }
        public override void OnKill()
        {
            AssetManager.PlaySound(SoundID.Death);
            for(int i =0; i <1; i++)
            {
                Debris.DebrisSet.CreateDebris(this);
            }
            /*
            for (int i = 0; i < 60; i++)
            {
                new Particle(position, 180, Color.Orange, Functions.PolarVector(2 + 5 * (float)Main.random.NextDouble(), (float)Math.PI * 2 * (float)Main.random.NextDouble()));
            }
            */
        }
        public virtual void ModifyHitByProjectile(ref int damage, float hitDirection)
        {

        }
        public virtual void HitByProjectile(int damage)
        {

        }
        public void BaseAI()
        {
            /*
            if(GetEnemy() != null && GetEnemy() is Missionary && !(this is Illusioner || (this is Escort && ((Escort)this).attached)))
            {
                AI_ResetControls();
                Projectile deathWave = ((Missionary)GetEnemy()).wave;
                if (deathWave != null)
                {
                    if (AI_ImpendingCollision(deathWave, 90))
                    {
                        AI_Dodge(deathWave);
                        AI_cThrust();
                        if (this is Palladin)
                        {
                            AI_cSpecial();
                        }
                        return;
                    }
                }
            }
            */
            AI();
        }
        public virtual void AI()
        {

        }
        public void AimAssist(float shootSpeed, float offset = 0)
        {
            if(GetEnemy() != null)
            {
                Entity enemyShip = GetEnemy();
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                float toward = (enemyPos - position).ToRotation();
                Vector2 enemyVel = enemyShip.velocity * (enemyShip.SlowTime > 0 ? 0.5f : 1);
                float aimAt = Functions.PredictiveAimWithOffset(position, shootSpeed, enemyPos, enemyVel - velocity, offset);
                if (!float.IsNaN(aimAt))
                {
                    if(Functions.AngularDifference(aimAt, rotation) < GetTurnSpeed() * 2)
                    {
                        rotation = aimAt;
                    }
                }
            }
        }
        protected void AI_Dodge(Entity entity, bool awayFromEnemy = false)
        {
            Vector2 entVel = entity.velocity * (entity.SlowTime > 0 ? 0.5f : 1f);
            float turnToRight = (entVel - velocity).ToRotation() + (float)Math.PI / 2;
            float turnToLeft = (entVel - velocity).ToRotation() - (float)Math.PI / 2;
            Entity enemyShip = GetEnemy();
            if(entity is Kugelblitz || entity is LingeringExplosion)
            {
                if (AI_TurnToward((position - Functions.screenLoopAdjust(position, entity.position)).ToRotation()) && !(this is Surge))
                {
                    AI_cThrust();
                }
            }
            if (awayFromEnemy && enemyShip != null)
            {
                
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                if (Functions.AngularDifference((enemyPos - position).ToRotation(), turnToRight) > Functions.AngularDifference((enemyPos - position).ToRotation(), turnToLeft))
                {
                    if (AI_TurnToward(turnToRight))
                    {
                        AI_cThrust();
                    }
                }
                else
                {
                    if (AI_TurnToward(turnToLeft))
                    {
                        AI_cThrust();
                    }
                }
            }
            else
            {
                if (Functions.AngularDifference(rotation, turnToRight) < Functions.AngularDifference(rotation, turnToLeft))
                {
                    if (AI_TurnToward(turnToRight))
                    {
                        AI_cThrust();
                    }
                }
                else
                {
                    if (AI_TurnToward(turnToLeft))
                    {
                        AI_cThrust();
                    }
                }
            }
        }
        protected void AI_ResetControls()
        {
            AI_cThrust(true);
            AI_cRight(true);
            AI_cLeft(true);
            AI_cDown(true);
            AI_cShoot(true);
            AI_cSpecial(true);
        }
        protected void AI_Retreat(Vector2 enemyPos)
        {
            if (velocity.Length() < maxSpeed * .2f - GetAcc() || Functions.AngularDifference(velocity.ToRotation(), (position - enemyPos).ToRotation()) > (float)Math.PI / 2f)
            {
                AI_cThrust();
                float aimAt = (position - enemyPos).ToRotation();
                AI_TurnToward(aimAt);
            }
        }
        protected void AI_AvoidFront(float distance)
        {
            Entity enemy = GetEnemy();
            Vector2 enemyPos = Functions.screenLoopAdjust(position, enemy.position);
            if (enemy != null)
            {
                Vector2 leftOfEnemy = enemyPos + Functions.PolarVector(6, enemy.rotation - (float)Math.PI / 2f);
                Vector2 rightOfEnemy = enemyPos + Functions.PolarVector(6, enemy.rotation + (float)Math.PI / 2f);
                bool clockWise = (leftOfEnemy - position).Length() > (rightOfEnemy - position).Length();
                AI_Circle(distance, clockWise);
            }
        }
        protected void AI_Circle(float distance, bool clockwise = true)
        {
            Entity enemy = GetEnemy();
            Vector2 enemyPos = Functions.screenLoopAdjust(position, enemy.position);
            if(enemy != null)
            {
                //Debug.WriteLine("Hey");
                float rotOffset = (float)Math.PI / 2f;
                float dist = (enemyPos - position).Length();
                float ratio = (dist / distance);
                if(ratio > 2)
                {
                    ratio = 2;
                    AI_TurnToward((enemyPos - position).ToRotation());
                }
                float offsetOffset = -1 * (((float)Math.PI / 2f) - ((float)Math.PI / 2f) * ratio);
                rotOffset += offsetOffset;
                if(!clockwise)
                {
                    rotOffset *= -1;
                }
                AI_TurnToward(enemy.rotation + rotOffset);
                //collisionLine = enemy.rotation + rotOffset;
                AI_cThrust();
            }
        }
        protected void AI_Kite(float shotVel, float range, float shootOffset = 0, float sensitivityAngle = (float)Math.PI / 2f)
        {

            Entity enemyShip = GetEnemy();
            if (enemyShip != null)
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                float toward = (enemyPos - position).ToRotation();

                if (velocity.Length() < (maxSpeed * 0.2f) - GetAcc() || Functions.AngularDifference(velocity.ToRotation(), (position - enemyPos).ToRotation()) >  sensitivityAngle)
                {
                    AI_Retreat(enemyPos);
                }
                else
                {
                    if(AI_AimAtEnemy(shotVel, shootOffset))
                    {
                        if ((enemyPos - position).Length() < range)
                        {
                            AI_cShoot();
                        }
                    }
                }
            }
        }
        protected bool AI_AimAtEnemy(float shotVel, float shootOffset = 0)
        {
            Entity enemyShip = GetEnemy();
            if (enemyShip != null)
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);

                float toward = (enemyPos - position).ToRotation();
                if (shotVel == -1)
                {
                    if (AI_TurnToward(toward))
                    {
                        return true;
                    }
                }
                else
                {
                    Vector2 enemyVel = enemyShip.velocity * (enemyShip.SlowTime > 0 ? 0.5f : 1);
                    float aimAt = Functions.PredictiveAimWithOffset(position, shotVel, enemyPos, enemyVel - velocity, shootOffset);
                    if (!float.IsNaN(aimAt))
                    {
                        if (AI_TurnToward(aimAt))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        AI_TurnToward(toward);
                    }
                }
            }
            return false;
        }
        protected bool AI_ImpendingBeamCollision(int frames)
        {
            Entity enemyShip = GetEnemy();
            if (enemyShip != null)
            {
                if (enemyShip is Strafer)
                {
                    Shape[] myHitboxes = AllHitboxes();
                    Polygon otherHitbox = new Polygon(new Vector2[]
                    {
                        enemyShip.position + Functions.PolarVector(Strafer.BeamRange + 10, enemyShip.rotation),
                        enemyShip.position + Functions.PolarVector(Strafer.BeamRange + 10, enemyShip.rotation + 2*(float)Math.PI/3f),
                        enemyShip.position + Functions.PolarVector(Strafer.BeamRange + 10, enemyShip.rotation - 2*(float)Math.PI/3f),
                    });
                    //Main.one = otherHitbox;
                    for (int i = 0; i < frames; i++)
                    {
                        for (int h = 0; h < 9; h++)
                        {
                            if (myHitboxes[h].Colliding(otherHitbox))
                            {
                                return true;
                            }
                            myHitboxes[h].Move(velocity);
                        }
                        otherHitbox.Move(enemyShip.velocity);
                    }
                    return false;
                }
                if (enemyShip is Escort)
                {
                    if (((Escort)enemyShip).attached)
                    {
                        Shape[] myHitboxes = AllHitboxes();
                        float turnAmt = frames * (enemyShip.SlowTime > 0 ? 0.5f : 1f) *(((Ship)enemyShip).turnSpeed / 15f * 2f * (float)Math.PI / 60f);
                        if (turnAmt > (float)Math.PI / 4f)
                        {
                            turnAmt = (float)Math.PI / 4f;
                        }
                        Polygon otherHitbox = new Polygon(new Vector2[]
                        {
                            Vector2.Zero,
                            new Vector2(Escort.BeamRange, Escort.BeamRange *(float)Math.Tan(turnAmt)),
                            new Vector2(Escort.BeamRange, Escort.BeamRange *(float)Math.Tan(-turnAmt)),
                        });
                        otherHitbox.Rotate(Vector2.Zero, enemyShip.rotation);
                        otherHitbox.Move(enemyShip.position);
                        //Main.one = otherHitbox;
                        for (int i = 0; i < frames; i++)
                        {
                            for (int h = 0; h < 9; h++)
                            {
                                if (myHitboxes[h].Colliding(otherHitbox))
                                {
                                    return true;
                                }
                                myHitboxes[h].Move(velocity);
                            }
                            otherHitbox.Move(enemyShip.velocity);
                        }
                        return false;
                    }
                }
                if (enemyShip is Assassin)
                {
                    Shape[] myHitboxes = AllHitboxes();
                    float turnAmt = frames * (((Ship)enemyShip).turnSpeed / 15f * 2f * (float)Math.PI / 60f);
                    if (turnAmt > (float)Math.PI / 4f)
                    {
                        turnAmt = (float)Math.PI / 4f;
                    }
                    Circle assassinRange = new Circle(enemyShip.position, Assassin.BeamRange + 7);
                    for (int i = 0; i < frames; i++)
                    {
                        for (int h = 0; h < 9; h++)
                        {
                            if (myHitboxes[h].Colliding(assassinRange))
                            {
                                return true;
                            }
                            myHitboxes[h].Move(velocity);
                        }
                        assassinRange.Move(enemyShip.velocity);
                    }
                    return false;
                }
                if (enemyShip is Apocalypse)
                {
                    ApocalypseTurret turret = ((Apocalypse)enemyShip).turret;
                    Shape[] myHitboxes = AllHitboxes();
                    float turnAmt = frames * (float)Math.PI / 210f;
                    if (turnAmt > (float)Math.PI / 4f)
                    {
                        turnAmt = (float)Math.PI / 4f;
                    }
                    Polygon otherHitbox = new Polygon(new Vector2[]
                    {
                            Vector2.Zero,
                            new Vector2(Apocalypse.TurretRange, Apocalypse.TurretRange *(float)Math.Tan(turnAmt)),
                            new Vector2(Apocalypse.TurretRange, Apocalypse.TurretRange *(float)Math.Tan(-turnAmt)),
                    });
                    otherHitbox.Rotate(Vector2.Zero, turret.AbsoluteRotation());
                    otherHitbox.Move(turret.AbsolutePosition());
                    //Main.one = otherHitbox;
                    for (int i = 0; i < frames; i++)
                    {
                        for (int h = 0; h < 9; h++)
                        {
                            if (myHitboxes[h].Colliding(otherHitbox))
                            {
                                return true;
                            }
                            myHitboxes[h].Move(velocity);
                        }
                        otherHitbox.Move(enemyShip.velocity);
                    }
                    return false;

                }
                if (enemyShip is Buccaneer)
                {
                    Shape[] myHitboxes = AllHitboxes();

                    Polygon swordArea = new Polygon(new Vector2[]
                    {
                        enemyShip.position,
                        enemyShip.position + Functions.PolarVector(36, rotation + (float)Math.PI / 2 - (float)Math.PI / 16f),
                        enemyShip.position + Functions.PolarVector(36, rotation),
                        enemyShip.position + Functions.PolarVector(36, rotation + -(float)Math.PI / 2 + (float)Math.PI / 16f),
                    });
                    for (int i = 0; i < frames; i++)
                    {
                        for (int h = 0; h < 9; h++)
                        {
                            if (myHitboxes[h].Colliding(swordArea))
                            {
                                return true;
                            }
                            myHitboxes[h].Move(velocity);
                        }
                        swordArea.Move(enemyShip.velocity);
                    }
                    return false;

                }
                if (enemyShip is Lancer)
                {
                    if (((Lancer)enemyShip).beamCharge > 30)
                    {
                        Shape[] myHitboxes = AllHitboxes();
                        float turnAmt = frames * (enemyShip.SlowTime > 0 ? 0.5f : 1f) * (((Ship)enemyShip).turnSpeed / 15f * 2f * (float)Math.PI / 60f);
                        if (turnAmt > (float)Math.PI / 4f)
                        {
                            turnAmt = (float)Math.PI / 4f;
                        }
                        Polygon otherHitbox = new Polygon(new Vector2[]
                        {
                            Vector2.Zero,
                            new Vector2(Lancer.BeamRange, Lancer.BeamRange *(float)Math.Tan(turnAmt)),
                            new Vector2(Lancer.BeamRange, Lancer.BeamRange *(float)Math.Tan(-turnAmt)),
                        });
                        otherHitbox.Rotate(Vector2.Zero, enemyShip.rotation);
                        otherHitbox.Move(enemyShip.position);
                        //Main.one = otherHitbox;
                        for (int i = 0; i < frames; i++)
                        {
                            for (int h = 0; h < 9; h++)
                            {
                                if (myHitboxes[h].Colliding(otherHitbox))
                                {
                                    return true;
                                }
                                myHitboxes[h].Move(velocity);
                            }
                            otherHitbox.Move(enemyShip.velocity);
                        }
                        return false;
                    }
                }
            }
            return false;
        }
        
        protected bool AI_ImpendingCollisionLine(Line line, int frames)
        {
            return AI_ImpendingCollisionLine(line, frames, out _);
        }
        protected bool AI_ImpendingCollisionLine(Line line, int frames, out int expectedTime)
        {
            Shape[] myHitboxes = AllHitboxes();
            for (int i = 0; i < frames; i++)
            {
                for (int h = 0; h < 9; h++)
                {
                    if (myHitboxes[h].Colliding(line))
                    {
                        expectedTime = i;
                        return true;
                    }
                    myHitboxes[h].Move(velocity);
                }
            }
            expectedTime = -1;
            return false;
        }
        protected bool AI_ImpendingCollision(Entity otherEntity, int frames)
        {
            return AI_ImpendingCollision(otherEntity, frames, out _);
        }
        protected bool AI_ImpendingCollision(Entity otherEntity, int frames, out int expectedTime)
        {
            Shape[] myHitboxes = AllHitboxes();
            Shape otherHitbox = otherEntity.Hitbox();
            for (int i = 0; i < frames; i++)
            {
                for (int h = 0; h < 9; h++)
                {
                    if (myHitboxes[h].Colliding(otherHitbox))
                    {
                        expectedTime = i;
                        return true;
                    }
                    myHitboxes[h].Move(velocity);
                }
                otherHitbox.Move(otherEntity.velocity);
            }
            expectedTime = -1;
            return false;
        }
        protected bool AI_ImpendingCollisionAlly(Entity ally, Entity otherEntity, int frames)
        {
            return AI_ImpendingCollisionAlly(ally, otherEntity, frames, out _);
        }
        protected bool AI_ImpendingCollisionAlly(Entity ally, Entity otherEntity, int frames, out int expectedTime)
        {
            Shape[] myHitboxes = ally.AllHitboxes();
            Shape otherHitbox = otherEntity.Hitbox();
            for (int i = 0; i < frames; i++)
            {
                for (int h = 0; h < 9; h++)
                {
                    if (myHitboxes[h].Colliding(otherHitbox))
                    {
                        expectedTime = i;
                        return true;
                    }
                    myHitboxes[h].Move(velocity);
                }
                otherHitbox.Move(otherEntity.velocity);
            }
            expectedTime = -1;
            return false;
        }
        protected bool AI_CollidingWithEnemy(Shape hitArea)
        {
            Entity enemyShip = GetEnemy();
            if(enemyShip != null)
            {
                return AI_CollidingWithEntitiy(enemyShip, hitArea);
            }
            return false;
        }
        protected bool AI_CollidingWithEntitiy(Entity entity, Shape hitArea)
        {
            entity.MakeHitboxes();
            List<Shape> enemyHitboxes = entity.GetCollisionBoxes();
            for (int i = 0; i < enemyHitboxes.Count(); i++)
            {
                if (enemyHitboxes[i].Colliding(hitArea))
                {
                    return true;
                }
            }
            return false;
        }
        
        protected bool AI_TurnToward(float targetAngle)
        {
            AI_cRight(true);
            AI_cLeft(true);
            int f = 1; //this is used to switch rotation direction
            float actDirection = Functions.ToRotation(new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)));
            targetAngle = Functions.ToRotation(new Vector2((float)Math.Cos(targetAngle), (float)Math.Sin(targetAngle)));
            
            //this makes f 1 or -1 to rotate the shorter distance
            if (Math.Abs(actDirection - targetAngle) > Math.PI)
            {
                f = -1;
            }
            else
            {
                f = 1;
            }
            if (Functions.AngularDifference(actDirection, targetAngle) < (((float)turnSpeed / 15f) * 2f * (Networking.IsConnected() ? Networking.framePacketSize : 1)  * (float)Math.PI) / 60f)
            {
                return true;
            }
            else
            {
                if (actDirection <= targetAngle)
                {
                    if (f == 1)
                    {
                        AI_cRight();
                    }
                    else
                    {
                        AI_cLeft();
                    }
                }
                else if (actDirection >= targetAngle)
                {
                    if (f == 1)
                    {
                        AI_cLeft();
                    }
                    else
                    {
                        AI_cRight();
                    }

                }
            }
            
            //collisionLine = targetAngle;
            return false;
        }
        public bool IsNPC()
        {
            return team >=2;
        }
        public void AI_cThrust(bool off = false)
        {
            if(IsNPC())
            {
                npcThrust = !off;
            }
            else
            {
                Controls.controlThrust[team] = !off;
            }
        }
        public void AI_cLeft(bool off = false)
        {
            if(IsNPC())
            {
                npcLeft = !off;
            }
            else
            {
                Controls.controlLeft[team] = !off;
            }
        }
        public void AI_cRight(bool off = false)
        {
            if(IsNPC())
            {
                npcRight = !off;
            }
            else
            {
                Controls.controlRight[team] = !off;
            }
        }
        public void AI_cDown(bool off = false)
        {
            if(IsNPC())
            {
                npcDown = !off;
            }
            else
            {
                Controls.controlDown[team] = !off;
            }
        }
        public void AI_cShoot(bool off = false)
        {
            if(IsNPC())
            {
                npcShoot = !off;
            }
            else
            {
                Controls.controlShoot[team] = !off;
            }
        }
        public void AI_cSpecial(bool off = false)
        {
            if(IsNPC())
            {
                npcSpecial = !off;
            }
            else
            {
                Controls.controlSpecial[team] = !off;
            }
        }
    }
}
