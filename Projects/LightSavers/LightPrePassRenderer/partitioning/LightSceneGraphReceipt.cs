using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightPrePassRenderer.partitioning
{
    public class LightSceneGraphReceipt
    {
        public Light light;
        public ICollection<Light> parentlist;
        public Matrix oldGlobalTransform;
    }
}
