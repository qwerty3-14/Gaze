using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GazeOGL.MyraUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL
{
    public static class UI 
    {
        public static float UIScale = 2f;
        static float spacing = 14;
        static bool usePortraits = true;
        public static void Draw(SpriteBatch spriteBatch, Rectangle ClientBounds)
        {

            float UIStartX = Camera.CameraDisplaySize;
            float verticalSpacing = ClientBounds.Height / 2;
            float UIWidth = ClientBounds.Width - UIStartX;
            float stretchX = UIWidth / 300f;
            float stretchY = ClientBounds.Height / 900f;
            UIScale = 2f * (float)Math.Min(stretchX, stretchY);
            spriteBatch.Draw(AssetManager.ui[0], Vector2.UnitX * Camera.CameraDisplaySize, null, Color.White, 0, Vector2.Zero, new Vector2(stretchX, stretchY), SpriteEffects.None, 0f);
            spriteBatch.Draw(AssetManager.ui[11], Vector2.UnitX * Camera.CameraDisplaySize, null, Color.White, 0, Vector2.Zero, new Vector2((ClientBounds.Width - UIWidth) / 900f, (ClientBounds.Height - Camera.CameraDisplaySize) / 300f), SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture: AssetManager.ui[0], position: Vector2.UnitX * Camera.CameraDisplaySize, color: Color.White, scale: new Vector2(stretchX, stretchY));
            //spriteBatch.Draw(texture: AssetManager.ui[11], position: Vector2.UnitY * Camera.CameraDisplaySize, color: Color.White, scale: new Vector2((ClientBounds.Width - UIWidth) / 900f, (ClientBounds.Height - Camera.CameraDisplaySize) / 300f));
            spriteBatch.End();
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            MyraMain.Draw(spriteBatch);
            if (Main.mode == Mode.Menu || Main.mode == Mode.ShipSelect)
            {
                
            }
            if (Main.mode == Mode.Menu)
            {
                
            }
            else
            {
                //Networking.DisplayDebug(spriteBatch);
                for (int i = 0; i < 2; i++)
                {
                    if (Arena.ships[i] != null)
                    {
                        Vector2 BarCenter = new Vector2((ClientBounds.Width + UIStartX) / 2, (i * verticalSpacing) + (usePortraits ? verticalSpacing / 3f : verticalSpacing / 2f));
                        Texture2D texture = ShipStats.GetIcon(Arena.ships[i].type);
                        int Width = texture.Width;
                        int Height = texture.Height;
                        float scale = 1f;
                        float Size = Math.Min((UIWidth / 2.5f), (usePortraits ? verticalSpacing / 3f : verticalSpacing / 2f));
                        if (Width > Height)
                        {
                            scale = Size / (float)Width;
                        }
                        else
                        {
                            scale = Size / (float)Height;
                        }
                        spriteBatch.Draw(texture, (BarCenter - (new Vector2(Width, Height) * .5f * scale)), null, Color.White, 0, Vector2.Zero, (new Vector2(1f, 1f) * scale), SpriteEffects.None, 0f);
                        //spriteBatch.Draw(texture: texture, position: (BarCenter - (new Vector2(Width, Height) * .5f * scale)), scale: (new Vector2(1f, 1f) * scale), color: Color.White);

                        if (usePortraits)
                        {
                            BarCenter = new Vector2((ClientBounds.Width + UIStartX) / 2, (i * verticalSpacing) + 2f * verticalSpacing / 3f);
                            texture = AssetManager.ui[15];
                            Width = texture.Width;
                            Height = 40;
                            scale = 1f;
                            Size = Math.Min((UIWidth / 2.5f), verticalSpacing / 3f);
                            if (Width > Height)
                            {
                                scale = Size / (float)Width;
                            }
                            else
                            {
                                scale = Size / (float)Height;
                            }
                            Vector2 portraitPosition = (BarCenter - (new Vector2(Width, Height) * .5f * scale));
                            spriteBatch.Draw(texture, portraitPosition, new Rectangle(0, 0 * Height, Width, Height), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
                            //spriteBatch.Draw(texture: texture, position: portraitPosition, scale: Vector2.One * scale, color: Color.White, sourceRectangle: new Rectangle(0, 0 * Height, Width, Height));
                            if (Controls.controlRight[i] && !Controls.controlLeft[i])
                            {
                                spriteBatch.Draw(texture, portraitPosition, new Rectangle(0, 1 * Height, Width, Height), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
                                //spriteBatch.Draw(texture: texture, position: portraitPosition, scale: Vector2.One * scale, color: Color.White, sourceRectangle: new Rectangle(0, 1 * Height, Width, Height));
                            }
                            if (!Controls.controlRight[i] && Controls.controlLeft[i])
                            {
                                spriteBatch.Draw(texture, portraitPosition, new Rectangle(0, 2 * Height, Width, Height), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
                                //spriteBatch.Draw(texture: texture, position: portraitPosition, scale: Vector2.One * scale, color: Color.White, sourceRectangle: new Rectangle(0, 2 * Height, Width, Height));
                            }
                            if (Controls.controlThrust[i])
                            {
                                spriteBatch.Draw(texture, portraitPosition, new Rectangle(0, 3 * Height, Width, Height), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
                                //spriteBatch.Draw(texture: texture, position: portraitPosition, scale: Vector2.One * scale, color: Color.White, sourceRectangle: new Rectangle(0, 3 * Height, Width, Height));
                            }
                            if (Controls.controlShoot[i])
                            {
                                spriteBatch.Draw(texture, portraitPosition, new Rectangle(0, 4 * Height, Width, Height), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
                                //spriteBatch.Draw(texture: texture, position: portraitPosition, scale: Vector2.One * scale, color: Color.White, sourceRectangle: new Rectangle(0, 4 * Height, Width, Height));
                            }
                            if (Controls.controlSpecial[i])
                            {
                                spriteBatch.Draw(texture, portraitPosition, new Rectangle(0, 5 * Height, Width, Height), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
                                //spriteBatch.Draw(texture: texture, position: portraitPosition, scale: Vector2.One * scale, color: Color.White, sourceRectangle: new Rectangle(0, 5 * Height, Width, Height));
                            }
                        }
                        //Arena.ships[i].LocalDraw(spriteBatch, BarCenter, true);
                        float startBars = verticalSpacing + (verticalSpacing * i) - spacing * UIScale * 3;


                        for (int e = 0; e < Arena.ships[i].energyCapacity / 2; e++)
                        {
                            spriteBatch.Draw(AssetManager.ui[2], new Vector2(ClientBounds.Width - spacing * UIScale - (AssetManager.ui[2].Width * UIScale) / 2, startBars - (AssetManager.ui[2].Height * UIScale) * e), null, Color.White, 0, Vector2.Zero, Vector2.One * UIScale, SpriteEffects.None, 0f);
                            //spriteBatch.Draw(texture: AssetManager.ui[2], position: new Vector2(ClientBounds.Width - spacing * UIScale - (AssetManager.ui[2].Width * UIScale) / 2, startBars - (AssetManager.ui[2].Height * UIScale) * e), scale: Vector2.One * UIScale);
                        }
                        for (int e = 0; e < Arena.ships[i].energy; e++)
                        {
                            spriteBatch.Draw(AssetManager.ui[3], new Vector2(ClientBounds.Width - spacing * UIScale + (e % 2 == 0 ? 0 : -(AssetManager.ui[3].Width * UIScale)), startBars - (AssetManager.ui[3].Height * UIScale) * (int)(e / 2)), null, Color.White, 0, Vector2.Zero, Vector2.One * UIScale, SpriteEffects.None, 0f);
                            //spriteBatch.Draw(texture: AssetManager.ui[3], position: new Vector2(ClientBounds.Width - spacing * UIScale + (e % 2 == 0 ? 0 : -(AssetManager.ui[3].Width * UIScale)), startBars - (AssetManager.ui[3].Height * UIScale) * (int)(e / 2)), scale: Vector2.One * UIScale);
                        }
                        //spriteBatch.Draw(texture: AssetManager.ui[19], position: new Vector2(ClientBounds.Width - spacing * UIScale - (AssetManager.ui[19].Width * UIScale) / 2, startBars + (AssetManager.ui[2].Height * UIScale)), scale: Vector2.One * UIScale);
                        spriteBatch.Draw(AssetManager.ui[19], new Vector2(ClientBounds.Width - spacing * UIScale - (AssetManager.ui[19].Width * UIScale) / 2, startBars + (AssetManager.ui[2].Height * UIScale)), null, Color.White, 0, Vector2.Zero, Vector2.One * UIScale, SpriteEffects.None, 0f);


                        for (int h = 0; h < Arena.ships[i].healthMax / 2; h++)
                        {
                            spriteBatch.Draw(AssetManager.ui[2], new Vector2(UIStartX + spacing * UIScale - (AssetManager.ui[2].Width * UIScale) / 2, startBars - (AssetManager.ui[2].Height * UIScale) * (h)), null, Color.White, 0, Vector2.Zero, Vector2.One * UIScale, SpriteEffects.None, 0f);
                            //spriteBatch.Draw(texture: AssetManager.ui[2], position: new Vector2(UIStartX + spacing * UIScale - (AssetManager.ui[2].Width * UIScale) / 2, startBars - (AssetManager.ui[2].Height * UIScale) * (h)), scale: Vector2.One * UIScale);
                        }

                        for (int k = 0; k < Arena.ships[i].health; k++)
                        {
                            spriteBatch.Draw(AssetManager.ui[4], new Vector2(UIStartX + spacing * UIScale + (k % 2 == 0 ? 0 : -(AssetManager.ui[4].Width * UIScale)), startBars - (AssetManager.ui[4].Height * UIScale) * (int)((k / 2))), null, Color.White, 0, Vector2.Zero, Vector2.One * UIScale, SpriteEffects.None, 0f);
                            //spriteBatch.Draw(texture: AssetManager.ui[4], position: new Vector2(UIStartX + spacing * UIScale + (k % 2 == 0 ? 0 : -(AssetManager.ui[4].Width * UIScale)), startBars - (AssetManager.ui[4].Height * UIScale) * (int)((k / 2))), scale: Vector2.One * UIScale);
                        }
                        spriteBatch.Draw(AssetManager.ui[18], new Vector2(UIStartX + spacing * UIScale - (AssetManager.ui[18].Width * UIScale) / 2, startBars + (AssetManager.ui[2].Height * UIScale)), null, Color.White, 0, Vector2.Zero, Vector2.One * UIScale, SpriteEffects.None, 0f);

                        int total = Arena.ships[i].healthMax;
                        int g = Arena.ships[i].healthMax / 2;
                        for (int b = 0; b < Arena.ships[i].ExtraHealthBoxes.Count; b++)
                        {
                            int HealthBox = Arena.ships[i].ExtraHealthBoxes[b];
                            g++;
                            total += 2;
                            total += HealthBox;
                            for (; g < total / 2; g++)
                            {
                                spriteBatch.Draw(AssetManager.ui[2], new Vector2(UIStartX + spacing * UIScale - (AssetManager.ui[2].Width * UIScale) / 2, startBars - (AssetManager.ui[2].Height * UIScale) * (g)), null, Color.White, 0, Vector2.Zero, Vector2.One * UIScale, SpriteEffects.None, 0f);
                                //spriteBatch.Draw(texture: AssetManager.ui[2], position: new Vector2(UIStartX + spacing * UIScale - (AssetManager.ui[2].Width * UIScale) / 2, startBars - (AssetManager.ui[2].Height * UIScale) * (g)), scale: Vector2.One * UIScale);
                            }
                            //Debug.WriteLine(Arena.ships[i].ExtraHealths[b]);
                            for (int k = 0; k < Arena.ships[i].ExtraHealths[b]; k++)
                            {
                                int pos = k + total - HealthBox;
                                spriteBatch.Draw(AssetManager.ui[4], new Vector2(UIStartX + spacing * UIScale + (pos % 2 == 0 ? 0 : -(AssetManager.ui[4].Width * UIScale)), startBars - (AssetManager.ui[4].Height * UIScale) * (int)(((pos) / 2))), null, Color.White, 0, Vector2.Zero, Vector2.One * UIScale, SpriteEffects.None, 0f);
                            }
                        }
                        ShipStats.GetTitlesFor(Arena.ships[i].type, out string race, out string shipName);
                        string name = race + " " + shipName;
                        float startName = (verticalSpacing * i) + spacing * UIScale;
                        CombatUI.SetLabel(i, name, new Vector2(UIStartX, startName), UIWidth, 25 * UIScale);
                        /*
                        ShipStats.GetTitlesFor(Arena.ships[i].type, out string race, out string shipName);
                        string name = race + " " + shipName;
                        float startName = (verticalSpacing * i) + spacing * UIScale;
                        Vector2 NameGrandness = Main.font.MeasureString(name) * UIScale;
                        float maxWidth = UIWidth * 0.8f;
                        float nameScale = 1f;
                        if (NameGrandness.X > maxWidth)
                        {
                            nameScale = maxWidth / NameGrandness.X;
                        }

                        spriteBatch.DrawString(Main.font, name, new Vector2((ClientBounds.Width + UIStartX) / 2 - (NameGrandness.X * nameScale) / 2, startName), Color.Black, scale: UIScale * nameScale, origin: Vector2.Zero, rotation: 0f, effects: SpriteEffects.None, layerDepth: 0);
                        */
                    }
                }
            }
        }
    }
}
