using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Entities
{
    class Planet : Entity
    {
        public Planet(Vector2 position)
        {
            this.position = position;
            team = 2;
            mass = 1000;
            health = 100;

            shape = new Circle(Vector2.Zero, 50);


        }

        public override void LocalDraw(SpriteBatch spriteBatch, Vector2 pos)
        {
            spriteBatch.Draw(AssetManager.planets[0], pos, null, null, new Vector2(50, 50), rotation, Vector2.One, Color.White, 0, 0);
        }
        public override void ModerateUpdate()
        {
            Circle gravityWell = new Circle(position, 170);
            for (int i = 0; i < Main.entities.Count; i++)
            {
                Main.entities[i].MakeHitboxes();
            }
            for (int i = 0; i < Main.entities.Count; i++)
            {
                if(Main.entities[i] != this && Main.entities[i].mass != 0)
                {
                    List<Shape> col = Main.entities[i].GetCollisionBoxes();
                    for(int k =0; k < col.Count; k++)
                    {
                        if(col[k].Colliding(gravityWell))
                        {
                            Main.entities[i].velocity += Functions.PolarVector(4 * 0.2f * (1f / 60f), (Functions.screenLoopAdjust(Main.entities[i].position, position) - Main.entities[i].position).ToRotation());
                            break;
                        }
                    }
                }
            }
        }
    }
}
