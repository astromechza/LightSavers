using LightSavers.Components.CampainManager;
using LightSavers.Utils;
using LightSavers.Utils.Geometry;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects.Aliens
{
    public class AlienSpawner2<AlienType> where AlienType : BaseAlien, new()
    {
        // spawn N of the given alien in the given section
        public static void Spawn(int n, CampaignSection s)
        {
            Vector3 r;
            RectangleF rr;
            while (n > 0)
            {
                r = new Vector3(
                  (float)Globals.random.NextDouble() * 30 + s.Index * 32 + 1,
                  0,
                  (float)Globals.random.NextDouble() * 30 + 1
               );
                rr = new RectangleF(r.X - 0.5f, r.Z - 0.5f, 1.0f, 1.0f);
                // check collision
                if (Globals.gameInstance.cellCollider.RectangleCollides(rr)) continue;

                AlienType a = new AlienType();
                a.Construct(r, (float)Globals.random.NextDouble() * MathHelper.TwoPi, s);
                s.AddAlienToPopulation(a);

                n--;
            }



        }
    }
}
