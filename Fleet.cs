using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL
{
    public class Fleet
    {
        public ShipID[] ships = new ShipID[12];
        public bool[] destroyed = new bool[12];
        public string name;
        public Fleet(string name)
        {
            for(int i = 0; i < 12; i++)
            {
                ships[i] = ShipID.Count;
            }
            this.name = name;
        }
        public bool IsDestroyed()
        {
            for (int i = 0; i < 12; i++)
            {
                if(ships[i] != ShipID.Count && !destroyed[i])
                {
                    return false;
                }
            }
            return true;
        }
        public int GetFleetScore()
        {
            int score = 0;
            for (int i = 0;i < 12;i++)
            {
                if(!destroyed[i])
                {
                    score += ShipStats.GetScore(ships[i]);
                }
            }
            return score;
        }
        public int CheapestShipIndex()
        {
            int cheapest = 31;
            int index = -1;
            for (int i = 0; i < 12; i++)
            {
                if(ships[i] != ShipID.Count && !destroyed[i])
                {
                    if(ShipStats.GetScore(ships[i]) < cheapest)
                    {
                        cheapest = ShipStats.GetScore(ships[i]);
                        index = i;
                    }
                }
            }
            return index;
        }
        public int BestMatchupVs(ShipID enemyType)
        {
            float best = -1.1f;
            int index = -1;
            for (int i = 0; i < 12; i++)
            {
                if (ships[i] != ShipID.Count && !destroyed[i])
                {
                    //Console.WriteLine(SaveData.MatchupSaver.loadedScores[(int)ships[i], (int)enemyType]);
                    if (SaveData.MatchupSaver.loadedScores[(int)ships[i], (int)enemyType] > best)
                    {
                        best = SaveData.MatchupSaver.loadedScores[(int)ships[i], (int)enemyType];
                        index = i;
                    }
                }
            }
            return index;
        }
        public Fleet Copy()
        {
            Fleet newFleet = new Fleet(name);
            for (int i = 0; i < 12; i++)
            {
                newFleet.ships[i] = ships[i];
            }
            return newFleet;
        }
    }
}
