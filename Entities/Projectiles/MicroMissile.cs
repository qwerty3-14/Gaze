using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities.Projectiles
{
    public class MicroMissile : Projectile
    {
        public MicroMissile(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            damage = 0;
            health = 0;
            //2.5 1.5
            shape = new Polygon(new Vector2[]
             {
                new Vector2(1.5f, -1.5f),
                new Vector2(1.5f, 1.5f),
                new Vector2(-2.5f, 1.5f),
                new Vector2(-2.5f, -1.5f)
             });
            mass = 0f;
            lifeTime = 35;
        }
        int counter;
        float maxSpeed = 3f;
        float acceleration = 3f;
        float turnSpeed = (float)Math.PI / 4;
        public override void LocalUpdate()
        {
            if(DefensiveTargetting(position, 500, out Entity target))
            {
                Vector2 targetPos = Functions.screenLoopAdjust(position, target.position);
                float aimAt = Functions.PredictiveAim(position, maxSpeed, targetPos, target.velocity);
                if (float.IsNaN(aimAt))
                {
                    aimAt = (targetPos - position).ToRotation();
                }
                rotation.SlowRotation(aimAt, turnSpeed);
                velocity += Functions.PolarVector(acceleration, rotation);
                if (velocity.Length() > maxSpeed)
                {
                    velocity.Normalize();
                    velocity *= maxSpeed;
                }
                counter++;
                if (counter % 2 == 0)
                {
                    new Particle(position + Functions.PolarVector(-1, rotation), 8, Color.Lime);
                }
            }

        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[10], pos, null, Color.White, rotation, new Vector2(2.5f, 1.5f), Vector2.One, SpriteEffects.None, 0f);
        }
        public override void OnKill()
        {
            for (int i = 0; i < 8; i++)
            {
                float r = velocity.ToRotation() + (float)Math.PI * 2 * (float)Main.random.NextDouble() * (Main.random.Next(2) == 0 ? -1 : 1);
                new Particle(position, 6, Color.Lime, Functions.PolarVector((float)Main.random.NextDouble() * 1.5f + 1, r));
            }
            AssetManager.PlaySound(SoundID.SmallExplosion);
            Functions.ProximityExplosion(new Circle(position, 20), 1, team);
        }
    }
}
