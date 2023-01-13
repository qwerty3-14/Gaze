using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.MyraUI
{
    public static class MyraMainMenu
    {
        public static Grid grid;
        static TextButton multiplayerToggle;
        static TextButton networking;
        public static void Setup()
        {

            grid = MyraMain.BaseGrid();

            grid.ColumnsProportions.Add(new Proportion());

            // Add widgets
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

            var startQuickPlay = new TextButton();
            startQuickPlay.Text = "Quick Play";
            startQuickPlay.Click += (s, a) =>
            {
                if (Networking.GetNetMode() == NetMode.server)
                {
                    Networking.ServerStartQuickPlay();
                }
                else if(Networking.GetNetMode() == NetMode.disconnected)
                {
                    Main.StartQuickPlay();
                }
            };
            MyraMain.StandardAlignment(grid, startQuickPlay);

            var fleets = new TextButton();
            fleets.Text = "Fleets";
            fleets.Click += (s, a) =>
            {
                if(Networking.GetNetMode() != NetMode.client)
                {
                    MyraMain.SwitchMenu(MenuType.FleetBuilder);
                    FleetBuilder.SetMultiplayerText(); 
                }
            };
            MyraMain.StandardAlignment(grid, fleets);

            var settings = new TextButton();
            settings.Text = "Settings";
            settings.Click += (s, a) =>
            {
                MyraMain.SwitchMenu(MenuType.Settings);
            };
            MyraMain.StandardAlignment(grid, settings);

            networking = new TextButton();
            networking.Text = "Network";
            networking.Click += (s, a) =>
            {
                if (Networking.IsConnected())
                {
                    Networking.Disconnect();
                    SetNetworkingText();
                    SetMultiplayerText();
                }
                else
                {
                    MyraMain.SwitchMenu(MenuType.HostJoinOption);
                }
                
            };
            MyraMain.StandardAlignment(grid, networking);

            var quit = new TextButton();
            quit.Text = "Quit";
            quit.Click += (s, a) =>
            {
                Main.instance.Exit();
            };
            MyraMain.StandardAlignment(grid, quit);

            
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
        public static void SetNetworkingText()
        {
            if(Networking.IsConnected())
            {
                networking.Text = "Disconnect";
            }
            else
            {
                networking.Text = "Network";
            }
        }
    }
}
