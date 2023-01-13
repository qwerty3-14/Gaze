using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities.Projectiles
{
    public class Capsule : Projectile
    {
        public Capsule(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            damage = 4;
            health = 1;
            mass = 0;
            lifeTime = 20;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(2, 2),
                new Vector2(-6, 2),
                new Vector2(-6, -2),
                new Vector2(2, -2)
            });
        }
        int counter = 0;
        public override void LocalUpdate()
        {
            counter++;
            if(counter % 2 == 0)
            {
                new Particle(position + Functions.PolarVector(-5, rotation), 12, Color.Orange);
            }
        }

        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[20], pos, null, Color.White, rotation, new Vector2(8.5f, 2.5f), Vector2.One, SpriteEffects.None, 0f);
        }
        public override void OnKill()
        {
            AssetManager.PlaySound(SoundID.MissileLaunch, -0.5f);
            new LingeringExplosion(position, Vector2.Zero, team);
        }
    }
    public class LingeringExplosion : Projectile
    {
        public const int radius = 40;
        public const int radiusBig = 100;
        bool big = false;
        public LingeringExplosion(Vector2 position, Vector2 velocity, int team = 0, bool big = false) : base(position, velocity, team)
        {
            this.big = big;
            damage = 0;
            health = -1;
            mass = 0;
            lifeTime = big ? 1200 : 150;
            shape = new Circle(Vector2.Zero, big ? radiusBig : radius);
            invulnerable = true;
        }
        int counter = 0;
        public override void LocalUpdate()
        {
            counter++;
            float vel = 3;
            int arms = 3;
            if (!big || counter > 120)
            {
                for (int i = 0; i < arms; i++)
                {
                    new Particle(position, (int)((float)(big ? radiusBig : radius) / vel), Color.Orange, Functions.PolarVector(vel, ((float)i / (float)arms) * (float)Math.PI * 2f + counter * 0.4f));
                }
            }
            if (counter > (big ? 150 : 30))
            {
                if (counter % 12 == 0)
                {
                    Functions.ProximityExplosion(new Circle(position, big ? radiusBig : radius), 1, team);
                }
            }
        }
    }
}
