using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities.Projectiles
{
    public class Kugelblitz : Projectile
    {
        public const int KugelTime = 45;
        public Kugelblitz(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            damage = 0;
            health = -1;
            shape = new Circle(Vector2.Zero, 4);
            mass = 0;
            lifeTime = KugelTime;
            invulnerable = true;
        }
        int frame = 0;
        float outAmount = 12;
        float range = 0;
        float curRange = 0;
        public override void Expire()
        {
            //Functions.ProximityExplosion(new Circle(position, 60), 12, team, true);
            AssetManager.PlaySound(SoundID.Death);
            new Effect(position, 2);
        }
        int frameCounter = 0;
        public override void LocalUpdate()
        {
            outAmount = ((float)lifeTime / (float)KugelTime) * 12;
            curRange = ((float)lifeTime / (float)KugelTime) * range;
            frameCounter++;
            if(frameCounter % 5 == 0)
            {
                frame++;
                if(frame > 3)
                {
                    frame = 0;
                }
            }
        }
        public void Release(float range)
        {
            this.range = range;
            position += Functions.PolarVector(range, rotation);
            shape = new Circle(Vector2.Zero, 50);
            
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[14], pos + Functions.PolarVector(outAmount, rotation + (float)Math.PI / 2) + Functions.PolarVector(-curRange, rotation), new Rectangle(0, frame * 7, 7, 7), Color.White, rotation, new Vector2(3.5f, 3.5f), Vector2.One, SpriteEffects.None, 0f);
            spriteBatch.Draw(AssetManager.projectiles[14], pos + Functions.PolarVector(outAmount, rotation - (float)Math.PI / 2) + Functions.PolarVector(-curRange, rotation), new Rectangle(0, frame * 7, 7, 7), Color.White, rotation, new Vector2(3.5f, 3.5f), Vector2.One, SpriteEffects.None, 0f);

            //spriteBatch.Draw(AssetManager.projectiles[14], pos + Functions.PolarVector(outAmount, rotation + (float)Math.PI/2) + Functions.PolarVector(-curRange, rotation), null, new Rectangle(0, frame * 7, 7, 7), new Vector2(3.5f, 3.5f), rotation, Vector2.One, Color.White, 0, 0);
            //spriteBatch.Draw(AssetManager.projectiles[14], pos + Functions.PolarVector(outAmount, rotation - (float)Math.PI / 2) + Functions.PolarVector(-curRange, rotation), null, new Rectangle(0, frame * 7, 7, 7), new Vector2(3.5f, 3.5f), rotation, Vector2.One, Color.White, 0, 0);
        }


    }
}
