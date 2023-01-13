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
    public class Grapple : Projectile
    {
        Ship parent;
        public Vector2 previusPosition;
        Vector2? stuckSpot = null;
        float stuckRot = 0f;
        Entity stuckTo;
        public Grapple(Ship parent, Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            this.parent = parent;
            damage = 0;
            health = 2;
            //1 2.5
            shape = new Polygon(new Vector2[]
             {
                new Vector2(4f, -1.5f),
                new Vector2(-1f, 1.5f),
                new Vector2(-1f, 1.5f)
             });
            mass = 0f;
            lifeTime = 10;
            previusPosition = position;
            invulnerable = true;
        }
        bool returning = false;
        public void Pull()
        {
            if (stuckSpot != null)
            {
                if(stuckTo is Illusion)
                {
                    stuckTo.Kill();
                }
                else
                {
                    float pullStrength = (float)14 * 0.2f * (1f / 60f);
                    float ratio = stuckTo.mass / parent.mass;

                    stuckTo.velocity += Functions.PolarVector((1f / ratio) * pullStrength, (Functions.screenLoopAdjust(position, parent.position) - position).ToRotation());
                    parent.velocity += Functions.PolarVector((ratio) * pullStrength, (Functions.screenLoopAdjust(parent.position, position) - parent.position).ToRotation());
                }
            }
            else
            {
                returning = true;
            }
        }
        public bool dontDrawChain = false;
        void chainBreak()
        {
            if (!dontDrawChain)
            {
                dontDrawChain = true;
                Kill();
            }
            float length = (Functions.screenLoopAdjust(previusPosition, parent.position) - previusPosition).Length();
            float rot = (Functions.screenLoopAdjust(previusPosition, parent.position) - previusPosition).ToRotation();
            for (float l = 0; l < length; l += 4)
            {
                float r = velocity.ToRotation() + (float)Math.PI * 2 * (float)Main.random.NextDouble() * (Main.random.Next(2) == 0 ? -1 : 1);
                new Particle(previusPosition + Functions.PolarVector(l, rot), 12, Color.SkyBlue, Functions.PolarVector((float)Main.random.NextDouble() * 1.5f + 1, r));
            }
        }
        public override void OnKill()
        {
            if(!dontDrawChain)
            {
                dontDrawChain = true;
                previusPosition = position;
                chainBreak();
            }
        }
        public override void LocalUpdate()
        {
            if(stuckSpot != null)
            {
                returning = false;
                if (!Arena.entities.Contains(stuckTo))
                {
                    previusPosition = position;
                    chainBreak();
                }
                else
                {
                    velocity = stuckTo.velocity;
                    position = stuckTo.position + Functions.PolarVector(((Vector2)stuckSpot).X, stuckTo.rotation)
                        + Functions.PolarVector(((Vector2)stuckSpot).Y, stuckTo.rotation + (float)Math.PI / 2f);
                    rotation = stuckRot + stuckTo.rotation;
                }
            }
            else
            {
                if(returning)
                {
                    velocity = parent.velocity + Functions.PolarVector(10f, (Functions.screenLoopAdjust(position, parent.position) - position).ToRotation());
                    if((Functions.screenLoopAdjust(position, parent.position) - position).Length() < 12f)
                    {
                        previusPosition = position;
                        chainBreak();
                    }
                }
                else
                {
                    rotation = (Functions.screenLoopAdjust(position, parent.position) - position).ToRotation() + (float)Math.PI;
                }
            }
            if((position - Functions.screenLoopAdjust(position, previusPosition)).Length() > 20 
                || (Functions.AngularDifference(
                    (Functions.screenLoopAdjust(position, parent.position) - position).ToRotation(),
                    (Functions.screenLoopAdjust(previusPosition, parent.position) - previusPosition).ToRotation())
                > (float)Math.PI/8f && (Functions.screenLoopAdjust(position, parent.position) - position).Length() > 200))
            {
                chainBreak();
            }
            previusPosition = position;
        }
        public override void OnHit(Entity Victim)
        {
            if (stuckSpot == null)
            {
                if (!(Victim is Projectile))
                {
                    stuckTo = Victim;
                    stuckRot = rotation - Victim.rotation;
                    stuckSpot = position - Functions.screenLoopAdjust(position, Victim.position);

                    float len = ((Vector2)stuckSpot).Length();
                    float rot = ((Vector2)stuckSpot).ToRotation();
                    rot -= Victim.rotation;
                    stuckSpot = Functions.PolarVector(len, rot);
                    velocity = Victim.velocity;
                }
            }
        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[15], pos, null, Color.White, rotation, new Vector2(1.5f, 2.5f), Vector2.One, SpriteEffects.None, 0f);
        }
        public bool IsFlying()
        {
            return stuckSpot == null && !returning;
        }

        public bool IsStuck()
        {
            return stuckSpot != null;
        }
        public bool IsReturning()
        {
            return stuckSpot == null && returning;
        }
        public Entity StuckTo()
        {
            return stuckTo;
        }

    }
}
