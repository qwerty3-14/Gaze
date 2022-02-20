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
    class Missile : Projectile
    {
        public Missile(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            damage = 6;
            lifeTime = 8 * 60;
            health = 2;
            mass = 0;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(2, 2),
                new Vector2(2, -2),
                new Vector2(-7, -2),
                new Vector2(-7, 2)
            });
        }
        int counter;
        float maxSpeed = 3.2f;
        float acceleration = 1f;
        float turnSpeed = (float)Math.PI / 360;
        public override void LocalUpdate()
        {
            counter++;
            if (counter % 4 == 0)
            {
                new Particle(position + Functions.PolarVector(-4, rotation), 8, Color.Orange);
            }
            if (counter > 60)
            {
                Entity enemy = GetEnemy();
                if (enemy != null)
                {
                    Vector2 aimAt = Functions.screenLoopAdjust(position, enemy.position);
                    rotation.SlowRotation(Functions.ToRotation(aimAt - position), turnSpeed);
                    velocity += Functions.PolarVector(acceleration, rotation);
                    if (velocity.Length() > maxSpeed)
                    {
                        velocity.Normalize();
                        velocity *= maxSpeed;
                    }
                }
                if (counter < 420)
                {
                    float ratio = ((float)(counter - 60) / 360f);
                    turnSpeed = (float)Math.PI / (360 - (ratio * 330));
                    maxSpeed = 3.5f - (ratio * 1.5f);
                }
            }
            
            
        }
        public override void OnKill()
        {
            for (int i = 0; i < 12; i++)
            {
                float r = velocity.ToRotation() + (float)Math.PI * 2 * (float)Main.random.NextDouble() * (Main.random.Next(2) == 0 ? -1 : 1);
                new Particle(position, 6, Color.Orange, Functions.PolarVector((float)Main.random.NextDouble() * 3f + 2, r));
            }
            AssetManager.PlaySound(SoundID.Death);
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[3], pos, null, null, new Vector2(8.5f, 2.5f), rotation, Vector2.One, Color.White, 0, 0);
        }
    }
}
