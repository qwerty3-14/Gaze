using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Menus
{
    public class Settings : MenuOption
    {
        public Settings(Rectangle Bounds) : base(Bounds)
        {
            text = "Settings";
        }
        public override void OnClick()
        {
            MenuManager.SetMenuType(MenuType.Settings);
        }
    }
}
