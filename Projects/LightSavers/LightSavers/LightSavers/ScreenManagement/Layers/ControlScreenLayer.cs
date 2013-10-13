using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LightSavers.Utils;
using LightSavers.Components.MenuObjects;

namespace LightSavers.ScreenManagement.Layers
{
    class ControlScreenLayer : ScreenLayer
    {
        private Viewport viewport;
        private RenderTarget2D menu3dscene;
        private SpriteBatch canvas;

        private MenuBackground menuBackground;

        // list of submenues
        private List<Submenu> submenus;
        // current index
        private int currentSubMenuIndex;

        private Color transitionColour;

        private Rectangle titleRect;

        /// <summary>
        /// The constructor for the Main Menu.
        /// </summary>
        public ControlScreenLayer() : base()
        {
            SetLayerAttributes();
            isTransparent = true;

            menuBackground = new MenuBackground();

            ConstructDrawingObjects();
        }

        #region == constructor submethods ==

        private void SetLayerAttributes()
        {
            isTransparent = false;
            transitionOnTime = TimeSpan.FromSeconds(0);
            transitionOffTime = TimeSpan.FromSeconds(0);
        }

        private void ConstructDrawingObjects()
        {
            viewport = Globals.graphics.GraphicsDevice.Viewport;
            canvas = new SpriteBatch(Globals.graphics.GraphicsDevice);
            menu3dscene = new RenderTarget2D(
                Globals.graphics.GraphicsDevice,
                viewport.Width,
                viewport.Height,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24,
                0,
                RenderTargetUsage.DiscardContents);


            int tx = (viewport.Width - 800) / 2;
            titleRect = new Rectangle(tx, 100, 800, 230);
        }

        #endregion

        /// <summary>
        /// The main draw method called by the ScreenManager
        /// </summary>
        public override void Draw()
        {
            // Draw the layers
            canvas.Begin();

            // Draw the 3d background
            canvas.Draw(menu3dscene, viewport.Bounds, Color.White);
            Color talpha = new Color(1.0f, 1.0f, 1.0f, 0.0f);

            canvas.Draw(AssetLoader.tex_black, viewport.Bounds, talpha);

            //drawing prompt to go back
            canvas.Draw(AssetLoader.diamond, new Rectangle(200 - 50, 200 + 6, 40, 15), Color.White);
            canvas.DrawString(AssetLoader.fnt_assetloadscreen, "Back", new Vector2(200, 200), Color.White);

            // finish drawing
            canvas.End();
        }


        /// <summary>
        /// The main update method, called by the ScreenManager
        /// </summary>
        public override void Update(float ms)
        {
            if (this.state == ScreenState.Active)
            {
                CheckControls();
            }

            base.Update(ms);
        }

        #region == update submethods ==

        private void CheckControls()
        {
            //back button
            if ((Globals.inputController.isButtonPressed(Buttons.B, null)) || (Globals.inputController.isButtonPressed(Buttons.A, null)))
            {
                this.fadeOutCompleteCallback = backToMain;
                this.StartTransitionOff();
            }            
        }

        public bool backToMain()
        {
            Globals.screenManager.Pop();
            Globals.screenManager.Push(new MainMenuLayer());
            return true;
        }
        #endregion
    }
}
