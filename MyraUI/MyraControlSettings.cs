using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.MyraUI
{
    public static class MyraControlSettings
    {
        public static Grid grid;
        static TextButton controllerPriorityToggle;
        public static void Setup()
        {
            grid = MyraMain.BaseGrid();

            grid.ColumnsProportions.Add(new Proportion());

            // Add widgets
            controllerPriorityToggle = new TextButton();
            controllerPriorityToggle.Click += (s, a) =>
            {
                Controls.player1ControllerPriority = !Controls.player1ControllerPriority;
                SetControlerPriorityText();
            };
            SetControlerPriorityText();
            MyraMain.StandardAlignment(grid, controllerPriorityToggle);

            var player1Label = new Label();
            player1Label.Text = "Player 1 Keybindings";
            MyraMain.StandardAlignment(grid, player1Label);
            Row("Thrust", 0, 0);
            Row("Right", 0, 1);
            Row("Left", 0, 2);
            Row("Down", 0, 3);
            Row("Main", 0, 4);
            Row("Special", 0, 5);
            var player2Label = new Label();
            player2Label.Text = "Player 2 Keybindings";
            MyraMain.StandardAlignment(grid, player2Label);
            Row("Thrust", 1, 0);
            Row("Right", 1, 1);
            Row("Left", 1, 2);
            Row("Down", 1, 3);
            Row("Main", 1, 4);
            Row("Special", 1, 5);


            var returnToMain = new TextButton();
            returnToMain.Text = "Back";
            returnToMain.Click += (s, a) =>
            {
                SaveData.SaveManager.SaveKeybindings();
                MyraMain.SwitchMenu(MenuType.Settings);
            };
            MyraMain.StandardAlignment(grid, returnToMain);


        }
        static void SetControlerPriorityText()
        {
            controllerPriorityToggle.Text = "Controller Prioity: Player " + (Controls.player1ControllerPriority ? "1" : "2");
        }
        static Grid Row(string labelText, int player, int control)
        {
            Grid row = new Grid();
            row.ColumnsProportions.Add(new Proportion());
            row.RowsProportions.Add(new Proportion());

            var label = new Label();
            label.Text = labelText;
            label.GridColumn = 0;
            row.Widgets.Add(label);

            var firstButton = new TextButton();
            firstButton.GridColumn = 1;
            firstButton.Text = " " + Controls.configuredControls[control, (player * 2) + 0];
            firstButton.Click += (s, a) =>
            {
                StartInputReader(firstButton, control, (player * 2) + 0);
            };
            row.Widgets.Add(firstButton);

            var secondButton = new TextButton();
            secondButton.GridColumn = 2;
            secondButton.Text = " " + Controls.configuredControls[ control, (player * 2) + 1];
            secondButton.Click += (s, a) =>
            {
                StartInputReader(secondButton, control, (player * 2) + 1);
            };
            row.Widgets.Add(secondButton);

            MyraMain.StandardAlignment(grid, row);
            return row;
        }
        static bool inputReaderActive = false;
        static TextButton controlButtonBeingModifed;
        static int indexI;
        static int indexJ;
        static void StartInputReader(TextButton button, int i, int j)
        {
            if(inputReaderActive)
            {
                Controls.configuredControls[indexI, indexJ] = Keys.None;
                controlButtonBeingModifed.Text = "" + Controls.configuredControls[indexI, indexJ];
            }
            inputReaderActive = true;
            controlButtonBeingModifed = button;
            indexI = i;
            indexJ = j;
        }
        public static void Update()
        {
            if(inputReaderActive)
            {
                Controls.configuredControls[indexI, indexJ] = Keys.None;
                if (Controls.GetNextKeyPress(out Keys key))
                {
                    inputReaderActive = false;
                    if (key == Keys.Escape)
                    {
                    }
                    else
                    {
                        Controls.configuredControls[indexI, indexJ] = key;
                    }
                    controlButtonBeingModifed.Text = "" + Controls.configuredControls[indexI, indexJ];
                }
                else
                {
                    controlButtonBeingModifed.Text = "???";
                }
            }
        }
    }
}
