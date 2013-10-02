using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightSavers.Components
{
    public abstract class GameObject
    {
        protected Vector3 position;
        public Vector3 Position { get { return position; } }

        public abstract RectangleF GetBoundRect();

        public abstract void Update(float millis);





    }
}
