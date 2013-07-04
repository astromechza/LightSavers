using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LightSavers.ScreenManagement
{
    public class TextLayer : ScreenLayer
    {

        private String text;
        private SpriteBatch spriteBatch;
        private Viewport viewport;
        private Texture2D tex;
        private SpriteFont spriteFont;

        public TextLayer(String text) : base()
        {
            this.text = text;

            isTransparent = false;
            spriteBatch = new SpriteBatch(Globals.graphics.GraphicsDevice);
            viewport = Globals.graphics.GraphicsDevice.Viewport;
            tex = new Texture2D(Globals.graphics.GraphicsDevice, 1, 1);
            tex.SetData(new Color[] { Color.White });
            spriteFont = Globals.content.Load<SpriteFont>("SpriteFont1");

            this.transitionOnTime = TimeSpan.FromSeconds(0.6);
            this.transitionOffTime = TimeSpan.FromSeconds(0.5);
            
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {

            spriteBatch.Begin();

            spriteBatch.Draw(tex, viewport.Bounds, new Color(transitionPercent, transitionPercent, transitionPercent, transitionPercent));

            spriteBatch.DrawString(spriteFont, text, new Vector2(viewport.Width / 2, viewport.Height / 2), Color.Black);

            spriteBatch.End();

        }


    }
}
