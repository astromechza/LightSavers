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
    class IntroStory : ScreenLayer
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

        GameLayer currentGame;

        /// <summary>
        /// The constructor for the Main Menu.
        /// </summary>
        public IntroStory(GameLayer cG) : base()
        {
            SetLayerAttributes();
            currentGame = cG;
            isTransparent = true;

            menuBackground = new MenuBackground();

            ConstructDrawingObjects();
        }

        #region == constructor submethods ==

        private void SetLayerAttributes()
        {
            //sTransparent = false;
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
            titleRect = new Rectangle(tx, 60, 800, 230);
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

            Color talpha = new Color(1.0f, 1.0f, 1.0f, 0.8f);

            canvas.Draw(AssetLoader.tex_black, viewport.Bounds, talpha);

            Draw2DLayers();

            canvas.Draw(AssetLoader.title2, new Rectangle(viewport.Bounds.Width / 2 - AssetLoader.about.Width / 2, 60, AssetLoader.about.Width, AssetLoader.about.Height), Color.White);
            canvas.DrawString(AssetLoader.fnt_paragraph, "Set far in the future, you are in a research\nfacility that has been abandoned for many years\nfor reasons unknown to you.\n\nYou must find your way to the transmatter\nteleporter room where lies your only means of\nescape. Upon entering the facility you find it\nshrouded in darkness...", new Vector2(50, 350), Color.White);
            canvas.Draw(AssetLoader.ship, new Rectangle(viewport.Bounds.Width - AssetLoader.ship.Width - 100, 350, AssetLoader.ship.Width, AssetLoader.ship.Height), Color.White);
            
            //drawing prompt to go back
            canvas.Draw(AssetLoader.diamond, new Rectangle(10, viewport.Bounds.Height -100 + 6, 40, 15), Color.White);
            canvas.DrawString(AssetLoader.fnt_assetloadscreen, "Enter Game", new Vector2(60, viewport.Bounds.Height - 100), Color.White);
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
            if ((Globals.inputController.isButtonPressed(Buttons.B, null)))
            {
                this.fadeOutCompleteCallback = backToMain;
                this.StartTransitionOff();
            }
            else if((Globals.inputController.isButtonPressed(Buttons.A, null)))
            {
                this.fadeOutCompleteCallback = StartGame;
                this.StartTransitionOff();
            }
        }

        public bool backToMain()
        {
            Globals.screenManager.Pop();
            return true;
        }

        public bool StartGame()
        {
            Globals.screenManager.Pop();
            Globals.screenManager.Push(currentGame);
            return true;
        }
        #endregion
    }
}
