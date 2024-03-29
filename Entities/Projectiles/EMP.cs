﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GazeOGL.Entities.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.Entities.Projectiles
{
    class EMP : Projectile
    {
        public EMP(Vector2 position, Vector2 velocity, int team = 0) : base(position, velocity, team)
        {
            damage = 0;
            health = 1;
            shape = new Polygon(new Vector2[]
             {
                new Vector2(1.5f, -1.5f),
                new Vector2(1.5f, 1.5f),
                new Vector2(-1.5f, 1.5f),
                new Vector2(-1.5f, -1.5f)
             });
            mass = 0;
            lifeTime = 80;
        }
        public override void LocalUpdate()
        {

        }
        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.projectiles[4], pos, null, Color.White, rotation, new Vector2(1.5f, 1.5f), Vector2.One, SpriteEffects.None, 0f);
        }
        public override void Expire()
        {
            new Particle(position, 5, Color.DarkCyan, velocity);
        }
        public override void OnHit(Entity Victim)
        {
            if(Victim is Ship)
            {
                ((Ship)Victim).energy = 0;
                ((Ship)Victim).EMPTime = ((Ship)Victim).energyCapacity >= 30 ? 60 : 120;
                AssetManager.PlaySound(SoundID.Zap);
            }
            if(Victim is Platform)
            {
                if(((Platform)Victim).parent.attached)
                {
                    ((Platform)Victim).parent.energy = 0;
                    ((Platform)Victim).parent.EMPTime = 120;
                    AssetManager.PlaySound(SoundID.Zap);
                }
            }
            new Effect(position, 0);
        }
    }
}
