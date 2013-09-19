//-----------------------------------------------------------------------------
// Mesh.cs
//
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightPrePassRenderer
{
    /// <summary>
    /// This class encapsulate a XNA model, a world transform and some methods
    /// for rendering it in our light pre-pass renderer
    /// </summary>
    public class Mesh
    {
        public class SubMesh
        {
            public SubMesh(Mesh parent)
            {
                _parent = parent;
            }

            public MeshMetadata.SubMeshMetadata _metadata;
            public ModelMeshPart _meshPart;
            public int _modelIndex = 0;

            public Matrix GlobalTransform = Matrix.Identity;
            public BoundingBox GlobalBoundingBox;
            public BoundingSphere GlobalBoundingSphere;
            private BaseRenderEffect _renderEffect;

            public Mesh _parent;
            public bool Enabled = true;
            private bool _instanceEnabled = false;

            public Effect Effect
            {
                set
                {
                    _renderEffect = new BaseRenderEffect(value.Clone());
                }
            }
            public bool CastShadows
            {
                get { return _metadata.CastShadows && _metadata.RenderQueue != MeshMetadata.ERenderQueue.Blend; }
            }

            public MeshMetadata.ERenderQueue RenderQueue
            {
                get { return _metadata.RenderQueue; }
            }

            public BaseRenderEffect RenderEffect
            {
                get { return _renderEffect; }
            }

            public bool InstanceEnabled
            {
                get
                {
                    return _instanceEnabled;
                }
                set
                {
                    _instanceEnabled = value;
                }
            }


            public virtual void RenderToGBuffer(Camera camera, GraphicsDevice graphicsDevice)
            {
                RenderEffect.SetCurrentTechnique(0);
                RenderEffect.SetMatrices(GlobalTransform, camera.EyeTransform, camera.ProjectionTransform);
                //our first pass is responsible for rendering into GBuffer
                RenderEffect.SetFarClip(camera.FarClip);

                if (_parent.BoneMatrixes != null)
                    RenderEffect.SetBones(_parent.BoneMatrixes);

                RenderEffect.Apply();

                graphicsDevice.SetVertexBuffer(_meshPart.VertexBuffer, _meshPart.VertexOffset);
                graphicsDevice.Indices = _meshPart.IndexBuffer;

                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _meshPart.NumVertices, _meshPart.StartIndex, _meshPart.PrimitiveCount);


            }

            public void ReconstructShading(Camera camera, GraphicsDevice graphicsDevice)
            {

                //this pass uses the light diffuse and specular accumulation texture (already bound in the setup stage) and reconstruct the mesh's shading
                //our parameters were already filled in the first pass
                RenderEffect.SetCurrentTechnique(1);

                //we don't need to do this again, it was done on the previous step

                //_renderEffect.SetMatrices(GlobalTransform, camera.EyeTransform, camera.ProjectionTransform);
                // if (_parent.BoneMatrixes != null)
                //    _renderEffect.SetBones(_parent.BoneMatrixes);

                RenderEffect.Apply();

                graphicsDevice.SetVertexBuffer(_meshPart.VertexBuffer, _meshPart.VertexOffset);
                graphicsDevice.Indices = _meshPart.IndexBuffer;

                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _meshPart.NumVertices, _meshPart.StartIndex, _meshPart.PrimitiveCount);
                

            }

            /// <summary>
            /// It assumes that this shader is not the default LPP shader, and it will use ONLY the first technique
            /// </summary>
            /// <param name="camera"></param>
            /// <param name="graphicsDevice"></param>
            /// <param name="renderStatistics"></param>
            public void GenericRender(Camera camera, GraphicsDevice graphicsDevice)
            {
                RenderEffect.SetMatrices(GlobalTransform, camera.EyeTransform, camera.ProjectionTransform);

                if (_parent.BoneMatrixes != null)
                    RenderEffect.SetBones(_parent.BoneMatrixes);

                RenderEffect.Apply();

                graphicsDevice.SetVertexBuffer(_meshPart.VertexBuffer, _meshPart.VertexOffset);
                graphicsDevice.Indices = _meshPart.IndexBuffer;

                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _meshPart.NumVertices, _meshPart.StartIndex, _meshPart.PrimitiveCount);
                

            }

            public virtual void RenderShadowMap(ref Matrix viewProj, GraphicsDevice graphicsDevice)
            {

                //render to shadow map
                RenderEffect.SetCurrentTechnique(2);
                RenderEffect.SetLightViewProj(viewProj);


                //we need to set this every frame, there are situations where the object is not on screen but it still cast shadows
                _renderEffect.SetWorld(GlobalTransform);
                if (_parent.BoneMatrixes != null)
                    _renderEffect.SetBones(_parent.BoneMatrixes);

                RenderEffect.Apply();

                graphicsDevice.SetVertexBuffer(_meshPart.VertexBuffer, _meshPart.VertexOffset);
                graphicsDevice.Indices = _meshPart.IndexBuffer;

                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _meshPart.NumVertices, _meshPart.StartIndex, _meshPart.PrimitiveCount);


            }

        }

        /// <summary>
        /// Stores the transforms for the submeshes
        /// </summary>
        protected Matrix[] _transforms;

        /// <summary>
        /// our global transform
        /// </summary>
        protected Matrix _transform = Matrix.Identity;

        /// <summary>
        /// Model
        /// </summary>
        protected Model _model;
        protected MeshMetadata _metadata;
        protected bool _castShadows = true;
        protected BoundingBox _globalBoundingBox;
        protected BoundingBox _localBoundingBox;

        private List<SubMesh> _subMeshes = new List<SubMesh>();
        public virtual Matrix[] BoneMatrixes
        {
            get { return null; }
            set { }
        }
        public virtual Model Model
        {
            get { return _model; }
            set
            {
                _subMeshes.Clear();
                _model = value;
                _metadata = _model.Tag as MeshMetadata;
                //the last bounding box is the sum of all others
                _localBoundingBox = _metadata.BoundingBox;
                _transforms = new Matrix[_model.Bones.Count];
                _model.CopyAbsoluteBoneTransformsTo(_transforms);

                CreateSubMeshList();
                UpdateSubMeshes();
            }
        }

        public Matrix Transform
        {
            get { return _transform; }
            set
            {
                _transform = value;
                UpdateSubMeshes();
            }
        }

        public BoundingBox GlobalBoundingBox
        {
            get { return _globalBoundingBox; }
        }

        public List<SubMesh> SubMeshes
        {
            get { return _subMeshes; }
        }

        public void SetInstancingEnabled(bool value)
        {
            for (int index = 0; index < SubMeshes.Count; index++)
            {
                SubMesh subMesh = SubMeshes[index];
                subMesh.InstanceEnabled = value;
            }
        }

        /// <summary>
        /// Propagate our global transform to the sub meshes, recomputing their bounding boxes
        /// </summary>
        private void UpdateSubMeshes()
        {
            Helpers.TransformBoundingBox(ref _localBoundingBox, ref _transform, out _globalBoundingBox);
            if (_model != null)
            {
                for (int i = 0; i < _model.Bones.Count; i++)
                {
                    _transforms[i] = _model.Bones[i].Transform * _transform;
                }
                for (int i = 0; i < _subMeshes.Count; i++)
                {
                    SubMesh subMesh = _subMeshes[i];
                    //compute the global transform for this submesh
                    subMesh.GlobalTransform = _transforms[_model.Meshes[subMesh._modelIndex].ParentBone.Index];
                    MeshMetadata.SubMeshMetadata metadata = subMesh._metadata;
                    BoundingBox source = metadata.BoundingBox;
                    //compute the global bounding box
                    Helpers.TransformBoundingBox(ref source, ref _transform, out subMesh.GlobalBoundingBox);
                    subMesh.GlobalBoundingSphere = BoundingSphere.CreateFromBoundingBox(subMesh.GlobalBoundingBox);

                }
            }
        }

        protected void CreateSubMeshList()
        {
            int idx = 0;
            for (int i = 0; i < _model.Meshes.Count; i++)
            {
                ModelMesh modelMesh = _model.Meshes[i];
                for (int index = 0; index < modelMesh.MeshParts.Count; index++)
                {
                    ModelMeshPart modelMeshPart = modelMesh.MeshParts[index];
                    SubMesh subMesh = new SubMesh(this);
                    subMesh._meshPart = modelMeshPart;
                    subMesh._modelIndex = idx;
                    subMesh.Effect = modelMeshPart.Effect;
                    subMesh._metadata = _metadata.SubMeshesMetadata[idx];
                    _subMeshes.Add(subMesh);
                }
                idx++;
            }
            UpdateSubMeshes();
        }

    }
}
