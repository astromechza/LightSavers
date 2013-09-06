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
        private const string floortexturefile = "textures/floortexture.png";
        private const string walltexturefile = "textures/walltexture.png";
        private const string blacktexturefile = "textures/black.bmp";

        public const float WallHeight = 2.5f;   // height of walls

        private Vector2[][] quarters = {
            new Vector2[] { new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.0f), new Vector2(0.0f, 0.5f), new Vector2(0.5f, 0.5f) },
            new Vector2[] { new Vector2(0.5f, 0.0f), new Vector2(1.0f, 0.0f), new Vector2(0.5f, 0.5f), new Vector2(1.0f, 0.5f) },
            new Vector2[] { new Vector2(0.0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.0f, 1.0f), new Vector2(0.5f, 1.0f) },
            new Vector2[] { new Vector2(0.5f, 0.5f), new Vector2(1.0f, 0.5f), new Vector2(0.5f, 1.0f), new Vector2(1.0f, 1.0f) }
        };

        public override ModelContent Process(Texture2DContent input, ContentProcessorContext context)
        {
            if (input == null) throw new ArgumentNullException("input");

            PixelBitmapContent<Color> bmpInput = (PixelBitmapContent<Color>)input.Mipmaps[0];

            NodeContent nodeContent = new NodeContent();

            nodeContent.Children.Add(buildFloorMesh(bmpInput));
            nodeContent.Children.Add(buildWallMesh(bmpInput));
            nodeContent.Children.Add(buildBlackMesh(bmpInput));

            return context.Convert<NodeContent, ModelContent>(nodeContent, typeof(ModelBakerProcessor).Name);
        }

        private NodeContent buildFloorMesh(PixelBitmapContent<Color> bitmap)
        {
            MeshBuilder mb = MeshBuilder.StartMesh("floor");

            // Create a material, and point it at the world section texture
            BasicMaterialContent material = new BasicMaterialContent();
            material.Texture = new ExternalReference<TextureContent>(floortexturefile);
            mb.SetMaterial(material);

            // Create data channels
            int channel_texCoord0 = mb.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            // First create vertex data
            
            // loop through all the pixels
            int quadcount = 0;
            for (int z = 0; z < bitmap.Height; z++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, z) == Color.White)
                    {
                        quadcount += AddQuadVertexPositions(mb, new Vector3(x, 0.0f, z), new Vector3(x + 1.0f, 0.0f, z + 1.0f));
                    }
                }
            }            

            Random r = new Random();

            for (int q = 0; q < quadcount; q++)
            {
                Vector2[] tex = quarters[r.Next(4)];
                AddTriangleVertices(mb, q, channel_texCoord0, tex);
            }

            return mb.FinishMesh();
        }

        private NodeContent buildWallMesh(PixelBitmapContent<Color> bitmap)
        {
            MeshBuilder mb = MeshBuilder.StartMesh("wall");

            // Create a material, and point it at the world section texture
            BasicMaterialContent material = new BasicMaterialContent();
            material.Texture = new ExternalReference<TextureContent>(walltexturefile);
            mb.SetMaterial(material);

            int channel_texCoord0 = mb.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            // First create vertex data
            // loop through all the pixels
            int quadcount = 0;
            for (int z = 0; z < bitmap.Height; z++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, z) == Color.Blue)
                    {
                        bool leftWall = (x > 0) && (bitmap.GetPixel(x - 1, z) == Color.White);
                        bool rightWall = (x < bitmap.Width - 1) && (bitmap.GetPixel(x + 1, z) == Color.White);
                        bool backWall = (z > 0) && (bitmap.GetPixel(x, z - 1) == Color.White);
                        bool frontWall = (z < bitmap.Height - 1) && (bitmap.GetPixel(x, z + 1) == Color.White);

                        if (leftWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(x, WallHeight, z), new Vector3(x, 0.0f, z + 1.0f));
                        }

                        if (rightWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(x + 1.0f, WallHeight, z + 1.0f), new Vector3(x + 1.0f, 0.0f, z));
                        }

                        if (frontWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(x, WallHeight, z + 1.0f), new Vector3(x + 1.0f, 0.0f, z + 1.0f));
                        }

                        if (backWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(x + 1.0f, WallHeight, z), new Vector3(x, 0.0f, z));
                        }
                    }
                }
            }

            Random r = new Random();

            for (int q = 0; q < quadcount; q++)
            {
                Vector2[] tex = quarters[r.Next(4)];
                AddTriangleVertices(mb, q, channel_texCoord0, tex);
            }

            return mb.FinishMesh();
        }

        private NodeContent buildBlackMesh(PixelBitmapContent<Color> bitmap)
        {
            MeshBuilder mb = MeshBuilder.StartMesh("black");

            // Create a material, and point it at the world section texture
            BasicMaterialContent material = new BasicMaterialContent();
            material.Texture = new ExternalReference<TextureContent>(blacktexturefile);
            mb.SetMaterial(material);

            int channel_texCoord0 = mb.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            // First create vertex data
            // loop through all the pixels
            int quadcount = 0;
            for (int z = 0; z < bitmap.Height; z++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, z) == Color.Blue)
                    {

                        quadcount += AddQuadVertexPositions(mb, new Vector3(x, WallHeight, z), new Vector3(x + 1.0f, WallHeight, z + 1.0f));

                        bool leftWall = (x == 0) || ((x > 0) && (bitmap.GetPixel(x - 1, z) == Color.Black));
                        bool rightWall = (x == bitmap.Width - 1) || ((x < bitmap.Width - 1) && (bitmap.GetPixel(x + 1, z) == Color.Black));
                        bool backWall = (z == 0) || ((z > 0) && (bitmap.GetPixel(x, z - 1) == Color.Black));
                        bool frontWall = (z == bitmap.Height - 1) || ((z < bitmap.Height - 1) && (bitmap.GetPixel(x, z + 1) == Color.Black));

                        if (leftWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(x, WallHeight, z), new Vector3(x, 0.0f, z + 1.0f));
                        }

                        if (rightWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(x + 1.0f, WallHeight, z + 1.0f), new Vector3(x + 1.0f, 0.0f, z));
                        }

                        if (frontWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(x, WallHeight, z + 1.0f), new Vector3(x + 1.0f, 0.0f, z + 1.0f));
                        }

                        if (backWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(x + 1.0f, WallHeight, z), new Vector3(x, 0.0f, z));
                        }
                    }
                }
            }

            Random r = new Random();

            for (int q = 0; q < quadcount; q++)
            {
                Vector2[] tex = quarters[r.Next(4)];
                AddTriangleVertices(mb, q, channel_texCoord0, tex);
            }

            return mb.FinishMesh();
        }

        private int AddQuadVertexPositions(MeshBuilder mb, Vector3 tl, Vector3 br)
        {
            mb.CreatePosition(tl);

            if (tl.Y != br.Y)
            {
                Vector3 tr = new Vector3(br.X, tl.Y, br.Z);
                mb.CreatePosition(tr);

                Vector3 bl = new Vector3(tl.X, br.Y, tl.Z);
                mb.CreatePosition(bl);
            }
            else
            {
                Vector3 tr = new Vector3(br.X, tl.Y, tl.Z);
                mb.CreatePosition(tr);

                Vector3 bl = new Vector3(tl.X, br.Y, br.Z);
                mb.CreatePosition(bl);
            }

            mb.CreatePosition(br);

            return 1;
        }

        private void AddTriangleVertices(MeshBuilder mb, int q, int texchannel0, Vector2[] tex)
        {
            mb.SetVertexChannelData(texchannel0, tex[0]);
            mb.AddTriangleVertex(q * 4 + 0);
            mb.SetVertexChannelData(texchannel0, tex[1]);
            mb.AddTriangleVertex(q * 4 + 1);
            mb.SetVertexChannelData(texchannel0, tex[3]);
            mb.AddTriangleVertex(q * 4 + 3);

            mb.SetVertexChannelData(texchannel0, tex[0]);
            mb.AddTriangleVertex(q * 4 + 0);
            mb.SetVertexChannelData(texchannel0, tex[3]);
            mb.AddTriangleVertex(q * 4 + 3);
            mb.SetVertexChannelData(texchannel0, tex[2]);
            mb.AddTriangleVertex(q * 4 + 2);
        }
    }
} 