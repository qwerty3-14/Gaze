using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GazeOGL.Entities;
using GazeOGL.Entities.Projectiles;
using GazeOGL.Entities.Ships;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL
{
    class Beam
    {
        Entity parent;
        Color color;
        float length;
        public int damage = 1;
        int lifeTime = 10;
        int immunityFrames = 10;
        float width = 1f;
        public Beam(Entity parent, Color color, float length = 100, int lifeTime = 10, int immunityFrames = -1, int damage = 1, float width = 1)
        {
            this.parent = parent;
            this.color = color;
            this.length = length;
            this.lifeTime = lifeTime;
            this.immunityFrames = immunityFrames;
            this.damage = damage;
            this.width = width;
        }
        Vector2 position;
        float rotation;
        int frames = 0;
        public bool Update(Vector2 position, float rotation)
        {
            this.position = position;
            this.rotation = rotation;
            if (frames > 0)
            {
                frames--;
            }
            lifeTime--;
            if (lifeTime < 0)
            {
                return false;
            }
            return true;
        }
        public Line GetLine()
        {
            return new Line(position, position + Functions.PolarVector(length, rotation));
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (sLine != null)
            {
                sLine.Draw(spriteBatch, color, width);
                sLine = null;
                return;
            }
            GetLine().Draw(spriteBatch, color, width);
        }
        Line sLine = null;
        List<Entity> hitThese = new List<Entity>();
        public void ProcessCollision()
        {
            Line line = GetLine();
            Vector2? closestHitSpot = null;
            Entity closestHitEntity = null;
            for (int i = 0; i < Arena.entities.Count; i++)
            {
                if (Arena.entities[i].health > 0 && Arena.entities[i].team != parent.team)
                {
                    Shape[] hit = Arena.entities[i].AllHitboxes();
                    for (int k = 0; k < hit.Length; k++)
                    {
                        if (hit[k].Colliding(line))
                        {

                            if (!(Arena.entities[i] is Projectile) || (Arena.entities[i].health > damage))
                            {
                                Vector2? hitAt = line.GetFirstHit(hit[k]);
                                if (hitAt != null)
                                {
                                    if (closestHitSpot == null || ((position - (Vector2)hitAt).Length() < (position - (Vector2)closestHitSpot).Length()))
                                    {
                                        closestHitSpot = (Vector2)hitAt;
                                        closestHitEntity = Arena.entities[i];
                                    }
                                }

                            }
                            else
                            {
                                CollisionEvent.DamagingHit(Arena.entities[i], damage, line.Rotation());
                            }
                            break;
                        }
                    }
                }
            }
            if (closestHitEntity != null)
            {
                //if (frames == 0)
                if(!hitThese.Contains(closestHitEntity))
                {
                    CollisionEvent.DamagingHit(closestHitEntity, damage, line.Rotation());
                    hitThese.Add(closestHitEntity);
                    if(closestHitEntity is Platform)
                    {
                        hitThese.Add(((Platform)closestHitEntity).parent);
                    }
                }
                Vector2 diff = (((Vector2)closestHitSpot) - line.GetStart());
                diff.Normalize();
                diff *= 2;
                sLine = new Line(line.GetStart(), (Vector2)closestHitSpot + diff);
                new Particle((Vector2)closestHitSpot, Main.random.Next(4) + 2, color, Functions.PolarVector(4, rotation + (float)Math.PI));
                //new Particle((Vector2)closestHitSpot, 30, color);
                frames = immunityFrames;
            }

        }
    }
}
