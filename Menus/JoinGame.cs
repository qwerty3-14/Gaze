using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Menus
{
    public class JoinGame : MenuOption
    {

        public JoinGame(Rectangle Bounds) : base(Bounds)
        {
            text = "Join Game";
        }
        public override void OnClick()
        {
            Networking.StartClient();
            MenuManager.SetMenuType(MenuType.Main);
        }
    }
}
