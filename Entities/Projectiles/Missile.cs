using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GazeOGL.Entities.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities.Projectiles
{
    class Missile : Projectile
    {
        public Missile(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            damage = 6;
            lifeTime = 10 * 60;
            health = 2;
            mass = 0;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(2, 2),
                new Vector2(2, -2),
                new Vector2(-9, -2),
                new Vector2(-9, 2)
            });
        }
        int counter;
        float maxSpeed = 3.2f;
        float acceleration = 1f;
        float turnSpeed = (float)Math.PI / 360;
        float finOut = 0f;
        public override void LocalUpdate()
        {
            counter++;
            if (counter % 4 == 0)
            {
                new Particle(position + Functions.PolarVector(-10, rotation), 8, Color.Orange);
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
                    maxSpeed = 4.5f - (ratio * 2.5f);
                }
                if(finOut < 3f)
                {
                    finOut += 0.1f;
                }
                else
                {
                    finOut = 3f;
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
            spriteBatch.Draw(AssetManager.extraEntities[8], pos, null, Color.White, rotation, new Vector2(10.5f, 2.5f -1f + finOut), Vector2.One, SpriteEffects.None, 0f);
            spriteBatch.Draw(AssetManager.extraEntities[9], pos, null, Color.White, rotation, new Vector2(10.5f, 2.5f -2f - finOut), Vector2.One, SpriteEffects.None, 0f);
            spriteBatch.Draw(AssetManager.projectiles[3], pos, null, Color.White, rotation, new Vector2(10.5f, 2.5f), Vector2.One, SpriteEffects.None, 0f);
        }
    }
}
