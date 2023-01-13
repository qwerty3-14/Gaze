using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities.Projectiles
{
    class Tripwire : Projectile
    {

        public Tripwire(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            shape = new Polygon(new Vector2[]
                {
                    new Vector2(length/2, 1.5f),
                    new Vector2(length/2, -1.5f),
                    new Vector2(-length/2, -1.5f),
                    new Vector2(-length/2, 1.5f)
                });
            health = -1;
            lifeTime = 40 * 60;
            damage = 0;
            mass = 0;
        }
        float length = 0;
        public override void LocalUpdate()
        {
            if (length >= 100)
            {
                length = 100;
            }
            else
            {
                length += 100f / 30f;
                shape = new Polygon(new Vector2[]
                {
                    new Vector2(length/2, 1.5f),
                    new Vector2(length/2, -1.5f),
                    new Vector2(-length/2, -1.5f),
                    new Vector2(-length/2, 1.5f)
                });
            }
            

        }
        int counter;

        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            counter++;
            spriteBatch.Draw(AssetManager.projectiles[1], pos + Functions.PolarVector(length / 2, rotation), null, Color.White, rotation, new Vector2(0f, 2.5f), Vector2.One, SpriteEffects.None, 0f);
            spriteBatch.Draw(AssetManager.projectiles[1], pos + Functions.PolarVector(length / 2, rotation + (float)Math.PI), null, Color.White, rotation, new Vector2(0f, 2.5f), Vector2.One, SpriteEffects.None, 0f);
            //spriteBatch.Draw(AssetManager.projectiles[1], pos + Functions.PolarVector(length/2, rotation), null, null, new Vector2(0, 2.5f), rotation, Vector2.One, Color.White, 0, 0);
            //spriteBatch.Draw(AssetManager.projectiles[1], pos + Functions.PolarVector(length / 2, rotation + (float)Math.PI), null, null, new Vector2(0, 2.5f), rotation + (float)Math.PI, Vector2.One, Color.White, 0, 0);
            for (int i = 0; i < length; i++)
            {
                spriteBatch.Draw(AssetManager.projectiles[2], pos + Functions.PolarVector(length / 2 - i - 1, rotation), new Rectangle((i + counter) % 6, 0, 1, 3), Color.White, rotation, new Vector2(0f, 1.5f), Vector2.One, SpriteEffects.None, 0f);
                //spriteBatch.Draw(texture: AssetManager.projectiles[2], position: pos + Functions.PolarVector(length / 2 - i - 1, rotation), origin: new Vector2(0, 1.5f), rotation: rotation, sourceRectangle: new Rectangle((i + counter) % 6, 0, 1, 3), color: Color.White);
            }
        }
        public override void OnHit(Entity Victim)
        {
            Victim.StunTime = 180;
            AssetManager.PlaySound(SoundID.Zap);
        }
    }
}
