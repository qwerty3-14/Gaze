using Microsoft.Xna.Framework;
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
    public class CollisionEvent
    {
        Entity one;
        Entity two;
        public CollisionEvent(Entity one, Entity two)
        {
            this.one = one;
            this.two = two;
            Main.CEs.Add(this);
        }
        public static bool DamagingHit(Entity victim, int damage, float hitDirection)
        {
            if (victim is Projectile)
            {
                if (victim.health != -1)
                {
                    if (damage >= ((Projectile)victim).health)
                    {
                        victim.Kill();
                    }
                }
            }
            else
            {
                if (victim is Ship)
                {
                    ((Ship)victim).ModifyHitByProjectile(ref damage, hitDirection);
                    ((Ship)victim).HitByProjectile(damage);
                }
                if (!victim.invulnerable)
                {
                    victim.health -= damage;
                }
                if (victim.health <= 0)
                {
                    victim.Kill();
                }
                return true;
            }
            return false;
        }
        public void Process()
        {
            float v1 = one.velocity.Length();
            float v2 = two.velocity.Length();
            float m1 = one.mass;
            float m2 = two.mass;
            float theta1 = one.velocity.ToRotation();
            float theta2 = two.velocity.ToRotation();
            float contactAngle1 = (Functions.screenLoopAdjust(one.position, two.position) - one.position).ToRotation();
            float contactAngle2 = (Functions.screenLoopAdjust(two.position, one.position) - two.position).ToRotation();
            if (m1 != 0 && m2 != 0)
            {
                if (one.incorpreal == 0 && two.incorpreal == 0)
                {
                    one.position -= one.velocity;
                    two.position -= two.velocity;
                }
            }
            if (one is Projectile)
            {
                if (!(two is Projectile))
                {
                    ((Projectile)one).OnHit(two);
                }
                if (DamagingHit(two, ((Projectile)one).damage, contactAngle1))
                {
                    if(!one.invulnerable)
                    {
                        one.Kill();
                    }
                }
            }
            else if (m1 != 0 && m2 != 0)
            {
                if (one.incorpreal == 0 && two.incorpreal == 0)
                {
                    float pre1 = ((v1 * (float)Math.Cos(theta1 - contactAngle1) * (m1 - m2) + 2 * m2 * v2 * (float)Math.Cos(theta2 - contactAngle1)) / (m1 + m2)) * (float)Math.Cos(contactAngle1);
                    one.velocity.X = pre1 + v1 * (float)Math.Sin(theta1 - contactAngle1) * (float)Math.Cos(contactAngle1 + (float)Math.PI / 2f);
                    one.velocity.Y = pre1 + v1 * (float)Math.Sin(theta1 - contactAngle1) * (float)Math.Sin(contactAngle1 + (float)Math.PI / 2f);
                }
            }
            if (two is Projectile)
            {
                if (!(one is Projectile))
                {
                    ((Projectile)two).OnHit(one);
                }
                if (DamagingHit(one, ((Projectile)two).damage, contactAngle2))
                {
                    if (!two.invulnerable)
                    {
                        two.Kill();
                    }
                }
            }
            else if (m1 != 0 && m2 != 0)
            {
                if (one.incorpreal == 0 && two.incorpreal == 0)
                {
                    float pre2 = ((v2 * (float)Math.Cos(theta2 - contactAngle2) * (m2 - m1) + 2 * m1 * v1 * (float)Math.Cos(theta1 - contactAngle2)) / (m2 + m1)) * (float)Math.Cos(contactAngle2);
                    two.velocity.X = pre2 + v2 * (float)Math.Sin(theta2 - contactAngle2) * (float)Math.Cos(contactAngle2 + (float)Math.PI / 2f);
                    two.velocity.Y = pre2 + v2 * (float)Math.Sin(theta2 - contactAngle2) * (float)Math.Sin(contactAngle2 + (float)Math.PI / 2f);
                }
            }
            if(!(one is Projectile) && !(two is Projectile))
            {
                if (one.incorpreal == 0 && two.incorpreal == 0)
                {
                    one.incorpreal = two.incorpreal = 10;
                }
            }
        }
    }
}
