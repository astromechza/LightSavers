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
        /***********************************/

        private String loading_msg = "Loading Assets";
        private int loading_bar_length = 0;
        private int loaded_assets = 0;
        private const int loaded_assets_count = 1;            // !! increase this 

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

            Thread t = new Thread(new ThreadStart(LoadAssets));
            t.Start();
        }

        // Asset loading thread
        // Each asset updates the message and increments the load count
        private void LoadAssets()
        {
            // assets
            loading_msg = "Loading Cube.fbx";
            System.Diagnostics.Debug.WriteLine(loading_msg);
            cube_mdl = Globals.content.Load<Model>("Cube");
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

            int w = (int)(spriteFont.MeasureString(loading_msg).X / 2);
            spriteBatch.DrawString(spriteFont, loading_msg, new Vector2(viewport.Width / 2 - w, 575), Color.White);

            spriteBatch.Draw(white_tex, new Rectangle(0, 600, loading_bar_length, 2), Color.White); 

            spriteBatch.End();
        }

        // Update the size of the loading bar
        public override void Update(GameTime gameTime)
        {
            loading_bar_length = (int)((loaded_assets / (float)loaded_assets_count) * 1024);
            base.Update(gameTime);
        }

        
    }
}
