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
    class SettingsMenuLayer: ScreenLayer
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
        public SettingsMenuLayer() : base()
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
            titleRect = new Rectangle(tx, 60, 800, 209);

        }

        private void ConstructSubMenus()
        {
            submenus = new List<Submenu>();

            Submenu s2 = new Submenu();
            s2.AddItem(new ToggleItem("Music", new String[] { "On", "Off" }));
            s2.AddItem(new ToggleItem("Volume", new String[] { "Low", "Medium", "High" }));
            s2.AddItem(new DelegateItem("Back", backToMain, Color.White, Color.Gray));

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
            
            Color talpha = new Color(1.0f, 1.0f, 1.0f, 0.5f);

            canvas.Draw(AssetLoader.tex_black, viewport.Bounds, talpha);

            Draw2DLayers();

            // finish drawing
            canvas.End();

            transitionColour = new Color(0, 0, 0);
        }

        #region == drawing submethods ==


        private void Draw2DLayers()
        {
            
            menuBackground.Draw(canvas);

            canvas.Draw(AssetLoader.settings, titleRect, Color.White);

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
            //back button
            if (Globals.inputController.isButtonPressed(Buttons.B, null))
            {
                this.fadeOutCompleteCallback = backToMain;
                this.StartTransitionOff();
            }
            
            //select (enter)
            else if (Globals.inputController.isButtonPressed(Buttons.A, null))
            {
                //Globals.audioManager.PlayMenuSound("menu_select");
                /*
                */
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
                else if(submenus[currentSubMenuIndex].items[submenus[currentSubMenuIndex].selected] is DelegateItem)
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
                int newIndex = selectedIndex-1;

                if (newIndex < 0)
                {
                    newIndex = size-1;
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
            
            //GOING LEFT IN MENUS (TOGGLE)
            else if (Globals.inputController.isButtonPressed(Buttons.DPadLeft, null) || Globals.inputController.isButtonPressed(Buttons.LeftThumbstickLeft, null))
            {
                if (submenus[currentSubMenuIndex].items[submenus[currentSubMenuIndex].selected] is ToggleItem)
                {
                    ToggleItem currentToggle = (ToggleItem)submenus[currentSubMenuIndex].items[submenus[currentSubMenuIndex].selected];

                    int selectedIndex = currentToggle.current;
                    int size = currentToggle.values.Count();
                    int newIndex = selectedIndex - 1;

                    if (newIndex < 0)
                    {
                        newIndex = size - 1;
                    }
                    currentToggle.current = newIndex;

                    if(currentToggle.label.Equals("Music"))
                    {
                        Globals.audioManager.Music = currentToggle.current;
                    }
                    else if (currentToggle.label.Equals("Volume"))
                    {
                        Globals.audioManager.Volume = currentToggle.current;
                    }
                }                
            }

            //GOING RIGHT IN MENUS (TOGGLE)
            else if (Globals.inputController.isButtonPressed(Buttons.DPadRight, null) || Globals.inputController.isButtonPressed(Buttons.LeftThumbstickRight, null))
            {
                if (submenus[currentSubMenuIndex].items[submenus[currentSubMenuIndex].selected] is ToggleItem)
                {
                    ToggleItem currentToggle = (ToggleItem)submenus[currentSubMenuIndex].items[submenus[currentSubMenuIndex].selected];

                    int selectedIndex = currentToggle.current;
                    int size = currentToggle.values.Count();
                    int newIndex = ++selectedIndex;

                    if (newIndex == size)
                    {
                        newIndex = 0;
                    }
                    currentToggle.current = newIndex;

                    if (currentToggle.label.Equals("Music"))
                    {
                        Globals.audioManager.Music = currentToggle.current;
                    }
                    else if (currentToggle.label.Equals("Volume"))
                    {
                        Globals.audioManager.Volume = currentToggle.current;
                    }
                }
            }
        }
        #endregion
        public bool backToMain()
        {
            Globals.screenManager.Pop();
            return true;
        }
    }
}
