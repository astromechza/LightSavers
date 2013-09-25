using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightPrePassRenderer.partitioning
{
    public class MeshSceneGraphReceipt
    {
        public Mesh mesh;
        public ICollection<Mesh> parentlist;
        public Matrix oldGlobalTransform;
    }
}
