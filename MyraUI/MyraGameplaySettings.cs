using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.MyraUI
{
    public static class MyraGameplaySettings
    {
        public static Grid grid;
        static  TextButton toggleCamera;
        public static void Setup()
        {

            grid = MyraMain.BaseGrid();

            grid.ColumnsProportions.Add(new Proportion());

            // Add
            toggleCamera = new TextButton();
            toggleCamera.Text = "Solo Camera: Off";
            toggleCamera.Click += (s, a) =>
            {
                Camera.caneUseSoloCamera = !Camera.caneUseSoloCamera;
                UpdateCaeraText();
            };
            UpdateCaeraText();
            MyraMain.StandardAlignment(grid, toggleCamera);

            var returnToMain = new TextButton();
            returnToMain.Text = "Back";
            returnToMain.Click += (s, a) =>
            {
                MyraMain.SwitchMenu(MenuType.Settings);
            };
            MyraMain.StandardAlignment(grid, returnToMain);
        }
        static void UpdateCaeraText()
        {
            toggleCamera.Text = "Solo Camera: " + (Camera.caneUseSoloCamera ? "On" : "Off");
        }
    }
}
