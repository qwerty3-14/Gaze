using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Menus
{
    public class MultiplayerToggle : MenuOption
    {
        public MultiplayerToggle(Rectangle Bounds) : base(Bounds)
        {
            SetText();
        }

        void SetText()
        {
            text = (Main.startAI[0] ? "AI" : "Player") + " Vs " + (Main.startAI[1] ? "AI" : "Player");
        }
        public override void OnClick()
        {
            if(!Main.startAI[0] && Main.startAI[1])
            {
                Main.startAI[0] = false;
                Main.startAI[1] = false;
            }
            else if (!Main.startAI[0] && !Main.startAI[1])
            {
                Main.startAI[0] = true;
                Main.startAI[1] = true;
            }
            else if (Main.startAI[0] && Main.startAI[1])
            {
                Main.startAI[0] = false;
                Main.startAI[1] = true;
            }
            SetText();
        }
    }
}
