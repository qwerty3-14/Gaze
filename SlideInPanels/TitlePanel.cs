﻿using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using GazeOGL.MyraUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.SlideInPanels
{
    public class TitlePanel : SlideInPanel
    {
        string text = "Error, did not update text";
        Label label;
        public TitlePanel(Vector2 position, Vector2 size, SlideDirection slideDirection, string text) : base(position, size, slideDirection)
        {
            this.text = text;
        }
        public void SetText(string text)
        {
            this.text = text;
        }
        public override void SetupPanelContent(Vector2 position, Vector2 size, ref Panel root)
        {
            label = new Label();
            label.TextAlign = TextAlign.Center;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            //label.VerticalAlignment = VerticalAlignment.Center;
            label.Width = root.Width;
            label.Height = root.Height;
            //MyraMain.AdjustFont(label);
            root.Widgets.Add(label);
        }
        public override void UpdatePanelContent(Vector2 position, Vector2 size, ref Panel root)
        {
            label.Text = text;
            label.Width = root.Width;
            label.Height = root.Height;
            label.Top = (int)root.Height / 4;
            MyraMain.AdjustFont(label);

        }
    }
}
