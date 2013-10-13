using LightSavers.Components.Projectiles;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Guns
{
    public class AssaultRifle : BaseGun
    {

        float accuracy = 0.1f;
        float halfAccuracy = 0.05f;

        public AssaultRifle() : base()
        {
            SetModel(AssetLoader.mdl_assault_rifle);
            SetHoldTransform(Matrix.Identity);
            SetEmmitterVector(new Vector3(-0.9f, 0, 0));
            this.coolDown = new TimeSpan(0, 0, 0, 0, 100);
            this.animationCoolDown = new TimeSpan(0, 0, 0, 0, 100);
        }

        public void SetAccuracy(float theta)
        {
            accuracy = theta;
            halfAccuracy = theta / 2;
        }

        public override void Fire(float rotation)
        {
            Globals.audioManager.PlayGameSound("assault");

            float r = (float)Globals.random.NextDouble() * accuracy;

            AssaultBullet b = Globals.gameInstance.projectileManager.assaultBulletPool.Provide();
            b.Construct(emmitterPosition, rotation + r - halfAccuracy);

            base.Fire(rotation);
        }
    }
}
