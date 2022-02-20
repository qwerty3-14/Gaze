using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze
{
    public static class Controls
    {
        public static bool[] controlThrust = new bool[2];
        public static bool[] controlRight = new bool[2];
        public static bool[] controlLeft = new bool[2];
        public static bool[] controlShoot = new bool[2];
        public static bool[] controlSpecial = new bool[2];
        public static bool[] controlDown = new bool[2];

        public static bool menuUp;
        public static bool menuDown;
        public static bool menuConfirm;

        static bool player2Controller = false;
        public static MouseState mouse;
        public static bool Process()
        {
            if(Main.mode == Mode.Menu)
            {
                mouse = Mouse.GetState();
                menuUp = Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up);
                menuDown = Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down);
                menuConfirm = Keyboard.GetState().IsKeyDown(Keys.Enter) || Keyboard.GetState().IsKeyDown(Keys.Space) || mouse.LeftButton == ButtonState.Pressed;
            }
            /*
            if (Networking.IsConnected())
            {

            }
            else
            */
            //if(Networking.netFrameCounter == 0)
            {
                //player 1 controls
                if (Main.isAI[0])
                {
                    if (Main.ships[0] != null)
                    {
                        Main.ships[0].BaseAI();
                    }
                }
                else
                {
                    ControlSet1(0);
                }
                //player 2 controls
                if (Main.isAI[1])
                {
                    if (Main.ships[1] != null)
                    {
                        Main.ships[1].BaseAI();
                    }
                }
                else
                {
                    if (GamePad.GetState(0).IsConnected)
                    {
                        player2Controller = true;
                    }
                    if (player2Controller)
                    {
                        GamePadState gamePad = GamePad.GetState(0);
                        controlThrust[1] = gamePad.DPad.Up == ButtonState.Pressed;
                        controlRight[1] = gamePad.DPad.Right == ButtonState.Pressed;
                        controlLeft[1] = gamePad.DPad.Left == ButtonState.Pressed;
                        controlDown[1] = gamePad.DPad.Down == ButtonState.Pressed;
                        controlShoot[1] = gamePad.Buttons.B == ButtonState.Pressed;
                        controlSpecial[1] = gamePad.Buttons.A == ButtonState.Pressed;
                    }
                    else
                    {
                        if(Networking.GetNetMode() == NetMode.client)
                        {

                            ControlSet1(1);
                        }
                        else
                        {
                            ControlSet2(1);
                        }
                    }
                }
                if (Networking.netFrameCounter == 0 && Networking.incomingControls.Count > 0)
                {
                    Networking.myControls.CopyTo(lockedLocal, 0);
                    Networking.incomingControls[0].CopyTo(lockedRemote, 0);
                    Networking.incomingControls.RemoveAt(0);
                }
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
                    Controls.Set(0, lockedRemote);
                    Controls.Set(1, lockedLocal);
                }
                else
                {
                    Controls.Set(1, lockedRemote);
                    Controls.Set(0, lockedLocal);
                }
            }
        }
        static void ControlSet1(int team)
        {
            controlThrust[team] = Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up);
            controlRight[team] = Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right);
            controlLeft[team] = Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Left);
            controlDown[team] = Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down);
            controlShoot[team] = Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.LeftControl) || Keyboard.GetState().IsKeyDown(Keys.RightControl);
            controlSpecial[team] = Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift);
        }
        static void ControlSet2(int team)
        {
            controlThrust[team] = Keyboard.GetState().IsKeyDown(Keys.T);
            controlRight[team] = Keyboard.GetState().IsKeyDown(Keys.H);
            controlLeft[team] = Keyboard.GetState().IsKeyDown(Keys.F);
            controlShoot[team] = Keyboard.GetState().IsKeyDown(Keys.K);
            controlSpecial[team] = Keyboard.GetState().IsKeyDown(Keys.O) || Keyboard.GetState().IsKeyDown(Keys.I);
            controlDown[team] = Keyboard.GetState().IsKeyDown(Keys.G);
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
