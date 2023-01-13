using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL
{
    public static class FleetsManager
    {
        public static Fleet[] fleets;
        public static List<Fleet> savedFleets;
        public static void Load()
        {
            fleets = new Fleet[2];
            fleets[0] = new Fleet("Syndicate");
            fleets[1] = new Fleet("Empire");

            fleets[0].ships[0] = ShipID.Mechanic;
            fleets[0].ships[1] = ShipID.Buccaneer;
            fleets[0].ships[2] = ShipID.Eagle;
            fleets[0].ships[3] = ShipID.Frigate;
            fleets[0].ships[4] = ShipID.Escort;
            fleets[0].ships[5] = ShipID.Palladin;

            fleets[1].ships[0] = ShipID.Brute;
            fleets[1].ships[1] = ShipID.Trooper;
            fleets[1].ships[2] = ShipID.Assassin;
            fleets[1].ships[3] = ShipID.Surge;
            fleets[1].ships[4] = ShipID.Hunter;
            fleets[1].ships[5] = ShipID.Conqueror;

            SaveData.FleetSaver.Load();
        }
        public static void Repair()
        {
            fleets[0].destroyed = new bool[12];
            fleets[1].destroyed = new bool[12];
        }
    }
}
