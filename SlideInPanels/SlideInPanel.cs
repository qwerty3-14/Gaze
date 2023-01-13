using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;
using GazeOGL.MyraUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.SlideInPanels
{
    public class SlideInPanel
    {
        Vector2 restPosition;
        Vector2 restSize;
        SlideDirection slideDirection;
        protected float storeOffset = 20;
        const float STORETIME = 20;
        protected Panel rootPanel;
        public SlideInPanel(Vector2 position, Vector2 size, SlideDirection slideDirection)
        {
            this.restPosition = position;
            this.restSize = size;
            this.slideDirection = slideDirection;
            rootPanel = new Panel();
            MyraMain.AddToRoot(rootPanel);

            GetBounds(out position, out size);
            SetupPanelContent(position, size, ref rootPanel);

        }
        public bool MoveOff()
        {
            storeOffset++;
            if(storeOffset > STORETIME)
            {
                storeOffset = STORETIME;
                return true;
            }
            return false;
        }
        public bool  MoveOn()
        {
            storeOffset--;
            if (storeOffset < 0)
            {
                storeOffset = 0;
                return true;
            }
            return false;
        }

        void GetBounds(out Vector2 poistion, out Vector2 size)
        {
            size = restSize * Camera.CameraDisplaySize;
            poistion = restPosition * Camera.CameraDisplaySize;
            switch(slideDirection)
            {
                case SlideDirection.top:
                    poistion -= Vector2.UnitY * (storeOffset / STORETIME) * Camera.CameraDisplaySize;
                    break;
                case SlideDirection.right:
                    poistion += Vector2.UnitX * (storeOffset / STORETIME) * Camera.CameraDisplaySize;
                    break;
                case SlideDirection.bottom:
                    poistion += Vector2.UnitY * (storeOffset / STORETIME) * Camera.CameraDisplaySize;
                    break;
                case SlideDirection.left:
                    poistion -= Vector2.UnitX * (storeOffset / STORETIME) * Camera.CameraDisplaySize;
                    break;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            GetBounds(out Vector2 position, out Vector2 size);

            Texture2D texture = AssetManager.ui[20];
            spriteBatch.Draw(texture, position, new Rectangle(2, 2, 28, 28), Color.White, 0, Vector2.Zero, size * (1 / 28f), SpriteEffects.None, 0f);

        }
        public void Update()
        {
            
            GetBounds(out Vector2 position, out Vector2 size);
            rootPanel.Left = (int)position.X;
            rootPanel.Top = (int)position.Y;
            rootPanel.Width = (int)size.X;
            rootPanel.Height = (int)size.Y;
            UpdatePanelContent(position, size, ref rootPanel);
        }

        //virtual methods
        public virtual void SetupPanelContent(Vector2 position, Vector2 size, ref Panel root)
        {

        }
        public virtual void UpdatePanelContent(Vector2 position, Vector2 size, ref Panel root)
        {

        }

    }
}
