using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightSavers.ScreenManagement
{
    public abstract class ScreenLayer
    {
        // Is this screen layer transparent.. should the layer below it be drawn first
        public bool isTransparent = false;

        public abstract void Draw(GameTime gameTime);
    }
}
