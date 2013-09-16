using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightPrePassRenderer.partitioning
{
    public class LightAndMeshContainer
    {
        # region Delegate Defines
        public delegate void AddSubMeshDelegate(Mesh.SubMesh subMesh);
        public delegate void AddLightDelegate(Light light);
        #endregion

        #region Properties
        private AddSubMeshDelegate addSubMeshFunc;
        private AddLightDelegate addLightFunc;

        private List<Mesh.SubMesh> _worldSubMeshes = new List<Mesh.SubMesh>(100);
        private List<Light> _worldLights = new List<Light>(20);
        #endregion

        public LightAndMeshContainer(AddSubMeshDelegate subMeshD, AddLightDelegate lightD)
        {
            addSubMeshFunc = subMeshD;
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

        public void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh>[] visibleSubMeshes)
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

        public void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes)
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

        public void GetShadowCasters(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes)
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

        /// <summary>
        /// Used for cascade shadow maps
        /// </summary>
        /// <param name="frustum"></param>
        /// <param name="additionalPlanes"></param>
        /// <param name="visibleSubMeshes"></param>
        public void GetShadowCasters(BoundingFrustum frustum, Plane[] additionalPlanes, List<Mesh.SubMesh> visibleSubMeshes)
        {
            for (int index = 0; index < _worldSubMeshes.Count; index++)
            {
                Mesh.SubMesh subMesh = _worldSubMeshes[index];
                if (subMesh.Enabled && subMesh.CastShadows)
                { 
                    //cull sub meshes outside the sub frustum
                    bool outside = false;
                    for (int p = 0; p < 6; p++)
                    {
                        PlaneIntersectionType intersectionType;
                        additionalPlanes[p].Intersects(ref subMesh.GlobalBoundingSphere, out intersectionType);
                        if (intersectionType == PlaneIntersectionType.Front)
                        {
                            outside = true;
                            break;
                        }
                        additionalPlanes[p].Intersects(ref subMesh.GlobalBoundingBox, out intersectionType);
                        if (intersectionType == PlaneIntersectionType.Front)
                        {
                            outside = true;
                            break;
                        }
                    }
                    if (outside)
                        continue;
                    visibleSubMeshes.Add(subMesh);
                }
            }
        }

        public void GetVisibleLights(BoundingFrustum frustum, List<Light> visibleLights)
        {
            foreach (Light light in _worldLights)
            {
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
