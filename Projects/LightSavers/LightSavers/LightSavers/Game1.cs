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
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        public Game1()
        {
            // Initialise Globals and Singleton references
            Globals.mainGame = this;
            Globals.graphics = new GraphicsDeviceManager(this);
            Globals.screenManager = new ScreenManager();

            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Test
            Globals.screenManager.push(new ColourLayer(Color.Red));
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            // FIXME: Emergency escape for XBOX. remove before deployment
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) this.Exit();

            //Globals.screenManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {     
            Globals.screenManager.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
