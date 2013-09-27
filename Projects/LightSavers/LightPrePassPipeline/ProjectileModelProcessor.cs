using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace LightPrePassProcessor
{
    [ContentProcessor(DisplayName = "Projectile Model Processor")]
    public class ProjectileModelProcessor : ContentProcessor<ProjectileXML, ModelContent>
    {

        private const string diffusetexturefile = "projectiles/green.png";
        private const string emittexturefile = "projectiles/green.png";

        public override ModelContent Process(ProjectileXML input, ContentProcessorContext context)
        {
            if (input == null) throw new ArgumentNullException("input");

            NodeContent nodeContent = new NodeContent();

            nodeContent.Children.Add(BuildMesh(input));

            return context.Convert<NodeContent, ModelContent>(nodeContent, typeof(LightPrePassProcessor).Name);

        }

        private NodeContent BuildMesh(ProjectileXML p)
        {
            MeshBuilder mb = MeshBuilder.StartMesh("projectile");

            BasicMaterialContent material = new BasicMaterialContent();

            material.Textures.Add(LightPrePassProcessor.DiffuseMapKey, new ExternalReference<TextureContent>(diffusetexturefile));
            material.Textures.Add(LightPrePassProcessor.EmissiveMapKey, new ExternalReference<TextureContent>(emittexturefile));

            mb.SetMaterial(material);

            // Create data channels
            int channel_texCoord0 = mb.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            // First create vertex data
            Vector2 t1 = new Vector2(0, 0);
            Vector2 t2 = new Vector2(1, 1);
            Vector2 t3 = new Vector2(1, 0);

            // loop through all the pixels
            float X1 = p.leftX;
            float X2 = p.rightX;
            float R = p.radius;

            mb.CreatePosition(new Vector3(X1, 0, 0));

            mb.CreatePosition(new Vector3(0, 0, R));
            mb.CreatePosition(new Vector3(0, R, 0));
            mb.CreatePosition(new Vector3(0, 0, -R));
            mb.CreatePosition(new Vector3(0, -R, 0));

            mb.CreatePosition(new Vector3(X2, 0, 0));

            #region left tris
            mb.SetVertexChannelData(channel_texCoord0, t1);
            mb.AddTriangleVertex(0);
            mb.SetVertexChannelData(channel_texCoord0, t2);
            mb.AddTriangleVertex(1);
            mb.SetVertexChannelData(channel_texCoord0, t3);
            mb.AddTriangleVertex(2);

            mb.SetVertexChannelData(channel_texCoord0, t1);
            mb.AddTriangleVertex(0);
            mb.SetVertexChannelData(channel_texCoord0, t2);
            mb.AddTriangleVertex(2);
            mb.SetVertexChannelData(channel_texCoord0, t3);
            mb.AddTriangleVertex(3);

            mb.SetVertexChannelData(channel_texCoord0, t1);
            mb.AddTriangleVertex(0);
            mb.SetVertexChannelData(channel_texCoord0, t2);
            mb.AddTriangleVertex(3);
            mb.SetVertexChannelData(channel_texCoord0, t3);
            mb.AddTriangleVertex(4);

            mb.SetVertexChannelData(channel_texCoord0, t1);
            mb.AddTriangleVertex(0);
            mb.SetVertexChannelData(channel_texCoord0, t2);
            mb.AddTriangleVertex(4);
            mb.SetVertexChannelData(channel_texCoord0, t3);
            mb.AddTriangleVertex(1);
            #endregion

            mb.SetVertexChannelData(channel_texCoord0, t1);
            mb.AddTriangleVertex(5);
            mb.SetVertexChannelData(channel_texCoord0, t2);
            mb.AddTriangleVertex(2);
            mb.SetVertexChannelData(channel_texCoord0, t3);
            mb.AddTriangleVertex(1);

            mb.SetVertexChannelData(channel_texCoord0, t1);
            mb.AddTriangleVertex(5);
            mb.SetVertexChannelData(channel_texCoord0, t2);
            mb.AddTriangleVertex(3);
            mb.SetVertexChannelData(channel_texCoord0, t3);
            mb.AddTriangleVertex(2);

            mb.SetVertexChannelData(channel_texCoord0, t1);
            mb.AddTriangleVertex(5);
            mb.SetVertexChannelData(channel_texCoord0, t2);
            mb.AddTriangleVertex(4);
            mb.SetVertexChannelData(channel_texCoord0, t3);
            mb.AddTriangleVertex(3);

            mb.SetVertexChannelData(channel_texCoord0, t1);
            mb.AddTriangleVertex(5);
            mb.SetVertexChannelData(channel_texCoord0, t2);
            mb.AddTriangleVertex(1);
            mb.SetVertexChannelData(channel_texCoord0, t3);
            mb.AddTriangleVertex(4);

            return mb.FinishMesh();
        }
    }
}