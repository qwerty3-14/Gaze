using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.MyraUI
{
    public class MyraMain
    {

		private static Desktop desktop;
        private static MenuType menuType;
        private static Panel rootPanel;
        private static FontSystem uiFont, uiBlackFont;
        public static MenuType GetMenuType()
        {
            return menuType;
        }
        //private static Grid sideGrid;
        public static void LoadContent(Game game, GameWindow Window)
		{
			MyraEnvironment.Game = game;
            
            byte[] ttfData = File.ReadAllBytes(@"Content/Roboto-Light.ttf");
            // Ordinary DynamicSpriteFont
            uiFont = new FontSystem();
            uiFont.AddFont(ttfData);

            ttfData = File.ReadAllBytes(@"Content/Roboto-Black.ttf");
            // Ordinary DynamicSpriteFont
            uiBlackFont = new FontSystem();
            uiBlackFont.AddFont(ttfData);

            desktop = new Desktop();

            rootPanel = new Panel();

            MyraMainMenu.Setup();
            MyraSettingsMenu.Setup();
            StartNetplayMenu.Setup();
            MyraGameplaySettings.Setup();
            MyraDisplaySettings.Setup();
            MyraAudioSettings.Setup();
            MyraControlSettings.Setup();
            FleetBuilder.Setup();
            CombatUI.Setup();

            rootPanel.Widgets.Add(MyraMainMenu.grid);
            rootPanel.Widgets.Add(MyraSettingsMenu.grid);
            rootPanel.Widgets.Add(StartNetplayMenu.grid);
            rootPanel.Widgets.Add(MyraGameplaySettings.grid);
            rootPanel.Widgets.Add(MyraDisplaySettings.grid);
            rootPanel.Widgets.Add(MyraAudioSettings.grid);
            rootPanel.Widgets.Add(MyraControlSettings.grid);
            rootPanel.Widgets.Add(FleetBuilder.grid);
            rootPanel.Widgets.Add(CombatUI.panel);

            SwitchMenu(MenuType.Main);
            BindGrid(Window.ClientBounds);
            //rootPanel.Widgets.Add(MVP());

            desktop.Root = rootPanel;

		}
        public static Grid BaseGrid()
        {
            return new Grid()
            {
                //ShowGridLines = true,
                Visible = false,
                ColumnSpacing = 8,
                RowSpacing = 8,
            };
        }
        public static void SwitchMenu(MenuType menu)
        {
            menuType = menu;
            MyraMainMenu.grid.Visible = false;
            MyraSettingsMenu.grid.Visible = false;
            StartNetplayMenu.grid.Visible = false;
            MyraGameplaySettings.grid.Visible = false;
            MyraDisplaySettings.grid.Visible = false;
            MyraAudioSettings.grid.Visible = false;
            MyraControlSettings.grid.Visible = false;
            FleetBuilder.grid.Visible = false;
            CombatUI.panel.Visible = false;
            switch (menuType)
            {
                case MenuType.Main:
                    MyraMainMenu.grid.Visible = true;
                    break;
                case MenuType.Settings:
                    MyraSettingsMenu.grid.Visible = true;
                    break;
                case MenuType.HostJoinOption:
                    StartNetplayMenu.grid.Visible = true;
                    break;
                case MenuType.Gameplay:
                    MyraGameplaySettings.grid.Visible = true;
                    break;
                case MenuType.Video:
                    MyraDisplaySettings.grid.Visible = true;
                    break;
                case MenuType.Audio:
                    MyraAudioSettings.grid.Visible = true;
                    break;
                case MenuType.Controls:
                    MyraControlSettings.grid.Visible = true;
                    break;
                case MenuType.FleetBuilder:
                    FleetBuilder.grid.Visible = true;
                    break;
                case MenuType.None:
                    CombatUI.panel.Visible = true;
                    break;
            }
        }
        public static void Update(GameWindow Window)
        {
            if(menuType == MenuType.Controls)
            {
                MyraControlSettings.Update();
            }
            BindGrid(Window.ClientBounds);
        }
        public static void BindGrid(Rectangle ClientBounds)
        {
            switch (menuType)
            {
                case MenuType.Main:
                    BindThisGrid(ClientBounds, MyraMainMenu.grid);
                    break;
                case MenuType.Settings:
                    BindThisGrid(ClientBounds, MyraSettingsMenu.grid);
                    break;
                case MenuType.HostJoinOption:
                    BindThisGrid(ClientBounds, StartNetplayMenu.grid);
                    break;
                case MenuType.Gameplay:
                    BindThisGrid(ClientBounds, MyraGameplaySettings.grid);
                    break;
                case MenuType.Video:
                    BindThisGrid(ClientBounds, MyraDisplaySettings.grid);
                    break;
                case MenuType.Audio:
                    BindThisGrid(ClientBounds, MyraAudioSettings.grid);
                    break;
                case MenuType.Controls:
                    BindThisGrid(ClientBounds, MyraControlSettings.grid);
                    break;
                case MenuType.FleetBuilder:
                    BindThisGrid(ClientBounds, FleetBuilder.grid);
                    break;
                    
            }
            
        }
        private static void BindThisGrid(Rectangle ClientBounds, Grid grid)
        {
            float UIStartX = Camera.CameraDisplaySize;
            float UIWidth = ClientBounds.Width - UIStartX;
            float UIHeight = ClientBounds.Height;
            int width = 7 * (int)UIWidth / 8;
            int height = (int)(25 * (float)ClientBounds.Height / 450f);
            int x = (int)UIStartX + (int)UIWidth / 16;
            int y = ClientBounds.Height / 8;


            grid.Left = x;
            grid.Width = width;
            grid.Top = y;
            grid.Height = (int)UIHeight - y;

            foreach (Widget widget in grid.Widgets)
            {
                widget.Height = height;
                widget.Width = width;
                AdjustFont(widget);
                if (widget is Grid)
                {
                    if(menuType == MenuType.Controls)
                    {
                        widget.Height = height / 2;
                    }
                    foreach(Widget subWidget in ((Grid)widget).Widgets)
                    {
                        subWidget.VerticalAlignment = VerticalAlignment.Center;

                        if (!(subWidget is HorizontalSlider))
                        {
                            subWidget.Height = widget.Height;
                        }
                        AdjustFont(subWidget);
                    }
                    if (((Grid)widget).Widgets.Count > 1)
                    {
                        if (((Grid)widget).Widgets[1] is HorizontalSlider || ((Grid)widget).Widgets[1] is TextBox)
                        {
                            ((Grid)widget).Widgets[0].Width = (int)((float)width / 4f);
                            ((Grid)widget).Widgets[1].Width = (int)(3f * (float)width / 4f);
                        }
                        else
                        {
                            foreach (Widget subWidget in ((Grid)widget).Widgets)
                            {
                                subWidget.Width = (int)((float)width / ((Grid)widget).Widgets.Count);
                            }
                        }
                    }
                }
                
            }
        }
        public static void AdjustFont(Widget widget, bool black = false,  float scale = 1f)
        {
            if (widget != null)
            {
                FontSystem useFont = black ? uiBlackFont : uiFont;
                if (widget is Label)
                {
                    ((Label)widget).Font = useFont.GetFont((int)(scale * widget.Height / 2f));
                }
                if (widget is TextBox)
                {
                    ((TextBox)widget).Font = useFont.GetFont((int)(scale * widget.Height / 2f));
                }
                if (widget is TextButton)
                {
                    ((TextButton)widget).Font = useFont.GetFont((int)(scale * widget.Height / 2f));
                }
            }
        }
        public static void StandardAlignment(Grid grid, Widget widget)
        {
            grid.RowsProportions.Add(new Proportion());
            widget.GridRow = grid.Widgets.Count;
            widget.HorizontalAlignment = HorizontalAlignment.Center;
            grid.Widgets.Add(widget);
        }

        public static void AddToRoot(Widget widget)
        {
            rootPanel.Widgets.Add(widget);
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            desktop.Render();
        }

    }
    public enum MenuType : byte
    {
        Main,
        Settings,
        HostJoinOption,
        Gameplay,
        Video,
        Audio,
        Controls,
        FleetBuilder,
        None
    }
}
