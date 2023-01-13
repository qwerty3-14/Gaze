using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.SaveData
{
    static class FleetSaver
    {
        public static void Save(List<Fleet> fleets)
        {
            var fs = new FileStream("Fleets", FileMode.Create);
            var writer = new BinaryWriter(fs);
            writer.Write(fleets.Count);
            for (int i = 0; i < fleets.Count; i++)
            {
                writer.Write(fleets[i].name);
                for(int j = 0; j < 12; j++)
                {
                    writer.Write((byte)fleets[i].ships[j]);
                }
            }
            writer.Close();
            fs.Close();
        }
        public static float[,] loadedScores = new float[(int)ShipID.Count, (int)ShipID.Count];
        public static void Load()
        {
            
            try
            {
                
                List<Fleet> fleets = new List<Fleet>();
                var fs = File.OpenRead("Fleets");
                var reader = new BinaryReader(fs);
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    fleets.Add(new Fleet(reader.ReadString()));
                    for (int j = 0; j < 12; j++)
                    {
                        fleets[i].ships[j] = (ShipID)reader.ReadByte();
                    }
                }


                fs.Close();
                FleetsManager.savedFleets = fleets;
                
            }
            catch
            {
                Console.WriteLine("Generating Fleets");
                List<Fleet> fleets = new List<Fleet>();
                
                Fleet syndicate = new Fleet("Syndicate");
                syndicate.ships[0] = ShipID.Mechanic;
                syndicate.ships[1] = ShipID.Buccaneer;
                syndicate.ships[2] = ShipID.Eagle;
                syndicate.ships[3] = ShipID.Frigate;
                syndicate.ships[4] = ShipID.Escort;
                syndicate.ships[5] = ShipID.Palladin;
                fleets.Add(syndicate);

                Fleet empire = new Fleet("Empire");
                empire.ships[0] = ShipID.Brute;
                empire.ships[1] = ShipID.Trooper;
                empire.ships[2] = ShipID.Assassin;
                empire.ships[3] = ShipID.Surge;
                empire.ships[4] = ShipID.Hunter;
                empire.ships[5] = ShipID.Conqueror;
                fleets.Add(empire);

                Fleet enlightenment = new Fleet("Enlightenment");
                enlightenment.ships[0] = ShipID.Strafer;
                enlightenment.ships[1] = ShipID.Illusioner;
                enlightenment.ships[2] = ShipID.Trebeche;
                enlightenment.ships[3] = ShipID.Lancer;
                enlightenment.ships[4] = ShipID.Apocalypse;
                enlightenment.ships[5] = ShipID.Missionary;
                fleets.Add(enlightenment);

                Fleet BalancedFleetA = new Fleet("Balanced Fleet A");
                BalancedFleetA.ships[0] = ShipID.Strafer;
                BalancedFleetA.ships[1] = ShipID.Buccaneer;
                BalancedFleetA.ships[2] = ShipID.Trooper;
                BalancedFleetA.ships[3] = ShipID.Trebeche;
                BalancedFleetA.ships[4] = ShipID.Assassin;
                BalancedFleetA.ships[5] = ShipID.Lancer;
                BalancedFleetA.ships[6] = ShipID.Escort;
                BalancedFleetA.ships[7] = ShipID.Conqueror;
                BalancedFleetA.ships[8] = ShipID.Palladin;
                fleets.Add(BalancedFleetA);

                Fleet BalancedFleetB = new Fleet("Balanced Fleet B");
                BalancedFleetB.ships[0] = ShipID.Mechanic;
                BalancedFleetB.ships[1] = ShipID.Brute;
                BalancedFleetB.ships[2] = ShipID.Illusioner;
                BalancedFleetB.ships[3] = ShipID.Eagle;
                BalancedFleetB.ships[4] = ShipID.Surge;
                BalancedFleetB.ships[5] = ShipID.Frigate;
                BalancedFleetB.ships[6] = ShipID.Hunter;
                BalancedFleetB.ships[7] = ShipID.Apocalypse;
                BalancedFleetB.ships[8] = ShipID.Missionary;
                fleets.Add(BalancedFleetB);

                Fleet blue = new Fleet("May Contain Blue");
                blue.ships[0] = ShipID.Mechanic;
                blue.ships[1] = ShipID.Strafer;
                blue.ships[2] = ShipID.Trooper;
                blue.ships[3] = ShipID.Illusioner;
                blue.ships[4] = ShipID.Eagle;
                blue.ships[5] = ShipID.Lancer;
                blue.ships[6] = ShipID.Surge;
                fleets.Add(blue);

                Fleet green = new Fleet("Feeling Green");
                green.ships[0] = ShipID.Brute;
                green.ships[1] = ShipID.Trebeche;
                green.ships[2] = ShipID.Hunter;
                green.ships[3] = ShipID.Apocalypse;
                green.ships[4] = ShipID.Palladin;
                fleets.Add(green);

                FleetsManager.savedFleets = fleets;
            }
        }
    }
}
