using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL
{
    public class Circle : Shape
    {
        Vector2 position;
        float radius;
        Polygon polyHitbox;
        public Circle(Vector2 position, float radius)
        {
            this.position = position; this.radius = radius;
            int polySideCount = 6 + (int)(radius / 100f);
            Vector2[] polySides = new Vector2[polySideCount];
            for(int i =0; i < polySideCount; i++)
            {
                polySides[i] = Functions.PolarVector(radius, ((float)i / polySideCount) * 2f * (float)Math.PI);
            }
            polyHitbox = new Polygon(polySides);
            polyHitbox.Move(position);
        }
        public Vector2 GetPosition()
        {
            return position;
        }
        public float GetRadius()
        {
            return radius;
        }
        public override void Move(Vector2 amount)
        {
            position += amount;
        }
        public override void Rotate(Vector2 center, float amount)
        {
            //lol
        }
        public override bool Colliding(Polygon otherPolygon)
        {
            return otherPolygon.Colliding(polyHitbox);
        }
        public override bool Colliding(Line otherLine)
        {
            if(Colliding(otherLine.GetStart()) || Colliding(otherLine.GetEnd()))
            {
                return true;
            }
            float len = otherLine.Length();
            float x1 = otherLine.GetStart().X;
            float y1 = otherLine.GetStart().Y;
            float x2 = otherLine.GetEnd().X;
            float y2 = otherLine.GetEnd().Y;
            float dot = (((position.X - x1) * (x2 - x1)) + ((position.Y - y1) * (y2 - y1))) / (len*len);

            float closestX = x1 + (dot * (x2 - x1));
            float closestY = y1 + (dot * (y2 - y1));
            Vector2 closest = new Vector2(closestX, closestY);

            if(!((otherLine.GetEnd() - closest).Length() + (otherLine.GetStart() - closest).Length() <= otherLine.Length() + 0.4f))
            {
                return false;
            }

            return (closest - position).Length() <= radius;

        }
        public override bool Colliding(Vector2 point)
        {
            return (point - position).Length() <= radius;
        }
        public override bool Colliding(Circle otherCircle)
        {
            return (otherCircle.position - position).Length() <= radius + otherCircle.radius;
        }
        static Texture2D circleTexture;
        public static void LoadDrawCircle()
        {
            float rad = 1000;
            int size = (int)rad * 2;
            Color[] dataColors = new Color[size * size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int x = i;
                    int y = j;
                    int centerX = (int)rad;
                    int centerY = (int)rad;
                    if ((new Vector2(x, y) - new Vector2(centerX, centerY)).Length() < rad)
                    {
                        //Debug.WriteLine((new Vector2(x, y) - new Vector2(centerX, centerY)).Length() + ", " + radius);
                        dataColors[x + y * size] = Color.White;
                    }
                }
            }
            circleTexture = new Texture2D(Main.graphicsDevice, size, size);
            circleTexture.SetData(0, null, dataColors, 0, size * size);
        }
        public override void Draw(SpriteBatch spriteBatch, Color color)
        {
            if (radius > 0)
            {
                spriteBatch.Draw(circleTexture, position, null, color,  0, Vector2.One * 1000, Vector2.One * (radius / 1000), 0, 0);
            }
        }
        public override Shape Clone()
        {
            return new Circle(position, radius);
        }
        public override Rectangle GetBounds()
        {
            Point topLeft = (position + Vector2.One * -radius).ToPoint();
            Point size = ((radius * 2) * Vector2.One).ToPoint();
            return new Rectangle(topLeft, size);
        }
    }
}
