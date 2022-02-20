using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectGaze.Entities.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Entities.Projectiles
{
    public class TwistedWormhole : Projectile
    {
        Assassin parent;
        public TwistedWormhole(Vector2 position, Vector2 velocity, int team, Assassin parent) : base(position, velocity, team)
        {
            this.parent = parent; 
            damage = 0;
            shape = new Circle(Vector2.Zero, 13);
            lifeTime = 15;
            health = -1;
            mass = 0;
        }

        public override void LocalUpdate()
        {
            rotation += (float)Math.PI / 10;
            new Particle(position - new Vector2(13, 13) + new Vector2(Main.random.Next(26), Main.random.Next(26)), 5,  Main.WarpPink, Functions.PolarVector(-.01f, rotation));
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[7], pos, null, null, new Vector2(13f, 13f), rotation, Vector2.One, Color.White, 0, 0);

        }
        public override void OnHit(Entity Victim)
        {
            for (int i = 0; i < 8; i++)
            {
                float dir = (float)Main.random.NextDouble() * 2f * (float)Math.PI;
                new Particle(parent.position, 5, Main.WarpPink, Functions.PolarVector(-4, dir));
            }
            parent.position = Victim.position + Functions.PolarVector(-30, Victim.rotation);
            parent.rotation = Victim.rotation;
            parent.velocity = Vector2.Zero;
            AssetManager.PlaySound(SoundID.Warp);
            for (int i = 0; i < 8; i++)
            {
                float dir = (float)Main.random.NextDouble() * 2f * (float)Math.PI;
                new Particle(parent.position + Functions.PolarVector(20, dir), 5, Main.WarpPink, Functions.PolarVector(-4, dir));
            }
            parent.instaAcc = true;
        }
    }
}
