using LightSavers.Components.Projectiles;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Guns
{
    public class Shotgun : BaseGun
    {

        float accuracy = 0.3f;
        float halfAccuracy = 0.15f;

        public Shotgun() : base()
        {
            SetModel(AssetLoader.mdl_shotgun);
            SetHoldTransform(Matrix.Identity);
            SetEmmitterVector(new Vector3(-0.5f, 0, 0));
            this.coolDown = new TimeSpan(0, 0, 0, 0, 800);
            this.animationCoolDown = new TimeSpan(0, 0, 0, 0, 300);
        }

        public void SetAccuracy(float theta)
        {
            accuracy = theta;
            halfAccuracy = theta / 2;
        }

        public override void Fire(float rotation)
        {
            float r;
            ShotgunBullet b;
            for (int i = 0; i < 8; i++)
            {
                r = (float)Globals.random.NextDouble() * accuracy;
                b = Globals.gameInstance.projectileManager.shotgunBulletPool.Provide();
                b.Construct(emmitterPosition, rotation + r - halfAccuracy);
            }

            base.Fire(rotation);
        }
    }
}
