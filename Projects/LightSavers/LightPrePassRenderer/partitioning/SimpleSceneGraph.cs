using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightPrePassRenderer.partitioning
{
    public class SimpleSceneGraph : BaseSceneGraph
    {
        #region Properties
        private BaseSceneGraph.AddSubMeshDelegate addSubMeshFunc;
        private BaseSceneGraph.AddLightDelegate addLightFunc;

        private List<Mesh.SubMesh> _worldSubMeshes = new List<Mesh.SubMesh>(100);
        private List<Light> _worldLights = new List<Light>(20);
        #endregion

        public SimpleSceneGraph() { }

        public override void SetSubMeshDelegate(BaseSceneGraph.AddSubMeshDelegate subMeshD)
        {
            addSubMeshFunc = subMeshD;
        }

        public override void SetLightDelegate(BaseSceneGraph.AddLightDelegate lightD)
        {
            addLightFunc = lightD;
        }

        public void AddMesh(Mesh mesh)
        {
            for (int index = 0; index < mesh.SubMeshes.Count; index++)
            {
                Mesh.SubMesh subMesh = mesh.SubMeshes[index];
                AddSubMesh(subMesh);
            }            
        }

        public void AddSubMesh(Mesh.SubMesh subMesh)
        {
            addSubMeshFunc(subMesh);
            _worldSubMeshes.Add(subMesh);
        }

        public void RemoveSubMesh(Mesh.SubMesh subMesh)
        {
            _worldSubMeshes.Remove(subMesh);
        }


        public void AddLight(Light light)
        {
            addLightFunc(light);
            _worldLights.Add(light);
        }

        public void RemoveLight(Light light)
        {
            _worldLights.Remove(light);
        }

        public override void DoPreFrameWork(BoundingFrustum frustum)
        {
            // do nothing
        }

        public override void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh>[] visibleSubMeshes)
        {
            for (int index = 0; index < _worldSubMeshes.Count; index++)
            {
                Mesh.SubMesh subMesh = _worldSubMeshes[index];
                if (subMesh.Enabled && 
                    frustum.Intersects(subMesh.GlobalBoundingSphere) &&
                    frustum.Intersects(subMesh.GlobalBoundingBox))
                {
                    visibleSubMeshes[(int)subMesh.RenderQueue].Add(subMesh);
                }
            }
        }

        public override void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes)
        {
            for (int index = 0; index < _worldSubMeshes.Count; index++)
            {
                Mesh.SubMesh subMesh = _worldSubMeshes[index];
                if (subMesh.Enabled && 
                    frustum.Intersects(subMesh.GlobalBoundingSphere) &&
                    frustum.Intersects(subMesh.GlobalBoundingBox))
                {
                    visibleSubMeshes.Add(subMesh);
                }
            }
        }

        public override void GetShadowCasters(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes)
        {
            for (int index = 0; index < _worldSubMeshes.Count; index++)
            {
                Mesh.SubMesh subMesh = _worldSubMeshes[index];
                if (subMesh.Enabled && subMesh.CastShadows && 
                    frustum.Intersects(subMesh.GlobalBoundingSphere) &&
                    frustum.Intersects(subMesh.GlobalBoundingBox))
                {
                    visibleSubMeshes.Add(subMesh);
                }
            }   
        }

        public override void GetVisibleLights(BoundingFrustum frustum, List<Light> visibleLights)
        {
            for(int i=0;i<_worldLights.Count;i++)
            {
                Light light = _worldLights[i];
                if (light.Enabled)
                {
                    switch (light.LightType)
                    {
                        case Light.Type.Point:
                            if (frustum.Intersects(light.BoundingSphere))
                                visibleLights.Add(light);
                            break;
                        case Light.Type.Spot:
                            if (frustum.Intersects(light.BoundingSphere) &&
                                frustum.Intersects(light.Frustum))
                                visibleLights.Add(light);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

    }
}
