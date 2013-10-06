using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightPrePassRenderer.partitioning
{
    public class BlockBasedSceneGraph : BaseSceneGraph
    {
        private BaseSceneGraph.AddSubMeshDelegate addSubMeshFunc;
        private BaseSceneGraph.AddLightDelegate addLightFunc;

        private SceneGraphBlock[] blocks;

        private int leftFrameBlock;
        private int rightFrameBlock;

        #region CONSTRUCTORS and INITIALISERS
        public BlockBasedSceneGraph(int numblocks)
        {
            blocks = new SceneGraphBlock[numblocks];
            for (int i = 0; i < numblocks; i++) blocks[i] = new SceneGraphBlock(i);
        }

        public override void SetSubMeshDelegate(BaseSceneGraph.AddSubMeshDelegate subMeshD)
        {
            addSubMeshFunc = subMeshD;
        }

        public override void SetLightDelegate(BaseSceneGraph.AddLightDelegate lightD)
        {
            addLightFunc = lightD;
        }
        #endregion

        #region SETUP FUNCTIONS for MESH and LIGHT
        public void Setup(Mesh m)
        {
            for (int index = 0; index < m.SubMeshes.Count; index++)
            {
                addSubMeshFunc(m.SubMeshes[index]);
            }
        }

        public void Setup(Light l)
        {
            addLightFunc(l);
        }
        #endregion

        #region ADD / REMOVE / RENEW
        public MeshSceneGraphReceipt Add(Mesh m)
        {
            int i = (int)m.Transform.Translation.X / 32;

            blocks[i].Add(m);

            MeshSceneGraphReceipt r = new MeshSceneGraphReceipt();
            r.mesh = m;
            r.ReceivedIndex = i;
            r.graph = this;

            return r;
        }

        public LightSceneGraphReceipt Add(Light l)
        {
            int i = (int)l.Transform.Translation.X / 32;

            blocks[i].Add(l);

            LightSceneGraphReceipt r = new LightSceneGraphReceipt();
            r.light = l;
            r.ReceivedIndex = i;
            r.graph = this;

            return r;
        }

        public void Remove(MeshSceneGraphReceipt receipt)
        {
            blocks[receipt.ReceivedIndex].Remove(receipt.mesh);
        }

        public void Remove(LightSceneGraphReceipt receipt)
        {
            blocks[receipt.ReceivedIndex].Remove(receipt.light);
        }

        public void Renew(MeshSceneGraphReceipt receipt)
        {
            int iold = receipt.ReceivedIndex;
            int inew = (int)receipt.mesh.Transform.Translation.X / 32;
            if (iold != inew)
            {
                receipt.ReceivedIndex = inew;
                blocks[iold].Remove(receipt.mesh);
                blocks[inew].Add(receipt.mesh);
            }
        }

        public void Renew(LightSceneGraphReceipt receipt)
        {
            int iold = receipt.ReceivedIndex;
            int inew = (int)receipt.light.Transform.Translation.X / 32;
            if (iold != inew)
            {
                receipt.ReceivedIndex = inew;
                blocks[iold].Remove(receipt.light);
                blocks[inew].Add(receipt.light);
            }
        }
        #endregion

        public override void DoPreFrameWork(Microsoft.Xna.Framework.BoundingFrustum frustum)
        {
            Vector3[] corners = frustum.GetCorners();

            float xx = corners[6].X - corners[2].X;
            float yy = corners[2].Y - corners[6].Y;

            float dx = (corners[2].Y * xx) / yy;

            int minx = (int)(corners[0].X - dx);
            int maxx = (int)(corners[2].X + dx);

            minx = (int)MathHelper.Clamp(minx / 32, 0, blocks.Length);
            maxx = (int)MathHelper.Clamp(maxx / 32, 0, blocks.Length);

            leftFrameBlock = minx;
            rightFrameBlock = maxx;
        }

        #region RENDERER CALLS : GET VISIBLE THINGS
        public override void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes)
        {
            for (int x = leftFrameBlock; x <= rightFrameBlock; x++) blocks[x].GetVisibleMeshes(frustum, visibleSubMeshes);
        }

        public override void GetVisibleMeshes(BoundingFrustum frustum, List<Mesh.SubMesh>[] visibleSubMeshes)
        {
            for (int x = leftFrameBlock; x <= rightFrameBlock; x++) blocks[x].GetVisibleMeshes(frustum, visibleSubMeshes);
        }

        public override void GetVisibleLights(BoundingFrustum frustum, List<Light> visibleLights)
        {
            for (int x = leftFrameBlock; x <= rightFrameBlock; x++) blocks[x].GetVisibleLights(frustum, visibleLights);
        }

        public override void GetShadowCasters(BoundingFrustum frustum, List<Mesh.SubMesh> visibleSubMeshes)
        {
            for (int x = leftFrameBlock; x <= rightFrameBlock; x++) blocks[x].GetShadowCasters(frustum, visibleSubMeshes);
        }
        #endregion
    }
}
