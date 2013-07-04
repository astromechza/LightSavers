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

namespace LightSavers
{
    public class LightSaverGame : Microsoft.Xna.Framework.Game
    {

        public LightSaverGame()
        {
            // Initialise Globals and Singleton references
            Globals.mainGame = this;
            Globals.graphics = new GraphicsDeviceManager(this);
            Globals.screenManager = new ScreenManager();
            Globals.content = Content;
            Globals.content.RootDirectory = "Content";

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
            ScreenLayer first = new TextLayer("Press A to fade this screen");
            first.fadeInCompleteCallback = Func1;
            first.fadeOutBeginCallback = Func2;
            first.fadeOutCompleteCallback = Func3;
            Globals.screenManager.Push(first);
        }

        public bool Func1()
        {
            System.Diagnostics.Debug.WriteLine("Func1");
            return true;
        }

        public bool Func2()
        {
            System.Diagnostics.Debug.WriteLine("Func2");
            return true;
        }

        public bool Func3()
        {
            System.Diagnostics.Debug.WriteLine("Func3");
            return true;
        }


        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
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
            Globals.graphics.GraphicsDevice.Clear(Color.Black);

            Globals.screenManager.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
