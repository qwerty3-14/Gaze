using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using GazeOGL.Entities.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.SlideInPanels
{
    public class ShipSelectPanel : SlideInPanel
    {
        int team;
        Grid grid, cursorGrid;
        ImageTextButton[,] buttons = new ImageTextButton[6, 2];
        ImageTextButton[,] cursorHighlight = new ImageTextButton[6, 2];
        Point cursorPos = new Point(0, 0);


        public ShipSelectPanel(Vector2 position, Vector2 size, SlideDirection slideDirection, int team) : base(position, size, slideDirection)
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
                    btn.ContentHorizontalAlignment = HorizontalAlignment.Center;
                    btn.ContentVerticalAlignment = VerticalAlignment.Center;
                    grid.Widgets.Add(btn);
                    buttons[x, y] = btn;
                }
            }
            root.Widgets.Add(grid);


            cursorGrid = new Grid()
            {
                ShowGridLines = false,
                ColumnSpacing = 0,
                RowSpacing = 0,
            };
            for (int i = 0; i < 6; i++)
            {
                cursorGrid.ColumnsProportions.Add(new Proportion());
            }
            cursorGrid.RowsProportions.Add(new Proportion());
            cursorGrid.RowsProportions.Add(new Proportion());

            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    ImageTextButton btn = new ImageTextButton();
                    btn.GridRow = y;
                    btn.GridColumn = x;
                    IImage image = new TextureRegion(AssetManager.ui[7]);
                    btn.Image = image;
                    btn.ContentHorizontalAlignment = HorizontalAlignment.Center;
                    btn.ContentVerticalAlignment = VerticalAlignment.Center;

                    btn.Background = new SolidBrush(new Color(0, 0, 0, 0));
                    btn.FocusedBackground = new SolidBrush(new Color(0, 0, 0, 0)); 
                    btn.DisabledBackground= new SolidBrush(new Color(0, 0, 0, 0));
                    btn.OverBackground = new SolidBrush(new Color(0, 0, 0, 0));
                    btn.PressedBackground = new SolidBrush(new Color(0, 0, 0, 0));

                    //btn.Visible = false;
                    cursorGrid.Widgets.Add(btn);
                    cursorHighlight[x, y] = btn;
                }
            }
            root.Widgets.Add(cursorGrid);
        }
        bool justMoved = false;
        public override void UpdatePanelContent(Vector2 position, Vector2 size, ref Panel root)
        {
            if (Main.mode == Mode.ShipSelect)
            {
                if (!justMoved)
                {
                    //Console.WriteLine(Controls.controlRight[team]);
                    if (Controls.controlRight[team])
                    {
                        cursorPos.X++;
                        if (cursorPos.X > 5)
                        {
                            cursorPos.X = 0;
                        }
                    }
                    if (Controls.controlLeft[team])
                    {
                        cursorPos.X--;
                        if (cursorPos.X < 0)
                        {
                            cursorPos.X = 5;
                        }
                    }

                    if (Controls.controlThrust[team])
                    {
                        cursorPos.Y--;
                        if (cursorPos.Y < 0)
                        {
                            cursorPos.Y = 1;
                        }
                    }
                    if (Controls.controlDown[team])
                    {
                        cursorPos.Y++;
                        if (cursorPos.Y > 1)
                        {
                            cursorPos.Y = 0;
                        }
                    }

                    if (Controls.controlShoot[team] && storeOffset == 0)
                    {
                        int x = cursorPos.X;
                        int y = cursorPos.Y;
                        if (!FleetsManager.fleets[team].destroyed[x + y * 6] && FleetsManager.fleets[team].ships[x + y * 6] != ShipID.Count)
                        {
                            Arena.ships[team] = Ship.Spawn(FleetsManager.fleets[team].ships[x + y * 6], team);
                            FleetsManager.fleets[team].destroyed[x + y * 6] = true;
                            PanelManager.shipSelectorActive[team] = false;
                            if (Arena.ships[team == 0 ? 1 : 0] != null)
                            {
                                Main.mode = Mode.Fleets;
                            }
                        }
                    }
                }
            }
            justMoved = false;
            if (Controls.controlRight[team] || Controls.controlLeft[team] || Controls.controlThrust[team] || Controls.controlDown[team])
            {
                justMoved = true;
            }
            //Console.WriteLine(cursorPos.X);
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
                    if (texture != null && !FleetsManager.fleets[team].destroyed[x + y * 6])
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

            cursorGrid.Width = root.Width;
            cursorGrid.Height = root.Height;
            //grid.Top = (int)root.Height / 4;
            foreach (Widget widget in cursorGrid.Widgets)
            {
                widget.Width = cursorGrid.Width / 6;
                widget.Height = cursorGrid.Height / 2;
            }
            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    Texture2D texture = AssetManager.ui[7];
                    cursorHighlight[x, y].Image = new TextureRegion(texture);
                    Point imageSize = cursorHighlight[x, y].Image.Size;
                    float scale = (float)cursorHighlight[x, y].Width / (float)texture.Width;
                    cursorHighlight[x, y].ImageWidth = (int)(scale * (float)imageSize.X);
                    cursorHighlight[x, y].ImageHeight = (int)(scale * (float)imageSize.Y);
                    //cursorHighlight[x, y].Visible = false;
                    cursorHighlight[x, y].Image = null;
                    if (cursorPos == new Point(x, y))
                    {
                        cursorHighlight[x, y].Image = new TextureRegion(AssetManager.ui[7]); ;
                        //cursorHighlight[x, y].Visible = true;
                    }
                }
            }
        }
        int AI_PickShip = -1;
        int AI_delay = 20;
        public void AISelect()
        {
            if(storeOffset == 0)
            {
                if(AI_PickShip == -1)
                {
                    if(Arena.ships[team == 0 ? 1 : 0] != null)
                    {
                        AI_PickShip = FleetsManager.fleets[team].BestMatchupVs(Arena.ships[team == 0 ? 1 : 0].type);
                    }
                    else
                    {
                        AI_PickShip = FleetsManager.fleets[team].CheapestShipIndex();
                    }
                }

                Controls.controlThrust[team] = false;
                Controls.controlRight[team] = false;
                Controls.controlLeft[team] = false;
                Controls.controlDown[team] = false;
                Controls.controlShoot[team] = false;
                if (AI_delay <= 0)
                {
                    int x = cursorPos.X;
                    int y = cursorPos.Y;
                    int gotoX = AI_PickShip % 6;
                    int gotoY = AI_PickShip / 6;
                    if (x < gotoX)
                    {
                        Controls.controlRight[team] = true;
                    }
                    else if (x > gotoX)
                    {
                        Controls.controlLeft[team] = true;
                    }
                    else if (y < gotoY)
                    {
                        Controls.controlDown[team] = true;
                    }
                    else if (y > gotoY)
                    {
                        Controls.controlThrust[team] = true;
                    }
                    else
                    {
                        Controls.controlShoot[team] = true;
                        AI_PickShip = -1;
                    }
                    AI_delay = 20;
                }
                else
                {
                    AI_delay--;
                }
            }
        }
        public void ResetCursor()
        {
            cursorPos = new Point(0, 0);
        }
    }
}
