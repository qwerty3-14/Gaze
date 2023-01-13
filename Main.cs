using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GazeOGL.Entities;
using GazeOGL.Entities.Projectiles;
using GazeOGL.Entities.Ships;
using GazeOGL.MyraUI;
using GazeOGL.SlideInPanels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GazeOGL.Debris;
using MonoSound;

namespace GazeOGL
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Random random = new Random(42);
        public static Main instance;
        public static Texture2D pixel;
        public static GraphicsDevice device;
        public static SpriteFont font;
        public static bool[] isAI = new bool[2];
        public static bool[] startAI = new bool[2];
        public static Vector2 defaultScreenSize = new Vector2(1200, 900);
        public static GraphicsDevice graphicsDevice;
        public static Color WarpPink = new Color(192, 81, 235);



        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferWidth = (int)defaultScreenSize.X;
            this.graphics.PreferredBackBufferHeight = (int)defaultScreenSize.Y;
            //graphics.IsFullScreen = true;
            
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }
        protected override void Initialize()
        {
            base.Initialize();
        }
        public static Mode mode = Mode.Menu;
        protected override void LoadContent()
        {
            instance = this;
            graphicsDevice = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = GraphicsDevice;
            MonoSoundManager.Init();
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            var dataColors = new Color[1 * 1]; //Color array
            dataColors[0] = Color.White;
            pixel.SetData(0, null, dataColors, 0, 1 * 1);
            Circle.LoadDrawCircle();
            AssetManager.Load(Content);
            DebrisSet.Load();
            SaveData.SaveManager.LoadSettings();
            SaveData.SaveManager.LoadKeybindings();
            SaveData.MatchupSaver.Load();

            Arena.Load();
            Camera.Load();

            FleetsManager.Load();

            
            isAI[0] = true;
            isAI[1] = true;
            startAI[0] = false;
            startAI[1] = true;

            MyraMain.LoadContent(this, Window);
            PanelManager.Load();
            //MenuManager.SetMenuType(MenuType.Main);
            Arena.Start();
        }
        protected override void UnloadContent()
        {
            MonoSoundManager.DeInit();
        }
        public static void ToMainMenu()
        {
            FleetsManager.Repair();
            if (mode != Mode.Menu)
            {
                mode = Mode.Menu;
                isAI[0] = true;
                isAI[1] = true;
                MyraMain.SwitchMenu(MenuType.Main);
            }
        }
        public static void StartQuickPlay()
        {
            if (mode != Mode.QuickPlay)
            {
                MyraMain.SwitchMenu(MenuType.None);
                mode = Mode.QuickPlay;
                isAI[0] = startAI[0];
                isAI[1] = startAI[1];
            }
        }
        public static void StartFleets()
        {
            if (mode != Mode.Fleets)
            {
                MyraMain.SwitchMenu(MenuType.None);
                FleetsManager.Repair();
                mode = Mode.Fleets;
                isAI[0] = startAI[0];
                isAI[1] = startAI[1];
                Arena.Start();

            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                ToMainMenu();
                FleetsManager.Repair();
                //Exit();
            }

            Arena.Update();
            Camera.Update(Window);
            PanelManager.Update();
            MyraMain.Update(Window);

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Camera.RenderArena(spriteBatch);


            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            PanelManager.Draw(spriteBatch);
            UI.Draw(spriteBatch, Window.ClientBounds);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
    public enum Mode : byte
    {
        QuickPlay,
        Test,
        Menu,
        Fleets,
        ShipSelect
    }
}
