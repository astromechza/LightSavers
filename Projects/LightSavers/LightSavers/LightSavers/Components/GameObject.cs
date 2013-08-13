﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightSavers.Components
{
    public abstract class GameObject
    {

        public float X;
        public float Y;

        public abstract RectangleF GetBoundRect();

        public abstract void Draw();

        public abstract void Update();





    }
}