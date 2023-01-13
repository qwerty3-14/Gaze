using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.MyraUI
{
    public class StartNetplayMenu
    {
        public static Grid grid;
        static TextBox IPTB;
        static TextBox PortTB;
        public static void Setup()
        {

            grid = MyraMain.BaseGrid();

            grid.ColumnsProportions.Add(new Proportion());

            // Add widgets

            var hostGame = new TextButton();
            hostGame.Text = "Host Game";
            hostGame.Click += (s, a) =>
            {
                MyraMain.SwitchMenu(MenuType.Main);
                Networking.StartServer();
                MyraMainMenu.SetNetworkingText();
                MyraMainMenu.SetMultiplayerText();
            };
            MyraMain.StandardAlignment(grid, hostGame);

            var joinGame = new TextButton();
            joinGame.Text = "Join Game";
            joinGame.Click += (s, a) =>
            {
                Networking.StartClient();
                MyraMain.SwitchMenu(MenuType.Main);
                MyraMainMenu.SetNetworkingText();
                MyraMainMenu.SetMultiplayerText();
            };
            MyraMain.StandardAlignment(grid, joinGame);

            var IP = new Grid();
            IP.ColumnsProportions.Add(new Proportion());
            IP.RowsProportions.Add(new Proportion());

            var IPLabel = new Label();
            IPLabel.Text = "IP:";
            IPLabel.GridColumn = 0;
            IP.Widgets.Add(IPLabel);

            IPTB = new TextBox();
            IPTB.GridColumn = 1;
            IPTB.Text = "127.0.0.1";
            IP.Widgets.Add(IPTB);

            MyraMain.StandardAlignment(grid, IP);

            var Port = new Grid();
            Port.ColumnsProportions.Add(new Proportion());
            Port.RowsProportions.Add(new Proportion());

            var PortLabel = new Label();
            PortLabel.Text = "Port:";
            PortLabel.GridColumn = 0;
            Port.Widgets.Add(PortLabel);

            PortTB = new TextBox();
            PortTB.GridColumn = 1;
            PortTB.Text = "12345";
            Port.Widgets.Add(PortTB);

            MyraMain.StandardAlignment(grid, Port);

            var returnToMain = new TextButton();
            returnToMain.Text = "Back";
            returnToMain.Click += (s, a) =>
            {
                MyraMain.SwitchMenu(MenuType.Main);
            };
            MyraMain.StandardAlignment(grid, returnToMain);


        }
        public static string getRemoteIP()
        {
            return IPTB.Text;
        }
        public static int getPort()
        {
            return int.Parse(PortTB.Text);
        }
    }
}
