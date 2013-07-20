using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using LightSavers.ScreenManagement;
using System.Threading;
using Microsoft.Xna.Framework;

namespace LightSavers
{
    /** AssetLoader layer ***
     * loads larger assets at the beginning of the game with nice fade in / out and progress bar.
     * To add a new asset, add a static variable for it and add it to the LoadAssets method. 
     ************************/ 
    public class AssetLoader : ScreenLayer
    {
        /************* ASSETS **************/
        public static Model cube_mdl;
        public static Texture2D black_tex;
        public static Texture2D white_tex;
        public static Texture2D sand_tex;
        /***********************************/

        private String loading_msg = "Loading Assets";
        private int loading_bar_length = 0;
        private int loaded_assets = 0;
        private int num_assets = -1;            

        private Viewport viewport;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        public AssetLoader() : base()
        {
            spriteFont = Globals.content.Load<SpriteFont>("LoadingFont");
            spriteBatch = new SpriteBatch(Globals.graphics.GraphicsDevice);
            viewport = Globals.graphics.GraphicsDevice.Viewport;

            black_tex = new Texture2D(Globals.graphics.GraphicsDevice, 1, 1);
            black_tex.SetData(new Color[] { Color.Black });

            white_tex = new Texture2D(Globals.graphics.GraphicsDevice, 1, 1);
            white_tex.SetData(new Color[] { Color.White });

            // Fade times
            transitionOnTime = TimeSpan.FromSeconds(0.6);
            transitionOffTime = TimeSpan.FromSeconds(0.5);           
        }

        public bool Start()
        {
            Thread t = new Thread(new ThreadStart(LoadAssets));
            t.Start();
            return true;
        }

        // Asset loading thread
        // Each asset updates the message and increments the load count
        private void LoadAssets()
        {
            // number of assets to be loaded. (used to compute progress bar size)
            num_assets = 2;

            // assets
            loading_msg = "Loading Cube.fbx";
            cube_mdl = Globals.content.Load<Model>("Cube");
            loaded_assets += 1;

            loading_msg = "Loading sand.jpg";
            sand_tex = Globals.content.Load<Texture2D>("sand");
            loaded_assets += 1;
            
            // once its loaded, fade out
            StartTransitionOff();
        }

        // Draw the layer
        // A black background + text + progress bar
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(black_tex, viewport.Bounds, new Color(transitionPercent, transitionPercent, transitionPercent, transitionPercent));

            spriteBatch.DrawString(spriteFont, loading_msg, new Vector2((viewport.Width - spriteFont.MeasureString(loading_msg).X) / 2, 575), Color.White);

            spriteBatch.Draw(white_tex, new Rectangle(0, 600, loading_bar_length, 2), Color.White); 

            spriteBatch.End();
        }

        // Update the size of the loading bar
        public override void Update(GameTime gameTime)
        {
            loading_bar_length = (int)((loaded_assets / (float)num_assets) * 1024);
            base.Update(gameTime);
        }

        
    }
}
