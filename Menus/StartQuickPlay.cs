using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Menus
{
    public class StartQuickPlay : MenuOption
    {
        public StartQuickPlay(Rectangle Bounds) : base(Bounds)
        {
           text = "Quick Play";
        }
        public override void OnClick()
        {
            if(Networking.GetNetMode() == NetMode.server)
            {
                Networking.ServerStartQuickPlay();
            }
            else
            {
                Main.StartQuickPlay();
            }
        }
    }
}
