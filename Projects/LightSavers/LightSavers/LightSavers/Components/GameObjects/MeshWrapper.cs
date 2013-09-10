using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    /// <summary>
    /// This class encapsulates a 3d model and provides interfaces and methods for
    /// drawing, bounding, collisions, etc. 
    /// </summary>
    public class MeshWrapper
    {
        private Matrix transform = Matrix.Identity;
        private Model model;

        // == CONSTRUCTORS ===

        public MeshWrapper(Model m)
        {
            model = m;
        }

        public MeshWrapper(Model m, Matrix t)
        {
            model = m;
            transform = t;
        }

        // == DRAWING METHODS ===

        public void RenderReconstructedShading(Camera camera, Texture2D lightBuffer)
        {

            Matrix worldView = transform * camera.ViewMatrix;
            Matrix worldViewProjection = transform * camera.ViewProjectionMatrix;
            Vector2 pixelSize = new Vector2(0.5f / lightBuffer.Width, 0.5f / lightBuffer.Height);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart subMesh in mesh.MeshParts)
                {
                    Effect effect = subMesh.Effect;
                    effect.CurrentTechnique = effect.Techniques[1];

                    effect.Parameters["LightBuffer"].SetValue(lightBuffer);
                    effect.Parameters["LightBufferPixelSize"].SetValue(pixelSize);

                    effect.Parameters["World"].SetValue(transform);
                    effect.Parameters["WorldView"].SetValue(worldView);
                    effect.Parameters["WorldViewProjection"].SetValue(worldViewProjection);
                    effect.CurrentTechnique.Passes[0].Apply();

                    Globals.graphics.GraphicsDevice.SetVertexBuffer(subMesh.VertexBuffer, subMesh.VertexOffset);
                    Globals.graphics.GraphicsDevice.Indices = subMesh.IndexBuffer;

                    Globals.graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, subMesh.NumVertices, subMesh.StartIndex, subMesh.PrimitiveCount);
                }

            }
        }

        public void RenderToGBuffer(Camera camera)
        {



            Matrix worldView = transform * camera.ViewMatrix;
            Matrix worldViewProjection = transform * camera.ViewProjectionMatrix;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart subMesh in mesh.MeshParts)
                {
                    Effect effect = subMesh.Effect;
                    effect.CurrentTechnique = effect.Techniques[0];
                    //our first pass is responsible for rendering into GBuffer
                    effect.Parameters["World"].SetValue(transform);
                    effect.Parameters["View"].SetValue(camera.ViewMatrix);
                    effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
                    effect.Parameters["WorldView"].SetValue(worldView);
                    effect.Parameters["WorldViewProjection"].SetValue(worldViewProjection);
                    effect.Parameters["FarClip"].SetValue(camera.FarClip);
                    effect.CurrentTechnique.Passes[0].Apply();

                    Globals.graphics.GraphicsDevice.SetVertexBuffer(subMesh.VertexBuffer, subMesh.VertexOffset);
                    Globals.graphics.GraphicsDevice.Indices = subMesh.IndexBuffer;

                    Globals.graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, subMesh.NumVertices, subMesh.StartIndex, subMesh.PrimitiveCount);
                }
            }
        }

        public void RenderShadowMap(ref Matrix viewProj)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart subMesh in mesh.MeshParts)
                {
                    Effect effect = subMesh.Effect;

                    //render to shadow map
                    effect.CurrentTechnique = effect.Techniques[2];
                    effect.Parameters["World"].SetValue(transform);
                    effect.Parameters["LightViewProj"].SetValue(viewProj);
                    effect.CurrentTechnique.Passes[0].Apply();
                    Globals.graphics.GraphicsDevice.SetVertexBuffer(subMesh.VertexBuffer, subMesh.VertexOffset);
                    Globals.graphics.GraphicsDevice.Indices = subMesh.IndexBuffer;

                    Globals.graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, subMesh.NumVertices, subMesh.StartIndex, subMesh.PrimitiveCount);
                }
            }
        }

        // == TRANSFORM STUFF ===

        public Matrix Transform 
        { 
            get { return transform; }
            set { transform = value; }
        }
         

        
    }
}
