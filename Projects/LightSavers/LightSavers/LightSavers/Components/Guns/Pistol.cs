﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Guns
{
    public class Pistol : BaseGun
    {
        public Pistol() : base()
        {
            SetModel(AssetLoader.mdl_pistol);
            SetHoldTransform(Matrix.CreateScale(0.5f) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90)) * Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(-0.3f, 0, 0));
            SetEmmitterVector(new Vector3(0.1f, 0.2f, -0.3f));
        }
    }
}