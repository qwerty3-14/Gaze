using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using GazeOGL.MyraUI;

namespace GazeOGL.SlideInPanels
{
    public static class PanelManager
    {
        static List<SlideInPanel> fleetPanels = new List<SlideInPanel>();
        static TitlePanel title;
        static FleetPalette fleetPalette;
        public static int fillShipSlot = -1;
        public static ShipSelectPanel[] shipSelector = new ShipSelectPanel[2];
        public static bool[] shipSelectorActive = new bool[2];
        public static LoadedFleetSelector fleetSelector;

        public static void Load()
        {
            float padding = 0.02f;
            float topBottomHeight = 0.05f;
            float fleetNameWidth = 0.6f * (1f - padding * 2);
            float playerSpacing = 0.5f;
            for(int p =0; p<2; p++)
            {
                fleetPanels.Add(new TitlePanel(new Vector2(padding, p * playerSpacing + padding), new Vector2(0.45f, topBottomHeight), SlideDirection.bottom, "Player " + (p+1)));
                fleetPanels.Add(new FleetEditor(new Vector2(padding, p * playerSpacing + padding * 2 + topBottomHeight), new Vector2(1f - padding * 2, 0.5f - padding * 4 - topBottomHeight * 2), SlideDirection.bottom, p));
                fleetPanels.Add(new TextBoxPanel(new Vector2(padding, p * playerSpacing + 0.5f - padding - topBottomHeight), new Vector2(fleetNameWidth, topBottomHeight), SlideDirection.bottom, "Fleet Name"));
                fleetPanels.Add(new TitlePanel(new Vector2(padding * 2 + fleetNameWidth, p * playerSpacing + 0.5f - padding - topBottomHeight), new Vector2(1f - fleetNameWidth - padding * 3, topBottomHeight), SlideDirection.bottom, "Points: 30"));

                fleetPanels.Add(new SaveLoadButton(new Vector2(padding * 2 + 0.45f, p * playerSpacing + padding), new Vector2(1f - 3 * padding - 0.45f, topBottomHeight), SlideDirection.bottom, p));

                shipSelector[p] = new ShipSelectPanel(new Vector2(padding, p * playerSpacing + padding * 2 + topBottomHeight), new Vector2(1f - padding * 2, 0.5f - padding * 4 - topBottomHeight * 2),  p == 0 ? SlideDirection.top : SlideDirection.bottom, p);

            }
            title = new TitlePanel(new Vector2(0.1f, 0.1f), new Vector2(0.8f, 0.2f), SlideDirection.top, "Project Gaze");
            fleetPalette = new FleetPalette(new Vector2(padding, padding), new Vector2(1f - 2 * padding, 1f - 2 * padding), SlideDirection.top);
            fleetSelector= new LoadedFleetSelector(new Vector2(padding, padding), new Vector2(1f - 2 * padding, 1f - 2 * padding), SlideDirection.top);
            UpdateName();
        }
        public static void UpdateName()
        {
            ((TextBoxPanel)fleetPanels[2]).SetText(FleetsManager.fleets[0].name);
            ((TextBoxPanel)fleetPanels[7]).SetText(FleetsManager.fleets[1].name);
        }
        public static void Update()
        {
            if(Main.mode == Mode.ShipSelect)
            {
                for(int i =0; i <2; i++)
                {
                    if(shipSelectorActive[i])
                    {
                        shipSelector[i].MoveOn();
                    }
                    else
                    {
                        shipSelector[i].MoveOff();
                    }
                }
            }
            else
            {
                shipSelectorActive[0] = false;
                shipSelectorActive[1] = false;
                shipSelector[0].MoveOff();
                shipSelector[1].MoveOff();
            }
            if (Main.mode != Mode.Menu)
            {
                title.MoveOff();
                foreach (SlideInPanel panel in fleetPanels)
                {
                    panel.MoveOff();
                }
                fleetPalette.MoveOff();
                fleetSelector.MoveOff();

            }
            else
            {
                if (MyraMain.GetMenuType() == MenuType.FleetBuilder )
                {
                    title.MoveOff();
                    if (fillShipSlot == -1)
                    {
                        foreach (SlideInPanel panel in fleetPanels)
                        {
                            panel.MoveOn();
                        }
                        fleetPalette.MoveOff();
                        fleetSelector.MoveOff();
                    }
                    else if(fillShipSlot < -1)
                    {
                        foreach (SlideInPanel panel in fleetPanels)
                        {
                            panel.MoveOff();
                        }
                        fleetPalette.MoveOff();
                        fleetSelector.MoveOn();
                    }
                    else
                    {
                        foreach (SlideInPanel panel in fleetPanels)
                        {
                            panel.MoveOff();
                        }
                        fleetPalette.MoveOn();
                        fleetSelector.MoveOff();
                    }
                }
                else
                {
                    title.MoveOn();
                    foreach (SlideInPanel panel in fleetPanels)
                    {
                        panel.MoveOff();
                    }
                    fleetPalette.MoveOff();
                    fleetSelector.MoveOff();
                }
            }
            foreach (SlideInPanel panel in fleetPanels)
            {
                panel.Update();
            }
            title.Update();
            for (int i = 0; i < 2; i++)
            {
                shipSelector[i].Update();
            }
            
            ((TitlePanel)fleetPanels[3]).SetText("Points: " + FleetsManager.fleets[0].GetFleetScore());
            ((TitlePanel)fleetPanels[8]).SetText("Points: " + FleetsManager.fleets[1].GetFleetScore());

            fleetPalette.Update();
            fleetSelector.Update();
        }
        public static void OpenShipSelectors(int team)
        {
            if(team == 2)
            {
                OpenShipSelectors(0);
                OpenShipSelectors(1);
            }
            else
            {
                if (FleetsManager.fleets[team].IsDestroyed())
                {
                    Main.ToMainMenu();
                }
                else
                {
                    shipSelectorActive[team] = true;
                    shipSelector[team].ResetCursor();
                }
            }
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (SlideInPanel panel in fleetPanels)
            {
                panel.Draw(spriteBatch);
            }
            title.Draw(spriteBatch);
            fleetPalette.Draw(spriteBatch); 
            fleetSelector.Draw(spriteBatch);
            for(int i =0; i <2; i++)
            {
                shipSelector[i].Draw(spriteBatch);
            }
        }
        public static string GetFleetName(int team)
        {
            return ((TextBoxPanel)fleetPanels[team == 0 ? 2 : 7]).GetText();
        }
    }
    public enum SlideDirection : byte
    {
        top,
        right,
        left,
        bottom,
    }
}
