using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Menus
{
    public class ToggleFullscreen : MenuOption
    {
        public ToggleFullscreen(Rectangle Bounds) : base(Bounds)
        {
            text = "Toggle Fullscreen";
        }

        public override void OnClick()
        {
            Main.instance.graphics.IsFullScreen = !Main.instance.graphics.IsFullScreen;
            Main.instance.graphics.ApplyChanges();
            //MenuManager.SetMenuType(MenuType.Settings);
        }
    }
}
