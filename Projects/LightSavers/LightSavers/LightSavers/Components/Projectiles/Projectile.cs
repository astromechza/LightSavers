using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Projectiles
{
    public interface IProjectile
    {
        void Update(float ms);
        bool MustBeDeleted();
    }
}
