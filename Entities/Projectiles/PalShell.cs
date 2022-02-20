using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Entities.Projectiles
{
    class PalShell : Projectile
    {
        public PalShell(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            damage = 1;
            health = 1;
            shape = new Polygon(new Vector2[]
             {
                new Vector2(1.5f, -1.5f),
                new Vector2(1.5f, 1.5f),
                new Vector2(-3.5f, 1.5f),
                new Vector2(-3.5f, -1.5f)
             });
            mass = 0f;
            lifeTime = Main.random.Next(15) + Main.random.Next(15) + 15;
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[8], pos, null, null, new Vector2(3.5f, 1.5f), rotation, Vector2.One, Color.White, 0, 0);
        }
        public override void OnKill()
        {
            //Functions.ProximityExplosion(new Circle(position, 15), 1, team);
            new Effect(position, 1);
            AssetManager.PlaySound(SoundID.SmallExplosion);
        }
    }
}
