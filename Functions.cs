using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL
{
    
    public static class Functions 
    {
        public static void ProximityExplosion(Circle explosion, int damage, int team, bool fallOff = false)
        {
            for (int i = 0; i < Arena.entities.Count; i++)
            {
                if (Arena.entities[i].team != team && !Arena.entities[i].ignoreMe)
                {
                    Shape[] col = Arena.entities[i].AllHitboxes();
                    for (int k = 0; k < col.Length; k++)
                    {
                        if (col[k].Colliding(explosion))
                        {
                            int locDamage = damage;
                            if(fallOff)
                            {
                                Vector2 v = (Functions.screenLoopAdjust(explosion.GetPosition(), Arena.entities[i].position)) - explosion.GetPosition();
                                float dist = v.Length();
                                if(dist < explosion.GetRadius() * 0.33f)
                                {
                                    locDamage = damage;
                                }
                                else
                                {
                                    locDamage = Math.Max(1, (int)(damage * (1f - (dist - explosion.GetRadius() * 0.33f) / (explosion.GetRadius() * 0.67f))));
                                }

                            }
                            if (Arena.entities[i].mass != 0)
                            {
                                Vector2 v = (Functions.screenLoopAdjust(explosion.GetPosition(), Arena.entities[i].position)) - explosion.GetPosition();
                                v.Normalize();
                                v *= (locDamage * 4) / Arena.entities[i].mass;
                                Arena.entities[i].velocity += v;
                            }
                            CollisionEvent.DamagingHit(Arena.entities[i], locDamage, (Functions.screenLoopAdjust(explosion.GetPosition(), Arena.entities[i].position) - explosion.GetPosition()).ToRotation());
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// give an angle to shoot at to attempt to hit a moving target, returns NaN when this is impossible
        /// </summary>
        public static float PredictiveAim(Vector2 shootFrom, float shootSpeed, Vector2 targetPos, Vector2 targetVelocity)
        {
            float angleToTarget = (targetPos - shootFrom).ToRotation();
            float targetTraj = targetVelocity.ToRotation();
            float targetSpeed = targetVelocity.Length();
            float dist = (targetPos - shootFrom).Length();

            //imagine a tirangle between the shooter, its target and where it think the target will be in the future
            // we need to find an angle in the triangle z this is the angle located at the target's corner
            float z = (float)Math.PI + (targetTraj - angleToTarget);

            //with this angle z we can now use the law of cosines to find time
            //the side opposite of z is equal to shootSpeed * time
            //the other sides are dist and targetSpeed * time
            //putting these values into law of cosines gets (shootSpeed * time)^2 = (targetSpeed * time)^2 + dist^2 -2*targetSpeed*time*cos(z)
            //we can rearange it to (shootSpeed^2 - targetSpeed^2)time^2 + 2*targetSpeed*dist*cos(z)*time - dist^2 = 0, this is a quadratic!

            //here we use the quadratic formula to find time
            float a = shootSpeed * shootSpeed - targetSpeed * targetSpeed;
            float b = 2 * targetSpeed * dist * (float)Math.Cos(z);
            float c = -(dist * dist);
            float time = (-b + (float)Math.Sqrt(b * b - 4 * a * c)) / (2 * a);

            //we now know the time allowing use to find all sides of the tirangle, now we use law of Sines to calculate the angle to shoot at.
            float calculatedShootAngle = angleToTarget - (float)Math.Asin((targetSpeed * time * (float)Math.Sin(z)) / (shootSpeed * time));
            return calculatedShootAngle;
        }

        /// <summary>
        /// give an angle to shoot at to attempt to hit a moving target, returns NaN when this is impossible, includes a shoot Offest
        /// </summary>
        public static float PredictiveAimWithOffset(Vector2 shootFrom, float shootSpeed, Vector2 targetPos, Vector2 targetVelocity, float shootOffset)
        {
            float angleToTarget = (targetPos - shootFrom).ToRotation();
            float targetTraj = targetVelocity.ToRotation();
            float targetSpeed = targetVelocity.Length();
            float dist = (targetPos - shootFrom).Length();
            if (dist < shootOffset)
            {
                shootOffset = 0;
            }

            //imagine a tirangle between the shooter, its target and where it think the target will be in the future
            // we need to find an angle in the triangle z this is the angle located at the target's corner
            float z = (float)Math.PI + (targetTraj - angleToTarget);

            //with this angle z we can now use the law of cosines to find time
            //the side opposite of z is equal to shootSpeed * time
            //the other sides are dist and targetSpeed * time
            //putting these values into law of cosines gets (shootSpeed * time + shootOffset)^2 = (targetSpeed * time)^2 + dist^2 -2*targetSpeed*time*cos(z)
            //we can rearange it to (shootSpeed^2 - targetSpeed^2)time^2 + (2*targetSpeed*dist*cos(z) + 2*shootOffest*shootSpeed)*time + shootOffset^2 - dist^2 = 0, this is a quadratic!

            //here we use the quadratic formula to find time
            float a = shootSpeed * shootSpeed - targetSpeed * targetSpeed;
            float b = 2 * targetSpeed * dist * (float)Math.Cos(z) + 2 * shootOffset * shootSpeed;
            float c = (shootOffset * shootOffset) - (dist * dist);
            float time = (-b + (float)Math.Sqrt(b * b - 4 * a * c)) / (2 * a);

            //we now know the time allowing use to find all sides of the tirangle, now we use law of Sines to calculate the angle to shoot at.
            float calculatedShootAngle = angleToTarget - (float)Math.Asin((targetSpeed * time * (float)Math.Sin(z)) / (shootSpeed * time));
            return calculatedShootAngle;
        }
        public static Vector2[] OffsetsForDrawing()
        {
            Vector2[] offsets = new Vector2[9];
            for (int i = 0; i < 9; i++)
            {
                Vector2 offset = Vector2.Zero;
                switch (i)
                {
                    case 1:
                        offset = Vector2.UnitX * Arena.boundrySize;
                        break;
                    case 2:
                        offset = Vector2.UnitY * Arena.boundrySize;
                        break;
                    case 3:
                        offset = Vector2.UnitX * -Arena.boundrySize;
                        break;
                    case 4:
                        offset = Vector2.UnitY * -Arena.boundrySize;
                        break;
                    case 5:
                        offset = Vector2.UnitX * Arena.boundrySize + Vector2.UnitY * Arena.boundrySize;
                        break;
                    case 6:
                        offset = Vector2.UnitY * Arena.boundrySize + Vector2.UnitX * -Arena.boundrySize;
                        break;
                    case 7:
                        offset = Vector2.UnitX * -Arena.boundrySize + Vector2.UnitY * -Arena.boundrySize;
                        break;
                    case 8:
                        offset = Vector2.UnitY * -Arena.boundrySize + Vector2.UnitX * Arena.boundrySize;
                        break;
                }
                offsets[i] = offset;
            }
            return offsets;
        }
        public static Vector2 LoopAroundCheck(Vector2 Position)
        {
            if (Position.X < 0)
            {
                Position.X += Arena.boundrySize;
            }
            if (Position.X > Arena.boundrySize)
            {
                Position.X -= Arena.boundrySize;
            }
            if (Position.Y < 0)
            {
                Position.Y += Arena.boundrySize;
            }
            if (Position.Y > Arena.boundrySize)
            {
                Position.Y -= Arena.boundrySize;
            }
            return Position;
        }
        public static Vector2 screenLoopAdjust(Vector2 myPosition, Vector2 targetPosition)
        {
            float arenaWidth = Arena.boundrySize;
            if (myPosition.X - targetPosition.X > arenaWidth / 2)
            {
                targetPosition.X = arenaWidth + targetPosition.X;
            }
            else if (myPosition.X - targetPosition.X < -arenaWidth / 2)
            {
                targetPosition.X = -(arenaWidth - targetPosition.X);
            }
            float arenaHeight = Arena.boundrySize;
            if (myPosition.Y - targetPosition.Y > arenaHeight / 2)
            {
                targetPosition.Y = arenaHeight + targetPosition.Y;
            }
            else if (myPosition.Y - targetPosition.Y < -arenaHeight / 2)
            {
                targetPosition.Y = -(arenaHeight - targetPosition.Y);
            }
            return targetPosition;
        }
        public static void Rotate(this ref Vector2 v, float amount)
        {
            float len = v.Length();
            float ang = v.ToRotation();
            v = PolarVector(len, ang + amount);
        }

        public static Vector2 PolarVector(float radius, float theta)
        {
            return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;
        }
        public static float ToRotation(this Vector2 v)
        {
            return (float)Math.Atan2((double)v.Y, (double)v.X);
        }
        public static void SlowRotation(this ref float currentRotation, float targetAngle, float speed)
        {
            int f = 1; //this is used to switch rotation direction
            float actDirection = new Vector2((float)Math.Cos(currentRotation), (float)Math.Sin(currentRotation)).ToRotation();
            targetAngle = new Vector2((float)Math.Cos(targetAngle), (float)Math.Sin(targetAngle)).ToRotation();
            //this makes f 1 or -1 to rotate the shorter distance
            if (Math.Abs(actDirection - targetAngle) > Math.PI)
            {
                f = -1;
            }
            else
            {
                f = 1;
            }

            if (actDirection <= targetAngle + speed * 2 && actDirection >= targetAngle - speed * 2)
            {
                actDirection = targetAngle;

            }
            else if (actDirection <= targetAngle)
            {
                actDirection += speed * f;
            }
            else if (actDirection >= targetAngle)
            {
                actDirection -= speed * f;
            }
            actDirection = new Vector2((float)Math.Cos(actDirection), (float)Math.Sin(actDirection)).ToRotation();
            currentRotation = actDirection;

        }
        public static float AngularDifference(float angle1, float angle2)
        {
            while (angle1 > (float)Math.PI)
            {
                angle1 -= (float)Math.PI * 2;
            }
            while (angle2 > (float)Math.PI)
            {
                angle2 -= (float)Math.PI * 2;
            }
            while (angle1 < -(float)Math.PI)
            {
                angle1 += (float)Math.PI * 2;
            }
            while (angle2 < -(float)Math.PI)
            {
                angle2 += (float)Math.PI * 2;
            }
            if (Math.Abs(angle1 - angle2) > Math.PI)
            {
                return (float)Math.PI * 2 - Math.Abs(angle1 - angle2);
            }
            return Math.Abs(angle1 - angle2);
        }
        // based on code from https://stackoverflow.com/questions/2353211/hsl-to-rgb-color-conversion
        public static Color ToRgb(float h, float s, float l)
        {
            float r, g, b;

            if (s == 0f)
            {
                r = g = b = l; // achromatic
            }
            else
            {
                float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                r = hueToRgb(p, q, h + 1f / 3f);
                g = hueToRgb(p, q, h);
                b = hueToRgb(p, q, h - 1f / 3f);
            }


            return new Color(r, g, b);
        }
        static int to255(float v) { return (int)Math.Min(255, 256 * v); }

        /** Helper method that converts hue to rgb */
        static float hueToRgb(float p, float q, float t)
        {
            if (t < 0f)
                t += 1f;
            if (t > 1f)
                t -= 1f;
            if (t < 1f / 6f)
                return p + (q - p) * 6f * t;
            if (t < 1f / 2f)
                return q;
            if (t < 2f / 3f)
                return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }
    }
}
