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
    public class Inq : Projectile
    {
        public Inq(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            damage = 2;
            health = 2;
            //3.5 1.5
            shape = new Polygon(new Vector2[]
             {
                new Vector2(1.5f, -1.5f),
                new Vector2(1.5f, 1.5f),
                new Vector2(-3.5f, 1.5f),
                new Vector2(-3.5f, -1.5f)
             });
            mass = 0f;
            lifeTime = (int)(Missionary.range / Missionary.vel);
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[17], pos, null, Color.White, rotation, new Vector2(3.5f, 1.5f), Vector2.One, SpriteEffects.None, 0f);
        }
        public override void Expire()
        {
            new Particle(position, 5, Color.Cyan, velocity);
        }
        public override void OnHit(Entity Victim)
        {

            float l = 4;
            for (int i = 0; i < 5; i++)
            {
                new Particle(position, Main.random.Next(3) + 3, Color.Cyan, Functions.PolarVector(l, rotation + (float)Math.PI - (float)Math.PI / 2 + (float)Math.PI * ((i + 1) / 6f)));
            }
            AssetManager.PlaySound(SoundID.WeakHit);
        }
    }
}
