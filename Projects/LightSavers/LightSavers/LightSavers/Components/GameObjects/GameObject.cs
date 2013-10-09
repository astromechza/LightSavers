using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightSavers.Components
{
    public abstract class GameObject
    {
        protected Vector3 _position;

        public Vector3 Position { get { return _position; } }
        public abstract void Update(float millis);





    }
}
