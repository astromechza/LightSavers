using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Guns
{
    public class SniperRifle : BaseGun
    {

        public SniperRifle()
            : base()
        {
            SetModel(AssetLoader.mdl_sniper_rifle);
            SetHoldTransform(Matrix.Identity);
            SetEmmitterVector(new Vector3(-0.7f, 0, 0));
        }
    }
}
