using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace CustomProcessors
{
    /// <summary>
    /// This ContentProcessor is designed to convert a bmp file into a model and to embed textures and defines
    /// </summary>
    [ContentProcessor(DisplayName = "WorldSectionProcessor - CustomProcessors")]
    public class WorldSectionProcessor : ContentProcessor<Texture2DContent, ModelContent>
    {        
        // Texture 
        public const string texturefile = "worldsectiontexture.bmp";

        public override ModelContent Process(Texture2DContent input, ContentProcessorContext context)
        {
            if (input == null) throw new ArgumentNullException("input");

            PixelBitmapContent<Color> bmpInput = (PixelBitmapContent<Color>)input.Mipmaps[0];

            NodeContent nodeContent = new NodeContent();

            nodeContent.Children.Add(buildFloorMesh(bmpInput));
            nodeContent.Children.Add(buildWallMesh(bmpInput));
            nodeContent.Children.Add(buildBlackMesh(bmpInput));

            return context.Convert<NodeContent, ModelContent>(nodeContent, "ModelProcessor");
        }

        private NodeContent buildFloorMesh(PixelBitmapContent<Color> bitmap)
        {
            MeshBuilder mb = MeshBuilder.StartMesh("floor");

            // Create a material, and point it at the world section texture
            BasicMaterialContent material = new BasicMaterialContent();
            material.Texture = new ExternalReference<TextureContent>(texturefile);
            mb.SetMaterial(material);

            // Create data channels
            int channel_texCoord0 = mb.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            // First create vertex data
            
            // loop through all the pixels
            for (int z = 0; z < bitmap.Height; z++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, z) == Color.White)
                    {
                        // Create the 4 vertices
                        Vector3 topleft = new Vector3(x, 0, z);
                        mb.CreatePosition(topleft);
                        Vector3 topright = new Vector3(x + 1.0f, 0, z);
                        mb.CreatePosition(topright);
                        Vector3 bottomleft = new Vector3(x, 0, z + 1.0f);
                        mb.CreatePosition(bottomleft);
                        Vector3 bottomright = new Vector3(x + 1.0f, 0, z + 1.0f);
                        mb.CreatePosition(bottomleft);
                    }
                }
            }


            int vertexindex = 0;

            // loop through all the pixels
            for (int z = 0; z < bitmap.Height; z++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, z) == Color.White)
                    {
                        // Create the 2 triangles. Clockwise winding
                        mb.SetVertexChannelData(channel_texCoord0, new Vector2(0.0f, 0.0f));
                        mb.AddTriangleVertex(vertexindex + 0);
                        mb.SetVertexChannelData(channel_texCoord0, new Vector2(1.0f, 0.0f));
                        mb.AddTriangleVertex(vertexindex + 1);
                        mb.SetVertexChannelData(channel_texCoord0, new Vector2(1.0f, 1.0f));
                        mb.AddTriangleVertex(vertexindex + 3);

                        mb.SetVertexChannelData(channel_texCoord0, new Vector2(0.0f, 0.0f));
                        mb.AddTriangleVertex(vertexindex + 0);
                        mb.SetVertexChannelData(channel_texCoord0, new Vector2(1.0f, 1.0f));
                        mb.AddTriangleVertex(vertexindex + 3);
                        mb.SetVertexChannelData(channel_texCoord0, new Vector2(0.0f, 1.0f));
                        mb.AddTriangleVertex(vertexindex + 2);
                        
                        vertexindex += 4;
                    }
                }
            }

            return mb.FinishMesh();
        }

        private NodeContent buildWallMesh(PixelBitmapContent<Color> bitmap)
        {
            MeshBuilder mb = MeshBuilder.StartMesh("wall");


            return mb.FinishMesh();
        }

        private NodeContent buildBlackMesh(PixelBitmapContent<Color> bitmap)
        {
            MeshBuilder mb = MeshBuilder.StartMesh("black");


            return mb.FinishMesh();
        }

    }
}