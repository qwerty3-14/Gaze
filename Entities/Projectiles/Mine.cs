using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Entities.Projectiles
{
    public class Mine : Projectile
    {
        public Mine(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            damage = 0;
            health = 1;
            //2.5 1.5
            //shape = new Circle(Vector2.Zero, 3);
            shape = new Polygon(new Vector2[]
             {
                new Vector2(3f, -3f),
                new Vector2(3f, 3f),
                new Vector2(-3f, 3f),
                new Vector2(-3f, -3f)
             });
            mass = 0f;
            lifeTime = 60 * 30;

        }
        float maxSpeed = 3.5f;
        float acceleration = (3.5f / 120f);
        public float alpha = 1f;
        float cloakTime = 60f;
        public override void LocalUpdate()
        {
            Entity enemyShip = GetEnemy();
            if(enemyShip != null)
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                if((enemyPos - position).Length() < 120)
                {
                    Vector2 dir = (enemyPos - position);
                    dir.Normalize();
                    dir *= acceleration;
                    velocity += dir;
                    if(velocity.Length() > maxSpeed)
                    {
                        velocity.Normalize();
                        velocity *= maxSpeed;
                    }
                    rotation += velocity.Length() * (float)Math.PI / 120f;
                    if(alpha < 1f)
                    {
                        alpha += (1f / cloakTime);
                    }
                    else
                    {
                        alpha = 1f;
                    }
                }
                else
                {
                    velocity *= 0.95f; 
                    
                    if (alpha > 0f)
                    {
                        alpha -= (1f / cloakTime);
                    }
                    else
                    {
                        alpha = 0f;
                    }
                }
            }
        }
        public override void OnHit(Entity Victim)
        {
            Functions.ProximityExplosion(new Circle(position, 25), 4, team);
            for (int i = 0; i < 12; i++)
            {
                float r = velocity.ToRotation() + (float)Math.PI * 2 * (float)Main.random.NextDouble() * (Main.random.Next(2) == 0 ? -1 : 1);
                new Particle(position, 6, Color.Red, Functions.PolarVector((float)Main.random.NextDouble() * 3f + 2, r));
            }
            AssetManager.PlaySound(SoundID.Death);
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[13], pos, null, null, new Vector2(5.5f, 5.5f), rotation, Vector2.One, new Color(alpha, alpha, alpha, alpha), 0, 0);
        }
    }
}
