using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GazeOGL.Entities.Projectiles;
using GazeOGL.Entities.Ships;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities
{
    public abstract class Entity
    {
        public Vector2 position;
        public Vector2 velocity;
        protected Shape shape;
        public float rotation = 0;
        public int health;
        public int team;
        public float mass = 10f;
        public int StunTime = 0;
        public int SlowTime = 0;
        public float? collisionLine = null;
        public const float MAXSPEED = 11;
        public bool invulnerable = false;
        public int incorpreal = 0;
        public bool noHurtOnCollision = false;
        public bool DrawOnTop = false;
        public bool ignoreMe = false;
        public Entity()
        {
            Arena.entities.Add(this);
        }
        public Shape Hitbox()
        {
            //Debug.WriteLine(shape.GetVertex(0));
            Shape hitbox = shape.Clone();
            
            hitbox.Rotate(Vector2.Zero, rotation);
            hitbox.Move(position);
            
            return hitbox;
        }
        public Shape Hitbox(Vector2 offset)
        {
            Shape hitbox = shape.Clone();

            hitbox.Rotate(Vector2.Zero, rotation);
            hitbox.Move(position);
            hitbox.Move(offset);

            return hitbox;
        }
        List<Shape> hitBoxes;
        public void MakeHitboxes()
        {
            List<Shape> returnList = new List<Shape>();
            returnList.Add(Hitbox());
            if(returnList[0].Colliding(Arena.boundryLines[0]))
            {
                returnList.Add(Hitbox(Vector2.UnitY * Arena.boundrySize));
                //top left corner
                if (returnList[0].Colliding(Arena.boundryLines[3]))
                {
                    returnList.Add(Hitbox(new Vector2(1, 1) * Arena.boundrySize));
                }
            }
            if (returnList[0].Colliding(Arena.boundryLines[1]))
            {
                returnList.Add(Hitbox(Vector2.UnitX * -Arena.boundrySize));
                //top Right Corner
                if (returnList[0].Colliding(Arena.boundryLines[0]))
                {
                    returnList.Add(Hitbox(new Vector2(-1, 1) * Arena.boundrySize));
                }
            }
            if (returnList[0].Colliding(Arena.boundryLines[2]))
            {
                returnList.Add(Hitbox(Vector2.UnitY * -Arena.boundrySize));
                //bottom right corner
                if (returnList[0].Colliding(Arena.boundryLines[1]))
                {
                    returnList.Add(Hitbox(new Vector2(-1, -1) * Arena.boundrySize));
                }
            }
            if (returnList[0].Colliding(Arena.boundryLines[3]))
            {
                returnList.Add(Hitbox(Vector2.UnitX * Arena.boundrySize));
                //bottom left corner
                if (returnList[0].Colliding(Arena.boundryLines[2]))
                {
                    returnList.Add(Hitbox(new Vector2(1, -1) * Arena.boundrySize));
                }
            }
            //Debug.WriteLine(returnList.Count);
            hitBoxes = returnList;
        }
        public List<Shape> GetCollisionBoxes()
        {
            return hitBoxes;
        }
        public Shape[] AllHitboxes()
        {
            Shape[] shapes = new Shape[9];
            Vector2[] offsets = Functions.OffsetsForDrawing();
            for(int i =0; i < 9; i++)
            {
                shapes[i] = Hitbox(offsets[i]);
            }
            return shapes;
        }
        public void PhysicsMovement()
        {
            if(incorpreal > 0)
            {
                incorpreal--;
            }
            if(StunTime >0)
            {
                StunTime--;
                float dir = (float)Main.random.NextDouble() * 2f * (float)Math.PI;
                new Particle(position, 10, Color.Red, Functions.PolarVector(2, dir));
                velocity = Vector2.Zero;
            }
            else
            {
                if(velocity.Length() > MAXSPEED)
                {
                    velocity.Normalize();
                    velocity *= MAXSPEED;
                }
                if (SlowTime > 0)
                {
                    position += velocity / 2f;
                    SlowTime--;
                }
                else
                {
                    position += velocity;
                }
            }
            if(position.X > Arena.boundrySize)
            {
                position.X -= Arena.boundrySize;
            }
            if(position.X < 0)
            {
                position.X += Arena.boundrySize;
            }
            if (position.Y > Arena.boundrySize)
            {
                position.Y -= Arena.boundrySize;
            }
            if (position.Y < 0)
            {
                position.Y += Arena.boundrySize;
            }
            LocalUpdate();
            ModerateUpdate();
        }
        public virtual void LocalUpdate()
        {

        }
        public virtual void ModerateUpdate()
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Vector2[] offsets = Functions.OffsetsForDrawing();
            for (int i = 0; i < 9; i++)
            {
                Rectangle screenView = new Rectangle((int)Camera.screenPosition.X, (int)Camera.screenPosition.Y, (int)Camera.CameraWorldSize, (int)Camera.CameraWorldSize);
                if (screenView.Intersects(Hitbox(offsets[i]).GetBounds()))
                {
                    LocalDraw(spriteBatch, Camera.CameraOffset( position + offsets[i]));
                    
                }

            }
        }
        public virtual void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {

        }
        public void Kill()
        {
            Arena.entities.Remove(this);
            OnKill();
        }
        public virtual void OnKill()
        {

        }
        protected List<Projectile> EnemyProjectiles()
        {
            List<Projectile> proj = new List<Projectile>();
            for (int i = 0; i < Arena.entities.Count; i++)
            {
                if (Arena.entities[i].team != team && Arena.entities[i] is Projectile)
                {
                    if(Arena.entities[i] is Mine)
                    {
                        if(((Mine)Arena.entities[i]).alpha > 0)
                        {
                            proj.Add((Projectile)Arena.entities[i]);
                        }
                    }
                    else if(Arena.entities[i] is Grapple)
                    {
                        if(!((Grapple)Arena.entities[i]).IsStuck())
                        {
                            proj.Add((Projectile)Arena.entities[i]);
                        }
                    }
                    else
                    {
                        proj.Add((Projectile)Arena.entities[i]);
                    }
                }
            }
            return proj;
        }
        
        protected Entity GetEnemy()
        {
            //add some code here if enemy is illusioner
            Ship enemy = Arena.ships[team == 0 ? 1 : 0];
            if(Arena.npcs.Count > 0)
            {
                float dist = enemy == null ? 2000 : (Functions.screenLoopAdjust(position, enemy.position) - position).Length();
                for(int i =0; i < Arena.npcs.Count; i++)
                {
                    if(Arena.npcs[i].team != team && (Functions.screenLoopAdjust(position, Arena.npcs[i].position) - position).Length() < dist)
                    {
                        enemy = Arena.npcs[i];
                        dist = (Functions.screenLoopAdjust(position, enemy.position) - position).Length();
                    }
                }
            }
            if(enemy is Illusioner)
            {
                List<Entity> illusionAndCo = new List<Entity>();
                illusionAndCo.Add(enemy);
                illusionAndCo.AddRange(((Illusioner)enemy).illusions);
                Entity closest = illusionAndCo[0];
                float minDist = 10000;
                for(int i =0; i < illusionAndCo.Count; i++)
                {
                    float dist = (illusionAndCo[i].position - position).Length();
                    if(dist < minDist)
                    {
                        minDist = dist;
                        closest = illusionAndCo[i];
                    }
                }
                return closest;
            }
            return enemy;
        }

        public delegate bool SpecialCondition(Entity possibleTarget);
        protected bool ClosestProjectile(Vector2 center, float range, out Entity targetThis, int projMaxHealth = 1, SpecialCondition specialCondition = null)
        {
            if (specialCondition == null)
            {
                specialCondition = delegate (Entity possibleTarget) { return true; };
            }
            List<Projectile> proj = EnemyProjectiles();
            targetThis = null;
            float closestDistance = 10000;
            for (int p = 0; p < proj.Count; p++)
            {
                Vector2 ProjPos = Functions.screenLoopAdjust(center, proj[p].position);
                float dist = (ProjPos - center).Length();
                if (proj[p].health != -1 && proj[p].health <= projMaxHealth && dist < range && dist < closestDistance && specialCondition(proj[p]))
                {
                    targetThis = proj[p];
                    closestDistance = dist;
                }
            }
            return targetThis != null;
        }
        protected bool DefensiveTargetting(Vector2 center, float range, out Entity targetThis, int projMaxHealth = 1, SpecialCondition specialCondition = null)
        {
            if (specialCondition == null)
            {
                specialCondition = delegate (Entity possibleTarget) { return true; };
            }
            if (ClosestProjectile(center, range, out targetThis, projMaxHealth, specialCondition))
            {
                return true;
            }
            for (int i = 0; i < Arena.entities.Count; i++)
            {
                if (Arena.entities[i].team != team && !(Arena.entities[i] is Projectile) && specialCondition(Arena.entities[i] ) && !Arena.entities[i].ignoreMe)
                {
                    Vector2 entityPos = Functions.screenLoopAdjust(center, Arena.entities[i].position);
                    if ((entityPos - center).Length() < range)
                    {
                        targetThis = Arena.entities[i];
                        return true;
                    }
                }
            }

            targetThis = null;
            return false;

        }
        protected bool AggressiveTargetting(Vector2 center, float range, out Entity targetThis, int projMaxHealth = 1, SpecialCondition specialCondition = null)
        {
            if (specialCondition == null)
            {
                specialCondition = delegate (Entity possibleTarget) { return true; };
            }

            for (int i = 0; i < Arena.entities.Count; i++)
            {
                if (Arena.entities[i].team != team && !(Arena.entities[i] is Projectile) && specialCondition(Arena.entities[i]) && !Arena.entities[i].ignoreMe)
                {
                    Vector2 entityPos = Functions.screenLoopAdjust(center, Arena.entities[i].position);
                    if ((entityPos - center).Length() < range)
                    {
                        targetThis = Arena.entities[i];
                        return true;
                    }
                }
            }
            if (ClosestProjectile(center, range, out targetThis, projMaxHealth, specialCondition))
            {
                return true;
            }
            targetThis = null;
            return false;

        }
    }
}
