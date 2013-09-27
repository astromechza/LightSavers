﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightPrePassRenderer.partitioning
{
    public class AwesomeSceneGraph : BaseSceneGraph
    {
        private BaseSceneGraph.AddSubMeshDelegate addSubMeshFunc;
        private BaseSceneGraph.AddLightDelegate addLightFunc;

        private List<SceneGraphBlock> blocks;

        public AwesomeSceneGraph()
        {
            blocks = new List<SceneGraphBlock>();
        }

        private void EnsureBlocks(int i)
        {
            if (blocks.Count <= i)
            {
                for(int j=0;j<i-blocks.Count+1;j++)
                {
                    blocks.Add(new SceneGraphBlock());
                }
            }
        }

        public MeshSceneGraphReceipt AddMesh(Mesh m)
        {
            int i = (int)m.Transform.Translation.X / 32;
            EnsureBlocks(i);

            for (int index = 0; index < m.SubMeshes.Count; index++)
            {
                addSubMeshFunc(m.SubMeshes[index]);
            }
            return blocks[i].AddMesh(m);
        }

        public LightSceneGraphReceipt AddLight(Light l)
        {
            int i = (int)l.Transform.Translation.X / 32;
            EnsureBlocks(i);

            addLightFunc(l);
            return blocks[i].AddLight(l);
        }

        public override void SetSubMeshDelegate(BaseSceneGraph.AddSubMeshDelegate subMeshD)
        {
            addSubMeshFunc = subMeshD;
        }

        public override void SetLightDelegate(BaseSceneGraph.AddLightDelegate lightD)
        {
            addLightFunc = lightD;
        }

        public override void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh>[] visibleSubMeshes)
        {
            Vector3[] corners = frustum.GetCorners();

            float xx = corners[6].X - corners[2].X;
            float yy = corners[2].Y - corners[6].Y;

            float dx = (corners[2].Y * xx) / yy;

            int minx = (int)MathHelper.Clamp(corners[0].X - dx, 0, 319);
            int maxx = (int)MathHelper.Clamp(corners[2].X + dx, 0, 319);

            minx /= 32;
            maxx /= 32;
            for(int x=minx;x<=maxx;x++) blocks[x].GetVisibleMeshes(frustum, visibleSubMeshes);
        }

        public override void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes)
        {
            Vector3[] corners = frustum.GetCorners();

            float xx = corners[6].X - corners[2].X;
            float yy = corners[2].Y - corners[6].Y;

            float dx = (corners[2].Y * xx) / yy;

            int minx = (int)MathHelper.Clamp(corners[0].X - dx, 0, 319);
            int maxx = (int)MathHelper.Clamp(corners[2].X + dx, 0, 319);

            minx /= 32;
            maxx /= 32;
            for (int x = minx; x <= maxx; x++) blocks[x].GetVisibleMeshes(frustum, visibleSubMeshes);
        }

        public override void GetShadowCasters(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes)
        {
            Vector3[] corners = frustum.GetCorners();

            float xx = corners[6].X - corners[2].X;
            float yy = corners[2].Y - corners[6].Y;

            float dx = (corners[2].Y * xx) / yy;

            int minx = (int)MathHelper.Clamp(corners[0].X - dx, 0, 319);
            int maxx = (int)MathHelper.Clamp(corners[2].X + dx, 0, 319);

            minx /= 32;
            maxx /= 32;
            for (int x = minx; x <= maxx; x++) blocks[x].GetShadowCasters(frustum, visibleSubMeshes);

        }

        public override void GetVisibleLights(BoundingFrustum frustum, List<Light> visibleLights)
        {
            Vector3[] corners = frustum.GetCorners();

            float xx = corners[6].X - corners[2].X;
            float yy = corners[2].Y - corners[6].Y;

            float dx = (corners[2].Y * xx) / yy;

            int minx = (int)MathHelper.Clamp(corners[0].X - dx, 0, 319);
            int maxx = (int)MathHelper.Clamp(corners[2].X + dx, 0, 319);

            minx /= 32;
            maxx /= 32;

            for (int x = minx; x <= maxx; x++) blocks[x].GetVisibleLights(frustum, visibleLights);
        }

        public class SceneGraphBlock
        {
            private List<Mesh> meshes;
            private List<Light> lights;

            public SceneGraphBlock()
            {
                meshes = new List<Mesh>();
                lights = new List<Light>();
            }

            public MeshSceneGraphReceipt AddMesh(Mesh m)
            {
                meshes.Add(m);

                MeshSceneGraphReceipt r = new MeshSceneGraphReceipt();
                r.mesh = m;
                r.parentlist = meshes;
                r.oldGlobalTransform = m.Transform;

                return r;
            }

            public LightSceneGraphReceipt AddLight(Light l)
            {
                lights.Add(l);

                LightSceneGraphReceipt r = new LightSceneGraphReceipt();
                r.light = l;
                r.parentlist = lights;
                r.oldGlobalTransform = l.Transform;

                return r;
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
                    if (frustum.Intersects(m.GlobalBoundingBox))
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
}
