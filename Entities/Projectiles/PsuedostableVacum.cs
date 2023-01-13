using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities.Projectiles
{
    public class PsuedostableVacum : Projectile
    {
        Entity parent;
        public PsuedostableVacum(Vector2 position, Vector2 velocity, Entity parent, int team = 0) : base(position, velocity, team)
        {
            damage = 30;
            health = 30;
            invulnerable = true;
            //3.5 1.5
            shape = new Polygon(new Vector2[]
             {
                new Vector2(1f, -8f),
                new Vector2(-8f, -18f),
                new Vector2(-8f, 18f),
                new Vector2(1f, 8f),
             });
            mass = 0f;
            lifeTime = 3 * 60 * 60;
            this.parent = parent;
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[18], pos, null, Color.White, rotation, new Vector2(8.5f, 19f), Vector2.One, SpriteEffects.None, 0f);
        }
        public override void LocalUpdate()
        {
            if(parent == null || !Arena.entities.Contains(parent))
            {
                Kill();
            }
        }
    }
}
