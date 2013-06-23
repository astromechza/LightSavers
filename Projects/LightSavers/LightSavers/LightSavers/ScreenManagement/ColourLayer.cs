using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightSavers.ScreenManagement
{
    public class ColourLayer : ScreenLayer
    {
        private Color background;

        public ColourLayer(Color bg)
        {
            background = bg;
            isTransparent = false;
        }

        public override void Draw(GameTime gameTime)
        {
            Globals.graphics.GraphicsDevice.Clear(background);
        }
    }
}
