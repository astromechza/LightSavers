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
using LightSavers.Components;

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
            Globals.inputController = new InputManager();
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
            Globals.screenManager.Push(new AssetLoader());
            Globals.viewport = Globals.graphics.GraphicsDevice.Viewport;
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            fps.updateTick(gameTime);
            
            // update input controller
            Globals.inputController.Update();

            // update the screen manager
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
