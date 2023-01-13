using Microsoft.Xna.Framework.Input;
using GazeOGL.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.SaveData
{
    public static class SaveManager
    {
        public static void SaveSettings()
        {
            var fs = new FileStream("Settings", FileMode.Create);
            var writer = new BinaryWriter(fs);

            writer.Write(AssetManager.defaultVolume);
            writer.Write(AssetManager.defaultMusicVolume);

            writer.Write(Main.instance.graphics.PreferredBackBufferWidth);
            writer.Write(Main.instance.graphics.PreferredBackBufferHeight);
            writer.Write(Main.instance.graphics.IsFullScreen);

            writer.Write(Controls.player1ControllerPriority);

            writer.Write(Camera.caneUseSoloCamera);


            writer.Close();
            fs.Close();
        }
        public static void LoadSettings()
        {
            try
            {
                var fs = File.OpenRead("Settings");
                var reader = new BinaryReader(fs);
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                AssetManager.defaultVolume = reader.ReadSingle();
                AssetManager.defaultMusicVolume = reader.ReadSingle();
                AssetManager.UpdateMusicVolume();

                Main.instance.graphics.PreferredBackBufferWidth = reader.ReadInt32();
                Main.instance.graphics.PreferredBackBufferHeight = reader.ReadInt32();
                Main.instance.graphics.IsFullScreen = reader.ReadBoolean();
                Main.instance.graphics.ApplyChanges();

                Controls.player1ControllerPriority = reader.ReadBoolean();

                Camera.caneUseSoloCamera = reader.ReadBoolean();

                fs.Close();
            }
            catch
            {
                Console.WriteLine("No settings data");
                Main.instance.graphics.PreferredBackBufferWidth = 1200;
                Main.instance.graphics.PreferredBackBufferHeight = 900;
                Main.instance.graphics.ApplyChanges();
            }
        }

        public static void SaveKeybindings()
        {
            var fs = new FileStream("KeyBinds", FileMode.Create);
            var writer = new BinaryWriter(fs);

            for(int i =0; i < Controls.configuredControls.GetLength(0); i++)
            {
                for (int j = 0; j < Controls.configuredControls.GetLength(1); j++)
                {
                    writer.Write((byte)Controls.configuredControls[i, j]);
                }
            }


            writer.Close();
            fs.Close();
        }
        public static void LoadKeybindings()
        {
            try            
            {
                var fs = File.OpenRead("KeyBinds");
                var reader = new BinaryReader(fs);
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                for (int i = 0; i < Controls.configuredControls.GetLength(0); i++)
                {
                    for (int j = 0; j < Controls.configuredControls.GetLength(1); j++)
                    {
                        Controls.configuredControls[i, j] = (Keys)reader.ReadByte();
                    }
                }

                fs.Close();
            }
            catch
            {
                Console.WriteLine("No keybind data");
            }
        }
    }
}
