using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightSavers.Components
{
    public class GameObject
    {

        public float X;
        public float Y;

        public abstract RectangleF getBoundRect();

        public abstract void Draw();

        public abstract void Update();





    }
}
