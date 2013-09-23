using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightPrePassRenderer.partitioning
{
    public abstract class BaseLightAndMeshContainer
    {
        public delegate void AddSubMeshDelegate(Mesh.SubMesh subMesh);
        public delegate void AddLightDelegate(Light light);

        public abstract void SetSubMeshDelegate(BaseLightAndMeshContainer.AddSubMeshDelegate subMeshD);
        public abstract void SetLightDelegate(BaseLightAndMeshContainer.AddLightDelegate lightD);

        public abstract void AddMesh(Mesh mesh);
        public abstract void AddSubMesh(Mesh.SubMesh subMesh);
        public abstract void RemoveSubMesh(Mesh.SubMesh subMesh);

        public abstract void AddLight(Light light);
        public abstract void RemoveLight(Light light);

        public abstract void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh>[] visibleSubMeshes);
        public abstract void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes);

        public abstract void GetShadowCasters(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes);
        public abstract void GetShadowCasters(BoundingFrustum frustum, Plane[] additionalPlanes, List<Mesh.SubMesh> visibleSubMeshes);

        public abstract void GetVisibleLights(BoundingFrustum frustum, List<Light> visibleLights);
    }
}
