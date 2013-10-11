using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Projectiles
{
    public interface IProjectile
    {
        void Update(float ms);
        void PreDestroy();
        void Destroy();
        bool MustBeDeleted();
        Vector3 GetCenter();
        int GetDamage();
    }
}
