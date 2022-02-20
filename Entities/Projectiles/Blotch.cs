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
    public class Blotch : Projectile
    {
        public Blotch(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            damage = 2;
            health = 1;
            //shape = new Circle(Vector2.Zero, 6);
            shape = new Polygon(new Vector2[]
             {
                new Vector2(4f, -2f),
                new Vector2(4f, 2f),
                new Vector2(-4f, 2f),
                new Vector2(-4f, -2f)
             });
            mass = 0;
            lifeTime = 120;
        }
        float trigCounter;
        public override void LocalUpdate()
        {
            trigCounter+= (float)Math.PI / 20f;
            new Particle(position + Functions.PolarVector(4 * (float)Math.Cos(trigCounter), velocity.ToRotation() + (float)Math.PI / 2f), 4, Color.Red);
            new Particle(position + Functions.PolarVector(-4 * (float)Math.Cos(trigCounter), velocity.ToRotation() + (float)Math.PI / 2f), 4, Color.Red);
        }
        public override void OnKill()
        {
            for(int i =0; i < 4; i++)
            {
                float r = velocity.ToRotation() + (float)Math.PI * 2 * (float)Main.random.NextDouble() * (Main.random.Next(2) == 0 ? -1 : 1);
                new Particle(position + Functions.PolarVector(4 * (float)Math.Cos(trigCounter), velocity.ToRotation() + (float)Math.PI / 2f), 6, Color.Red, Functions.PolarVector((float)Main.random.NextDouble() * 1f + 1, r));
                new Particle(position + Functions.PolarVector(-4 * (float)Math.Cos(trigCounter), velocity.ToRotation() + (float)Math.PI / 2f), 6, Color.Red, Functions.PolarVector((float)Main.random.NextDouble() * 1f + 1, r));
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[12], pos + Functions.PolarVector(4 * (float)Math.Cos(trigCounter), velocity.ToRotation() + (float)Math.PI / 2f), null, null, new Vector2(2f, 2f), rotation, Vector2.One, Color.White, 0, 0);
            spriteBatch.Draw(AssetManager.projectiles[12], pos + Functions.PolarVector(-4 * (float)Math.Cos(trigCounter), velocity.ToRotation() + (float)Math.PI / 2f), null, null, new Vector2(2f, 2f), rotation, Vector2.One, Color.White, 0, 0);
        }
        public override void OnHit(Entity Victim)
        {

            if (Victim is Ship)
            {
                ((Ship)Victim).energy -= 2;
                if(((Ship)Victim).energy < 0)
                {
                    ((Ship)Victim).energy = 0;
                }
            }
            if (Victim is Platform)
            {
                if (((Platform)Victim).parent.attached)
                {
                    ((Platform)Victim).parent.energy -= 2;
                    if (((Platform)Victim).parent.energy < 0)
                    {
                        ((Platform)Victim).parent.energy = 0;
                    }
                }
            }
        }
    }
}
