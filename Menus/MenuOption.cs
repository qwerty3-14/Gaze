using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Menus
{
    public abstract class MenuOption
    {
        Rectangle Bounds;
        protected string text;
        public bool isHovering = false;
        public MenuOption(Rectangle Bounds)
        {
            this.Bounds = Bounds;
        }
        public bool Hovering()
        {
            if(Bounds.Contains(Controls.mouse.Position))
            {
                isHovering = true;
                return true;
            }
            return false;
        }
        public virtual void OnClick()
        {
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 NameGrandness = Main.font.MeasureString(text);
            float nameScale = Math.Min(Bounds.Width / NameGrandness.X , Bounds.Height / NameGrandness.Y );
            Vector2 size = NameGrandness * nameScale;
            spriteBatch.DrawString(Main.font, text, Bounds.Center.ToVector2(), Color.Black, scale: nameScale * 0.9f, origin: NameGrandness * 0.5f, rotation: 0f, effects: SpriteEffects.None, layerDepth: 0);
            if(isHovering)
            {
                spriteBatch.Draw(Main.pixel, Bounds.Location.ToVector2() + Vector2.UnitX * Bounds.Width / 2, null, Color.Black, 0, new Vector2(.5f, 0), new Vector2(Bounds.Width, 3), SpriteEffects.None, 0);
                spriteBatch.Draw(Main.pixel, Bounds.Location.ToVector2() + Vector2.UnitY * Bounds.Height + Vector2.UnitX * Bounds.Width / 2, null, Color.Black, 0, new Vector2(.5f, 0), new Vector2(Bounds.Width, 3), SpriteEffects.None, 0);
                spriteBatch.Draw(Main.pixel, Bounds.Location.ToVector2() + Vector2.UnitY * Bounds.Height / 2, null, Color.Black, 0, new Vector2(0, .5f), new Vector2(3, Bounds.Height), SpriteEffects.None, 0);
                spriteBatch.Draw(Main.pixel, Bounds.Location.ToVector2() + Vector2.UnitX * Bounds.Width + Vector2.UnitY * Bounds.Height / 2, null, Color.Black, 0, new Vector2(0, .5f), new Vector2(3, Bounds.Height), SpriteEffects.None, 0);
            }
        }
    }
}
