using Microsoft.Xna.Framework.Graphics;
using GazeOGL.Entities.Ships;
using GazeOGL.Entities;
using Microsoft.Xna.Framework;
using System;

namespace GazeOGL.Debris
{
    public class DebrisSet
    {
        public Texture2D[] textures;
        public float[] directions;
        public Polygon[] shapes;
        public DebrisSet(Texture2D[] textures, float[] directions, Polygon[] shapes)
        {
            this.textures = textures;
            this.directions = directions;
            this.shapes = shapes;
        }
        static DebrisSet[] shipDebris = new DebrisSet[18];
        static DebrisSet EscortPlatform;
        public static void Load()
        {
            EscortPlatform = DebrisDrawer.Generate(AssetManager.extraEntities[13], 4);
            for(byte i = 0; i < (byte)ShipID.Count; i++)
            {
                int pieces = 2;
                switch((ShipID)i)
                {
                    case ShipID.Mechanic:
                    case ShipID.Illusioner:
                    case ShipID.Assassin:
                    case ShipID.Brute:
                    case ShipID.Strafer:
                    case ShipID.Escort:
                    pieces = 2;
                    break;
                    case ShipID.Buccaneer:
                    case ShipID.Trooper:
                    case ShipID.Eagle:
                    case ShipID.Hunter:
                    case ShipID.Lancer:
                    pieces = 3;
                    break;
                    case ShipID.Frigate:
                    case ShipID.Surge:
                    case ShipID.Trebeche:
                    case ShipID.Apocalypse:
                    pieces = 4;
                    break;
                    case ShipID.Missionary:
                    case ShipID.Palladin:
                    case ShipID.Conqueror:
                    pieces = 5;
                    break;
                }
                Texture2D texture =ShipStats.GetIcon((ShipID)i);
                if((ShipID)i == ShipID.Escort)
                {
                    texture = AssetManager.ships[3];
                }
                shipDebris[i] = DebrisDrawer.Generate(texture, pieces);
            }
        }
        public static void CreatePlatformDebris(Entity platform)
        {
            Vector2 origin = new Vector2(5.5f, 5.5f);
            DebrisSet debrisSet = EscortPlatform;
            for(int i =0; i < debrisSet.textures.Length; i++)
            {
                new DebrisPiece(debrisSet.textures[i], debrisSet.shapes[i], platform.position, platform.velocity + Functions.PolarVector(1.5f * (float)Main.random.NextDouble(), debrisSet.directions[i]), platform.rotation, (float)Main.random.NextDouble() * ((float)Math.PI / 240f) - ((float)Math.PI / 480f));
            }
        }
        public static void CreateDebris(Ship ship)
        {
            ShipID shipID = ship.type;
            Vector2 origin;
            switch(shipID)
            {
                case ShipID.Hunter:
                    origin = new Vector2(7.5f, 5.5f);
                    break;
                case ShipID.Conqueror:
                    origin = new Vector2(17.5f, 13.5f);
                    break;
                case ShipID.Strafer:
                    origin = new Vector2(7.5f, 5.5f);
                    break;
                case ShipID.Escort:
                    origin = new Vector2(5.5f, 5.5f);
                    break;
                case ShipID.Illusioner:
                    origin = new Vector2(5.5f, 4.5f);
                    break;
                case ShipID.Assassin:
                    origin = new Vector2(6.5f, 8.5f);
                    break;
                case ShipID.Palladin:
                    origin = new Vector2(16.5f, 11f);
                    break;
                case ShipID.Trooper:
                    origin = new Vector2(3.5f, 9.5f);
                    break;
                case ShipID.Eagle:
                    origin = new Vector2(7.5f, 11.5f);
                    break;
                case ShipID.Apocalypse:
                    origin = new Vector2(7.5f, 13.5f);
                    break;
                case ShipID.Trebeche:
                    origin = new Vector2(9f, 8.5f);
                    break;
                case ShipID.Buccaneer:
                    origin = new Vector2(5.5f, 8f);
                    break;
                case ShipID.Missionary:
                    origin = new Vector2(9f, 20.5f);
                    break;
                case ShipID.Lancer:
                    origin = new Vector2(14f, 6f);
                    break;
                case ShipID.Mechanic:
                    origin = new Vector2(6.5f, 5.5f);
                    break;
                case ShipID.Frigate:
                    origin = new Vector2(15.5f, 6.5f);
                    break;
                case ShipID.Brute:
                    origin = new Vector2(5.5f, 7.5f);
                    break;
                case ShipID.Surge:
                    origin = new Vector2(7.5f, 10.5f);
                    break;
            }
            DebrisSet debrisSet = shipDebris[(int)shipID];
            for(int i =0; i < debrisSet.textures.Length; i++)
            {
                new DebrisPiece(debrisSet.textures[i], debrisSet.shapes[i], ship.position, ship.velocity + Functions.PolarVector(1.5f * (float)Main.random.NextDouble(), debrisSet.directions[i]), ship.rotation, (float)Main.random.NextDouble() * ((float)Math.PI / 240f) - ((float)Math.PI / 480f));
            }
        }
    }
}