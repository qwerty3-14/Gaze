using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GazeOGL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL
{
    public abstract class Turret
    {
        protected float rotation;
        protected float homeRotation;
        Vector2 anchorAt;
        protected Vector2 relativePosition;
        protected float rotSpeed = (float)Math.PI / 30f;
        protected Entity parent;


        protected float turretLength;
        protected Texture2D texture;
        protected Vector2 origin;
        public Turret(Entity parent, Vector2 anchorAt, float homeRotation = 0)
        {
            this.parent = parent;
            this.anchorAt = anchorAt;
            this.homeRotation = homeRotation;
        }
        public bool AimHome()
        {
            float old = rotation;
            rotation.SlowRotation(homeRotation, rotSpeed);
            return rotation == old;
        }
        public bool AimAt(Vector2 here)
        {
            rotation.SlowRotation((here - AbsolutePosition()).ToRotation() - parent.rotation, rotSpeed);
            return Functions.AngularDifference(rotation, (here - AbsolutePosition()).ToRotation() - parent.rotation) < rotSpeed * 2;
        }
        public bool AimAt(float here)
        {
            rotation.SlowRotation(here - parent.rotation, rotSpeed);
            return Functions.AngularDifference(rotation, here - parent.rotation) < rotSpeed * 2;
        }
        public virtual void UpdateRelativePosition(Vector2? move = null)
        {
            if (move != null)
            {
                anchorAt = (Vector2)move;
            }
            relativePosition = Functions.PolarVector(anchorAt.X, parent.rotation) + Functions.PolarVector(anchorAt.Y, parent.rotation + (float)Math.PI / 2);
            Update();
        }
        public float AbsoluteRotation()
        {
            return rotation + parent.rotation;
        }
        public Vector2 AbsolutePosition()
        {
            return relativePosition + parent.position;
        }
        public virtual Vector2 AbsoluteShootPosition()
        {
            return AbsolutePosition() + Functions.PolarVector(turretLength, AbsoluteRotation());
        }
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 Pos)
        {
            spriteBatch.Draw(texture, Pos + relativePosition, null, Color.White, AbsoluteRotation(), origin, new Vector2(1, 1), SpriteEffects.None, 0);
        }
        public virtual void Fire()
        {

        }
        public virtual void Update()
        {

        }
    }
}
