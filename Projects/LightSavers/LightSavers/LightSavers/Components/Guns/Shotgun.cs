using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Guns
{
    public class Shotgun : BaseGun
    {
        public Shotgun() : base()
        {
            SetModel(AssetLoader.mdl_shotgun);
            SetHoldTransform(Matrix.Identity);
        }
    }
}
