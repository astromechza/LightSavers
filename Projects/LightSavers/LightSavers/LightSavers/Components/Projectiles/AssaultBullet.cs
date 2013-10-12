using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Projectiles
{
    public class AssaultBullet : BaseBullet
    {
        public AssaultBullet()
        {
            this.mesh = new Mesh();
            this.mesh.Model = AssetLoader.mdl_shotgunBullet;
            this.mesh.SetInstancingEnabled(true);
            this.mesh.SetCastShadows(false);
            this.speed = 0.4f;
        }

        public override int GetDamage()
        {
            return 15;
        }
    }
}
