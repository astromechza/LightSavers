using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Guns
{
    class Sword : BaseGun
    {
        public Sword() : base()
        {
            SetModel(AssetLoader.mdl_sword);
            SetHoldTransform(Matrix.Identity);
        }
    }
}
