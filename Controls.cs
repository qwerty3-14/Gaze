using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GazeOGL.Entities.Ships;
using GazeOGL.SlideInPanels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL
{
    public static class Controls
    {
        public static bool[] controlThrust = new bool[2];
        public static bool[] controlRight = new bool[2];
        public static bool[] controlLeft = new bool[2];
        public static bool[] controlShoot = new bool[2];
        public static bool[] controlSpecial = new bool[2];
        public static bool[] controlDown = new bool[2];


        public static bool player1ControllerPriority = true;
        public static MouseState mouse;
        public static bool Process()
        {
            if (Main.mode == Mode.Menu)
            {
                mouse = Mouse.GetState();
            }
            
            //player 1 controls
            if (Main.isAI[0])
            {
                if (Arena.ships[0] != null)
                {
                    Arena.ships[0].BaseAI();
                }
                else if(Main.mode == Mode.ShipSelect)
                {
                    PanelManager.shipSelector[0].AISelect();
                }
            }
            else
            {
                ControlSet1(0);
            }
            //player 2 controls
            if (Main.isAI[1])
            {
                if (Arena.ships[1] != null)
                {
                    Arena.ships[1].BaseAI();
                }
                else if (Main.mode == Mode.ShipSelect)
                {
                    PanelManager.shipSelector[1].AISelect();
                }
            }
            else
            {

                if (Networking.GetNetMode() == NetMode.client)
                {
                    ControlSet1(1);
                }
                else
                {
                    ControlSet2(1);
                }

            }
            if (Networking.netFrameCounter == 0 && Networking.incomingControls.Count > 0)
            {
                Networking.myControls.CopyTo(lockedLocal, 0);
                Networking.incomingControls[0].CopyTo(lockedRemote, 0);
                Networking.incomingControls.RemoveAt(0);
            }

            return false;
        }
        static bool[] lockedLocal = new bool[6];
        static bool[] lockedRemote = new bool[6];
        public static void LockControls()
        {
            if (Networking.IsConnected())
            {
                if (Networking.GetNetMode() == NetMode.client)
                {
                    Set(0, lockedRemote);
                    Set(1, lockedLocal);
                }
                else
                {
                    Set(1, lockedRemote);
                    Set(0, lockedLocal);
                }
            }
        }
        public static Keys[,] configuredControls = new Keys[,] 
        { 
            { Keys.W, Keys.Up, Keys.T, Keys.None},
            { Keys.D, Keys.Right, Keys.H, Keys.None },
            { Keys.A, Keys.Left, Keys.F, Keys.None},
            { Keys.S, Keys.Down, Keys.G, Keys.None},
            { Keys.Space, Keys.RightControl, Keys.K, Keys.None},
            { Keys.LeftShift, Keys.RightShift, Keys.O, Keys.I }
        };
        public static bool GetNextKeyPress(out Keys outKey)
        {
            outKey = Keys.None;
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (Keyboard.GetState().IsKeyDown(key))
                {
                    outKey = key;
                    return true;
                }
            }
            return false;
        }
        static void ControlSet1(int team)
        {
            controlThrust[team] = Keyboard.GetState().IsKeyDown(configuredControls[0, 0]) || Keyboard.GetState().IsKeyDown(configuredControls[0, 1]);
            controlRight[team] = Keyboard.GetState().IsKeyDown(configuredControls[1, 0]) || Keyboard.GetState().IsKeyDown(configuredControls[1, 1]);
            controlLeft[team] = Keyboard.GetState().IsKeyDown(configuredControls[2, 0]) || Keyboard.GetState().IsKeyDown(configuredControls[2, 1]);
            controlDown[team] = Keyboard.GetState().IsKeyDown(configuredControls[3, 0]) || Keyboard.GetState().IsKeyDown(configuredControls[3, 1]);
            controlShoot[team] = Keyboard.GetState().IsKeyDown(configuredControls[4, 0]) || Keyboard.GetState().IsKeyDown(configuredControls[4, 1]);
            controlSpecial[team] = Keyboard.GetState().IsKeyDown(configuredControls[5, 0]) || Keyboard.GetState().IsKeyDown(configuredControls[5, 1]);

            int controlerIndex = 0;
            if (!player1ControllerPriority && !Main.isAI[1] && !Networking.IsConnected())
            {
                controlerIndex = 1;
            }
            ControllerControls(team, controlerIndex);
        }
        static void ControlSet2(int team)
        {
            controlThrust[team] = Keyboard.GetState().IsKeyDown(configuredControls[0, 2]) || Keyboard.GetState().IsKeyDown(configuredControls[0, 3]);
            controlRight[team] = Keyboard.GetState().IsKeyDown(configuredControls[1, 2]) || Keyboard.GetState().IsKeyDown(configuredControls[1, 3]);
            controlLeft[team] = Keyboard.GetState().IsKeyDown(configuredControls[2, 2]) || Keyboard.GetState().IsKeyDown(configuredControls[2, 3]);
            controlDown[team] = Keyboard.GetState().IsKeyDown(configuredControls[3, 2]) || Keyboard.GetState().IsKeyDown(configuredControls[3, 3]);
            controlShoot[team] = Keyboard.GetState().IsKeyDown(configuredControls[4, 2]) || Keyboard.GetState().IsKeyDown(configuredControls[4, 3]);
            controlSpecial[team] = Keyboard.GetState().IsKeyDown(configuredControls[5, 2]) || Keyboard.GetState().IsKeyDown(configuredControls[5, 3]);

            int controlerIndex = 0;
            if (player1ControllerPriority)
            {
                controlerIndex = 1;
            }
            ControllerControls(team, controlerIndex);
        }
        static void ControllerControls(int team, int controllerIndex)
        {
            if(GamePad.GetState(controllerIndex).IsConnected)
            {

                GamePadState gamePad = GamePad.GetState(controllerIndex);
                controlShoot[team] |= gamePad.IsButtonDown(Buttons.A) || gamePad.IsButtonDown(Buttons.Y);
                controlSpecial[team] |= gamePad.IsButtonDown(Buttons.B) || gamePad.IsButtonDown(Buttons.X);
                if(gamePad.ThumbSticks != null)
                {
                    Vector2 stickCoords = new Vector2(gamePad.ThumbSticks.Left.X, -gamePad.ThumbSticks.Left.Y);
                    if(stickCoords.Length() > 0.1f)
                    {
                        float turnTo = stickCoords.ToRotation();
                        ControllerTurnToward(team, turnTo);
                        if (stickCoords.Length() > 0.5f)
                        {
                            if(gamePad.IsButtonDown(Buttons.LeftStick))
                            {
                                controlDown[team] = true;
                            }
                            else
                            {
                                controlThrust[team] = true;
                            }
                        }
                        return;
                    }
                }
                controlThrust[team] |= gamePad.IsButtonDown(Buttons.DPadUp);
                controlRight[team] |= gamePad.IsButtonDown(Buttons.DPadRight);
                controlLeft[team] |= gamePad.IsButtonDown(Buttons.DPadLeft);
                controlDown[team] |= gamePad.IsButtonDown(Buttons.DPadDown);
            }
        }
        static bool ControllerTurnToward(int team, float targetAngle)
        {
            Ship ship = Arena.ships[team];
            if (ship != null)
            {
                int f = 1; //this is used to switch rotation direction
                float actDirection = Functions.ToRotation(new Vector2((float)Math.Cos(ship.rotation), (float)Math.Sin(ship.rotation)));
                targetAngle = Functions.ToRotation(new Vector2((float)Math.Cos(targetAngle), (float)Math.Sin(targetAngle)));

                //this makes f 1 or -1 to rotate the shorter distance
                if (Math.Abs(actDirection - targetAngle) > Math.PI)
                {
                    f = -1;
                }
                else
                {
                    f = 1;
                }
                if (Functions.AngularDifference(actDirection, targetAngle) < (((float)ship.turnSpeed / 15f) * 2f * (Networking.IsConnected() ? Networking.framePacketSize : 1) * (float)Math.PI) / 60f)
                {
                    return true;
                }
                else
                {
                    if (actDirection <= targetAngle)
                    {
                        if (f == 1)
                        {
                            Controls.controlRight[team] = true;
                        }
                        else
                        {
                            Controls.controlLeft[team] = true;
                        }
                    }
                    else if (actDirection >= targetAngle)
                    {
                        if (f == 1)
                        {
                            Controls.controlLeft[team] = true;
                        }
                        else
                        {
                            Controls.controlRight[team] = true;
                        }

                    }
                }
            }
            return false;
        }
        public static void Set( int team, bool[] input)
        {
            controlThrust[team] = input[0];
            controlRight[team] = input[1];
            controlLeft[team] = input[2];
            controlDown[team] = input[3];
            controlShoot[team] = input[4];
            controlSpecial[team] = input[5];
        }
        public static void Reset()
        {
            for (int i = 0; i < 2; i++)
            {
                controlThrust[i] = false;
                controlRight[i] = false;
                controlLeft[i] = false;
                controlDown[i] = false;
                controlShoot[i] = false;
                controlSpecial[i] = false;
            }
        }
    }
}
