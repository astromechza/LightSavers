using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LightSavers.Utils;
using LightSavers.Components.MenuObjects;
using LightSavers.Utils.Geometry;

namespace LightSavers.ScreenManagement.Layers
{
    class AboutScreenLayer : ScreenLayer
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
        public AboutScreenLayer() : base()
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

            Draw2DLayers();

            Color talpha = new Color(1.0f, 1.0f, 1.0f, 0.5f);

            canvas.Draw(AssetLoader.tex_black, viewport.Bounds, talpha);

            canvas.Draw(AssetLoader.about, new Rectangle(viewport.Bounds.Width / 2 - AssetLoader.about.Width / 2, 50, AssetLoader.about.Width, AssetLoader.about.Height), Color.White);
            canvas.DrawString(AssetLoader.fnt_paragraph, "Light Savers was developed by Daniel Burnham-King,\nBenjamin Meier and Siobhan O'Donovan for their final\n3D Distributed Games Development project. The models\nand animations were supplied by City Varsity\nanimators, Altus Barry and Jason Burrows\n", new Vector2(50, 150), Color.White);
            canvas.Draw(AssetLoader.group, new Rectangle(viewport.Bounds.Width - AssetLoader.group.Width - 50, 150, AssetLoader.group.Width, AssetLoader.group.Height), Color.White);
            
            //drawing prompt to go back
            canvas.Draw(AssetLoader.diamond, new Rectangle(50, viewport.Bounds.Height -100 + 6, 40, 15), Color.White);
            canvas.DrawString(AssetLoader.fnt_assetloadscreen, "Back", new Vector2(110, viewport.Bounds.Height - 100), Color.White);
            canvas.End();
        }

        private void Draw2DLayers()
        {

            menuBackground.Draw(canvas);

            if (state == ScreenState.TransitioningOff || state == ScreenState.TransitioningOn)
            {
                int trans = (int)((1 - transitionPercent) * 255.0f);
                transitionColour = new Color(trans, trans, trans, trans);
                canvas.Draw(AssetLoader.tex_black, viewport.Bounds, transitionColour);
            }
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
            return true;
        }
        #endregion
    }
}
