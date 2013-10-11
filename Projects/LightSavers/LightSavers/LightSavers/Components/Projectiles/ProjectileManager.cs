using LightSavers.Components.GameObjects.Aliens;
using LightSavers.Utils.Geometry;
using ObjectPool;
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
        private const int PRE_BUILD = 50;

        public GObjectPool<PistolBullet> pistolBulletPool;
        public GObjectPool<ShotgunBullet> shotgunBulletPool;

        public ProjectileManager()
        {
            pistolBulletPool = new GObjectPool<PistolBullet>(MAX_PROJECTILES, PRE_BUILD);
            shotgunBulletPool = new GObjectPool<ShotgunBullet>(MAX_PROJECTILES, PRE_BUILD);
        }

        /// <summary>
        /// Update all of the bullets held in the list. 
        /// Add the ones that are still alive to a new list. 
        /// </summary>
        /// <param name="ms">Milliseconds passed since last update</param>
        public void Update(float ms)
        {
            int i = pistolBulletPool.GetFirst();
            while (i != -1)
            {
                PistolBullet b = pistolBulletPool.GetByIndex(i);
                b.Update(ms);
                if (b.MustBeDeleted())
                {
                    pistolBulletPool.Dispose(b);
                }
                i = pistolBulletPool.NextIndex(b);
            }

            i = shotgunBulletPool.GetFirst();
            while (i != -1)
            {
                ShotgunBullet b = shotgunBulletPool.GetByIndex(i);
                b.Update(ms);
                if (b.MustBeDeleted())
                {
                    shotgunBulletPool.Dispose(b);
                }
                i = shotgunBulletPool.NextIndex(b);
            }
        }

        public IProjectile CheckHit(BaseAlien alien)
        {
            int i = pistolBulletPool.GetFirst();
            while (i != -1)
            {
                PistolBullet b = pistolBulletPool.GetByIndex(i);
                if (Collider.Collide(alien.GetBoundRect(), b.GetCenter())) return b;
                i = pistolBulletPool.NextIndex(b);
            }

            i = shotgunBulletPool.GetFirst();
            while (i != -1)
            {
                ShotgunBullet b = shotgunBulletPool.GetByIndex(i);
                if (Collider.Collide(alien.GetBoundRect(), b.GetCenter())) return b;
                i = shotgunBulletPool.NextIndex(b);
            }
            return null;
        }
    }
}
