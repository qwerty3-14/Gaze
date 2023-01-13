using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using GazeOGL.MyraUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.SlideInPanels
{
    public class FleetPalette : SlideInPanel
    {
        Grid grid;
        Grid statBars;
        Grid abilities;
        ImageTextButton[,] buttons = new ImageTextButton[6, 3];
        TextButton cancelButton;
        TextButton clearButton;
        Label ShipTitle, pointsLabel, mainAbility, specialAbility, passiveAbility, mainDesc, specialDesc, passiveDesc;
        
        HorizontalProgressBar healthBar, batteryBar, reactorBar, accelerationBar, maxSpeedBar, turnSpeedBar, damageBar, rangeBar;
        public FleetPalette(Vector2 position, Vector2 size, SlideDirection slideDirection) : base(position, size, slideDirection)
        {
        }

        public override void SetupPanelContent(Vector2 position, Vector2 size, ref Panel root)
        {
            ShipTitle = new Label();
            ShipTitle.TextAlign = TextAlign.Center;
            ShipTitle.HorizontalAlignment = HorizontalAlignment.Center;
            ShipTitle.Text = "Ship Stats";
            root.Widgets.Add(ShipTitle);

            pointsLabel = new Label();
            pointsLabel.TextAlign = TextAlign.Right;
            pointsLabel.HorizontalAlignment = HorizontalAlignment.Right;
            pointsLabel.Text = "0";
            root.Widgets.Add(pointsLabel);


            statBars = new Grid()
            {
                ShowGridLines = true,
                ColumnSpacing = 0,
                RowSpacing = 0,
            };
            for (int i = 0; i < 8; i++)
            {
                statBars.RowsProportions.Add(new Proportion());
            }
            statBars.ColumnsProportions.Add(new Proportion());
            statBars.ColumnsProportions.Add(new Proportion());
            healthBar = new HorizontalProgressBar();
            healthBar.GridColumn = 1;
            healthBar.GridRow = 0;
            statBars.Widgets.Add(healthBar);
            batteryBar = new HorizontalProgressBar();
            batteryBar.GridColumn = 1;
            batteryBar.GridRow = 1;
            statBars.Widgets.Add(batteryBar);
             reactorBar = new HorizontalProgressBar();
            reactorBar.GridColumn = 1;
            reactorBar.GridRow = 2;
            statBars.Widgets.Add(reactorBar);
             accelerationBar = new HorizontalProgressBar();
            accelerationBar.GridColumn = 1;
            accelerationBar.GridRow = 3;
            statBars.Widgets.Add(accelerationBar);
             maxSpeedBar = new HorizontalProgressBar();
            maxSpeedBar.GridColumn = 1;
            maxSpeedBar.GridRow = 4;
            statBars.Widgets.Add(maxSpeedBar);
             turnSpeedBar = new HorizontalProgressBar();
            turnSpeedBar.GridColumn = 1;
            turnSpeedBar.GridRow = 5;
            statBars.Widgets.Add(turnSpeedBar);
             damageBar = new HorizontalProgressBar();
            damageBar.GridColumn = 1;
            damageBar.GridRow = 6;
            statBars.Widgets.Add(damageBar);
             rangeBar = new HorizontalProgressBar();
            rangeBar.GridColumn = 1;
            rangeBar.GridRow = 7;
            statBars.Widgets.Add(rangeBar);

            Label healthLabel = new Label();
            healthLabel.TextAlign = TextAlign.Center;
            healthLabel.GridColumn = 0;
            healthLabel.GridRow = 0;
            healthLabel.Text = "Armor:";
            statBars.Widgets.Add(healthLabel);
            Label batteryLabel = new Label();
            batteryLabel.TextAlign = TextAlign.Center;
            batteryLabel.GridColumn = 0;
            batteryLabel.GridRow = 1;
            batteryLabel.Text = "Battery:";
            statBars.Widgets.Add(batteryLabel);
            Label reactorLabel = new Label();
            reactorLabel.TextAlign = TextAlign.Center;
            reactorLabel.GridColumn = 0;
            reactorLabel.GridRow = 2;
            reactorLabel.Text = "Reactor:";
            statBars.Widgets.Add(reactorLabel);
            Label accelerationLabel = new Label();
            accelerationLabel.TextAlign = TextAlign.Center;
            accelerationLabel.GridColumn = 0;
            accelerationLabel.GridRow = 3;
            accelerationLabel.Text = "Acc:";
            statBars.Widgets.Add(accelerationLabel);
            Label speedLabel = new Label();
            speedLabel.TextAlign = TextAlign.Center;
            speedLabel.GridColumn = 0;
            speedLabel.GridRow = 4;
            speedLabel.Text = "Speed:";
            statBars.Widgets.Add(speedLabel);
            Label turnLabel = new Label();
            turnLabel.TextAlign = TextAlign.Center;
            turnLabel.GridColumn = 0;
            turnLabel.GridRow = 5;
            turnLabel.Text = "Turn:";
            statBars.Widgets.Add(turnLabel);

            Label damageLabel = new Label();
            damageLabel.TextAlign = TextAlign.Center;
            damageLabel.GridColumn = 0;
            damageLabel.GridRow = 6;
            damageLabel.Text = "Damage:";
            statBars.Widgets.Add(damageLabel);
            Label rangeLabel = new Label();
            rangeLabel.TextAlign = TextAlign.Center;
            rangeLabel.GridColumn = 0;
            rangeLabel.GridRow = 7;
            rangeLabel.Text = "Range:";
            statBars.Widgets.Add(rangeLabel);

            root.Widgets.Add(statBars);

            grid = new Grid()
            {
                ShowGridLines = true,
                ColumnSpacing = 0,
                RowSpacing = 0,
            };
            for (int i = 0; i < 6; i++)
            {
                grid.ColumnsProportions.Add(new Proportion());
            }
            grid.RowsProportions.Add(new Proportion());
            grid.RowsProportions.Add(new Proportion());
            grid.RowsProportions.Add(new Proportion());



            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    ShipID shipID = (ShipID)(x + y * 6);
                    ImageTextButton btn = new ImageTextButton();
                    btn.GridRow = y;
                    btn.GridColumn = x;
                    IImage image = new TextureRegion(ShipStats.GetIcon(shipID));
                    btn.Image = image;
                    btn.Click += (s, a) =>
                    {
                        int team = PanelManager.fillShipSlot / 12;
                        int slot = PanelManager.fillShipSlot % 12;
                        FleetsManager.fleets[team].ships[slot] = shipID;

                        PanelManager.fillShipSlot = -1;
                    };
                    btn.MouseEntered += (s, a) =>
                    {
                        ShipStats.GetTitlesFor(shipID, out string race, out string ship);
                        ShipTitle.Text = race + " " + ship;

                        ShipStats.GetStatsFor(shipID, out int health, out int energyMax, out int energyGen, out int acceleration, out int maxSpeed, out int turnSpeed, true);
                        healthBar.Value = 100f * (float)health / 15f;
                        batteryBar.Value = 100f * (float)energyMax / 15f;
                        reactorBar.Value = 100f * (float)energyGen / 8f;
                        accelerationBar.Value = 100f * (float)acceleration / 15f;
                        maxSpeedBar.Value = 100f * (float)maxSpeed / 15f;
                        turnSpeedBar.Value = 100f * (float)turnSpeed / 15f;

                        ShipStats.GetWeaponStats(shipID, out int damage, out int range, true);
                        damageBar.Value = 100f * (float)damage / 15f;
                        rangeBar.Value = 100f * (float)range / 15f;

                        
                        pointsLabel.Text = ShipStats.GetScore(shipID) + "";
                        ShipAbilities.GetMain(shipID, out string mTitle, out string mDesc);
                        ShipAbilities.GetSecondary(shipID, out string sTitle, out string sDesc);
                        ShipAbilities.GetPerk(shipID, out string pTitle, out string pDesc);
                        
                        mainAbility.Text = "Main\n" + mTitle;
                        mainDesc.Text = mDesc;
                        specialAbility.Text = "Seconday\n" + sTitle;
                        specialDesc.Text = sDesc;
                        passiveAbility.Text = "Passive\n" + pTitle;
                        passiveDesc.Text = pDesc;
                    }; 
                    btn.ContentHorizontalAlignment = HorizontalAlignment.Center;
                    btn.ContentVerticalAlignment = VerticalAlignment.Center;
                    grid.Widgets.Add(btn);
                    buttons[x, y] = btn;
                }
            }
            root.Widgets.Add(grid);

            cancelButton = new TextButton();
            cancelButton.Text = "Cancel";
            cancelButton.Click += (s, a) =>
            {
                PanelManager.fillShipSlot = -1;
            };
            root.Widgets.Add(cancelButton);

            clearButton = new TextButton();
            clearButton.Text = "Clear";
            clearButton.Click += (s, a) =>
            {
                int team = PanelManager.fillShipSlot / 12;
                int slot = PanelManager.fillShipSlot % 12;
                FleetsManager.fleets[team].ships[slot] = ShipID.Count;

                PanelManager.fillShipSlot = -1;
            };
            root.Widgets.Add(clearButton);

            abilities= new Grid()
            {
                ShowGridLines = true,
                ColumnSpacing = 0,
                RowSpacing = 0,
            };

            abilities.ColumnsProportions.Add(new Proportion());
            abilities.ColumnsProportions.Add(new Proportion());
            abilities.ColumnsProportions.Add(new Proportion());
            abilities.RowsProportions.Add(new Proportion());
            abilities.RowsProportions.Add(new Proportion());

            mainAbility = new Label();
            mainAbility.TextAlign = TextAlign.Center;
            mainAbility.GridColumn = 0;
            mainAbility.GridRow = 0;
            mainAbility.Text = "Primary:";
            abilities.Widgets.Add(mainAbility);

            specialAbility = new Label();
            specialAbility.TextAlign = TextAlign.Center;
            specialAbility.GridColumn = 1;
            specialAbility.GridRow = 0;
            specialAbility.Text = "Secondary:";
            abilities.Widgets.Add(specialAbility);

            passiveAbility = new Label();
            passiveAbility.TextAlign = TextAlign.Center;
            passiveAbility.GridColumn = 2;
            passiveAbility.GridRow = 0;
            passiveAbility.Text = "Passive:";
            abilities.Widgets.Add(passiveAbility);

            mainDesc = new Label();
            mainDesc.TextAlign = TextAlign.Left;
            mainDesc.GridColumn = 0;
            mainDesc.GridRow = 1;
            mainDesc.Text = "";
            mainDesc.Wrap = true;
            abilities.Widgets.Add(mainDesc);

            specialDesc = new Label();
            specialDesc.TextAlign = TextAlign.Left;
            specialDesc.GridColumn = 1;
            specialDesc.GridRow = 1;
            specialDesc.Text = "";
            specialDesc.Wrap = true;
            abilities.Widgets.Add(specialDesc);

            passiveDesc = new Label();
            passiveDesc.TextAlign = TextAlign.Left;
            passiveDesc.GridColumn = 2;
            passiveDesc.GridRow = 1;
            passiveDesc.Text = "";
            passiveDesc.Wrap = true;
            abilities.Widgets.Add(passiveDesc);

            root.Widgets.Add(abilities);

        }
        public override void UpdatePanelContent(Vector2 position, Vector2 size, ref Panel root)
        {
            grid.Width = root.Width / 2;
            grid.Height = root.Height / 4;
            /*
            pointsLabel.Width = root.Width/8;
            pointsLabel.Height = grid.Height / 3;
            pointsLabel.Left = (int)root.Width - (int)pointsLabel.Width;
            pointsLabel.Top = (int)pointsLabel.Height / 4;
            */
            pointsLabel.Width = root.Width / 2;
            pointsLabel.Height = grid.Height / 3;
            //pointsLabel.Left = (int)grid.Width;
            pointsLabel.Top = (int)pointsLabel.Height / 4;
            MyraMain.AdjustFont(pointsLabel);

            ShipTitle.Width = root.Width / 2;
            ShipTitle.Height = grid.Height / 3;
            ShipTitle.Left = (int)grid.Width / 2;
            ShipTitle.Top = (int)ShipTitle.Height / 4;
            MyraMain.AdjustFont(ShipTitle);

            statBars.Width = root.Width / 2;
            statBars.Height = root.Height / 4;
            statBars.Left = (int)root.Width / 2;
            statBars.Top = (int)ShipTitle.Height;

            clearButton.Top = (int)grid.Height;
            clearButton.Width = grid.Width / 2;
            clearButton.Height = grid.Height / 3;

            cancelButton.Top = (int)grid.Height;
            cancelButton.Left = (int)clearButton.Width;
            cancelButton.Width = grid.Width / 2;
            cancelButton.Height = grid.Height / 3;
            //grid.Top = (int)root.Height / 4;
            abilities.Top = (int)grid.Height  + (int)clearButton.Height;
            abilities.Width = root.Width;
            abilities.Height = root.Height - abilities.Height;

            foreach(Widget widget in abilities.Widgets)
            {
                widget.Width = abilities.Width/3;
                if(widget.GridRow == 0)
                {
                    widget.Height = grid.Height/3;
                    MyraMain.AdjustFont(widget, false, 0.8f);
                }
                if(widget.GridRow == 1)
                {
                    widget.Height = grid.Height/3;
                    MyraMain.AdjustFont(widget, false, 0.6f);
                    widget.Height = null;
                }
            }
            foreach (Widget widget in grid.Widgets)
            {
                widget.Width = grid.Width / 6;
                widget.Height = grid.Height / 3;

                Point imageSize = ((ImageTextButton)widget).Image.Size;
                float scale = (int)((float)widget.Width / (float)33);
                ((ImageTextButton)widget).ImageWidth = (int)(scale * (float)imageSize.X);
                ((ImageTextButton)widget).ImageHeight = (int)(scale * (float)imageSize.Y);
            }
            //Console.WriteLine(healthBar.Value);
            foreach (Widget widget in statBars.Widgets)
            {
                widget.Height = statBars.Height / 8;
                if(widget is Label)
                {
                    widget.Width = (int)(1f * statBars.Width / 5f);
                    MyraMain.AdjustFont(widget);

                }
                else
                {
                    widget.Width = (int)(4f * statBars.Width / 5f);
                }
            }
        }
        string wrapText(Label label, string inText)
        {
            string[] words = inText.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            string outText = "";
            float length = 0f;
            for(int i = 0; i < words.Length; i++)
            {
                words[i] += " ";
                length += label.Font.MeasureString(words[i]).X;
                if(length > label.Width)
                {
                    outText += "\n";
                    length = label.Font.MeasureString(words[i]).X;
                }
                outText += words[i];
            }
            return outText;
        }
    }
}
