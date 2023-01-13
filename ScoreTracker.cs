using Microsoft.Xna.Framework.Input;
using GazeOGL.Entities.Ships;
using GazeOGL.SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GazeOGL
{
    public static class ScoreTracker
    {
        static int firstShip = 0;
        static int secondShip = -1;
        static int firstShipStock = 10;
        static int secondShipStock = 10;
        static float[,] matchUpScores = new float[(int)ShipID.Count, (int)ShipID.Count];

        static bool fastTest = true;

        public static void TestSetup()
        {
            firstShipStock = 10;
            secondShipStock = 10;
            secondShip++;
            if (secondShip > (int)ShipID.Count - 1)
            {
                secondShip = 0;
                firstShip++;
                if (firstShip > (int)ShipID.Count - 1)
                {

                    //print matchup scores
                    ShowResults2();
                    Arena.simSpeed = 1;
                    Main.mode = Mode.QuickPlay;
                }
            }
            Arena.ships[0] = Ship.Spawn((ShipID)firstShip, 0);
            Arena.ships[1] = Ship.Spawn((ShipID)secondShip, 1);
            Arena.ships[1].rotation = (float)Math.PI;
        }
        public static void Update()
        {
            if (secondShip <= firstShip)
            {
                Arena.Start();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space) || fastTest)
            {
                Arena.simSpeed = 40;
            }
        }
        public static void ProcessBattleResults(int r)
        {
            if (r == 0 || r == 2)
            {
                firstShipStock--;
                Arena.ships[0] = Ship.Spawn((ShipID)firstShip, 0);
            }
            if (r == 1 || r == 2)
            {
                secondShipStock--;
                Arena.ships[1] = Ship.Spawn((ShipID)secondShip, 1);
            }
            if (firstShipStock <= 0 || secondShipStock <= 0)
            {

                Console.WriteLine(firstShipStock * 0.1f + ", " + secondShipStock * 0.1f);
                matchUpScores[firstShip, secondShip] = firstShipStock * 0.1f + secondShipStock * -.1f;
                matchUpScores[secondShip, firstShip] = secondShipStock * 0.1f + firstShipStock * -.1f;
                Arena.simSpeed = 1;
                Arena.Start();
            }
        }
        static void ShowResults2()
        {
            Console.WriteLine("Matchup Scores");
            float[] totalScore = new float[(int)ShipID.Count];
            float biggest = -200;
            float smallest = 200;
            for (int i = 0; i < matchUpScores.GetLength(0); i++)
            {

                ShipStats.GetTitlesFor((ShipID)i, out string r, out _);
                string o = r + ": ";
                for (int j = 0; j < matchUpScores.GetLength(1); j++)
                {
                    totalScore[i] += matchUpScores[i, j];
                    o += matchUpScores[i, j] + "f, ";
                }
                if (totalScore[i] > biggest)
                {
                    biggest = totalScore[i];
                }
                if (totalScore[i] < smallest)
                {
                    smallest = totalScore[i];
                }
                Console.WriteLine(o);
            }
            MatchupSaver.Save(matchUpScores);
            float range = biggest - smallest;
            float scaler = 25f / range;
            for (int i = 0; i < totalScore.Length; i++)
            {
                totalScore[i] *= scaler;
            }
            smallest *= scaler;
            float diff = 5f - smallest;
            for (int i = 0; i < totalScore.Length; i++)
            {
                totalScore[i] += diff;
            }
            for (int i = 0; i < totalScore.Length; i++)
            {
                totalScore[i] = (int)Math.Round(totalScore[i]);
            }

            Console.WriteLine("Estimated Scores");
            for (int i = 0; i < totalScore.Length; i++)
            {
                ShipStats.GetTitlesFor((ShipID)i, out string r, out _);
                Console.WriteLine(r + ": " + totalScore[i]);
            }

        }
        static void ShowResults()
        {
            Console.WriteLine("Matchup Scores");
            float[] totalScore = new float[(int)ShipID.Count];
            for (int i = 0; i < matchUpScores.GetLength(0); i++)
            {

                ShipStats.GetTitlesFor((ShipID)i, out string r, out _);
                string o = r + ": ";
                for (int j = 0; j < matchUpScores.GetLength(1); j++)
                {
                    totalScore[i] += matchUpScores[i, j];
                    o += matchUpScores[i, j] + "f, ";
                }
                Console.WriteLine(o);
            }
            float biggestSecondScore = 0;
            float[] secondScore = new float[(int)ShipID.Count];
            for (int i = 0; i < matchUpScores.GetLength(0); i++)
            {
                for (int j = 0; j < matchUpScores.GetLength(1); j++)
                {
                    secondScore[i] += matchUpScores[i, j] * totalScore[j];
                }
                if (secondScore[i] > biggestSecondScore)
                {
                    biggestSecondScore = secondScore[i];
                }
            }
            int maxScore = 30;
            float scaler = maxScore / biggestSecondScore;
            int[] estimatedScore = new int[(int)ShipID.Count];
            for (int i = 0; i < matchUpScores.GetLength(0); i++)
            {
                estimatedScore[i] = (int)Math.Round(secondScore[i] * scaler);

            }
            Console.WriteLine("Estimated Scores");
            for (int i = 0; i < totalScore.Length; i++)
            {
                ShipStats.GetTitlesFor((ShipID)i, out string r, out _);
                Console.WriteLine(r + ": " + estimatedScore[i]);
            }
        }
    }
}
