using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ProjectGaze.Entities;
using ProjectGaze.Entities.Projectiles;
using ProjectGaze.Entities.Ships;
using SpriteFontPlus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ProjectGaze
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
        public static Polygon one;
        Polygon two;
        public static Texture2D pixel;
        public static GraphicsDevice device;
        public static SpriteFont font;
        

        public static List<Entity> entities = new List<Entity>();
        public static List<CollisionEvent> CEs = new List<CollisionEvent>();
        public static List<Particle> particles = new List<Particle>();
        public static List<Effect> effects = new List<Effect>();


        public static Ship[] ships = new Ship[2];
        public static bool[] isAI = new bool[2];
        public static bool[] startAI = new bool[2];
        public static Vector2 defaultScreenSize = new Vector2(1200, 900);
        //public static Vector2 defaultScreenSize = new Vector2(600, 450);
        public static float boundrySize = 800;
        public static Line[] boundryLines = new Line[4];
        public static float zoom = 1f;
        public static float CameraWorldSize = boundrySize/2;
        public static float CameraWorlMaxSize = boundrySize / 2;
        public static float CameraWorldMinSize = 100;
        public static float CameraDisplaySize = 900;
        public static Vector2 screenPosition = Vector2.Zero;
        public static GraphicsDevice graphicsDevice;

        public static Color WarpPink = new Color(192, 81, 235);

        public static Vector2[] Stars = new Vector2[300];
        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferWidth = (int)defaultScreenSize.X;
            this.graphics.PreferredBackBufferHeight = (int)defaultScreenSize.Y;
            //graphics.IsFullScreen = true;
            CameraDisplaySize = defaultScreenSize.Y;
            Window.AllowUserResizing = true;
        }
        protected override void Initialize()
        {
            base.Initialize();
        }
        public static Mode mode = Mode.Menu;
        static int simSpeed = 1;
        protected override void LoadContent()
        {
            /*
            int[] testI = new int[] { 1, 2, 3 };
            int[] testJ = new int[3];
            testI.CopyTo(testJ, 0);
            testI[2] = 4;
            Debug.WriteLine(testJ[2]);
            */
            //Debug.WriteLine((int)Math.Round(1.7f));
            //Debug.WriteLine((int)Math.Round(1.1f));
            instance = this;
            graphicsDevice = graphics.GraphicsDevice;
            font = TtfFontBaker.Bake(File.ReadAllBytes(@"C:\\Windows\\Fonts\arial.ttf"), 25, 1024, 1024, new[] { CharacterRange.BasicLatin, CharacterRange.Latin1Supplement,CharacterRange.LatinExtendedA, CharacterRange.Cyrillic }).CreateSpriteFont(GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = GraphicsDevice;
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            var dataColors = new Color[1 * 1]; //Color array
            dataColors[0] = Color.White;
            pixel.SetData(0, null, dataColors, 0, 1 * 1);
            Circle.LoadDrawCircle();

            Song song = Content.Load<Song>("CombatTheme");
            MediaPlayer.Volume = .15f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(song);
            AssetManager.Load(Content);

            Vector2 middle = new Vector2(400, 400);
            one = new Polygon(new Vector2[] { middle + Vector2.UnitY * 100, middle + Vector2.UnitX * 100, middle + Vector2.UnitY * -100, middle + Vector2.UnitX * -100 });
            two = new Polygon(new Vector2[] { middle + Vector2.UnitY * 120, middle + Vector2.UnitX * 120, middle + Vector2.UnitY * -120, middle + Vector2.UnitX * -120 });

            
            boundryLines[0] = new Line(Vector2.Zero, Vector2.UnitX * boundrySize);
            boundryLines[1] = new Line(Vector2.UnitX * boundrySize, Vector2.One * boundrySize);
            boundryLines[2] = new Line(Vector2.One * boundrySize, Vector2.UnitY * boundrySize);
            boundryLines[3] = new Line(Vector2.UnitY * boundrySize, Vector2.Zero);
            isAI[0] = true;
            isAI[1] = true;
            startAI[0] = false;
            startAI[1] = true;
            for (int s = 0; s < Stars.Length; s++)
            {
                Stars[s] = new Vector2(random.Next((int)(boundrySize)), random.Next((int)boundrySize));
            }
            MenuManager.SetMenuType(MenuType.Main);
            Start();
        }
        const int shipTypeCount = 16;
        static int resetTimer = 0;
        static int firstShip = 0;
        static int secondShip = -1;
        static int firstShipStock = 10;
        static int secondShipStock = 10;
        static float[,] matchUpScores = new float[shipTypeCount, shipTypeCount];
        public static void Start()
        {
            for (int i = 0; i < 2; i++)
            {
                if (ships[i] != null)
                {
                    ships[i].Kill();
                    ships[i] = null;
                }
            }
            entities.Clear();
            switch (mode)
            {
                case Mode.QuickPlay:
                case Mode.Menu:
                    ships[0] = Ship.Spawn(ShipID.Apocalypse, 0);
                    ships[1] = Ship.Spawn(ShipID.Assassin, 1);
                    ships[1].rotation = (float)Math.PI;
                    //SpawnRandomShip(1);
                    break;
                case Mode.Test:
                    firstShipStock = 10;
                    secondShipStock = 10;
                    secondShip++;
                    if(secondShip > shipTypeCount -1)
                    {
                        secondShip = 0;
                        firstShip++;
                        if(firstShip > shipTypeCount - 1)
                        {

                            //print matchup scores
                            ShowResults();
                            simSpeed = 1;
                            mode = Mode.QuickPlay;
                        }
                    }
                    ships[0] = Ship.Spawn((ShipID)firstShip, 0);
                    ships[1] = Ship.Spawn((ShipID)secondShip, 1);
                    ships[1].rotation = (float)Math.PI;
                    break;
            }
            //new Planet(new Vector2(200 + 400 * random.Next(2), 200 + 400 * random.Next(2)));
            resetTimer = 0;
        }
        void ToMainMenu()
        {
            if (mode != Mode.Menu)
            {
                mode = Mode.Menu;
                isAI[0] = true;
                isAI[1] = true;
            }
        }
        public static void StartQuickPlay()
        {
            if (mode != Mode.QuickPlay)
            {
                mode = Mode.QuickPlay;
                isAI[0] = startAI[0];
                isAI[1] = startAI[1];
            }
        }
        static void ShowResults()
        {
            Debug.WriteLine("Matchup Scores");
            float[] totalScore = new float[shipTypeCount];
            for (int i =0; i < matchUpScores.GetLength(0); i++)
            {

                ShipStats.GetTitlesFor((ShipID)i, out string r, out _);
                string o = r + ": ";
                for(int j =0; j < matchUpScores.GetLength(1); j++)
                {
                    totalScore[i] += matchUpScores[i, j];
                    o += matchUpScores[i, j] + ", ";
                }
                Debug.WriteLine(o);
            }
            float biggestSecondScore = 0;
            float[] secondScore = new float[shipTypeCount];
            for (int i = 0; i < matchUpScores.GetLength(0); i++)
            {
                for (int j = 0; j < matchUpScores.GetLength(1); j++)
                {
                    secondScore[i] += matchUpScores[i, j] * totalScore[j];
                }
                if(secondScore[i] > biggestSecondScore)
                {
                    biggestSecondScore = secondScore[i];
                }
            }
            int maxScore = 30;
            float scaler = maxScore / biggestSecondScore;
            int[] estimatedScore = new int[shipTypeCount];
            for (int i = 0; i < matchUpScores.GetLength(0); i++)
            {
                estimatedScore[i] = (int)Math.Round(secondScore[i] * scaler);

            }
            Debug.WriteLine("Estimated Scores");
            for (int i = 0; i < totalScore.Length; i++)
            {
                ShipStats.GetTitlesFor((ShipID)i, out string r, out _);
                Debug.WriteLine(r +": " + estimatedScore[i]);
            }
        }
        int combatTimer = 0;
        int Simulate()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update();
            }
            Controls.Process();
            if (Networking.Update())
            {
                return -1;
            }
            Controls.LockControls();
            for (int i = 0; i < 2; i++)
            {
                if (ships[i] != null && ships[i].StunTime <= 0)
                {
                    if (Controls.controlShoot[i])
                    {
                        ships[i].Shoot();
                    }
                    if (Controls.controlSpecial[i])
                    {
                        ships[i].Special();
                    }
                    if (Controls.controlThrust[i])
                    {
                        ships[i].Thrust();
                    }
                    if (Controls.controlRight[i])
                    {
                        ships[i].Right();
                    }
                    if (Controls.controlLeft[i])
                    {
                        ships[i].Left();
                    }
                }
            }
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].PhysicsMovement();
            }
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].MakeHitboxes();
            }
            for (int i = 0; i < entities.Count; i++)
            {
                for (int j = i + 1; j < entities.Count; j++)
                {
                    if (entities[i].team != entities[j].team)
                    {
                        List<Shape> entityIBoxes = entities[i].GetCollisionBoxes();
                        List<Shape> entityJBoxes = entities[j].GetCollisionBoxes();
                        bool colConfirm = false;
                        for (int k = 0; k < entityIBoxes.Count; k++)
                        {
                            for (int l = 0; l < entityJBoxes.Count; l++)
                            {
                                if (entityIBoxes[k].Colliding(entityJBoxes[l]))
                                {
                                    colConfirm = true;
                                    break;
                                }
                            }
                            if (colConfirm)
                            {
                                break;
                            }
                        }
                        if (colConfirm)
                        {
                            new CollisionEvent(entities[i], entities[j]);
                        }
                    }
                }
            }
            for (int i = 0; i < CEs.Count; i++)
            {
                CEs[i].Process();
            }
            CEs.Clear();
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].Update();
            }
            if(mode == Mode.Test)
            {
                combatTimer++;
                if(combatTimer > 60 * 60 * 10)
                {
                    Debug.WriteLine("Time Out!!");
                    combatTimer = 0;
                    for (int i = 0; i < 2; i++)
                    {
                        if (ships[i] != null)
                        {
                            ships[i].Kill();
                            ships[i] = null;
                        }
                    }
                }
            }
            for (int i = 0; i < 2; i++)
            {
                if (ships[i] != null && ships[i].health <= 0)
                {
                    ships[i] = null;
                }
            }

            if (ships[0] == null || ships[1] == null)
            {
                resetTimer++;
                if (resetTimer > 180)
                {
                    resetTimer = 0;
                    if (ships[0] == null && ships[1] == null)
                    {
                        return 2;
                    }
                    if(ships[0] == null)
                    {
                        return 0;
                    }
                    if(ships[1] == null)
                    {
                        return 1;
                    }
                }
            }
            return -1;
        }
        static void SpawnRandomShip(int team)
        {
            if (team == 2)
            {
                SpawnRandomShip(0);
                SpawnRandomShip(1);
            }
            else
            {
                
                int otherTeam = team == 1 ? 0 : 1;
                if (ships[team] == null)
                {
                    if (ships[otherTeam] != null)
                    {
                        int pick = random.Next(shipTypeCount - 1);
                        if (pick >= (int)ships[otherTeam].type)
                        {
                            pick++;
                        }
                        ships[team] = Ship.Spawn((ShipID)pick, team);
                        ships[team].rotation = ships[1].rotation + (float)Math.PI;
                    }
                    else
                    {
                        ships[team] = Ship.Spawn((ShipID)random.Next(shipTypeCount), team);
                    }
                }
            }
        }
        protected override void Update(GameTime gameTime)
        {

            colorCounter++;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                ToMainMenu();
                //Exit();
            }




            for (int k =0; k < simSpeed; k++)
            {
                if(mode == Mode.Test)
                {
                    if(secondShip <= firstShip)
                    {
                        Start();
                    }
                    if(Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        simSpeed = 40;
                    }
                }
                int r = Simulate();
                if (r != -1)
                {
                    combatTimer = 0;
                    switch (mode)
                    {
                        case Mode.QuickPlay:
                        case Mode.Menu:
                            SpawnRandomShip(r);
                            break;
                        case Mode.Test:
                            if(r == 0 || r == 2)
                            {
                                firstShipStock--;
                                ships[0] = Ship.Spawn((ShipID)firstShip, 0);
                            }
                            if(r == 1 || r == 2)
                            {
                                secondShipStock--;
                                ships[1] =  Ship.Spawn((ShipID)secondShip, 1);
                            }
                            if(firstShipStock <= 0 || secondShipStock <= 0)
                            {

                                Debug.WriteLine(firstShipStock * 0.1f + ", " + secondShipStock * 0.1f);
                                matchUpScores[firstShip, secondShip] = firstShipStock * 0.1f;
                                matchUpScores[secondShip, firstShip] = secondShipStock * 0.1f;
                                simSpeed = 1;
                                Start();
                            }
                            break;
                    }
                }
            }

            MenuManager.Update(Window.ClientBounds);

            //one = ships[0].Hitbox();
            //two = ships[1].Hitbox();
            CameraDisplaySize = Math.Min(Window.ClientBounds.Height, Window.ClientBounds.Width * 0.75f);
            if (ships[0] != null && ships[1] != null)
            {
                screenPosition = (Functions.screenLoopAdjust(ships[0].position, ships[1].position) + ships[0].position) / 2f - Vector2.One * (CameraWorldSize / 2);
                Vector2 diff = (Functions.screenLoopAdjust(ships[0].position, ships[1].position) - ships[0].position);
                CameraWorldSize = Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y)) * 1.8f;
                if (CameraWorldSize > CameraWorlMaxSize)
                {
                    CameraWorldSize = CameraWorlMaxSize;
                }
                if (CameraWorldSize < CameraWorldMinSize)
                {
                    CameraWorldSize = CameraWorldMinSize;
                }
            }
            else
            {
                if(ships[0] != null)
                {
                    screenPosition = (ships[0].position - Vector2.One * (CameraWorldSize / 2));
                }
                if (ships[1] != null)
                {
                    screenPosition = (ships[1].position - Vector2.One * (CameraWorldSize / 2));
                }
                CameraWorldSize = CameraWorldMinSize;
            }
            zoom = CameraDisplaySize / CameraWorldSize;

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Matrix trans = new Matrix(
                zoom, 0, 0, 0,
                0, zoom, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
                );
            spriteBatch.Begin(transformMatrix: trans, samplerState: SamplerState.PointClamp);
            Vector2[] offsets = Functions.OffsetsForDrawing();
            Rectangle screenView = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, (int)Main.CameraWorldSize, (int)Main.CameraWorldSize);
            for (int i = 0; i < 9; i++)
            {
                for(int j =0; j < Stars.Length; j++)
                {
                    if (screenView.Contains((Stars[j] + offsets[i]).ToPoint()))
                    {
                        spriteBatch.Draw(Main.pixel, Main.CameraOffset(Stars[j] + offsets[i]), null, Color.White, 0, new Vector2(.5f, .5f), Vector2.One, SpriteEffects.None, 0);
                    }
                }
            }
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].Draw(spriteBatch);
            }
            foreach (Particle drawMe in particles)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (screenView.Contains((drawMe.Position + offsets[i]).ToPoint()))
                    {
                        spriteBatch.Draw(texture: pixel, position: CameraOffset(drawMe.Position + offsets[i]), origin: new Vector2(pixel.Width, pixel.Height) * .5f, color: drawMe.color, rotation: Functions.ToRotation(drawMe.velocity));
                    }
                }
            }
            for (int i =0; i < entities.Count; i++)
            {
                if(entities[i] is Projectile && !entities[i].DrawOnTop)
                {
                    entities[i].Draw(spriteBatch);
                    if(entities[i].collisionLine != null)
                    {
                        float dir = (float)entities[i].collisionLine;
                        Line newLine = new Line(entities[i].position, entities[i].position + Functions.PolarVector(20, dir));
                        newLine.Draw(spriteBatch, Color.Red);
                    }
                }
            }
            for (int i = 0; i < entities.Count; i++)
            {
                if (!(entities[i] is Projectile) && !entities[i].DrawOnTop)
                {
                    entities[i].Draw(spriteBatch);
                    if (entities[i].collisionLine != null)
                    {
                        float dir = (float)entities[i].collisionLine;
                        Line newLine = new Line(entities[i].position, entities[i].position + Functions.PolarVector(20, dir));
                        newLine.Draw(spriteBatch, Color.Red);
                    }
                }
            }
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].DrawOnTop)
                {
                    entities[i].Draw(spriteBatch);
                    if (entities[i].collisionLine != null)
                    {
                        float dir = (float)entities[i].collisionLine;
                        Line newLine = new Line(entities[i].position, entities[i].position + Functions.PolarVector(20, dir));
                        newLine.Draw(spriteBatch, Color.Red);
                    }
                }
            }
            //useful for debugging
            if (ships[0] != null)
            {
                //ships[0].Hitbox().Draw(spriteBatch, Color.White);
            }
            if (ships[1] != null)
            {
                //ships[1].Hitbox().Draw(spriteBatch, Color.White);
            }
            if(one != null)
            {
                //one.Draw(spriteBatch, Color.Pink);
            }
            /*
            for(int i =0; i < 4; i++)
            {
                boundryLines[i].Draw(spriteBatch, Color.Red);
            }
            */
            spriteBatch.End();
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            UI.Draw(spriteBatch, Window.ClientBounds);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        public static Vector2 CameraOffset(Vector2 here)
        {
            return here - screenPosition;
        }
        static int colorCounter = 0;
        public static Color Rainbow()
        {
            return Functions.ToRgb((float)(colorCounter % 120) / 120f, 1f, .5f);
        }
    }
    public enum Mode : byte
    {
        QuickPlay,
        Test,
        Menu
    }
}
