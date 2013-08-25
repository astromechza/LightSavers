using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LightSavers.Components.Shader;

namespace LightSavers.Components
{
    public abstract class GameObject
    {
        public float X;
        public float Y;
        public float Z;

        public abstract RectangleF GetBoundRect();

        public abstract void Draw(float millis, TestShader shader);

        public abstract void Update(float millis);





    }
}
