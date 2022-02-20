using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectGaze.Entities;
using ProjectGaze.Entities.Projectiles;
using ProjectGaze.Entities.Ships;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze
{
    class Beam
    {
        Entity parent;
        Color color;
        float length;
        int damage = 1;
        int lifeTime = 10;
        int immunityFrames = 10;
        public Beam(Entity parent, Color color, float length = 100, int lifeTime = 10, int immunityFrames = -1, int damage = 1)
        {
            this.parent = parent;
            this.color = color;
            this.length = length;
            this.lifeTime = lifeTime;
            this.immunityFrames = immunityFrames;
            this.damage = damage;
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
                sLine.Draw(spriteBatch, color);
                sLine = null;
                return;
            }
            GetLine().Draw(spriteBatch, color);
        }
        Line sLine = null;
        List<Entity> hitThese = new List<Entity>();
        public void ProcessCollision()
        {
            Line line = GetLine();
            Vector2? closestHitSpot = null;
            Entity closestHitEntity = null;
            for (int i = 0; i < Main.entities.Count; i++)
            {
                if (Main.entities[i].health > 0 && Main.entities[i].team != parent.team)
                {
                    Shape[] hit = Main.entities[i].AllHitboxes();
                    for (int k = 0; k < hit.Length; k++)
                    {
                        if (hit[k].Colliding(line))
                        {

                            if (!(Main.entities[i] is Projectile))
                            {
                                Vector2? hitAt = line.GetFirstHit(hit[k]);
                                if (hitAt != null)
                                {
                                    if (closestHitSpot == null || ((position - (Vector2)hitAt).Length() < (position - (Vector2)closestHitSpot).Length()))
                                    {
                                        closestHitSpot = (Vector2)hitAt;
                                        closestHitEntity = Main.entities[i];
                                    }
                                }

                            }
                            else
                            {
                                CollisionEvent.DamagingHit(Main.entities[i], damage, line.Rotation());
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
