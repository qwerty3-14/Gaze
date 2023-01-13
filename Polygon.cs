using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL
{
    public class Polygon : Shape
    {
        protected Vector2[] vertices;
        Rectangle boundingBox;
        public Polygon(Vector2[] vertices)
        {
            this.vertices = vertices;
            Rebound();
        }
        void Rebound()
        {
            float minX = vertices[0].X;
            float maxX = vertices[0].X;
            float minY = vertices[0].Y;
            float maxY = vertices[0].Y;
            for (int i = 1; i < vertices.Length; i++)
            {
                if(minX > vertices[i].X)
                {
                    minX = vertices[i].X;
                }
                if (maxX < vertices[i].X)
                {
                    maxX = vertices[i].X;
                }
                if (minY > vertices[i].Y)
                {
                    minY = vertices[i].Y;
                }
                if (maxY < vertices[i].Y)
                {
                    maxY = vertices[i].Y;
                }
            }
            int x = (int)minX;
            int y = (int)minY;
            int width = (int)(maxX - minX);
            int height = (int)Math.Abs(maxY - minY);
            boundingBox = new Rectangle(x, y, width, height);
            cachedLines = null;
        }
        public override void Move(Vector2 amount)
        {
            for(int i =0; i < vertices.Length; i++)
            {
                vertices[i] += amount;
            }
            Rebound();
        }
        public override void Rotate(Vector2 center, float amount)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 diff = vertices[i] - center;
                diff.Rotate(amount);
                vertices[i] = center + diff;
            }
            Rebound();
        }
        Line[] cachedLines;
        public Line[] Edges()
        {
            if(cachedLines != null)
            {
                return cachedLines;
            }
            Line[] lines = new Line[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 start = vertices[i];
                int next = i + 1;
                if(next >= vertices.Length)
                {
                    next = 0;
                }
                Vector2 end = vertices[next];
                lines[i] = new Line(start, end);
            }
            cachedLines = lines;
            return lines;
        }
        public override bool Colliding(Polygon otherPolygon)
        {
            if(BoundingCollision(otherPolygon))
            {
                Line[] edges = Edges();
                for(int i =0; i < edges.Length; i++)
                {
                    if(otherPolygon.Colliding(edges[i]))
                    {
                        return true;
                    }
                }
                return Colliding(otherPolygon.vertices[0]) || otherPolygon.Colliding(vertices[0]);
            }
            return false;
        }
        public override bool Colliding(Line otherLine)
        {
            if(BoundingCollision(otherLine))
            {
                Line[] edges = Edges();
                for(int i =0; i < edges.Length; i++)
                {
                    if(otherLine.Colliding(edges[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override bool Colliding(Vector2 point)
        {
            bool collision = false;

            // go through each of the vertices, plus
            // the next vertex in the list
            int next = 0;
            for (int current = 0; current < vertices.Length; current++)
            {

                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current + 1;
                if (next == vertices.Length)
                {
                    next = 0;
                }

                Vector2 vc = vertices[current];    // c for "current"
                Vector2 vn = vertices[next];       // n for "next"

                // compare position, flip 'collision' variable
                // back and forth
                float px = point.X;
                float py = point.Y;
                if (((vc.Y >= py && vn.Y < py) || (vc.Y < py && vn.Y >= py)) &&
                    (px < (vn.X - vc.X) * (py - vc.Y) / (vn.Y - vc.Y) + vc.X))
                {
                    collision = !collision;
                }
            }
            return collision;
        }
        public override bool Colliding(Circle otherCircle)
        {
            Line[] edges = Edges();
            for (int i = 0; i < edges.Length; i++)
            {
                if (otherCircle.Colliding(edges[i]))
                {
                    return true;
                }
            }

            if (Colliding(otherCircle.GetPosition()))
            {
                return true;
            }
            return false;
        }
        bool BoundingCollision(Line otherline)
        {
            return otherline.boundingBox.Intersects(boundingBox);
        }
        bool BoundingCollision(Polygon otherPolygon)
        {
            return otherPolygon.boundingBox.Intersects(boundingBox);
        }

        public override void Draw(SpriteBatch spriteBatch, Color color)
        {
            Line[] edges = Edges();
            for (int i = 0; i < edges.Length; i++)
            {
                edges[i].Draw(spriteBatch, color);
            }
        }
        public Vector2 GetVertex(int id)
        {
            return vertices[id];
        }
        public override Shape Clone()
        {
            Vector2[] cloneVertices = new Vector2[vertices.Length];
            for(int i = 0; i < vertices.Length; i++)
            {
                cloneVertices[i] = vertices[i];
            }
            return new Polygon(cloneVertices);
        }
        public override Rectangle GetBounds()
        {
            return boundingBox;
        }
        public Vector2 Center()
        {
            Vector2 output = Vector2.Zero;
            for(int i =0; i < vertices.Length; i++)
            {
                output += vertices[i];
            } 
            output /= vertices.Length;
            return output;
        }
    }
}
