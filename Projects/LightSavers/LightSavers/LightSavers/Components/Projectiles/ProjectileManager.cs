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


        public GObjectPool<PistolBullet> pistolBulletPool;
        public GObjectPool<ShotgunBullet> shotgunBulletPool;
        public GObjectPool<AssaultBullet> assaultBulletPool;

        public ProjectileManager()
        {
            pistolBulletPool = new GObjectPool<PistolBullet>(100, 10);
            shotgunBulletPool = new GObjectPool<ShotgunBullet>(100, 50);
            assaultBulletPool = new GObjectPool<AssaultBullet>(500, 100);
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

            i = assaultBulletPool.GetFirst();
            while (i != -1)
            {
                AssaultBullet b = assaultBulletPool.GetByIndex(i);
                b.Update(ms);
                if (b.MustBeDeleted())
                {
                    assaultBulletPool.Dispose(b);
                }
                i = assaultBulletPool.NextIndex(b);
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

            i = assaultBulletPool.GetFirst();
            while (i != -1)
            {
                AssaultBullet b = assaultBulletPool.GetByIndex(i);
                if (Collider.Collide(alien.GetBoundRect(), b.GetCenter())) return b;
                i = assaultBulletPool.NextIndex(b);
            }
            return null;
        }
    }
}
