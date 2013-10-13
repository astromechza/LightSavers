using LightSavers.Components.Projectiles;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Guns
{
    public class SniperRifle : BaseGun
    {

        public SniperRifle() : base()
        {
            SetModel(AssetLoader.mdl_sniper_rifle);
            SetHoldTransform(Matrix.Identity);
            SetEmmitterVector(new Vector3(-0.7f, 0, 0));
            this.coolDown = new TimeSpan(0, 0, 0, 0, 1000);
            this.animationCoolDown = new TimeSpan(0, 0, 0, 0, 500);
        }

        public override void Fire(float rotation)
        {
            Globals.audioManager.PlayGameSound("sniper");

            SniperBullet b = Globals.gameInstance.projectileManager.sniperBulletPool.Provide();
            b.Construct(emmitterPosition, rotation);

            base.Fire(rotation);
        }
    }
}
