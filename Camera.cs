using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GazeOGL.Entities.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GazeOGL.Entities.Ships;

namespace GazeOGL
{
    public static class Camera
    {
        static float zoom = 1f;
        static float CameraWorldMinSize = 100;
        static float CameraWorlMaxSize = 400;
        public static float CameraDisplaySize = 900;
        public static float CameraWorldSize = 400;
        public static Vector2 screenPosition = Vector2.Zero;
        public static bool caneUseSoloCamera = false;
        public static void Load()
        {
            CameraWorldSize = Arena.boundrySize / 2;
            CameraWorlMaxSize = Arena.boundrySize / 2;
            CameraDisplaySize = Main.defaultScreenSize.Y;
        }
        public static void Update(GameWindow Window)
        {
            bool npcMode = false;
            if(Arena.npcs.Count > 0)
            {
                //npcMode = true;
                //CameraWorlMaxSize = Arena.boundrySize;
            }
            else
            {
                CameraWorlMaxSize = Arena.boundrySize / 2;
            }
            CameraDisplaySize = Math.Min(Window.ClientBounds.Height, Window.ClientBounds.Width * 0.75f);
            if ((Arena.ships[0] != null && Arena.ships[1] != null) || npcMode)
            {
                if(npcMode)
                {
                    CameraWorldSize = CameraWorlMaxSize;
                    screenPosition = Vector2.Zero;
                    if(!Main.isAI[0] && Arena.ships[0] != null )
                    {
                        screenPosition = ShipPos(Arena.ships[0]) - Vector2.One * Arena.boundrySize /2;
                    }
                }
                else if (caneUseSoloCamera && (Networking.GetNetMode() != NetMode.disconnected || Main.isAI[1]))
                {
                    Vector2 shipPos0 = ShipPos(Arena.ships[0]);
                    Vector2 shipPos1 = ShipPos(Arena.ships[1]);
                    Vector2 center = shipPos0;
                    if(Networking.GetNetMode() == NetMode.client)
                    {
                        center = shipPos1;
                    }
                    screenPosition = center - Vector2.One * (CameraWorldSize / 2);
                    Vector2 diff = (Functions.screenLoopAdjust(center, Networking.GetNetMode() == NetMode.client ? shipPos0 : shipPos1) - center);
                    CameraWorldSize = Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y)) * 2.4f;
                    if (CameraWorldSize > CameraWorlMaxSize * 2)
                    {
                        CameraWorldSize = CameraWorlMaxSize * 2;
                    }
                    if (CameraWorldSize < CameraWorldMinSize)
                    {
                        CameraWorldSize = CameraWorldMinSize;
                    }

                }
                else
                {
                    Vector2 shipPos0 = ShipPos(Arena.ships[0]);
                    Vector2 shipPos1 = ShipPos(Arena.ships[1]);
                    screenPosition = (Functions.screenLoopAdjust(shipPos0, shipPos1) + shipPos0) / 2f - Vector2.One * (CameraWorldSize / 2);
                    Vector2 diff = (Functions.screenLoopAdjust(shipPos0, shipPos1) - shipPos0);
                    CameraWorldSize = Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y)) * 1.8f;
                    if (CameraWorldSize > CameraWorlMaxSize)
                    {
                        CameraWorldSize = CameraWorlMaxSize;
                    }
                    if (CameraWorldSize < CameraWorldMinSize)
                    {
                        CameraWorldSize = CameraWorldMinSize;
                    }

                }
            }
            else
            {
                if (Arena.ships[0] != null)
                {
                    screenPosition = (Arena.ships[0].position - Vector2.One * (CameraWorldSize / 2));
                }
                if (Arena.ships[1] != null)
                {
                    screenPosition = (Arena.ships[1].position - Vector2.One * (CameraWorldSize / 2));
                }
                CameraWorldSize = CameraWorldMinSize;
            }
            zoom = CameraDisplaySize / CameraWorldSize;
        }
        static Vector2 ShipPos(Ship ship)
        {
            if(ship is Illusioner)
            {
                Illusioner illusionist = (Illusioner)ship;
                Vector2 averageArea = illusionist.position;
                for(int i =0; i < illusionist.illusions.Count; i++)
                {
                    averageArea += Functions.screenLoopAdjust(illusionist.position, illusionist.illusions[i].position);
                }
                averageArea /= (1 + illusionist.illusions.Count);
                return averageArea;
            }
            return ship.position;
        }
        public static Vector2 CameraOffset(Vector2 here)
        {

            return here - screenPosition;
        }
        public static void RenderArena(SpriteBatch spriteBatch)
        {

            Matrix trans = new Matrix(
                zoom, 0, 0, 0,
                0, zoom, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
                );
            spriteBatch.Begin(transformMatrix: trans, samplerState: SamplerState.PointClamp);
            Rectangle screenView = new Rectangle((int)screenPosition.X, (int)screenPosition.Y, (int)CameraWorldSize, (int)CameraWorldSize);
            Vector2[] offsets = Functions.OffsetsForDrawing();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < Arena.Stars.Length; j++)
                {
                    if (screenView.Contains((Arena.Stars[j] + offsets[i]).ToPoint()))
                    {
                        spriteBatch.Draw(Main.pixel, CameraOffset(Arena.Stars[j] + offsets[i]), null, Color.White, 0, new Vector2(.5f, .5f), Vector2.One, SpriteEffects.None, 0);
                    }
                }
            }
            for (int i = 0; i < Arena.effects.Count; i++)
            {
                Arena.effects[i].Draw(spriteBatch);
            }
            foreach (Particle drawMe in Arena.particles)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (screenView.Contains((drawMe.Position + offsets[i]).ToPoint()))
                    {
                        spriteBatch.Draw(Main.pixel, CameraOffset(drawMe.Position + offsets[i]), null, drawMe.color, 0f, Vector2.One * 0.5f, Vector2.One, SpriteEffects.None, 0);
                    }
                }
            }


            for (int i = 0; i < Arena.entities.Count; i++)
            {
                if (Arena.entities[i] is Projectile && !Arena.entities[i].DrawOnTop)
                {
                    Arena.entities[i].Draw(spriteBatch);
                    if (Arena.entities[i].collisionLine != null)
                    {
                        float dir = (float)Arena.entities[i].collisionLine;
                        Line newLine = new Line(Arena.entities[i].position, Arena.entities[i].position + Functions.PolarVector(20, dir));
                        newLine.Draw(spriteBatch, Color.Red);
                    }
                }
            }
            for (int i = 0; i < Arena.entities.Count; i++)
            {
                if (!(Arena.entities[i] is Projectile) && !Arena.entities[i].DrawOnTop)
                {
                    Arena.entities[i].Draw(spriteBatch);
                    if (Arena.entities[i].collisionLine != null)
                    {
                        float dir = (float)Arena.entities[i].collisionLine;
                        Line newLine = new Line(Arena.entities[i].position, Arena.entities[i].position + Functions.PolarVector(20, dir));
                        newLine.Draw(spriteBatch, Color.Red);
                    }
                }
            }
            for (int i = 0; i < Arena.entities.Count; i++)
            {
                if (Arena.entities[i].DrawOnTop)
                {
                    Arena.entities[i].Draw(spriteBatch);
                    if (Arena.entities[i].collisionLine != null)
                    {
                        float dir = (float)Arena.entities[i].collisionLine;
                        Line newLine = new Line(Arena.entities[i].position, Arena.entities[i].position + Functions.PolarVector(20, dir));
                        newLine.Draw(spriteBatch, Color.Red);
                    }
                }
            }


            //useful for debugging
            if (Arena.ships[0] != null)
            {
                //Arena.ships[0].Hitbox().Draw(spriteBatch, Color.White);
            }
            if (Arena.ships[1] != null)
            {
                //Arena.ships[1].Hitbox().Draw(spriteBatch, Color.White);
            }
            /*
            for(int i =0; i < 4; i++)
            {
                Arena.boundryLines[i].Draw(spriteBatch, Color.Red);
            }
            */
            spriteBatch.End();
        }
    }
}
