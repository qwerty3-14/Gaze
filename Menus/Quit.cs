using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Menus
{
    public class Quit : MenuOption
    {
        public Quit(Rectangle Bounds) : base(Bounds)
        {
            text = "Quit";
        }
        public override void OnClick()
        {
            Main.instance.Exit();
        }
    }
}
