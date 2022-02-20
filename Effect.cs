using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze
{
    public class Effect
    {
        int lifeTime = 60;
        int id = 0;
        Vector2 position;
        public Effect(Vector2 position, int id)
        {
            this.position = position;
            this.id = id;
            switch (id)
            {
                case 0:
                    lifeTime = 30;
                    break;
                case 1:
                    lifeTime = 10;
                    break;
                case 2:
                    lifeTime = 12;
                    break;
            }
            Main.effects.Add(this);
        }
        public void Update()
        {
            lifeTime--;
            if(lifeTime < 0)
            {
                Main.effects.Remove(this);
            }
            if (id == 2)
            {
                float radius = ((10f - lifeTime) / 10f) * 50;
                if (radius > 50)
                {
                    radius = 50;
                }
                Functions.ProximityExplosion(new Circle(position, radius), 1, 2);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2[] offsets = Functions.OffsetsForDrawing();
            Rectangle screenView = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, (int)Main.CameraWorldSize, (int)Main.CameraWorldSize);
            for (int i = 0; i < 9; i++)
            {
                if (screenView.Contains((position + offsets[i]).ToPoint()))
                {
                    if(id == 2)
                    {
                        float radius = ((10f - lifeTime) / 10f) * 50;
                        if(radius > 50)
                        {
                            radius = 50;
                        }
                        new Circle(Main.CameraOffset(position + offsets[i]), radius).Draw(spriteBatch, Color.Yellow);
                    }
                    else
                    {
                        spriteBatch.Draw(texture: AssetManager.effects[id], position: Main.CameraOffset(position + offsets[i]), origin: new Vector2(AssetManager.effects[id].Width, AssetManager.effects[id].Height) * .5f, color: Color.White, rotation: 0f);
                    }
                }
            }
        }
    }
}
