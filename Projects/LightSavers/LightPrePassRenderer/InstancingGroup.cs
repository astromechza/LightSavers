using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightPrePassRenderer
{
    public class InstancingGroup
    {
        private List<Mesh.SubMesh> _subMeshes = new List<Mesh.SubMesh>();
        Matrix[] _instanceTransforms = new Matrix[64];
        DynamicVertexBuffer _instanceVertexBuffer;

        static VertexDeclaration _instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3)
        );

        public void Reset()
        {
            _subMeshes.Clear();
        }

        public ModelMeshPart GetModelMeshPart()
        {
            if (_subMeshes.Count > 0)
                return _subMeshes[0]._meshPart;
            return null;
        }

        public void AddSubMesh(Mesh.SubMesh subMesh)
        {
            _subMeshes.Add(subMesh);
        }

        public void GenerateInstanceInfo(GraphicsDevice device)
        {
            if (_subMeshes.Count == 0)
                return;
            if (_instanceTransforms.Length < _subMeshes.Count)
                _instanceTransforms = new Matrix[_subMeshes.Count * 2];
            for (int index = 0; index < _subMeshes.Count; index++)
            {
                Mesh.SubMesh subMesh = _subMeshes[index];
                _instanceTransforms[index] = subMesh.GlobalTransform;
            }

            // If we have more instances than room in our vertex buffer, grow it to the necessary size.
            if ((_instanceVertexBuffer == null) ||
                (_instanceTransforms.Length > _instanceVertexBuffer.VertexCount))
            {
                if (_instanceVertexBuffer != null)
                    _instanceVertexBuffer.Dispose();

                _instanceVertexBuffer = new DynamicVertexBuffer(device, _instanceVertexDeclaration,
                                                               _instanceTransforms.Length, BufferUsage.WriteOnly);
            }
            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            _instanceVertexBuffer.SetData(_instanceTransforms, 0, _subMeshes.Count, SetDataOptions.Discard);
        }

        public void RenderToGBuffer(Camera camera, GraphicsDevice graphicsDevice)
        {
            if (_subMeshes.Count == 0)
                return;

            // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
            Mesh.SubMesh subMesh = _subMeshes[0];
            ModelMeshPart meshPart = subMesh._meshPart;
            graphicsDevice.SetVertexBuffers(
                new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                new VertexBufferBinding(_instanceVertexBuffer, 0, 1)
            );

            subMesh.RenderEffect.SetCurrentTechnique(3);
            subMesh.RenderEffect.SetMatrices(Matrix.Identity, camera.EyeTransform, camera.ProjectionTransform);
            //our first pass is responsible for rendering into GBuffer
            subMesh.RenderEffect.SetFarClip(camera.FarClip);

            //no individual skinned models for now
            if (subMesh._parent.BoneMatrixes != null)
                subMesh.RenderEffect.SetBones(subMesh._parent.BoneMatrixes);

            subMesh.RenderEffect.Apply();

            graphicsDevice.Indices = meshPart.IndexBuffer;

            graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, _subMeshes.Count);


        }

        public void ReconstructShading(Camera camera, GraphicsDevice graphicsDevice)
        {

            if (_subMeshes.Count == 0)
                return;

            // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
            Mesh.SubMesh subMesh = _subMeshes[0];
            ModelMeshPart meshPart = subMesh._meshPart;
            graphicsDevice.SetVertexBuffers(
                new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                new VertexBufferBinding(_instanceVertexBuffer, 0, 1)
            );

            subMesh.RenderEffect.SetCurrentTechnique(4);

            subMesh.RenderEffect.Apply();

            graphicsDevice.Indices = meshPart.IndexBuffer;

            graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, _subMeshes.Count);


        }

        public virtual void RenderShadowMap(ref Matrix viewProj, GraphicsDevice graphicsDevice)
        {
            if (_subMeshes.Count == 0)
                return;

            // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
            Mesh.SubMesh subMesh = _subMeshes[0];
            ModelMeshPart meshPart = subMesh._meshPart;
            graphicsDevice.SetVertexBuffers(
                new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                new VertexBufferBinding(_instanceVertexBuffer, 0, 1)
            );

            subMesh.RenderEffect.SetCurrentTechnique(5);
            subMesh.RenderEffect.SetLightViewProj(viewProj);


            //we need to set this every frame, there are situations where the object is not on screen but it still cast shadows
            subMesh.RenderEffect.SetWorld(Matrix.Identity);
            //no individual skinned models for now
            if (subMesh._parent.BoneMatrixes != null)
                subMesh.RenderEffect.SetBones(subMesh._parent.BoneMatrixes);


            subMesh.RenderEffect.Apply();

            graphicsDevice.Indices = meshPart.IndexBuffer;

            graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, _subMeshes.Count);

        }
    }

    public class InstancingGroupManager
    {

        private List<InstancingGroup> _instancingGroups = new List<InstancingGroup>();

        public void Reset()
        {
            for (int index = 0; index < _instancingGroups.Count; index++)
            {
                InstancingGroup instancingGroup = _instancingGroups[index];
                instancingGroup.Reset();
            }
        }

        public void AddInstancedSubMesh(Mesh.SubMesh subMesh)
        {
            GetInstanceGroupForSubMesh(subMesh).AddSubMesh(subMesh);
        }

        private InstancingGroup GetInstanceGroupForSubMesh(Mesh.SubMesh subMesh)
        {
            for (int index = 0; index < _instancingGroups.Count; index++)
            {
                InstancingGroup instancingGroup = _instancingGroups[index];
                ModelMeshPart firstMeshPart = instancingGroup.GetModelMeshPart();
                if (firstMeshPart == subMesh._meshPart || firstMeshPart == null)
                    return instancingGroup;
            }
            InstancingGroup newGroup = new InstancingGroup();
            _instancingGroups.Add(newGroup);
            return newGroup;
        }

        public void GenerateInstanceInfo(GraphicsDevice graphicsDevice)
        {
            for (int index = 0; index < _instancingGroups.Count; index++)
            {
                InstancingGroup instancingGroup = _instancingGroups[index];
                instancingGroup.GenerateInstanceInfo(graphicsDevice);
            }
        }

        public void RenderToGBuffer(Camera camera, GraphicsDevice graphicsDevice)
        {
            for (int index = 0; index < _instancingGroups.Count; index++)
            {
                InstancingGroup instancingGroup = _instancingGroups[index];
                instancingGroup.RenderToGBuffer(camera, graphicsDevice);
            }
        }

        public void ReconstructShading(Camera camera, GraphicsDevice graphicsDevice)
        {
            for (int index = 0; index < _instancingGroups.Count; index++)
            {
                InstancingGroup instancingGroup = _instancingGroups[index];
                instancingGroup.ReconstructShading(camera, graphicsDevice);
            }
        }

        public void RenderShadowMap(ref Matrix viewProj, GraphicsDevice graphicsDevice)
        {
            for (int index = 0; index < _instancingGroups.Count; index++)
            {
                InstancingGroup instancingGroup = _instancingGroups[index];
                instancingGroup.RenderShadowMap(ref viewProj, graphicsDevice);
            }
        }
    }
}
