using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Guns
{
    public class AssaultRifle : BaseGun
    {
        public AssaultRifle() : base()
        {
            SetModel(AssetLoader.mdl_assault_rifle);
            SetHoldTransform(Matrix.Identity);
        }
    }
}
