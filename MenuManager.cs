using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectGaze.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze
{
    public static class MenuManager 
    {
        public static List<MenuOption> menuOptions = new List<MenuOption>();
        static bool clicked = false;
        static bool pressed = false;
        static bool previusFullscreenSetting;
        static Rectangle previusBound;
        static int selectedIndex = -1;
        static Point mousePosOld;
        static MenuType menuType = MenuType.Main;
        public static void SetMenuType(MenuType type)
        {
            menuType = type;
            selectedIndex = -1;
            SetupMenu(Main.instance.Window.ClientBounds);
        }
        static void SetupMenu(Rectangle ClientBounds)
        {
            //Console.WriteLine("Rebound");
            float UIStartX = Main.CameraDisplaySize;
            float UIWidth = ClientBounds.Width - UIStartX;
            int width = 3 * (int)UIWidth / 4;
            int height = (int)(Main.font.MeasureString("Quit").Y * (float)ClientBounds.Height / 450f);
            int x = (int)UIStartX + (int)UIWidth / 8;
            int y = ClientBounds.Height / 4;
            menuOptions.Clear();
            //selectedIndex = -1;
            switch (menuType)
            {
                case MenuType.Main:
                    menuOptions.Add(new MultiplayerToggle(new Rectangle(x, y, width, height)));
                    y += (int)(height * 1.25f);
                    
                    if (Networking.GetNetMode() != NetMode.client)
                    {
                        menuOptions.Add(new StartQuickPlay(new Rectangle(x, y, width, height)));
                        y += (int)(height * 1.25f);
                    }
                    menuOptions.Add(new Settings(new Rectangle(x, y, width, height)));
                    y += (int)(height * 1.25f);
                    if (Networking.IsConnected())
                    {
                        menuOptions.Add(new Disconnect(new Rectangle(x, y, width, height)));
                    }
                    else
                    {
                        menuOptions.Add(new StartNetPlay(new Rectangle(x, y, width, height)));
                    }
                    y += (int)(height * 1.25f);
                    menuOptions.Add(new Quit(new Rectangle(x, y, width, height)));
                    break;
                case MenuType.Settings:
                    menuOptions.Add(new ToggleFullscreen(new Rectangle(x, y, width, height)));
                    y += (int)(height * 1.25f);
                    menuOptions.Add(new BackToMainMenu(new Rectangle(x, y, width, height)));
                    break;
                case MenuType.HostJoinOption:
                    menuOptions.Add(new HostGame(new Rectangle(x, y, width, height)));
                    y += (int)(height * 1.25f);
                    menuOptions.Add(new JoinGame(new Rectangle(x, y, width, height)));
                    y += (int)(height * 1.25f);
                    menuOptions.Add(new BackToMainMenu(new Rectangle(x, y, width, height)));
                    break;
            }
        }
        public static void Update(Rectangle ClientBounds)
        {
            //if(previusFullscreenSetting != Main.instance.graphics.IsFullScreen || ClientBounds != previusBound)
            {
                SetupMenu(ClientBounds);
            }
            if (Controls.mouse.Position != mousePosOld)
            {
                selectedIndex = -1;
                for (int i = 0; i < menuOptions.Count; i++)
                {
                    if (menuOptions[i].Hovering())
                    {
                        selectedIndex = i;
                    }
                    else
                    {
                        menuOptions[i].isHovering = false;
                    }
                }
                if (Controls.mouse.LeftButton == ButtonState.Pressed)
                {
                    if (selectedIndex != -1)
                    {
                        if (!clicked)
                        {
                            menuOptions[selectedIndex].OnClick();
                        }
                        clicked = true;
                    }
                }
                else
                {
                    clicked = false;
                }
            }
            else
            {
                if (!pressed)
                {
                    if (Controls.menuDown)
                    {
                        pressed = true;
                        if (selectedIndex == -1)
                        {
                            selectedIndex = 0;
                        }
                        else
                        {
                            selectedIndex++;
                            if (selectedIndex >= menuOptions.Count)
                            {
                                selectedIndex = 0;
                            }
                        }
                    }
                    else if (Controls.menuUp)
                    {
                        pressed = true;
                        if (selectedIndex == -1)
                        {
                            selectedIndex = 0;
                        }
                        else
                        {
                            selectedIndex--;
                            if (selectedIndex < 0)
                            {
                                selectedIndex = menuOptions.Count - 1;
                            }
                        }
                    }
                    if(selectedIndex != -1 && Controls.menuConfirm)
                    {
                        pressed = true;
                        menuOptions[selectedIndex].OnClick();
                    }
                }
                if(!Controls.menuDown && !Controls.menuUp && !Controls.menuConfirm)
                {
                    pressed = false;
                }
                //if(pressed)
                {
                    for (int i = 0; i < menuOptions.Count; i++)
                    {
                        menuOptions[i].isHovering = false;
                    }
                    if (selectedIndex != -1)
                    {
                        menuOptions[selectedIndex].isHovering = true;
                    }
                }
            }
            mousePosOld = Controls.mouse.Position;
            previusFullscreenSetting = Main.instance.graphics.IsFullScreen;
            previusBound = ClientBounds;
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            for(int i =0; i < menuOptions.Count; i++)
            {
                menuOptions[i].Draw(spriteBatch);
            }
        }
    }
    public enum MenuType : byte
    {
        Main,
        Settings,
        HostJoinOption
    }
}
