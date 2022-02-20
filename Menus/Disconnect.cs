using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Menus
{
    public class Disconnect : MenuOption
    {
        public Disconnect(Rectangle Bounds) : base(Bounds)
        {
            text = "Disconnect";
        }
        public override void OnClick()
        {
            Networking.Disconnect();
        }
    }
}
