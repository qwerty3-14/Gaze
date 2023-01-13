using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.SaveData
{
    static class MatchupSaver
    {
        public static void Save(float[,] matchUpScores)
        {
            var fs = new FileStream("Matchups", FileMode.Create);
            var writer = new BinaryWriter(fs);

            for (int i = 0; i < matchUpScores.GetLength(0); i++)
            {

                for (int j = 0; j < matchUpScores.GetLength(1); j++)
                {
                    writer.Write(matchUpScores[i, j]);
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
                var fs = File.OpenRead("Matchups");
                var reader = new BinaryReader(fs);
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                for (int i = 0; i < (int)ShipID.Count; i++)
                {

                    for (int j = 0; j < (int)ShipID.Count; j++)
                    {
                        loadedScores[i, j] =  reader.ReadSingle();
                    }
                }


                fs.Close();
            }
            catch
            {
                Console.WriteLine("No matchup data, using defaults");
                loadedScores = new float[,]
                {
                    {0f, -0.8f, -0.7f, -0.3f, -0.6f, -0.90000004f, -0.3f, -0.7f, -0.6f, 0.5f, -0.90000004f, -0.8f, -0.4f, 1f, -0.5f, -0.6f, -1f, 0.5f},
                    {0.8f, 0f, -0.5f, -0.7f, -0.2f, -0.6f, -0.4f, -0.3f, -0.1f, -0.5f, -0.8f, 0.3f, 0.5f, -0.7f, 0.8f, -0.3f, -0.90000004f, -0.4f},
                    {0.7f, 0.5f, 0f, -0.6f, -0.90000004f, -0.7f, 0.8f, 0.6f, -0.3f, -0.7f, 0.5f, -0.7f, 0.6f, -0.5f, -0.3f, -0.7f, -0.3f, -0.90000004f},
                    {0.3f, 0.7f, 0.6f, 0f, -0.4f, -0.2f, 0.4f, -0.1f, 0.2f, -0.5f, 0.2f, -0.90000004f, 0.7f, 0.2f, -0.1f, 0.6f, 0.4f, -0.3f},
                    {0.6f, 0.2f, 0.90000004f, 0.4f, 0f, -0.8f, 1f, 0.5f, 0.4f, 0.8f, -0.5f, -1f, 0.6f, 0.8f, -0.90000004f, 0.8f, 0.1f, -0.4f},
                    {0.90000004f, 0.6f, 0.7f, 0.2f, 0.8f, 0f, -0.2f, 0.8f, 0.6f, 0.8f, 0.2f, 0.5f, 0.5f, 0.1f, -0.1f, 0.7f, 0.5f, -0.1f},
                    {0.3f, 0.4f, -0.8f, -0.4f, -1f, 0.2f, 0f, 0.6f, -0.4f, -0.90000004f, 0.1f, -1f, 0.2f, -0.6f, 0.90000004f, -0.8f, -0.3f, -0.6f},
                    {0.7f, 0.3f, -0.6f, 0.1f, -0.5f, -0.8f, -0.6f, 0f, 0.1f, -0.3f, -0.2f, -0.7f, -0.4f, 0.7f, 0.90000004f, -0.5f, -0.8f, -0.8f},
                    {0.6f, 0.1f, 0.3f, -0.2f, -0.4f, -0.6f, 0.4f, -0.1f, 0f, -0.4f, -0.2f, 0.7f, 0.90000004f, 0.90000004f, 0.90000004f, 0.3f, -0.5f, -0.6f},
                    {-0.5f, 0.5f, 0.7f, 0.5f, -0.8f, -0.8f, 0.90000004f, 0.3f, 0.4f, 0f, -0.2f, -0.5f, -0.2f, 0.90000004f, -0.6f, 0.1f, 0.5f, -0.3f},
                    {0.90000004f, 0.8f, -0.5f, -0.2f, 0.5f, -0.2f, -0.1f, 0.2f, 0.2f, 0.2f, 0f, -0.3f, 0.8f, -0.90000004f, 1f, 0.7f, -0.5f, -0.6f},
                    {0.8f, -0.3f, 0.7f, 0.90000004f, 1f, -0.5f, 1f, 0.7f, -0.7f, 0.5f, 0.3f, 0f, 0f, -0.4f, 0.5f, 0.1f, 0.3f, -0.5f},
                    {0.4f, -0.5f, -0.6f, -0.7f, -0.6f, -0.5f, -0.2f, 0.4f, -0.90000004f, 0.2f, -0.8f, 0f, 0f, -0.5f, 0.90000004f, 0.4f, -0.90000004f, -0.90000004f},
                    {-1f, 0.7f, 0.5f, -0.2f, -0.8f, -0.1f, 0.6f, -0.7f, -0.90000004f, -0.90000004f, 0.90000004f, 0.4f, 0.5f, 0f, -0.6f, -1f, -0.7f, -0.5f},
                    {0.5f, -0.8f, 0.3f, 0.1f, 0.90000004f, 0.1f, -0.90000004f, -0.90000004f, -0.90000004f, 0.6f, -1f, -0.5f, -0.90000004f, 0.6f, 0f, -1f, 0.3f, -0.1f},
                    {0.6f, 0.3f, 0.7f, -0.6f, -0.8f, -0.7f, 0.8f, 0.5f, -0.3f, -0.1f, -0.7f, -0.1f, -0.4f, 1f, 1f, 0f, -0.7f, -0.4f},
                    {1f, 0.90000004f, 0.3f, -0.4f, -0.1f, -0.5f, 0.3f, 0.8f, 0.5f, -0.5f, 0.5f, -0.3f, 0.90000004f, 0.7f, -0.3f, 0.7f, 0f, -0.4f},
                    {-0.5f, 0.4f, 0.90000004f, 0.3f, 0.4f, 0.1f, 0.6f, 0.8f, 0.6f, 0.3f, 0.6f, 0.5f, 0.90000004f, 0.5f, 0.1f, 0.4f, 0.4f, 0f},
                };
            }
        }
    }
}
