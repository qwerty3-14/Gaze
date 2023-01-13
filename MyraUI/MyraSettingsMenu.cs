using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.MyraUI
{
    public static class MyraSettingsMenu
    {
        public static Grid grid;
        public static void Setup()
        {

            grid = MyraMain.BaseGrid();

            grid.ColumnsProportions.Add(new Proportion());

            // Add widgets

            var gameplay = new TextButton();
            gameplay.Text = "Gameplay";
            gameplay.Click += (s, a) =>
            {
                MyraMain.SwitchMenu(MenuType.Gameplay);
            };
            MyraMain.StandardAlignment(grid, gameplay);

            var display = new TextButton();
            display.Text = "Display";
            display.Click += (s, a) =>
            {
                MyraMain.SwitchMenu(MenuType.Video);
            };
            MyraMain.StandardAlignment(grid, display);

            var audio = new TextButton();
            audio.Text = "Audio";
            audio.Click += (s, a) =>
            {
                MyraMain.SwitchMenu(MenuType.Audio);
            };
            MyraMain.StandardAlignment(grid, audio);

            var controls = new TextButton();
            controls.Text = "Controls";
            controls.Click += (s, a) =>
            {
                MyraMain.SwitchMenu(MenuType.Controls);
            };
            MyraMain.StandardAlignment(grid, controls);


            var returnToMain = new TextButton();
            returnToMain.Text = "Back";
            returnToMain.Click += (s, a) =>
            {
                SaveData.SaveManager.SaveSettings();
                MyraMain.SwitchMenu(MenuType.Main);
            };
            MyraMain.StandardAlignment(grid, returnToMain);


        }
    }
}
