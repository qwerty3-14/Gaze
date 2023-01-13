using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL
{
    public class Line
    {
        Vector2 start;
        Vector2 end;
        public Rectangle boundingBox;
        
        public Line(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
            Rebound();
        }
        void Rebound()
        {
            int x = (int)Math.Min(start.X, end.X);
            int y = (int)Math.Min(start.Y, end.Y);
            int width = (int)Math.Abs(start.X - end.X);
            int height = (int)Math.Abs(start.Y - end.Y);
            boundingBox = new Rectangle(x, y, width, height);
        }
        Rectangle OffsetBoundingBox(Vector2 offset)
        {
            int x = (int)Math.Min(start.X, end.X) + (int)offset.X;
            int y = (int)Math.Min(start.Y, end.Y) + (int)offset.Y;
            int width = (int)Math.Abs(start.X - end.X);
            int height = (int)Math.Abs(start.Y - end.Y);
            return new Rectangle(x, y, width, height);
        }
        public bool Colliding(Line otherLine, ref Vector2 intersection)
        {
            if (this.boundingBox.Intersects(otherLine.boundingBox))
            {
                float uA = ((otherLine.end.X - otherLine.start.X) * (this.start.Y - otherLine.start.Y) - (otherLine.end.Y - otherLine.start.Y) * (this.start.X - otherLine.start.X)) / ((otherLine.end.Y - otherLine.start.Y) * (this.end.X - this.start.X) - (otherLine.end.X - otherLine.start.X) * (this.end.Y - this.start.Y));
                float uB = ((this.end.X - this.start.X) * (this.start.Y - otherLine.start.Y) - (this.end.Y - this.start.Y) * (this.start.X - otherLine.start.X)) / ((otherLine.end.Y - otherLine.start.Y) * (this.end.X - this.start.X) - (otherLine.end.X - otherLine.start.X) * (this.end.Y - this.start.Y));
                if(uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
                {
                    intersection.X = this.start.X + (uA * (this.end.X - this.start.X));
                    intersection.Y = this.start.Y + (uA * (this.end.Y - this.start.Y));
                    return true;
                }
            }
            return false;
        }
        public bool Colliding(Line otherLine)
        {
            if(this.boundingBox.Intersects(otherLine.boundingBox))
            {
                float uA = ((otherLine.end.X - otherLine.start.X) * (this.start.Y - otherLine.start.Y) - (otherLine.end.Y - otherLine.start.Y) * (this.start.X - otherLine.start.X)) / ((otherLine.end.Y - otherLine.start.Y) * (this.end.X - this.start.X) - (otherLine.end.X - otherLine.start.X) * (this.end.Y - this.start.Y));
                float uB = ((this.end.X - this.start.X) * (this.start.Y - otherLine.start.Y) - (this.end.Y - this.start.Y) * (this.start.X - otherLine.start.X)) / ((otherLine.end.Y - otherLine.start.Y) * (this.end.X - this.start.X) - (otherLine.end.X - otherLine.start.X) * (this.end.Y - this.start.Y));
                return uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1;
                
            }
            return false;
        }
        public Vector2? GetFirstHit(Shape shape)
        {
            if (shape is Polygon)
            {
                Polygon polygon = (Polygon)shape;
                List<Vector2> outputs = new List<Vector2>();
                Line[] edges = polygon.Edges();
                for (int i = 0; i < edges.Length; i++)
                {
                    Vector2 intersect = Vector2.Zero;
                    if (this.Colliding(edges[i], ref intersect))
                    {
                        outputs.Add(intersect);
                    }
                }
                float max = Length() + 2;
                Vector2? closest = null;
                for (int i = 0; i < outputs.Count; i++)
                {
                    float dist = (outputs[i] - start).Length();
                    if (dist < max)
                    {
                        closest = outputs[i];
                        max = dist;
                    }
                }
                return closest;
            }
            return null;
        }
        public float Length()
        {
            return (end - start).Length();
        }
        public float Rotation()
        {
            return (end - start).ToRotation();
        }
        public Vector2 GetStart()
        {
            return start;
        }
        public Vector2 GetEnd()
        {
            return end;
        }
        public void ChangeEnd(Vector2 end)
        {
            this.end = end;
            Rebound();
        }
        public void Draw(SpriteBatch spriteBatch, Color color, float width = 1)
        {
            Vector2[] offsets = Functions.OffsetsForDrawing();
            for (int i = 0; i < 9; i++)
            {
                Rectangle screenView = new Rectangle((int)Camera.screenPosition.X, (int)Camera.screenPosition.Y, (int)Camera.CameraWorldSize, (int)Camera.CameraWorldSize);
                if (screenView.Intersects(OffsetBoundingBox(offsets[i])))
                {
                    spriteBatch.Draw(Main.pixel, Camera.CameraOffset(start + offsets[i]), null, color, (end - start).ToRotation(), new Vector2(0, .5f), new Vector2((end - start).Length(), width), SpriteEffects.None, 0);
                }

            }
            
        }

    }
}
