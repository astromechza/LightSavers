using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightSavers.Utils;


namespace LightSavers.Components.Guns
{
    class Sword : BaseGun
    {
        public Sword() : base()
        {
            SetModel(AssetLoader.mdl_sword);
            SetHoldTransform(Matrix.Identity);
            this.coolDown = new TimeSpan(0, 0, 0, 0, 1000);
            this.animationCoolDown = new TimeSpan(0, 0, 0, 0, 1000);
        }

        public override void Fire(float rotation)
        {
            Globals.audioManager.PlayInstaceOf("sword", 0.3f);
            base.Fire(rotation);
        }
    }
}
