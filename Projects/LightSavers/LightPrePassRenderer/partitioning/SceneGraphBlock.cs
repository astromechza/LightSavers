using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightPrePassRenderer.partitioning
{
    public class SceneGraphBlock
    {
        private List<Mesh> meshes;
        private List<Light> lights;
        private int blockIndex;

        public SceneGraphBlock(int i)
        {
            blockIndex = i;
            meshes = new List<Mesh>(100);
            lights = new List<Light>(20);
        }

        public void Add(Mesh m)
        {
            meshes.Add(m);
        }

        public void Add(Light l)
        {
            lights.Add(l);
        }

        public void Remove(Mesh mesh)
        {
            meshes.Remove(mesh);
        }

        public void Remove(Light light)
        {
            lights.Remove(light);
        }

        public void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh>[] visibleSubMeshes)
        {
            for (int index = 0; index < meshes.Count; index++)
            {
                Mesh m = meshes[index];
                if (frustum.Intersects(m.GlobalBoundingBox))
                {
                    for (int si = 0; si < m.SubMeshes.Count; si++)
                    {
                        Mesh.SubMesh sm = m.SubMeshes[si];
                        if (sm.Enabled) visibleSubMeshes[(int)sm.RenderQueue].Add(sm);
                    }
                }
            }
        }

        public void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes)
        {
            for (int index = 0; index < meshes.Count; index++)
            {
                Mesh m = meshes[index];
                if (frustum.Intersects(m.GlobalBoundingBox))
                {
                    for (int si = 0; si < m.SubMeshes.Count; si++)
                    {
                        Mesh.SubMesh sm = m.SubMeshes[si];
                        if (sm.Enabled) visibleSubMeshes.Add(sm);
                    }
                }
            }
        }

        public void GetVisibleLights(BoundingFrustum frustum, List<Light> visibleLights)
        {
            for (int index = 0; index < lights.Count; index++)
            {
                Light l = lights[index];
                if (frustum.Intersects(l.BoundingSphere))
                {
                    visibleLights.Add(l);
                }
            }
        }

        public void GetShadowCasters(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes)
        {
            for (int index = 0; index < meshes.Count; index++)
            {
                Mesh m = meshes[index];
                if (m.GetCastShadows() && frustum.Intersects(m.GlobalBoundingBox))
                {
                    for (int si = 0; si < m.SubMeshes.Count; si++)
                    {
                        Mesh.SubMesh sm = m.SubMeshes[si];
                        if (sm.Enabled && sm.CastShadows) visibleSubMeshes.Add(sm);
                    }
                }
            }
        }

    }
}
