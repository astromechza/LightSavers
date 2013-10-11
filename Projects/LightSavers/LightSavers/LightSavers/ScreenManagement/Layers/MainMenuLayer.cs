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
    public class MainMenuLayer : ScreenLayer
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


            int tx = (viewport.Width - 800) / 2;
            titleRect = new Rectangle(tx, 100, 800, 230);

        }

        private void ConstructSubMenus()
        {
            submenus = new List<Submenu>();

            Submenu s1 = new Submenu();

            s1.AddItem(new TransitionItem("New Game", 1));
            s1.AddItem(new DummyItem("Controls"));
            s1.AddItem(new DummyItem("About"));
            s1.AddItem(new TransitionItem("Exit", -1));

            submenus.Add(s1);

            Submenu s2 = new Submenu();
            s2.AddItem(new ToggleItem("Players", new String[] { "1", "2" }));
            s2.AddItem(new ToggleItem("Level Length", new String[] { "Short", "Medium", "Tiring" }));
            s2.AddItem(new ToggleItem("Difficulty", new String[] { "Easy", "Medium", "Hard" }));
            s2.AddItem(new ToggleItem("Music", new String[] { "On", "Off"}));
            s2.AddItem(new TransitionItem("Back", 0));

            submenus.Add(s2);           

            currentSubMenuIndex = 0;

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

            transitionColour = new Color(0, 0, 0);
        }

        #region == drawing submethods ==


        private void Draw2DLayers()
        {
            
            menuBackground.Draw(canvas);

            canvas.Draw(AssetLoader.title2, titleRect, Color.White);

            submenus[currentSubMenuIndex].Draw(canvas, 60, 400);
            
            if (state == ScreenState.TransitioningOff || state == ScreenState.TransitioningOn)
            {
                int trans = (int)((1 - transitionPercent) * 255.0f);
                transitionColour = new Color(trans, trans, trans, trans);
                canvas.Draw(AssetLoader.tex_black, viewport.Bounds, transitionColour);
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
                //Globals.audioManager.PlayMenuSound("menu_select");
                Globals.audioManager.StopMusic();
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
