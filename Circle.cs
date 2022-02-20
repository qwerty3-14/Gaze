using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze
{
    public class Circle : Shape
    {
        Vector2 position;
        float radius;
        public Circle(Vector2 position, float radius)
        {
            this.position = position; this.radius = radius;
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
            return otherPolygon.Colliding(this);
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
                spriteBatch.Draw(circleTexture, position, null, null, Vector2.One * 1000, 0, Vector2.One * (radius / 1000), color, 0, 0);
            }
        }
        Texture2D d;
        public void LegacyDraw(SpriteBatch spriteBatch, Color color)
        {
            if (radius > 0)
            {
                if (d != null)
                {
                    d.Dispose();
                }
                int size = (int)radius * 2;
                Color[] dataColors = new Color[size * size];
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        int x = i;
                        int y = j;
                        int centerX = (int)radius;
                        int centerY = (int)radius;
                        if ((new Vector2(x, y) - new Vector2(centerX, centerY)).Length() < radius)
                        {
                            //Debug.WriteLine((new Vector2(x, y) - new Vector2(centerX, centerY)).Length() + ", " + radius);
                            dataColors[x + y * size] = color;
                        }
                    }
                }
                d = new Texture2D(Main.graphicsDevice, size, size);
                d.SetData(0, null, dataColors, 0, size * size);
                spriteBatch.Draw(d, position, null, null, Vector2.One * radius, 0, Vector2.One, Color.White, 0, 0);
            }
            //d.Dispose();
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
