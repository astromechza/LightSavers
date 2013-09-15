using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components
{
    public abstract class AGridContainable
    {
        private GridContainer parentContainer; 

        public abstract BoundingBox GetBoundingBox();

        public void SetParentContainer(GridContainer gridContainer)
        {
            parentContainer = gridContainer;
        }

        public GridContainer GetParentContainer()
        {
            return parentContainer;
        }
    }
}
