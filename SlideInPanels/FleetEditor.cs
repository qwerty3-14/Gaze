using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.SlideInPanels
{
    public class FleetEditor : SlideInPanel
    {
        int team;
        Grid grid;
        ImageTextButton[,] buttons = new ImageTextButton[6, 2];
        public FleetEditor(Vector2 position, Vector2 size, SlideDirection slideDirection, int team) : base(position, size, slideDirection)
        {
            this.team = team;
        }
        public override void SetupPanelContent(Vector2 position, Vector2 size, ref Panel root)
        {

            grid = new Grid()
            {
                ShowGridLines = true,
                ColumnSpacing = 0,
                RowSpacing = 0,
            };
            for (int i = 0; i < 6; i++)
            {
                grid.ColumnsProportions.Add(new Proportion());
            }
            grid.RowsProportions.Add(new Proportion());
            grid.RowsProportions.Add(new Proportion());

            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    ImageTextButton btn = new ImageTextButton();
                    btn.GridRow = y;
                    btn.GridColumn = x;
                    IImage image = new TextureRegion(ShipStats.GetIcon(ShipID.Strafer));
                    btn.Image = image;
                    btn.Click += (s, a) =>
                    {
                        PanelManager.fillShipSlot = team * 12 + 6 * btn.GridRow + btn.GridColumn;
                        //Console.WriteLine(team * 12 + 6 * btn.GridRow + btn.GridColumn);
                    };
                    btn.ContentHorizontalAlignment = HorizontalAlignment.Center;
                    btn.ContentVerticalAlignment = VerticalAlignment.Center;
                    grid.Widgets.Add(btn);
                    buttons[x, y] = btn;
                }
            }
            root.Widgets.Add(grid);
        }
        public override void UpdatePanelContent(Vector2 position, Vector2 size, ref Panel root)
        {
            grid.Width = root.Width;
            grid.Height = root.Height;
            //grid.Top = (int)root.Height / 4;
            foreach (Widget widget in grid.Widgets)
            {
                widget.Width = grid.Width / 6;
                widget.Height = grid.Height / 2;
            }
            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    Texture2D texture = ShipStats.GetIcon(FleetsManager.fleets[team].ships[x + y * 6]);
                    if (texture != null)
                    {
                        buttons[x, y].Image = new TextureRegion(texture);

                       

                        Point imageSize = buttons[x, y].Image.Size;
                        float scale = (float)buttons[x, y].Width / (float)33;
                        buttons[x, y].ImageWidth = (int)(scale * (float)imageSize.X);
                        buttons[x, y].ImageHeight = (int)(scale * (float)imageSize.Y);
                    }
                    else
                    {
                        buttons[x, y].Image = null;
                    }
                }
            }
        }
    }
}
