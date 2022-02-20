using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Menus
{
    public class HostGame : MenuOption
    {
        public HostGame(Rectangle Bounds) : base(Bounds)
        {
            text = "Host Game";
        }
        public override void OnClick()
        {
            Networking.StartServer();
            MenuManager.SetMenuType(MenuType.Main);
        }
    }
}
