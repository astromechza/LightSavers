using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Projectiles
{
    /// <summary>
    /// A basic projectile container. All it does is hold a list of projectils and allow for Updating / Adding
    /// The update method could be made more effifient somehow. As it is tons of bullets 
    /// causes a huge heap movement and garbage collection.
    /// </summary>
    public class ProjectileManager
    {
        //maximum number of projectiles expected live at any point in time
        private const int MAX_PROJECTILES = 1000;

        private List<IProjectile> allProjectiles;

        public ProjectileManager()
        {
            allProjectiles = new List<IProjectile>(MAX_PROJECTILES);
        }

        /// <summary>
        /// Update all of the bullets held in the list. 
        /// Add the ones that are still alive to a new list. 
        /// </summary>
        /// <param name="ms">Milliseconds passed since last update</param>
        public void Update(float ms)
        {
            List<IProjectile> temp = new List<IProjectile>(MAX_PROJECTILES);
            foreach (IProjectile p in allProjectiles)
            {
                p.Update(ms);
                if (!p.MustBeDeleted())
                {
                    temp.Add(p);
                }
            }
            allProjectiles = temp;
        }

        /// <summary>
        /// Add a new Projectile to this container
        /// </summary>
        /// <param name="p">The projectile to add</param>
        public void Add(IProjectile p)
        {
            allProjectiles.Add(p);
        }

    }
}
