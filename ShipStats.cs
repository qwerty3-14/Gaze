using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze
{
    public static class ShipStats
    {
        public static void GetStatsFor(ShipID type, out int health, out int energyMax, out int energyGen, out int acceleration, out int maxSpeed, out int turnSpeed, bool alt = false)
        {
            health = 9;
            energyMax = 15;
            energyGen = 2;
            acceleration = 6;
            maxSpeed = 6;
            turnSpeed = 8;
            switch (type)
            {
                case ShipID.Hunter:
                    health = 11;
                    energyMax = 15;
                    energyGen = 4;
                    acceleration = 9;
                    maxSpeed = 7;
                    turnSpeed = 9;
                    break;
                case ShipID.Conqueror:
                    health = 15;
                    energyMax = 15;
                    energyGen = 2;
                    acceleration = 4;
                    maxSpeed = 6;
                    turnSpeed = 4;
                    break;
                case ShipID.Strafer:
                    health = 6;
                    energyMax = 5;
                    energyGen = 4;
                    acceleration = 15;
                    maxSpeed = 15;
                    turnSpeed = 3;
                    break;
                case ShipID.Escort:
                    health = 5;
                    energyMax = 12;
                    energyGen = 2;
                    acceleration = alt ?  3 : 10;
                    maxSpeed = 8;
                    turnSpeed = alt ? 3 : 8;
                    break;
                case ShipID.Illusioner:
                    health = 3;
                    energyMax = 5;
                    energyGen = 2;
                    acceleration = 13;
                    maxSpeed = 11;
                    turnSpeed = 13;
                    break;
                case ShipID.Assassin:
                    health = 4;
                    energyMax = 10;
                    energyGen = 4;
                    acceleration = 15;
                    maxSpeed = 13;
                    turnSpeed = 15;
                    break;
                case ShipID.Palladin:
                    health = 15;
                    energyMax = 15;
                    energyGen = 3;
                    acceleration = 12;
                    maxSpeed = 10;
                    turnSpeed = 12;
                    break;
                case ShipID.Trooper:
                    health = 8;
                    energyMax = 8;
                    energyGen = 3;
                    acceleration = 15;
                    maxSpeed = 4;
                    turnSpeed = 9;
                    break;
                case ShipID.Eagle:
                    health = 9;
                    energyMax = 6;
                    energyGen = 3;
                    acceleration = 15;
                    maxSpeed = 8;
                    turnSpeed = 11;
                    break;
                case ShipID.Apocalypse:
                    health = 13;
                    energyMax = 10;
                    energyGen = 8;
                    acceleration = 4;
                    maxSpeed = 4;
                    turnSpeed = 4;
                    break;
                case ShipID.Trebeche:
                    health = 9;
                    energyMax = 12;
                    energyGen = 3;
                    acceleration = 8;
                    maxSpeed = 7;
                    turnSpeed = 4;
                    break;
                case ShipID.Buccaneer:
                    health = 7;
                    energyMax = 3;
                    energyGen = 1;
                    acceleration = 13;
                    maxSpeed = 10;
                    turnSpeed = 11;
                    break;
                case ShipID.Missionary:
                    health = 15;
                    energyMax = 15;
                    energyGen = 6;
                    acceleration = 9;
                    maxSpeed = 8;
                    turnSpeed = 12;
                    break;
                case ShipID.Lancer:
                    health = 9;
                    energyMax = 12;
                    energyGen = 4;
                    acceleration = 10;
                    maxSpeed = 9;
                    turnSpeed = 10;
                    break;
                case ShipID.Mechanic:
                    health = 5;
                    energyMax = 4;
                    energyGen = 2;
                    acceleration = 14;
                    maxSpeed = 11;
                    turnSpeed = 12;
                    break;
                case ShipID.Frigate:
                    health = 10;
                    energyMax = 12;
                    energyGen = 4;
                    acceleration = 7;
                    maxSpeed = 5;
                    turnSpeed = 6;
                    break;
                case ShipID.Brute:
                    health = 5;
                    energyMax = 8;
                    energyGen = 3;
                    acceleration = 5;
                    maxSpeed = 8;
                    turnSpeed = 10;
                    break;
            }
        }
        public static Texture2D GetIcon(ShipID type)
        {
            switch (type)
            {
                case ShipID.Hunter:
                    return AssetManager.ships[0];
                case ShipID.Conqueror:
                    return AssetManager.ships[1];
                case ShipID.Strafer:
                    return AssetManager.ui[9];
                case ShipID.Escort:
                    return AssetManager.ui[10];
                case ShipID.Illusioner:
                    return AssetManager.ships[4];
                case ShipID.Assassin:
                    return AssetManager.ships[5];
                case ShipID.Palladin:
                    return AssetManager.ships[6];
                case ShipID.Trooper:
                    return AssetManager.ships[7];
                case ShipID.Eagle:
                    return AssetManager.ships[8];
                case ShipID.Apocalypse:
                    return AssetManager.ui[13];
                case ShipID.Trebeche:
                    return AssetManager.ships[10];
                case ShipID.Buccaneer:
                    return AssetManager.ui[14];
                case ShipID.Missionary:
                    return AssetManager.ships[12];
                case ShipID.Lancer:
                    return AssetManager.ships[13];
                case ShipID.Mechanic:
                    return AssetManager.ships[14];
                case ShipID.Frigate:
                    return AssetManager.ships[15];
                case ShipID.Brute:
                    return AssetManager.ships[16];
            }
            return null;
        }
        public static void GetTitlesFor(ShipID type, out string race, out string ship)
        {
            race = "";
            ship = "";
            switch (type)
            {
                case ShipID.Hunter:
                    race = "Galvin";
                    ship = "Hunter";
                    break;
                case ShipID.Conqueror:
                    race = "Purt";
                    ship = "Conqueror";
                    break;
                case ShipID.Strafer:
                    race = "Laee";
                    ship = "Strafer";
                    break;
                case ShipID.Escort:
                    race = "Guave";
                    ship = "Escort";
                    break;
                case ShipID.Illusioner:
                    race = "Xant";
                    ship = "Illusionist";
                    break;
                case ShipID.Assassin:
                    race = "Hashen";
                    ship = "Assassin";
                    break;
                case ShipID.Palladin:
                    race = "Zupin";
                    ship = "Palladin";
                    break;
                case ShipID.Trooper:
                    race = "Tyfe";
                    ship = "Trooper";
                    break;
                case ShipID.Eagle:
                    race = "U.S.G.";
                    ship = "Eagle";
                    break;
                case ShipID.Apocalypse:
                    race = "Werx";
                    ship = "Apocalypse";
                    break;
                case ShipID.Trebeche:
                    race = "T.D.";
                    ship = "Trebuchet";
                    break;
                case ShipID.Buccaneer:
                    race = "Blark";
                    ship = "Buccaneer";
                    break;
                case ShipID.Missionary:
                    race = "Gorm-Ta";
                    ship = "Missionary";
                    break;
                case ShipID.Lancer:
                    race = "Ooboo";
                    ship = "Lancer";
                    break;
                case ShipID.Mechanic:
                    race = "Jifle";
                    ship = "Mechanic";
                    break;
                case ShipID.Frigate:
                    race = "Embi";
                    ship = "Frigate";
                    break;
                case ShipID.Brute:
                    race = "Lasque";
                    ship = "Brute";
                    break;

            }
        }
    }
    public enum ShipID : byte
    {
        Mechanic,
        Strafer,
        Trooper,
        Buccaneer,
        Illusioner,
        Trebeche,
        Lancer,
        Eagle,
        Frigate,
        Assassin,
        Hunter,
        Escort,
        Apocalypse,
        Palladin,
        Conqueror,
        Missionary,
        Brute,

    }
}
