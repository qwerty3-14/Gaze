using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Menus
{
    public class BackToMainMenu : MenuOption
    {
        public BackToMainMenu(Rectangle Bounds) : base(Bounds)
        {
            text = "Back";
        }
        public override void OnClick()
        {
            MenuManager.SetMenuType(MenuType.Main);
        }
    }
}
