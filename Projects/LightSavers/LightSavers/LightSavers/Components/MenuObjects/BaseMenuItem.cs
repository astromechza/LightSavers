using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.MenuObjects
{
    public abstract class BaseMenuItem
    {
        public abstract void Draw(SpriteBatch canvas, int x, int y, bool selected);
    }
}
