﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities.Projectiles
{
    class HunterPelt : Projectile
    {
        public HunterPelt(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            damage = 1;
            health = 2; 
            shape = new Polygon(new Vector2[]
             {
                new Vector2(0.5f, -0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-2.5f, 0.5f),
                new Vector2(-2.5f, -0.5f)
             });
            mass = 0f;
            lifeTime = 52;
        }
        public override void LocalUpdate()
        {

        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[0], pos, null, Color.White, rotation, new Vector2(2.5f, 0.5f), Vector2.One, SpriteEffects.None, 0f);
        }
        public override void Expire()
        {
            new Particle(position, 5, Color.Gray, velocity);
        }
        public override void OnHit(Entity Victim)
        {

            float l = 4;
            for (int i =0; i < 5; i++)
            {
                new Particle(position, Main.random.Next(3) + 3, Color.Gray, Functions.PolarVector(l, rotation + (float)Math.PI - (float)Math.PI / 2 + (float)Math.PI * ((i+1)/6f)));
            }
            AssetManager.PlaySound(SoundID.WeakHit);
        }
    }
}
