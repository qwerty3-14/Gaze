using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.MyraUI
{
    public static class MyraDisplaySettings
    {
        public static Grid grid;
        public static void Setup()
        {

            grid = MyraMain.BaseGrid();

            grid.ColumnsProportions.Add(new Proportion());

            // Add widgets
            var toggleFullscreen = new TextButton();
            toggleFullscreen.Text = "Fullscreen";
            toggleFullscreen.Click += (s, a) =>
            {
                Main.instance.graphics.IsFullScreen = !Main.instance.graphics.IsFullScreen;
                Main.instance.graphics.ApplyChanges();
            };
            MyraMain.StandardAlignment(grid, toggleFullscreen);

            /*
            var resolutionLabel = new Label();
            resolutionLabel.Text = "Resolution";
            MyraMain.StandardAlignment(grid, resolutionLabel);
            */

            var lowRes = new TextButton();
            lowRes.Text = "600x450";
            lowRes.Click += (s, a) =>
            {
                Main.instance.graphics.PreferredBackBufferWidth = 600;
                Main.instance.graphics.PreferredBackBufferHeight = 450;
                Main.instance.graphics.ApplyChanges();
            };
            MyraMain.StandardAlignment(grid, lowRes);

            var smallRes = new TextButton();
            smallRes.Text = "900x675";
            smallRes.Click += (s, a) =>
            {
                Main.instance.graphics.PreferredBackBufferWidth = 900;
                Main.instance.graphics.PreferredBackBufferHeight = 625;
                Main.instance.graphics.ApplyChanges();
            };
            MyraMain.StandardAlignment(grid, smallRes);

            var midRes = new TextButton();
            midRes.Text = "1200x900";
            midRes.Click += (s, a) =>
            {
                Main.instance.graphics.PreferredBackBufferWidth = 1200;
                Main.instance.graphics.PreferredBackBufferHeight = 900;
                Main.instance.graphics.ApplyChanges();
            };
            MyraMain.StandardAlignment(grid, midRes);

            var highRes = new TextButton();
            highRes.Text = "1440x1080";
            highRes.Click += (s, a) =>
            {
                Main.instance.graphics.PreferredBackBufferWidth = 1440;
                Main.instance.graphics.PreferredBackBufferHeight = 1080;
                Main.instance.graphics.ApplyChanges();
            };
            MyraMain.StandardAlignment(grid, highRes);

            var returnToMain = new TextButton();
            returnToMain.Text = "Back";
            returnToMain.Click += (s, a) =>
            {
                MyraMain.SwitchMenu(MenuType.Settings);
            };
            MyraMain.StandardAlignment(grid, returnToMain);


        }
    }
}
