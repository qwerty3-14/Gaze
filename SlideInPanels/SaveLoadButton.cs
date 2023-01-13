using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using GazeOGL.MyraUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.SlideInPanels
{
    public class SaveLoadButton : SlideInPanel
    {
        TextButton saveButton, loadButton;
        int team;
        public SaveLoadButton(Vector2 position, Vector2 size, SlideDirection slideDirection, int team) : base(position, size, slideDirection)
        {
            this.team = team;
        }
       
        public override void SetupPanelContent(Vector2 position, Vector2 size, ref Panel root)
        {
            saveButton = new TextButton();
            saveButton.Text = "Save";
            saveButton.Click += (s, a) =>
            {
                FleetsManager.fleets[team].name = PanelManager.GetFleetName(team);
                for(int i =0; i < FleetsManager.savedFleets.Count; i++)
                {
                    if(FleetsManager.savedFleets[i].name == PanelManager.GetFleetName(team))
                    {
                        FleetsManager.savedFleets.RemoveAt(i);
                    }
                }
                FleetsManager.savedFleets.Add(FleetsManager.fleets[team].Copy());
                SaveData.FleetSaver.Save(FleetsManager.savedFleets);
                Console.WriteLine("Saved");
                PanelManager.fleetSelector.ReloadGrid();
            };
            root.Widgets.Add(saveButton);

            loadButton = new TextButton();
            loadButton.Text = "Load";
            loadButton.Click += (s, a) =>
            {
                PanelManager.fillShipSlot = -2 - team;
                PanelManager.fleetSelector.ReloadGrid();
            };
            root.Widgets.Add(loadButton);
        }
        public override void UpdatePanelContent(Vector2 position, Vector2 size, ref Panel root)
        {
            saveButton.Width = root.Width/2;
            saveButton.Height = root.Height;

            loadButton.Width = root.Width/2;
            loadButton.Height = root.Height;
            loadButton.Left = (int)saveButton.Width;
        }
    }
}
