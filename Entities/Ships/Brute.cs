using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectGaze.Entities.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Entities.Ships
{
    public class Brute : Ship
    {
        public Brute(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Brute;
            ShipStats.GetStatsFor(ShipID.Brute, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed, true);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;
            //5.5 7.5
            energyRate = 1;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(5, 5),
                new Vector2(5, -5),
                new Vector2(-5, -5),
                new Vector2(-5, 5),
            });
            mass = 8;
        }
        int shotCooldown = 0;
        public override void Shoot()
        {
            if (shotCooldown <= 0 && (energy >= 6 || specialCooldown > 0))
            {

                if (specialCooldown <= 0)
                { 
                    energy -= 6;
                }
                shotCooldown = specialCooldown > 0 ? 10 : 30;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 shotPs = position + Functions.PolarVector(3, rotation) + Functions.PolarVector(Main.random.Next(-4, 5), rotation + (float)Math.PI / 2);
                    Vector2 vel = velocity + Functions.PolarVector((float)Main.random.NextDouble() * 2.5f + 1.5f, rotation + (float)Main.random.NextDouble() * (float)Math.PI / 4 - (float)Math.PI / 8);
                    new HunterPelt(shotPs, vel, team)
                    {
                        lifeTime = 20,
                        rotation = (vel - velocity).ToRotation()
                    };
                }
                if(specialCooldown <=0)
                {
                    velocity = Functions.PolarVector(-3.5f, rotation);
                }
            }
        }
        int specialCooldown = 0;
        public override void Special()
        {
            if (specialCooldown <= 0 && health > 2)
            {
                health -= 2;
                energy = energyCapacity;
                specialCooldown = 120;
            }
        }
        int counter;
        public override void LocalUpdate()
        {
            if(shotCooldown > 0)
            {
                shotCooldown--;
            }
            if (specialCooldown > 0)
            {
                specialCooldown--;
            }
            if (thrusting)
            {
                counter++;
                if (counter % 4 == 0)
                {
                    new Particle(position + Functions.PolarVector(4, rotation + (float)Math.PI / 2) + Functions.PolarVector(-5, rotation), 7, Color.Orange);
                    new Particle(position + Functions.PolarVector(-4, rotation + (float)Math.PI / 2) + Functions.PolarVector(-5, rotation), 7, Color.Orange);
                }
            }
        }

        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[16], pos, null, null, new Vector2(5.5f, 7.5f), rotation, Vector2.One, specialCooldown % 20 < 10 ? Color.White : Color.Red, 0, 0);
        }
    }
}
