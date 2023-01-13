using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.MyraUI
{
    public static class FleetBuilder
    {
        static TextButton multiplayerToggle;

        public static Grid grid;
        public static void Setup()
        {

            grid = MyraMain.BaseGrid();

            grid.ColumnsProportions.Add(new Proportion());

            multiplayerToggle = new TextButton();
            multiplayerToggle.Text = "Player Vs AI";
            multiplayerToggle.Click += (s, a) =>
            {
                if (Networking.IsConnected())
                {
                    Main.startAI[0] = false;
                    Main.startAI[1] = false;
                }
                else
                {
                    if (!Main.startAI[0] && Main.startAI[1])
                    {
                        Main.startAI[0] = false;
                        Main.startAI[1] = false;
                    }
                    else if (!Main.startAI[0] && !Main.startAI[1])
                    {
                        Main.startAI[0] = true;
                        Main.startAI[1] = true;
                    }
                    else if (Main.startAI[0] && Main.startAI[1])
                    {
                        Main.startAI[0] = false;
                        Main.startAI[1] = true;
                    }
                }
                SetMultiplayerText();
            };
            MyraMain.StandardAlignment(grid, multiplayerToggle);

            var start = new TextButton();
            start.Text = "Start";
            start.Click += (s, a) =>
            {
                if (Networking.GetNetMode() == NetMode.server)
                {
                    Networking.ServerStartFleets();
                }
                else if (Networking.GetNetMode() == NetMode.disconnected)
                {
                    Main.StartFleets();
                }
                
            };
            MyraMain.StandardAlignment(grid, start);

            var returnToMain = new TextButton();
            returnToMain.Text = "Back";
            returnToMain.Click += (s, a) =>
            {
                MyraMain.SwitchMenu(MenuType.Main);
                MyraMainMenu.SetMultiplayerText();
            };
            MyraMain.StandardAlignment(grid, returnToMain);


        }
        public static void SetMultiplayerText()
        {
            multiplayerToggle.Text = (Main.startAI[0] ? "AI" : "Player") + " Vs " + (Main.startAI[1] ? "AI" : "Player");
            if (Networking.GetNetMode() == NetMode.server)
            {
                multiplayerToggle.Text = "Net Host";
            }
            if (Networking.GetNetMode() == NetMode.client)
            {
                multiplayerToggle.Text = "Net Guest";
            }
        }
    }
}
