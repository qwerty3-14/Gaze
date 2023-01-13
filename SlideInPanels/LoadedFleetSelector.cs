using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using GazeOGL.MyraUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Myra.Graphics2D.UI.Styles;

namespace GazeOGL.SlideInPanels
{
    public class LoadedFleetSelector : SlideInPanel
    {
        ScrollViewer scrollViewer;
        Grid grid;
        List<ImageTextButton> buttons = new List<ImageTextButton>();
        List<Widget> labels = new List<Widget>();
        public LoadedFleetSelector(Vector2 position, Vector2 size, SlideDirection slideDirection) : base(position, size, slideDirection)
        {
            
        }
        public override void SetupPanelContent(Vector2 position, Vector2 size, ref Panel root)
        {
            ReloadGrid();
            
        }
        public override void UpdatePanelContent(Vector2 position, Vector2 size, ref Panel root)
        {
            scrollViewer.Width = root.Width;
            scrollViewer.Height = root.Height;
            foreach(ImageTextButton btn in buttons)
            {
                btn.Width = btn.Height = root.Width / 12;
            }
            foreach(Widget name in labels)
            {
                name.Height = root.Width / 18;
                MyraMain.AdjustFont(name);
                name.Width = root.Width;
            }
        }
        public void ReloadGrid()
        {
            buttons.Clear();
            rootPanel.Widgets.Clear();
            grid = new Grid()
            {
                ShowGridLines = true,
                ColumnSpacing = 0,
                RowSpacing = 0,
            };
            for(int i =0; i <1; i++)
            {
                grid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
            }
            for(int i =0; i < FleetsManager.savedFleets.Count; i++)
            {
                 grid.RowsProportions.Add(new Proportion(ProportionType.Part));
                 grid.Widgets.Add(DisplayFleet(FleetsManager.savedFleets[i]));
            }
            scrollViewer = new ScrollViewer();
            scrollViewer.Content = grid;
            rootPanel.Widgets.Add(scrollViewer);
            
        }
        Panel DisplayFleet(Fleet fleet)
        {
            Panel panel = new Panel();
            Console.WriteLine(rootPanel.Width);
            Console.WriteLine(rootPanel.Height);
            panel.GridRow = grid.Widgets.Count;
            Grid shipDisplayGrid = new Grid()
            {
                //ShowGridLines = true,
                ColumnSpacing = 0,
                RowSpacing = 0,
            };
            shipDisplayGrid.Width = panel.Width;
            shipDisplayGrid.Height = panel.Height / 2;
            shipDisplayGrid.RowsProportions.Add(new Proportion(ProportionType.Part));
            for(int i =0; i <12; i++)
            {
                shipDisplayGrid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
                ImageTextButton btn = new ImageTextButton();
                btn.GridRow = 0;
                btn.GridColumn = i; 
                Texture2D texture = ShipStats.GetIcon(fleet.ships[i]);
                if(texture != null)
                {
                    IImage image = new TextureRegion(texture);
                    btn.Image = image;
                }
                btn.ContentHorizontalAlignment = HorizontalAlignment.Center;
                btn.ContentVerticalAlignment = VerticalAlignment.Center;
                buttons.Add(btn);
                shipDisplayGrid.Widgets.Add(btn);
            }
            shipDisplayGrid.RowsProportions.Add(new Proportion(ProportionType.Part));
            TextButton select = new TextButton();
            select.GridColumn = 0;
            select.GridRow = 1;
            select.Text = fleet.name;
            select.GridColumnSpan = 12;
            select.Click += (s, a) =>
            {
                FleetsManager.fleets[PanelManager.fillShipSlot == -2 ? 0 : 1] = FleetsManager.savedFleets[panel.GridRow].Copy();
                PanelManager.fillShipSlot = -1;
                PanelManager.UpdateName();
            };
            labels.Add(select);
            shipDisplayGrid.Widgets.Add(select);

            panel.Widgets.Add(shipDisplayGrid);
            return panel;
        }
    }
}
