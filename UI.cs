using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze
{
    public static class UI 
    {
        public static float UIScale = 2f;
        static float spacing = 14;
        static bool usePortraits = true;
        public static void Draw(SpriteBatch spriteBatch, Rectangle ClientBounds)
        {
            float UIStartX = Main.CameraDisplaySize;
            float verticalSpacing = ClientBounds.Height / 2;
            float UIWidth = ClientBounds.Width - UIStartX;
            float stretchX = UIWidth / 300f;
            float stretchY = ClientBounds.Height / 900f;
            UIScale = 2f * (float)Math.Min(stretchX, stretchY);
            spriteBatch.Draw(texture: AssetManager.ui[0], position: Vector2.UnitX * Main.CameraDisplaySize, color: Color.White, scale: new Vector2(stretchX, stretchY));
            spriteBatch.Draw(texture: AssetManager.ui[11], position: Vector2.UnitY * Main.CameraDisplaySize, color: Color.White, scale: new Vector2((ClientBounds.Width - UIWidth) / 900f, (ClientBounds.Height - Main.CameraDisplaySize) / 300f));
            if (Main.mode == Mode.Menu)
            {
                float textWidth = UIStartX / 2;
                string title = "Project Gaze";
                float startName = ClientBounds.Height / 8f;
                Vector2 NameGrandness = Main.font.MeasureString(title);
                float nameScale = textWidth / NameGrandness.X;
                //Debug.WriteLine(nameScale);
                spriteBatch.DrawString(Main.font, title, new Vector2((UIStartX / 2f) - (nameScale * NameGrandness.X * 0.5f), startName), Color.White, scale: nameScale, origin: Vector2.Zero, rotation: 0f, effects: SpriteEffects.None, layerDepth: 0);

                MenuManager.Draw(spriteBatch);

                spriteBatch.Draw(texture: AssetManager.ui[12], position: Controls.mouse.Position.ToVector2(), scale: Vector2.One * 2f);
            }
            else
            {
                Networking.DisplayDebug(spriteBatch);
                for (int i = 0; i < 2; i++)
                {
                    if (Main.ships[i] != null)
                    {
                        Vector2 BarCenter = new Vector2((ClientBounds.Width + UIStartX) / 2, (i * verticalSpacing) + (usePortraits ? verticalSpacing / 3f : verticalSpacing / 2f));
                        Texture2D texture = ShipStats.GetIcon(Main.ships[i].type);
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
                        spriteBatch.Draw(texture: texture, position: (BarCenter - (new Vector2(Width, Height) * .5f * scale)), scale: (new Vector2(1f, 1f) * scale), color: Color.White);

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
                            spriteBatch.Draw(texture: texture, position: portraitPosition, scale: Vector2.One * scale, color: Color.White, sourceRectangle: new Rectangle(0, 0 * Height, Width, Height));
                            if(Controls.controlRight[i] && !Controls.controlLeft[i])
                            {
                                spriteBatch.Draw(texture: texture, position: portraitPosition, scale: Vector2.One * scale, color: Color.White, sourceRectangle: new Rectangle(0, 1 * Height, Width, Height));
                            }
                            if (!Controls.controlRight[i] && Controls.controlLeft[i])
                            {
                                spriteBatch.Draw(texture: texture, position: portraitPosition, scale: Vector2.One * scale, color: Color.White, sourceRectangle: new Rectangle(0, 2 * Height, Width, Height));
                            }
                            if (Controls.controlThrust[i])
                            {
                                spriteBatch.Draw(texture: texture, position: portraitPosition, scale: Vector2.One * scale, color: Color.White, sourceRectangle: new Rectangle(0, 3 * Height, Width, Height));
                            }
                            if (Controls.controlShoot[i])
                            {
                                spriteBatch.Draw(texture: texture, position: portraitPosition, scale: Vector2.One * scale, color: Color.White, sourceRectangle: new Rectangle(0, 4 * Height, Width, Height));
                            }
                            if (Controls.controlSpecial[i])
                            {
                                spriteBatch.Draw(texture: texture, position: portraitPosition, scale: Vector2.One * scale, color: Color.White, sourceRectangle: new Rectangle(0, 5 * Height, Width, Height));
                            }
                        }
                        //Main.ships[i].LocalDraw(spriteBatch, BarCenter, true);
                        float startBars = verticalSpacing + (verticalSpacing * i) - spacing * UIScale * 3;


                        for (int e = 0; e < Main.ships[i].energyCapacity / 2; e++)
                        {
                            spriteBatch.Draw(texture: AssetManager.ui[2], position: new Vector2(ClientBounds.Width - spacing * UIScale - (AssetManager.ui[2].Width * UIScale) / 2, startBars - (AssetManager.ui[2].Height * UIScale) * e), scale: Vector2.One * UIScale);
                        }
                        for (int e = 0; e < Main.ships[i].energy; e++)
                        {
                            spriteBatch.Draw(texture: AssetManager.ui[3], position: new Vector2(ClientBounds.Width - spacing * UIScale + (e % 2 == 0 ? 0 : -(AssetManager.ui[3].Width * UIScale)), startBars - (AssetManager.ui[3].Height * UIScale) * (int)(e / 2)), scale: Vector2.One * UIScale);
                        }


                        for (int h = 0; h < Main.ships[i].healthMax / 2; h++)
                        {

                            spriteBatch.Draw(texture: AssetManager.ui[2], position: new Vector2(UIStartX + spacing * UIScale - (AssetManager.ui[2].Width * UIScale) / 2, startBars - (AssetManager.ui[2].Height * UIScale) * (h)), scale: Vector2.One * UIScale);
                        }

                        for (int k = 0; k < Main.ships[i].health; k++)
                        {
                            spriteBatch.Draw(texture: AssetManager.ui[4], position: new Vector2(UIStartX + spacing * UIScale + (k % 2 == 0 ? 0 : -(AssetManager.ui[4].Width * UIScale)), startBars - (AssetManager.ui[4].Height * UIScale) * (int)((k / 2))), scale: Vector2.One * UIScale);
                        }

                        int total = Main.ships[i].healthMax;
                        int g = Main.ships[i].healthMax / 2;
                        for (int b = 0; b < Main.ships[i].ExtraHealthBoxes.Count; b++)
                        {
                            int HealthBox = Main.ships[i].ExtraHealthBoxes[b];
                            g++;
                            total += 2;
                            total += HealthBox;
                            for (; g < total / 2; g++)
                            {
                                spriteBatch.Draw(texture: AssetManager.ui[2], position: new Vector2(UIStartX + spacing * UIScale - (AssetManager.ui[2].Width * UIScale) / 2, startBars - (AssetManager.ui[2].Height * UIScale) * (g)), scale: Vector2.One * UIScale);
                            }
                            //Debug.WriteLine(Main.ships[i].ExtraHealths[b]);
                            for (int k = 0; k < Main.ships[i].ExtraHealths[b]; k++)
                            {
                                int pos = k + total - HealthBox;
                                spriteBatch.Draw(texture: AssetManager.ui[4], position: new Vector2(UIStartX + spacing * UIScale + (pos % 2 == 0 ? 0 : -(AssetManager.ui[4].Width * UIScale)), startBars - (AssetManager.ui[4].Height * UIScale) * (int)(((pos) / 2))), scale: Vector2.One * UIScale);
                            }
                        }
                        ShipStats.GetTitlesFor(Main.ships[i].type, out string race, out string shipName);
                        string name = race + " " + shipName;
                        float startName = (verticalSpacing * i) + spacing * UIScale;
                        Vector2 NameGrandness = Main.font.MeasureString(name) * UIScale;
                        float maxWidth = UIWidth * 0.8f;
                        float nameScale = 1f;
                        if (NameGrandness.X > maxWidth)
                        {
                            nameScale = maxWidth / NameGrandness.X;
                        }
                        //Debug.WriteLine(nameScale);
                        spriteBatch.DrawString(Main.font, name, new Vector2((ClientBounds.Width + UIStartX) / 2 - (NameGrandness.X * nameScale) / 2, startName), Color.Black, scale: UIScale * nameScale, origin: Vector2.Zero, rotation: 0f, effects: SpriteEffects.None, layerDepth: 0);
                        NameGrandness = Main.font.MeasureString("Armor");
                        spriteBatch.DrawString(Main.font, "Armor", new Vector2(UIStartX + spacing * UIScale - (.25f * UIScale) * NameGrandness.X / 2, startBars + spacing * UIScale), Color.Black, scale: .25f * UIScale, origin: Vector2.Zero, rotation: 0f, effects: SpriteEffects.None, layerDepth: 0);
                        NameGrandness = Main.font.MeasureString("Energy");
                        spriteBatch.DrawString(Main.font, "Energy", new Vector2(ClientBounds.Width - spacing * UIScale - (.25f * UIScale) * NameGrandness.X / 2, startBars + spacing * UIScale), Color.Black, scale: .25f * UIScale, origin: Vector2.Zero, rotation: 0f, effects: SpriteEffects.None, layerDepth: 0);
                    }
                }
            }
        }
    }
}
