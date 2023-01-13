using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GazeOGL.Entities;
using GazeOGL.Entities.Projectiles;
using GazeOGL.Entities.Ships;
using GazeOGL.MyraUI;
using GazeOGL.SlideInPanels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL
{
    public static class Arena
    {
        public static Line[] boundryLines = new Line[4];
        public static float boundrySize = 800;
        public static Vector2[] Stars = new Vector2[300];


        public static List<Entity> entities = new List<Entity>();
        public static List<CollisionEvent> CEs = new List<CollisionEvent>();
        public static List<Particle> particles = new List<Particle>();
        public static List<Effect> effects = new List<Effect>();
        public static Ship[] ships = new Ship[2];
        public static List<Ship> npcs = new List<Ship>();

        public static int simSpeed = 1;
        static int resetTimer = 0;
        static int combatTimer = 0;

        public static void Load()
        {
            boundryLines[0] = new Line(Vector2.Zero, Vector2.UnitX * boundrySize);
            boundryLines[1] = new Line(Vector2.UnitX * boundrySize, Vector2.One * boundrySize);
            boundryLines[2] = new Line(Vector2.One * boundrySize, Vector2.UnitY * boundrySize);
            boundryLines[3] = new Line(Vector2.UnitY * boundrySize, Vector2.Zero);


            for (int s = 0; s < Stars.Length; s++)
            {
                Stars[s] = new Vector2(Main.random.Next((int)(boundrySize)), Main.random.Next((int)boundrySize));
            }
        }
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
            npcs.Clear();
            entities.Clear();
            switch (Main.mode)
            {
                case Mode.Fleets:
                    OpenShipSelect(2);
                    break;
                case Mode.QuickPlay:
                case Mode.Menu:
                    ships[0] = Ship.Spawn(ShipID.Missionary, 0);
                    ships[1] = Ship.Spawn(ShipID.Hunter, 1);
                    ships[1].rotation = (float)Math.PI;
                    //SpawnRandomShip(1);
                    break;
                case Mode.Test:
                    ScoreTracker.TestSetup();
                    break;
            }
            //new Planet(new Vector2(200 + 400 * random.Next(2), 200 + 400 * random.Next(2)));
            resetTimer = 0;
        }
        static int npcSpawncounter = 0;
        public static int npcsSpawned = 0;
        static int Simulate()
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
            if (Main.mode == Mode.ShipSelect)
            {
                return -1;
            }
            for(int i = 0; i < npcs.Count; i++)
            {
                npcs[i].AI();
            }
            npcSpawncounter++;
            if(npcSpawncounter > 60 * 60 * 5 && npcs.Count < 12)
            {
                npcs.Add(Ship.Spawn(ShipID.Count, 3 + npcsSpawned));
                npcSpawncounter = 0;
                npcsSpawned++;
            }
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
                    if (ships[i] is Surge && Controls.controlDown[i])
                    {
                        ships[i].Thrust(true);
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
            for(int i =0; i < npcs.Count; i++)
            {
                if (npcs[i] != null && npcs[i].StunTime <= 0)
                {
                    if (npcs[i].npcShoot)
                    {
                        npcs[i].Shoot();
                    }
                    if (npcs[i].npcSpecial)
                    {
                        npcs[i].Special();
                    }
                    if (npcs[i].npcThrust)
                    {
                        npcs[i].Thrust();
                    }
                    if (npcs[i] is Surge && npcs[i].npcDown)
                    {
                        npcs[i].Thrust(true);
                    }
                    if (npcs[i].npcRight)
                    {
                        npcs[i].Right();
                    }
                    if (npcs[i].npcLeft)
                    {
                        npcs[i].Left();
                    }
                    if(!entities.Contains(npcs[i]))
                    {
                        npcs.Remove(npcs[i]);
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
                    if ((entities[i].team != entities[j].team || entities[i].team == 2) && !(entities[i].ignoreMe || entities[j].ignoreMe))
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
            if (Main.mode == Mode.Test)
            {
                combatTimer++;
                if (combatTimer > 60 * 60 * 10)
                {
                    Console.WriteLine("Time Out!!");
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
                    if (ships[0] == null)
                    {
                        return 0;
                    }
                    if (ships[1] == null)
                    {
                        return 1;
                    }
                }
                if(resetTimer == 2)
                {
                    for(int i =0; i < 2; i++)
                    {
                        if(ships[i] != null)
                        {
                            AssetManager.PlayDitty(ships[i].type);
                        }
                    }
                }
            }
            return -1;
        }
        static void SpawnFromFleet(int team)
        {
            if (team == 2)
            {
                SpawnFromFleet(0);
                SpawnFromFleet(1);
            }
            else
            {

                if (ships[team] == null)
                {
                    if (FleetsManager.fleets[team].IsDestroyed())
                    {
                        Main.ToMainMenu();
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        if (FleetsManager.fleets[team].ships[i] != ShipID.Count && !FleetsManager.fleets[team].destroyed[i])
                        {
                            ships[team] = Ship.Spawn(FleetsManager.fleets[team].ships[i], team);
                            FleetsManager.fleets[team].destroyed[i] = true;
                            break;
                        }
                    }
                }
            }
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
                        int pick = Main.random.Next((int)ShipID.Count - 1);
                        if (pick >= (int)ships[otherTeam].type)
                        {
                            pick++;
                        }
                        ships[team] = Ship.Spawn((ShipID)pick, team);
                        ships[team].rotation = ships[1].rotation + (float)Math.PI;
                    }
                    else
                    {
                        ships[team] = Ship.Spawn((ShipID)Main.random.Next((int)ShipID.Count), team);
                    }
                }
            }
        }
        static void OpenShipSelect(int team)
        {

            Main.mode = Mode.ShipSelect;
            MyraMain.SwitchMenu(MenuType.None);
            PanelManager.OpenShipSelectors(team);

        }
        public static void Update()
        {
            for (int k = 0; k < simSpeed; k++)
            {
                if (Main.mode == Mode.Test)
                {
                    ScoreTracker.Update();
                }
                int r = Simulate();
                if (r != -1)
                {
                    combatTimer = 0;
                    switch (Main.mode)
                    {
                        case Mode.Fleets:
                            OpenShipSelect(r);
                            //SpawnFromFleet(r);
                            break;
                        case Mode.QuickPlay:
                        case Mode.Menu:
                            SpawnRandomShip(r);
                            break;
                        case Mode.Test:
                            ScoreTracker.ProcessBattleResults(r);
                            break;
                    }
                }

            }
        }
    }
}
