using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL
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
                    health = 10;
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
                    turnSpeed = 5;
                    break;
                case ShipID.Strafer:
                    health = 4;
                    energyMax = 4;
                    energyGen = 4;
                    acceleration = 15;
                    maxSpeed = 15;
                    turnSpeed = 3;
                    break;
                case ShipID.Escort:
                    health = alt ? 11 : 5;
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
                    maxSpeed = 9;
                    turnSpeed = 14;
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
                    health = 7;
                    energyMax = 10;
                    energyGen = 2;
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
                    acceleration = 7;
                    maxSpeed = 8;
                    turnSpeed = 7;
                    break;
                case ShipID.Lancer:
                    health = 8;
                    energyMax = 12;
                    energyGen = 4;
                    acceleration = 10;
                    maxSpeed = 9;
                    turnSpeed = 10;
                    break;
                case ShipID.Mechanic:
                    health = 6;
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
                    acceleration = 9;
                    maxSpeed = 8;
                    turnSpeed = 11;
                    break;
                case ShipID.Surge:
                    health = 9;
                    energyMax = 5;
                    energyGen = 4;
                    acceleration = 12;
                    maxSpeed = 7;
                    turnSpeed = 3;
                    break;
            }
        }
        public static void GetWeaponStats(ShipID type, out int damage, out int range, bool alt = false)
        {
            damage = 0;
            range = 0;
            switch (type)
            {
                case ShipID.Hunter:
                    damage = 10;
                    range = 6;
                    break;
                case ShipID.Conqueror:
                    damage = 15;
                    range = 15;
                    break;
                case ShipID.Strafer:
                    damage = 9;
                    range = 2;
                    break;
                case ShipID.Escort:
                    damage = alt ? 12 : 6;
                    range = alt ? 6 : 4;
                    break;
                case ShipID.Illusioner:
                    damage = 4;
                    range = 10;
                    break;
                case ShipID.Assassin:
                    damage = 15;
                    range = 1;
                    break;
                case ShipID.Palladin:
                    damage = 15;
                    range = 4;
                    break;
                case ShipID.Trooper:
                    damage = 8;
                    range = 15;
                    break;
                case ShipID.Eagle:
                    damage = 5;
                    range = 6;
                    break;
                case ShipID.Apocalypse:
                    damage = 11;
                    range = 11;
                    break;
                case ShipID.Trebeche:
                    damage = 10;
                    range = 15;
                    break;
                case ShipID.Buccaneer:
                    damage = 12;
                    range = 1;
                    break;
                case ShipID.Missionary:
                    damage = 15;
                    range = 8;
                    break;
                case ShipID.Lancer:
                    damage = 3;
                    range = 6;
                    break;
                case ShipID.Mechanic:
                    damage = 6;
                    range = 6;
                    break;
                case ShipID.Frigate:
                    damage = 12;
                    range = 15;
                    break;
                case ShipID.Brute:
                    damage = 15;
                    range = 5;
                    break;
                case ShipID.Surge:
                    damage = 6;
                    range = 4;
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
                    return AssetManager.extraEntities[14];
                case ShipID.Buccaneer:
                    return AssetManager.ui[14];
                case ShipID.Missionary:
                    return AssetManager.ui[17];
                case ShipID.Lancer:
                    return AssetManager.ships[13];
                case ShipID.Mechanic:
                    return AssetManager.ships[14];
                case ShipID.Frigate:
                    return AssetManager.ships[15];
                case ShipID.Brute:
                    return AssetManager.ships[16];
                case ShipID.Surge:
                    return AssetManager.ui[16];
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
                case ShipID.Surge:
                    race = "Vuu-Vii-Vaa";
                    ship = "Surge";
                    break;

            }
        }
        public static int GetScore(ShipID type)
        {
            switch (type)
            {
                case ShipID.Hunter:
                    return 21;
                case ShipID.Conqueror:
                    return 25+2;
                case ShipID.Strafer:
                    return 9;
                case ShipID.Escort:
                    return 23;
                case ShipID.Illusioner:
                    return 11;
                case ShipID.Assassin:
                    return 21-6;
                case ShipID.Palladin:
                    return 30;
                case ShipID.Trooper:
                    return 11;
                case ShipID.Eagle:
                    return 12;
                case ShipID.Apocalypse:
                    return 24;
                case ShipID.Trebeche:
                    return 11;
                case ShipID.Buccaneer:
                    return 10;
                case ShipID.Missionary:
                    return 30;
                case ShipID.Lancer:
                    return 17;
                case ShipID.Mechanic:
                    return 5+2;
                case ShipID.Frigate:
                    return 20;
                case ShipID.Brute:
                    return 10;
                case ShipID.Surge:
                    return 19-1;
            }
            return 0;
        }
    }
    public enum ShipID : byte
    {
        Mechanic,
        Buccaneer,
        Eagle,
        Frigate,
        Escort,
        Palladin,
        Brute,
        Trooper,
        Assassin,
        Surge,
        Hunter,
        Conqueror,
        Strafer,
        Illusioner,
        Trebeche,
        Lancer,
        Apocalypse,
        Missionary,
        Count

    }
}
