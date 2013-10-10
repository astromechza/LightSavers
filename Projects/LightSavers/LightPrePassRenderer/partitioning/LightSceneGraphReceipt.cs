using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightPrePassRenderer.partitioning
{
    public class LightSceneGraphReceipt
    {
        public BlockBasedSceneGraph graph;
        public Light light;
        public int ReceivedIndex { get; set; }
    }
}
