using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightPrePassRenderer.partitioning
{
    public class MeshSceneGraphReceipt
    {
        public BlockBasedSceneGraph graph;
        public Mesh mesh;
        public int ReceivedIndex { get; set; }
    }
}
