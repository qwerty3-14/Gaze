using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.MyraUI
{
    public class ShipImage : IImage
    {
        ShipID shipID;
        public ShipImage(ShipID shipID)
        {
            this.shipID = shipID;
        }
        public Point Size => throw new NotImplementedException();

        public void Draw(RenderContext context, Rectangle dest, Color color)
        {
            Texture2D texture = ShipStats.GetIcon(shipID);
            context.Draw(texture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
            //throw new NotImplementedException();
        }
    }
}
