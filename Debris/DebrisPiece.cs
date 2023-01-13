using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GazeOGL.Entities;

namespace GazeOGL.Debris
{
    public class DebrisPiece : Entity
    {
        Texture2D texture;
        float rotSpeed = 0;
        public DebrisPiece(Texture2D texture, Polygon shape, Vector2 position, Vector2 velocity, float rotation, float rotSpeed = 0)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
            this.rotation = rotation;
            this.rotSpeed = rotSpeed;
            this.shape = shape.Clone();
            this.ignoreMe = true;
            this.shape.Move(texture.Bounds.Size.ToVector2() / -2f);
            mass = 2;
            this.incorpreal = -1;
            this.team = 2;
        }
        int counter = 0;
        public override void LocalUpdate()
        {
            rotation += rotSpeed;
            counter++;
            if(counter > 600)
            {
                Kill();
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            float c = 1f - (1f * (counter / 600f));
            spriteBatch.Draw(texture, pos, null, new Color(c, c, c, c), rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), Vector2.One, SpriteEffects.None, 0f);
            //this.Hitbox().Draw(spriteBatch, Color.White);
        }
    }
}