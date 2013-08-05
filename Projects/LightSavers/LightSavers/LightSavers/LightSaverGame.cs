using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using LightSavers.ScreenManagement;
using LightSavers.ScreenManagement.Layers.Menus;
using LightSavers.Utils;

namespace LightSavers
{
    public class LightSaverGame : Microsoft.Xna.Framework.Game
    {
        FPScounter fps = new FPScounter();

        public LightSaverGame()
        {
            // Initialise Globals and Singleton references
            Globals.mainGame = this;
            Globals.graphics = new GraphicsDeviceManager(this);
            Globals.screenManager = new ScreenManager();
            Globals.content = Content;
            Globals.content.RootDirectory = "Content";

            //anti alias
            //Globals.graphics.PreferMultiSampling = true;

            //uncap FPS
            Globals.graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            // Graphics 
            Globals.graphics.PreferredBackBufferWidth = 1024;
            Globals.graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            AssetLoader loader = new AssetLoader();
            loader.fadeInCompleteCallback = loader.Start;
            loader.fadeOutCompleteCallback = DisplayMainMenu;
            Globals.screenManager.Push(loader);
            
        }

        public bool DisplayMainMenu()
        {
            // remove the loading layer since its not needed
            Globals.screenManager.Pop();
            // add main menu screen
            Globals.screenManager.Push(new MainMenu());            
            return true;
        }


        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            fps.updateTick(gameTime);
            // FIXME: Emergency escape for XBOX. remove before deployment
            if (Keyboard.GetState().IsKeyDown(Keys.A) && !Globals.screenManager.IsEmpty())
            {
                Globals.screenManager.GetTop().StartTransitionOff();
            }

            Globals.screenManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            fps.frameTick();
            Globals.graphics.GraphicsDevice.Clear(Color.Black);

            Globals.screenManager.Draw(gameTime);



            base.Draw(gameTime);
        }
    }
}
