using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GazeOGL.Entities.Ships;
namespace GazeOGL.Entities.Projectiles
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
            lifeTime = Main.random.Next((int)(Palladin.Range / (3.7f * 2f))) + Main.random.Next((int)(Palladin.Range / (3.7f * 2f))) + (int)(Palladin.Range / (3.7f * 2f));
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[8], pos, null, Color.White, rotation, new Vector2(3.5f, 1.5f), Vector2.One, SpriteEffects.None, 0f);
        }
        public override void OnKill()
        {
            //Functions.ProximityExplosion(new Circle(position, 15), 1, team);
            new Effect(position, 1);
            AssetManager.PlaySound(SoundID.SmallExplosion);
        }
    }
}
