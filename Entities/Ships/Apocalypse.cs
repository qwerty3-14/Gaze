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
    public class Apocalypse : Ship
    {
        public ApocalypseTurret turret;
        public const int TurretRange = 80;
        public Apocalypse(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Apocalypse;
            ShipStats.GetStatsFor(ShipID.Apocalypse, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed, true);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            energyRate = 1;

            shape = new Polygon(new Vector2[]
            {
                new Vector2(8, 5),
                new Vector2(0, 9),
                new Vector2(-7, 9),
                new Vector2(-7, -9),
                new Vector2(0, -9),
                new Vector2(8, -5),
            });
            mass = 50;
            turret = new ApocalypseTurret(this, Vector2.Zero, 0);
        }
        int shotCooldown = 0;
        public override void Shoot()
        {
            if (shotCooldown <= 0 && energy >= 4)
            {
                energy -= 4;
                shotCooldown = 15;
                Blotch b = new Blotch(position + Functions.PolarVector(12, rotation), velocity + Functions.PolarVector(2.5f, rotation), team);
                b.rotation = rotation;
                AssetManager.PlaySound(SoundID.SmallExplosion);
            }
        }
        int specialCooldown = 0;
        public override void Special()
        {
            if (specialCooldown <= 0 && energy >= 8)
            {
                energy -= 8;
                specialCooldown = 60;
                Mine b = new Mine(position + Functions.PolarVector(-4, rotation), velocity + Functions.PolarVector(-1f, rotation), team);
                b.rotation = rotation;
                AssetManager.PlaySound(SoundID.SmallExplosion);
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.ships[9], pos, null, null, new Vector2(7.5f, 13.5f), rotation, Vector2.One, Color.White, 0, 0);
            turret.Draw(spriteBatch, pos);
        }
        int counter;
        public override void LocalUpdate()
        {
            if (specialCooldown > 0)
            {
                specialCooldown--;
            }
            if (shotCooldown > 0)
            {
                shotCooldown--;
            }
            if (thrusting)
            {
                counter++;
                if (counter % 5 == 0)
                {
                    new Particle(position
                        + Functions.PolarVector(4, rotation + (float)Math.PI / 2)
                        + Functions.PolarVector(-6, rotation),
                        10, Color.Orange);
                    new Particle(position
                        + Functions.PolarVector(-4, rotation + (float)Math.PI / 2)
                        + Functions.PolarVector(-6, rotation),
                        10, Color.Orange);
                }
            }
            if (StunTime <= 0)
            {
                turret.UpdateRelativePosition();
                if (DefensiveTargetting(position, 900, out Entity target, 4))
                {
                    Vector2 enemyPos = Functions.screenLoopAdjust(position, target.position);
                    if (turret.AimAt(enemyPos))
                    {
                        if ((enemyPos - position).Length() < TurretRange)
                        {
                            turret.Fire();
                        }
                    }
                }
                else
                {
                    turret.AimHome();
                }
            }
        }
        public override void AI()
        {

            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            if(enemyShip != null)
            {
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                if((enemyPos - position).Length() > 200f)
                {
                    Controls.controlThrust[team] = true;
                }
                if((enemyPos - position).Length() < 300f)
                {
                    float aimAt = Functions.PredictiveAim(position, 2.5f, enemyPos, enemyShip.velocity - velocity);

                    if(!float.IsNaN(aimAt))
                    {
                        if(AI_TurnToward(aimAt))
                        {
                            Controls.controlShoot[team] = true;
                        }
                    }
                    else
                    {
                        AI_TurnToward((enemyPos - position).ToRotation());
                    }
                }
                else
                {
                    AI_TurnToward((enemyPos - position).ToRotation());
                }
                if(energy == energyCapacity)
                {
                    Controls.controlSpecial[team] = true;
                }
            }
        }
    }
    public class ApocalypseTurret : Turret
    {
        public ApocalypseTurret(Apocalypse parent, Vector2 anchorAt, float homeRotation = 0) : base(parent, anchorAt, homeRotation)
        {
            this.parent = parent;
            origin = new Vector2(4.5f, 5.5f);
            texture = AssetManager.turrets[2];
            turretLength = 9f;
            rotSpeed = (float)Math.PI / 210f;
        }
        int timer = 0;
        Beam beam;
        public override void Update()
        {
            timer++;
            if(timer > 90 && timer % 2 == 0)
            {
                new Particle(AbsoluteShootPosition(), 10, Color.Red, parent.velocity + Functions.PolarVector(.75f, AbsoluteRotation() + (float)Main.random.NextDouble() * (float)Math.PI / 2f - (float)Math.PI / 4f) );
            }
            if(beam != null)
            {
                if (beam.Update(AbsoluteShootPosition(), AbsoluteRotation()))
                {
                    beam.ProcessCollision();
                }
                else
                {
                    beam = null;
                }
            }
        }
        public override void Fire()
        {
            if(timer > 90)
            {
                timer = 0;
                beam = new Beam(parent, Color.Red, Apocalypse.TurretRange, 10, -1, 4);
                beam.Update(AbsoluteShootPosition(), AbsoluteRotation());
                AssetManager.PlaySound(SoundID.Pew2);
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 Pos)
        {
            spriteBatch.Draw(texture, Pos + relativePosition, null, Color.White, AbsoluteRotation(), origin, new Vector2(1, 1), SpriteEffects.None, 0);
            if(beam != null)
            {
                beam.Draw(spriteBatch);
            }
        }
    }
}
