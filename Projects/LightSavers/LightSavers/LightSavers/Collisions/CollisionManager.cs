using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Collisions
{
    public class CollisionManager
    {

        private List<CollisionManagerBlock> blocks;

        public CollisionManager()
        {
            blocks = new List<CollisionManagerBlock>();
        }

        public class CollisionManagerBlock
        {
            Color[] blockGrid;
        }

        public class Receipt
        {

        }
    }
}
