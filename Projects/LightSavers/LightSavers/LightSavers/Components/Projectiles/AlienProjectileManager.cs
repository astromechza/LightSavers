using LightSavers.Components.GameObjects;
using LightSavers.Utils.Geometry;
using ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Projectiles
{
    public class AlienProjectileManager
    {
        public GObjectPool<AlienBullet> alienProjectilePool;

        public AlienProjectileManager()
        {
            alienProjectilePool = new GObjectPool<AlienBullet>(40, 30);
        }

        /// <summary>
        /// Update all of the bullets held in the list. 
        /// Add the ones that are still alive to a new list. 
        /// </summary>
        /// <param name="ms">Milliseconds passed since last update</param>
        public void Update(float ms)
        {
            int i = alienProjectilePool.GetFirst();
            while (i != -1)
            {
                AlienBullet b = alienProjectilePool.GetByIndex(i);
                b.Update(ms);
                if (b.MustBeDeleted())
                {
                    alienProjectilePool.Dispose(b);
                }
                i = alienProjectilePool.NextIndex(b);
            }
        }

        public IProjectile CheckHit(PlayerObject player)
        {
            int i = alienProjectilePool.GetFirst();
            while (i != -1)
            {
                AlienBullet b = alienProjectilePool.GetByIndex(i);
                if (Collider.Collide(player.collisionRectangle, b.GetCenter())) return b;
                i = alienProjectilePool.NextIndex(b);
            }

            return null;
        }
    }
}
