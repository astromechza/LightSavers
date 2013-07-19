using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightSavers.ScreenManagement.Layers
{
    public class LoadingLayer : ScreenLayer
    {
        private SpriteBatch spriteBatch;
        private Viewport viewport;
        private Texture2D tex;
        private int gifProgress = 0;

        public LoadingLayer()
            : base()
        {
            spriteBatch = new SpriteBatch(Globals.graphics.GraphicsDevice);
            viewport = Globals.graphics.GraphicsDevice.Viewport;
            tex = new Texture2D(Globals.graphics.GraphicsDevice, 1, 1);
            tex.SetData(new Color[] { Color.Black });

            this.transitionOnTime = TimeSpan.FromSeconds(0.1);
            this.transitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(tex, viewport.Bounds, new Color(transitionPercent, transitionPercent, transitionPercent, transitionPercent));

            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            gifProgress += 1;
            if (gifProgress == 100) gifProgress = 0;

            base.Update(gameTime);
        }


    }
}
