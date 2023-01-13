using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GazeOGL.Entities.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities.Ships
{
    public class Surge : Ship
    {
        int lightningDamage = 3;
        float range = 120;
        public Surge(Vector2 position, int team = 0) : base(position, team)
        {
            type = ShipID.Surge;
            ShipStats.GetStatsFor(ShipID.Surge, out healthMax, out energyCapacity, out energyGen, out acceleration, out maxSpeed, out turnSpeed, true);
            healthMax *= 2;
            health = healthMax;
            energyCapacity *= 2;
            energy = energyCapacity;

            //7.5 10.5
            energyRate = 1;
            shape = new Polygon(new Vector2[]
            {
                new Vector2(15, 2),
                new Vector2(6, 10),
                new Vector2(-6, 10),
                new Vector2(-7, 4),
                new Vector2(-7, -4),
                new Vector2(-6, -10),
                new Vector2(6, -10),
                new Vector2(15, -2),
            });
            mass = 24;
        }
        int shotCooldown = 0;
        public override void Shoot()
        {
            if(energy > 5 && shotCooldown <=0)
            {
                List<Entity> zappables = EnemyEntites();
                zappables.AddRange(lightningAnchors);

                if (InitialLightningTargetting(zappables, out Entity target))
                {
                    zappables.Remove(target);
                    Vector2 shootFrom = position + Functions.PolarVector(20, rotation);

                    DrawLightning(shootFrom, Functions.screenLoopAdjust(shootFrom, target.position));
                    if(!(target is LightningAnchor))
                    {
                        CollisionEvent.DamagingHit(target, lightningDamage, (Functions.screenLoopAdjust(shootFrom, target.position) - shootFrom).ToRotation());
                    }
                    LightningSource(target, ref zappables);
                    energy -= 5;
                    frameCounter = 20;
                    AssetManager.PlaySound(SoundID.Zap, -0.4f);
                    shotCooldown = 30;
                }
                    
            }
        }
        public List<LightningAnchor> lightningAnchors = new List<LightningAnchor>();
        int specialCooldown = 0;
        public override void Special()
        {
            if (energy > 3 && specialCooldown <= 0)
            {
                specialCooldown = 30;
                AssetManager.PlaySound(SoundID.SmallExplosion, 0.4f);

                lightningAnchors.Add(new LightningAnchor(position, velocity + Functions.PolarVector(4f, rotation), team));

                energy -= 3;
            }
        }
        List<Entity> EnemyEntites()
        {
            List<Entity> proj = new List<Entity>();
            for (int i = 0; i < Arena.entities.Count; i++)
            {
                if (Arena.entities[i].team != team && Arena.entities[i].health >= 0 && !Arena.entities[i].ignoreMe)
                {
                    if (Arena.entities[i] is Mine)
                    {
                        if (((Mine)Arena.entities[i]).alpha > 0)
                        {
                            proj.Add(Arena.entities[i]);
                        }
                    }
                    else
                    {
                        proj.Add(Arena.entities[i]);
                    }
                }
            }
            return proj;
        }

        bool InitialLightningTargetting(List<Entity> zappables, out Entity target)
        {
            target = null;
            float locRange = range;
            Vector2 shootFrom = position + Functions.PolarVector(20, rotation);
            Polygon hitArea = new Polygon(new Vector2[]
            {
                    position,
                    position + Functions.PolarVector(range, rotation - (float)Math.PI / 4f),
                    position + Functions.PolarVector(range, rotation + (float)Math.PI / 4f),
            });
            for(int i =0; i < zappables.Count; i++)
            {
                if(AI_CollidingWithEntitiy(zappables[i], hitArea) && (Functions.screenLoopAdjust(shootFrom, zappables[i].position) - shootFrom).Length() < locRange)
                {
                    target = zappables[i];
                    locRange = (Functions.screenLoopAdjust(shootFrom, zappables[i].position) - shootFrom).Length();
                }
            }
            return target != null;
        }
        void LightningSource(Entity entity, ref List<Entity> zappables, bool justChecking = false)
        {
            LightningMultiJumpTargetting(entity.position, range, zappables, out List<Entity> targets);
            foreach(Entity bounceTarget in targets)
            {
                if (justChecking)
                {

                }
                else
                {
                    if (!(bounceTarget is LightningAnchor))
                    {
                        CollisionEvent.DamagingHit(bounceTarget, lightningDamage, (Functions.screenLoopAdjust(entity.position, bounceTarget.position) - entity.position).ToRotation());
                    }
                    DrawLightning(entity.position, Functions.screenLoopAdjust(entity.position, bounceTarget.position));
                }
                zappables.Remove(bounceTarget);
            }
            foreach (Entity bounceTarget in targets)
            {
                if(!(bounceTarget is PsuedostableVacum))
                {
                    LightningSource(bounceTarget, ref zappables, justChecking);
                }
            }
        }
        bool LightningMultiJumpTargetting(Vector2 shootFrom, float radius, List<Entity> zappables, out List<Entity> targets)
        {
            targets = new List<Entity>();
            Circle hitArea = new Circle(shootFrom, radius);
            for (int i = 0; i < zappables.Count; i++)
            {
                if (AI_CollidingWithEntitiy(zappables[i], hitArea) )
                {
                    targets.Add(zappables[i]);
                }
            }
            return targets.Count > 0;
        }
        void DrawLightning(Vector2 here, Vector2 there)
        {
            float rot = (there - here).ToRotation();
            for (int i = 0; i < (here-there).Length(); i++)
            {
                new Particle(here + Functions.PolarVector(i, rot), 6, Color.Cyan);
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            int frame = frameCounter % 40 < 18 ?  ((frameCounter % 40) / 3): 0;
            spriteBatch.Draw(AssetManager.ships[17], pos, new Rectangle(0, frame * 21, 31, 21), Color.White, rotation, new Vector2(7.5f, 10.5f), Vector2.One, SpriteEffects.None, 0f);
            //spriteBatch.Draw(AssetManager.ships[17], pos, null, new Rectangle(0, frame * 21, 31, 21), new Vector2(7.5f, 10.5f), rotation, Vector2.One, Color.White, 0, 0);
        }
        int counter;
        int frameCounter = 0;
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
            for (int i =0; i < lightningAnchors.Count; i++)
            {
                if(!Arena.entities.Contains(lightningAnchors[i]))
                {
                    lightningAnchors.RemoveAt(i);
                }
            }
            frameCounter++;
            if (thrusting)
            {
                counter++;
                if (counter % 6 == 0)
                {
                    new Particle(position + Functions.PolarVector(6, rotation + (float)Math.PI / 2) + Functions.PolarVector(0.5f, rotation), 14, Color.Orange);
                    new Particle(position + Functions.PolarVector(-6, rotation + (float)Math.PI / 2) + Functions.PolarVector(0.5f, rotation), 14, Color.Orange);
                }
            }
        }
        List<Entity> TheseWillbeZapped()
        {
            if (energy > 5)
            {
                List<Entity> zapped = new List<Entity>();
                List<Entity> zappables = EnemyEntites();
                zappables.AddRange(lightningAnchors);

                if (InitialLightningTargetting(zappables, out Entity target))
                {
                    zappables.Remove(target);
                    Vector2 shootFrom = position + Functions.PolarVector(20, rotation);

                    zapped.Add(target);
                    LightningSource(target, ref zappables, true);
                }
                return zapped;
            }
            return new List<Entity>();
        }
        public override void AI()
        {
            AI_ResetControls();
            Entity enemyShip = GetEnemy();
            List<Projectile> enemyProjectiles = EnemyProjectiles();
            bool AI_Dodging = false;
            List<Entity> zappable = TheseWillbeZapped();
            for (int i = 0; i < enemyProjectiles.Count(); i++)
            {
                if (AI_ImpendingCollision(enemyProjectiles[i], 30) || enemyProjectiles[i] is Missile)
                {
                    if(enemyProjectiles[i].health >=0 && enemyProjectiles[i].health <= lightningDamage )
                    {
                        if(zappable.Contains(enemyProjectiles[i]))
                        {
                            AI_cShoot();
                        }
                    }
                    else if(!(enemyProjectiles[i] is Missile))
                    {
                        AI_Dodging = true;
                        AI_Dodge(enemyProjectiles[i]);
                        if(Functions.AngularDifference((Functions.screenLoopAdjust(position, enemyProjectiles[i].position) - position).ToRotation(), rotation) < (float)Math.PI / 2)
                        {
                            AI_cDown();
                        }
                        else
                        {
                            AI_cThrust();
                        }
                    }
                }
            }
            if (zappable.Contains(enemyShip))
            {
                AI_cShoot();
            }
            else
            {
                for(int i =0; i < zappable.Count; i++)
                {
                    if(!(zappable[i] is Projectile) && zappable[i].team != team)
                    {
                        AI_cShoot();
                        break;
                    }
                }
            }
            if (!AI_Dodging && enemyShip != null)
            {
                
                Vector2 enemyPos = Functions.screenLoopAdjust(position, enemyShip.position);
                AI_TurnToward((enemyPos - position).ToRotation());
                if((enemyPos - position).Length() < range)
                {
                    AI_cShoot();
                }
                if ((enemyPos - position).Length() > range-20 && (energy >= 4 || enemyShip is Conqueror))
                {
                    AI_cThrust();
                    if(energy == energyCapacity && zappable.Count == 0)
                    {
                        AI_cSpecial();
                    }
                }
                else
                {
                    
                    AI_cDown();
                }
            }
        }
    }
    public class LightningAnchor : Entity
    {
        public LightningAnchor(Vector2 position, Vector2 velocity, int team)
        {
            this.team = team;
            health = 1;
            this.velocity = velocity;
            this.position = position; 
            shape = new Polygon(new Vector2[]
             {
                new Vector2(3, 1),
                new Vector2(1, 3),
                new Vector2(-1, 3),
                new Vector2(-3, 1),
                new Vector2(-3, -1),
                new Vector2(-1, -3),
                new Vector2(1, -3),
                new Vector2(3, -1),
             });
            mass = 4;
            
        }
        int lifeTimeCounter = 0;
        public override void LocalUpdate()
        {
            rotation += (float)Math.PI / 30f;
            velocity *= 0.96f;
            lifeTimeCounter++;
            if(lifeTimeCounter > 60*7)
            {
                Kill();
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.extraEntities[5], pos, null, Color.White, rotation, new Vector2(3.5f, 3.5f), Vector2.One, SpriteEffects.None, 0f);
        }

    }
}
