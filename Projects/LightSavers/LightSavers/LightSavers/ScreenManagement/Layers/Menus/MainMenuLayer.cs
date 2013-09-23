using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LightSavers.Utils;
using LightSavers.Components.MenuObjects;

namespace LightSavers.ScreenManagement.Layers.Menus
{
    public class MainMenuLayer : ScreenLayer
    {
        private Viewport viewport;
        private RenderTarget2D menu3dscene;
        private SpriteBatch canvas;

        private MenuBackground menuBackground;

        /// <summary>
        /// The constructor for the Main Menu.
        /// </summary>
        public MainMenuLayer() : base()
        {
            SetLayerAttributes();

            menuBackground = new MenuBackground();

            ConstructDrawingObjects();

            ConstructSubMenus();
        }

        #region == constructor submethods ==

        private void SetLayerAttributes()
        {
            isTransparent = false;
            transitionOnTime = TimeSpan.FromSeconds(0.6);
            transitionOffTime = TimeSpan.FromSeconds(0.5);
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
        }

        private void ConstructSubMenus()
        {

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

            Draw2DLayers();

            // finish drawing
            canvas.End();            
        }

        #region == drawing submethods ==


        private void Draw2DLayers()
        {

            menuBackground.Draw(canvas);

            //draw menu here
            //canvas.Draw(AssetLoader.tex_black, new Rectangle(50, 0, 300, viewport.Height), new Color(0, 0, 0, 150));

            canvas.DrawString(AssetLoader.fnt_assetloadscreen, "Start", new Vector2(60, 400), Color.White);

            if (state == ScreenState.TransitioningOff || state == ScreenState.TransitioningOn)
            {
                int trans = (int)((1 - transitionPercent) * 255.0f);
                canvas.Draw(AssetLoader.tex_black, viewport.Bounds, new Color(trans, trans, trans, trans));
            }
        }


        #endregion

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
            if (Globals.inputController.isButtonReleased(Buttons.B, null))
            {
                this.StartTransitionOff();
            }
            else if (Globals.inputController.isButtonReleased(Buttons.A, null))
            {
                this.fadeOutCompleteCallback = StartGame;
                this.StartTransitionOff();
            }
        }

        #endregion

        public bool StartGame()
        {

            Globals.screenManager.Pop();

            Globals.screenManager.Push(new GameLayer());

            return true;
        }


    }
}
