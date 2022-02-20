using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze
{
    public abstract class Shape
    {
        public virtual void Move(Vector2 amount)
        {
            
        }
        public virtual void Rotate(Vector2 center, float amount)
        {
            //lol
        }
        public virtual bool Colliding(Polygon otherPolygon)
        {
            return false;
        }
        public virtual bool Colliding(Line otherLine)
        {
            return false;

        }
        public virtual bool Colliding(Vector2 point)
        {
            return false;
        }
        public virtual bool Colliding(Circle otherCircle)
        {
            return false;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Color color)
        {

        }
        public virtual Shape Clone()
        {
            return this;
        }
        public virtual Rectangle GetBounds()
        {
            return new Rectangle(1, 1, 1, 1);
        }
        public bool Colliding(Shape otherShape)
        {
            if(otherShape is Circle)
            {
                return Colliding((Circle)otherShape);
            }
            if(otherShape is Polygon)
            {
                return Colliding((Polygon)otherShape);
            }
            return false;
        }
    }
}
