using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Menus
{
    public class StartNetPlay : MenuOption
    {
        public StartNetPlay(Rectangle Bounds) : base(Bounds)
        {
            text = "Network";
        }
        public override void OnClick()
        {
            MenuManager.SetMenuType(MenuType.HostJoinOption);
        }
    }
}
