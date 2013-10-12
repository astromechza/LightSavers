using LightPrePassRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Projectiles
{
    public class SniperBullet : BaseBullet
    {
        public SniperBullet()
        {
            this.mesh = new Mesh();
            this.mesh.Model = AssetLoader.mdl_sniper_bullet;
            this.mesh.SetInstancingEnabled(true);
            this.mesh.SetCastShadows(false);
            this.speed = 0.8f;
        }

        public override int GetDamage()
        {
            return 150;
        }
    }
}
