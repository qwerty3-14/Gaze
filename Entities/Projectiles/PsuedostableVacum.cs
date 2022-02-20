using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Entities.Projectiles
{
    public class PsuedostableVacum : Projectile
    {
        public PsuedostableVacum(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
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
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[18], pos, null, null, new Vector2(8.5f, 19f), rotation, Vector2.One, Color.White, 0, 0);
        }
        public override void LocalUpdate()
        {
            if(Main.ships[team] == null)
            {
                Kill();
            }
        }
    }
}
