using LightSavers.Components.GameObjects.Aliens;
using LightSavers.Utils.Geometry;
using Microsoft.Xna.Framework;
using ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.HitParticle
{
    /// <summary>
    /// A basic projectile container. All it does is hold a list of projectils and allow for Updating / Adding
    /// The update method could be made more effifient somehow. As it is tons of bullets 
    /// causes a huge heap movement and garbage collection.
    /// </summary>
    public class DropFragmentManager
    {

        //maximum number of projectiles expected live at any point in time
        private const int MAX_PROJECTILES = 100;
        private const int PRE_BUILD = 100;

        public GObjectPool<DropFragment> fragments;

        private RealGame game;

        public DropFragmentManager(RealGame game)
        {
            this.game = game;
            this.fragments = new GObjectPool<DropFragment>(MAX_PROJECTILES, PRE_BUILD);
        }

        /// <summary>
        /// Update all of the bullets held in the list. 
        /// Add the ones that are still alive to a new list. 
        /// </summary>
        /// <param name="ms">Milliseconds passed since last update</param>
        public void Update(float ms)
        {
            int i = fragments.GetFirst();
            while (i != -1)
            {
                DropFragment b = this.fragments.GetByIndex(i);
                b.Update(ms);
                if (b.mustBeDeleted)
                {
                    this.fragments.Dispose(b);
                }
                i = this.fragments.NextIndex(b);
            }
        }

        public void SpawnX(Vector3 o, int n)
        {
            if (!this.fragments.HasAvailable()) this.fragments.Dispose(this.fragments.GetFirst());
            
            DropFragment b = this.fragments.Provide();
            b.Construct(game, o);
        }
    }
}
