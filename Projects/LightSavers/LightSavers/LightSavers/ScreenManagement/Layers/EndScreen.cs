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
    class EndScreen : ScreenLayer
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

        public String message;

        bool hasWon;

        /// <summary>
        /// The constructor for the Main Menu.
        /// </summary>
        public EndScreen(String message, bool hasWon) : base()
        {
            currentSubMenuIndex = 0;
            this.message = message;
            SetLayerAttributes();
            this.hasWon = hasWon;

            isTransparent = true;

            menuBackground = new MenuBackground();
            ConstructSubMenus();

            ConstructDrawingObjects();
        }

        #region == constructor submethods ==

        private void SetLayerAttributes()
        {
            //isTransparent = false;
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
            titleRect = new Rectangle(tx, 60, 800, 209);
        }

        private void ConstructSubMenus()
        {
            submenus = new List<Submenu>();

            Submenu s2 = new Submenu();
            s2.AddItem(new DelegateItem("Yes", restartEverything, Color.White, Color.Gray));
            s2.AddItem(new DelegateItem("No, exit", No, Color.White, Color.Gray));

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

            Color talpha = new Color(1.0f, 1.0f, 1.0f, 0.8f);

            canvas.Draw(AssetLoader.tex_black, viewport.Bounds, talpha);

            Draw2DLayers();
            //Vector2 fontWidth = AssetLoader.fnt_paragraph


            canvas.DrawString(AssetLoader.fnt_paragraph, "Would you like to go back to Main Menu?", new Vector2((viewport.Bounds.Width - 463) / 2 + 30, (viewport.Bounds.Height - 214) / 2 + 60), Color.White);


            // finish drawing
            canvas.End();
        }

        private void Draw2DLayers()
        {
            if(hasWon ==true)
            {
                canvas.Draw(AssetLoader.won, titleRect, Color.White);
            }
            else
            {
                canvas.Draw(AssetLoader.lost, titleRect, Color.White);
            }

            submenus[currentSubMenuIndex].Draw(canvas, (viewport.Width - 30) / 2, 370);

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
            if (Globals.inputController.isButtonPressed(Buttons.B, null))
            {
                this.fadeOutCompleteCallback = backToMain;
                this.StartTransitionOff();
            }

            //select (enter)
            else if (Globals.inputController.isButtonPressed(Buttons.A, null))
            {
                if (submenus[currentSubMenuIndex].items[submenus[currentSubMenuIndex].selected] is TransitionItem)
                {
                    TransitionItem current = (TransitionItem)submenus[currentSubMenuIndex].items[submenus[currentSubMenuIndex].selected];
                    int destination = current.destination;
                    if (destination == -1)
                    {
                        this.StartTransitionOff();
                    }

                    else
                    {
                        currentSubMenuIndex = destination;
                    }
                }
                else if (submenus[currentSubMenuIndex].items[submenus[currentSubMenuIndex].selected] is DelegateItem)
                {
                    DelegateItem current = (DelegateItem)submenus[currentSubMenuIndex].items[submenus[currentSubMenuIndex].selected];

                    this.fadeOutCompleteCallback = current.function;
                    this.StartTransitionOff();
                }
            }

            //GOING UP IN MENUS
            else if (Globals.inputController.isButtonPressed(Buttons.DPadUp, null) || Globals.inputController.isButtonPressed(Buttons.LeftThumbstickUp, null))
            {
                int selectedIndex = submenus[currentSubMenuIndex].selected;
                int size = submenus[currentSubMenuIndex].items.Count();
                int newIndex = selectedIndex - 1;

                if (newIndex < 0)
                {
                    newIndex = size - 1;
                }

                submenus[currentSubMenuIndex].selected = newIndex;
            }

            //GOING DOWN IN MENUS
            else if (Globals.inputController.isButtonPressed(Buttons.DPadDown, null) || Globals.inputController.isButtonPressed(Buttons.LeftThumbstickDown, null))
            {
                int selectedIndex = submenus[currentSubMenuIndex].selected;
                int size = submenus[currentSubMenuIndex].items.Count();
                int newIndex = ++selectedIndex;

                if (newIndex == size)
                {
                    newIndex = 0;
                }

                submenus[currentSubMenuIndex].selected = newIndex;
            }

        }
        #endregion
        public bool backToMain()
        {
            Globals.screenManager.Pop();
            return true;
        }

        public bool No()
        {
            while (Globals.screenManager.layers.Count > 0)
            {
                Globals.screenManager.Pop();
            }
            return true;
        }

        public bool restartEverything()
        {
            while (Globals.screenManager.layers.Count > 0)
            {
                Globals.screenManager.Pop();
            }
            Globals.screenManager.Push(new MainMenuLayer(false));
            //MainMenuLayer current = (MainMenuLayer)Globals.screenManager.layers[0];
            //current.currentSubMenuIndex = 1;
            return true;
        }
    }
}
