using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Entities.Projectiles
{
    public class TrooperWave : Projectile
    {
        public TrooperWave(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            damage = 4;
            health = 2;
            //2.5 2.5
            shape = new Polygon(new Vector2[]
             {
                new Vector2(1.5f, -2.5f),
                new Vector2(1.5f, 2.5f),
                new Vector2(-2.5f, 2.5f),
                new Vector2(-2.5f, -2.5f)
             });
            mass = 0f;
            lifeTime = 120;
        }
        public override void LocalUpdate()
        {

        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[9], pos, null, null, new Vector2(2.5f, 2.5f), rotation, Vector2.One, Color.White, 0, 0);
        }
        public override void Expire()
        {
            new Particle(position, 5, Color.Lime, velocity);
        }
        public override void OnHit(Entity Victim)
        {
            float l = 4;
            for (int i = 0; i < 5; i++)
            {
                new Particle(position, Main.random.Next(3) + 3, Color.Lime, Functions.PolarVector(l, rotation + (float)Math.PI - (float)Math.PI / 2 + (float)Math.PI * ((i + 1) / 6f)));
            }
            AssetManager.PlaySound(SoundID.MediumHit);
        }
    }
}
