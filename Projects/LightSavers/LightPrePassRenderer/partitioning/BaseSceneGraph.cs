using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightPrePassRenderer.partitioning
{
    public abstract class BaseSceneGraph
    {
        public delegate void AddSubMeshDelegate(Mesh.SubMesh subMesh);
        public delegate void AddLightDelegate(Light light);

        public abstract void SetSubMeshDelegate(BaseSceneGraph.AddSubMeshDelegate subMeshD);
        public abstract void SetLightDelegate(BaseSceneGraph.AddLightDelegate lightD);

        public abstract void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh>[] visibleSubMeshes);
        public abstract void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes);

        public abstract void GetShadowCasters(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes);
        
        public abstract void GetVisibleLights(BoundingFrustum frustum, List<Light> visibleLights);
    }
}
