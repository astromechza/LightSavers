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
    /// <summary>
    /// This ContentProcessor is designed to convert a bmp file into a model and to embed textures and defines
    /// </summary>
    [ContentProcessor(DisplayName = "WorldSectionProcessor - LightPrePassProcessor")]
    public class WorldSectionProcessor : ContentProcessor<Texture2DContent, ModelContent>
    {        
        // Texture 
        private const string floortexturefile = "textures/floortexture.png";
        private const string walltexturefile = "textures/walltexture.png";
        private const string blacktexturefile = "textures/black.bmp";
        private const string floortexturenormalfile = "textures/floortexture_normal.png";

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

            return context.Convert<NodeContent, ModelContent>(nodeContent, typeof(LightPrePassProcessor).Name);
        }

        private NodeContent buildFloorMesh(PixelBitmapContent<Color> bitmap)
        {
            MeshBuilder mb = MeshBuilder.StartMesh("floor");

            // Create a material, and point it at the world section texture
            BasicMaterialContent material = new BasicMaterialContent();

            material.Textures.Add(LightPrePassProcessor.DiffuseMapKey, new ExternalReference<TextureContent>(floortexturefile));
            material.Textures.Add(LightPrePassProcessor.NormalMapKey, new ExternalReference<TextureContent>(floortexturenormalfile));


            mb.SetMaterial(material);

            // Create data channels
            int channel_texCoord0 = mb.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            // First create vertex data
            
            // loop through all the pixels
            int quadcount = 0;
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {

                    if (IsFloorTile(bitmap, x, y))
                    {
                        quadcount += AddQuadVertexPositions(mb, new Vector3(-16 + x, 0, -16 + y), new Vector3(-16 + x + 1.0f, 0, -16 + y + 1.0f));
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

            material.Textures.Add(LightPrePassProcessor.DiffuseMapKey, new ExternalReference<TextureContent>(walltexturefile));
            
            mb.SetMaterial(material);

            int channel_texCoord0 = mb.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            // First create vertex data
            // loop through all the pixels
            int quadcount = 0;
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    if (IsWallTile(bitmap, x, y))
                    {
                        bool leftWall = IsFloorTileSafe(bitmap, x - 1, y);
                        bool rightWall = IsFloorTileSafe(bitmap, x + 1, y);
                        bool backWall = IsFloorTileSafe(bitmap, x, y - 1);
                        bool frontWall = IsFloorTileSafe(bitmap, x, y + 1);

                        if (leftWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(-16 + x, WallHeight, -16 + y), new Vector3(-16 + x, 0.0f, -16 + y + 1.0f));
                        }

                        if (rightWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(-16 + x + 1.0f, WallHeight, -16 + y + 1.0f), new Vector3(-16 + x + 1.0f, 0.0f, -16 + y));
                        }

                        if (frontWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(-16 + x, WallHeight, -16 + y + 1.0f), new Vector3(-16 + x + 1.0f, 0.0f, -16 + y + 1.0f));
                        }

                        if (backWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(-16 + x + 1.0f, WallHeight, -16 + y), new Vector3(-16 + x, 0.0f, -16 + y));
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

            material.Textures.Add(LightPrePassProcessor.DiffuseMapKey, new ExternalReference<TextureContent>(blacktexturefile));

            mb.SetMaterial(material);

            int channel_texCoord0 = mb.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            // First create vertex data
            // loop through all the pixels
            int quadcount = 0;
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    if (IsWallTile(bitmap, x, y))
                    {

                        quadcount += AddQuadVertexPositions(mb, new Vector3(-16 + x, WallHeight, -16 + y), new Vector3(-16 + x + 1.0f, WallHeight, -16 + y + 1.0f));

                        bool leftWall = IsNullTileSafe(bitmap, x-1, y);
                        bool rightWall = IsNullTileSafe(bitmap, x + 1, y);
                        bool backWall = IsNullTileSafe(bitmap, x, y - 1);
                        bool frontWall = IsNullTileSafe(bitmap, x, y + 1);

                        if (leftWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(-16 + x, WallHeight, -16 + y), new Vector3(-16 + x, 0.0f, -16 + y + 1.0f));
                        }

                        if (rightWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(-16 + x + 1.0f, WallHeight, -16 + y + 1.0f), new Vector3(-16 + x + 1.0f, 0.0f, -16 + y));
                        }

                        if (frontWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(-16 + x, WallHeight, -16 + y + 1.0f), new Vector3(-16 + x + 1.0f, 0.0f, -16 + y + 1.0f));
                        }

                        if (backWall)
                        {
                            quadcount += AddQuadVertexPositions(mb, new Vector3(-16 + x + 1.0f, WallHeight, -16 + y), new Vector3(-16 + x, 0.0f, -16 + y));
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

        private bool IsFloorTile(PixelBitmapContent<Color> bitmap, int x, int y)
        {
            return !(IsWallTile(bitmap, x, y) || IsNullTile(bitmap, x, y));
        }

        private bool IsWallTile(PixelBitmapContent<Color> bitmap, int x, int y)
        {
            return bitmap.GetPixel(x * 3, y * 3) == Color.Blue;
        }

        private bool IsNullTile(PixelBitmapContent<Color> bitmap, int x, int y)
        {
            return bitmap.GetPixel(x * 3, y * 3) == Color.Black;
        }

        // Safe type checking : checks if the tile is in the bitmap and handles accordingly
        #region safe tile type checking methods
        private bool IsFloorTileSafe(PixelBitmapContent<Color> bitmap, int x, int y)
        {
            if (IsNotInBitmap(bitmap, x, y)) return false;
            return IsFloorTile(bitmap, x, y);
        }

        private bool IsWallTileSafe(PixelBitmapContent<Color> bitmap, int x, int y)
        {
            if (IsNotInBitmap(bitmap, x, y)) return false;
            return IsWallTile(bitmap, x, y);
        }

        private bool IsNullTileSafe(PixelBitmapContent<Color> bitmap, int x, int y)
        {
            if (IsNotInBitmap(bitmap, x, y)) return true;
            return IsNullTile(bitmap, x, y);
        }
        #endregion

        private bool IsNotInBitmap(PixelBitmapContent<Color> bitmap, int x, int y)
        {
            if (x < 0) return true;
            if (y < 0) return true;
            if (x > 31) return true;
            if (y > 31) return true;
            return false;
        }
    
    }
} 