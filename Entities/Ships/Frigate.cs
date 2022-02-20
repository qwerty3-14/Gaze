using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectGaze;
using ProjectGaze.Entities.Projectiles;
using ProjectGaze.Entities.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Entities.Ships
{
    public class Frigate : Ship
    {
        public Frigate(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Frigate;
            ShipStats.GetStatsFor(type, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 1;

            mass = 20;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(10, 2),
                new Vector2(5, 6),
                new Vector2(1, 6),
                new Vector2(-12, 6),
                new Vector2(-15, 3),
                new Vector2(-15, -3),
                new Vector2(-12, -6),
                new Vector2(1, -6),
                new Vector2(5, -6),
                new Vector2(10, -2),
            });
        }
        Capsule capsule;
        bool pressedShoot = false;
        public override void Shoot()
        {
            if (capsule != null)
            {
                if (capsule.lifeTime < 2)
                {
                    capsule.lifeTime++;
                }
                if(!Main.entities.Contains(capsule))
                {
                    capsule = null;
                }
            }
            else if (energy >= 8 && !pressedShoot)
            {
                AssetManager.PlaySound(SoundID.IllusionDown, -0.5f);
                energy -= 8;
                capsule = new Capsule(position, Functions.PolarVector(5f, rotation) + velocity, team);
                capsule.rotation = rotation;
            }
            pressedShoot = true;
        }
        public override void Special()
        {
            if(energy == energyCapacity)
            {
                energy = 0;
                for(int i =0; i < 6; i++)
                {
                    float r = ((float)i / 6f) * 2f * (float)Math.PI + rotation;
                    capsule = new Capsule(position, Functions.PolarVector(5f, r) + velocity, team);
                    capsule.rotation = r;
                }
            }
        }
        int counter;
        public override void LocalUpdate()
        {

            if (!Controls.controlShoot[team])
            {
                pressedShoot = false;
            }

            if (capsule != null && !Main.entities.Contains(capsule))
            {
                capsule = null;
            }
            if (thrusting)
            {
                counter++;
                if (counter % 8 == 0)
                {
                    new Particle(position + Functions.PolarVector(1, rotation + (float)Math.PI / 2) + Functions.PolarVector(-14, rotation), 14, Color.Orange);
                }
                if (counter % 8 == 4)
                {
                    new Particle(position + Functions.PolarVector(-1, rotation + (float)Math.PI / 2) + Functions.PolarVector(-14, rotation), 14, Color.Orange);
                }
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[15], pos, null, null, new Vector2(15.5f, 6.5f), rotation, Vector2.One, Color.White, 0, 0);
        }
        public override void AI()
        {
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            if (enemyShip != null)
            {
                
                {
                    if ((enemyShip is Ship && ((Ship)enemyShip).maxSpeed < maxSpeed) || energy < 8)
                    {
                        AI_Kite(7, 800);
                    }
                    else
                    {
                        if (AI_AimAtEnemy(7))
                        {
                            Controls.controlShoot[team] = true;
                        }
                    }
                    if (pressedShoot)
                    {
                        Controls.controlShoot[team] = false;
                    }
                }
                if (capsule != null)
                {
                    Controls.controlShoot[team] = true;
                    Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                    Vector2 capsulePos = Functions.screenLoopAdjust(position, capsule.position);
                    if ((enemyPos - position).Length() < (capsulePos - position).Length())
                    {
                        Controls.controlShoot[team] = false;
                    }
                    List<Projectile> enemyProjectiles = EnemyProjectiles();
                    Circle circle = new Circle(capsulePos, LingeringExplosion.radius);
                    for (int i = 0; i < enemyProjectiles.Count; i++)
                    {
                        if (enemyProjectiles[i].health == 1 && AI_CollidingWithEntitiy(enemyProjectiles[i], circle))
                        {
                            Controls.controlShoot[team] = false;
                            break;
                        }
                    }
                }
            }
        }
    }
}
