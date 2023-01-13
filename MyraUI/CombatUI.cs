using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GazeOGL.MyraUI
{
    public static class CombatUI
    {
        public static Panel panel;
        static Label[] labels = new Label[2];
        public static void Setup()
        {
            panel = new Panel();
            labels[0] = new Label();
            labels[0].TextAlign = TextAlign.Center;
            panel.Widgets.Add(labels[0]);
            labels[1] = new Label();
            labels[1].TextAlign = TextAlign.Center;
            panel.Widgets.Add(labels[1]);
            panel.Visible = false;
        }
        public static void SetLabel(int index, string text, Vector2 position, float width, float height)
        {
            labels[index].Text = text;
            labels[index].Left = (int)position.X;
            labels[index].Top = (int)position.Y;
            labels[index].Width = (int)width;
            labels[index].Height = (int)height;
            MyraMain.AdjustFont(labels[index], true);
        }
    }
}
