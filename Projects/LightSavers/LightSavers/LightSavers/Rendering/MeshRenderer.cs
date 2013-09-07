//-----------------------------------------------------------------------------
// MeshRenderer.cs
//
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightSavers.Rendering
{
    public class MeshRenderer
    {

        /// <summary>
        /// Sphere model
        /// </summary>
        private Model _model;

        /// <summary>
        /// Store also the mesh part, to faster access
        /// </summary>
        private ModelMeshPart meshPart;

        public MeshRenderer(Model model)
        {
            _model = model;
            meshPart = _model.Meshes[0].MeshParts[0];
        }

        public void RenderMesh(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
        }
        public void BindMesh(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);
            graphicsDevice.Indices = meshPart.IndexBuffer;
        }
    }
}
