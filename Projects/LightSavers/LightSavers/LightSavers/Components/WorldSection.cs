using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LightSavers.Components.WorldBuilding;
using Microsoft.Xna.Framework.Graphics;

namespace LightSavers.Components
{
    public class WorldSection
    {
        // statics
        public const float TileSize = 1.0f;
        public const float WallHeight = 2.0f;

        private Tile.TileType[,] tiles;
        private int tilesX;
        private int tilesZ;
        private Vector3 origin;

        private VertexIndiceSet floorVIS;
        private VertexIndiceSet wallVIS;
        private VertexIndiceSet blackVIS;

        public WorldSection(Color[] data, int tilesX, int tilesZ, Vector3 origin)
        {
            this.tilesX = tilesX;
            this.tilesZ = tilesZ;
            this.origin = origin;

            tiles = new Tile.TileType[tilesZ, tilesX];

            // first create the tile grid
            for (int z = 0; z < tilesZ; z++)
            {
                for (int x = 0; x < tilesX; x++)
                {                    
                    tiles[z, x] = Tile.GetTileForColor(data[z * tilesX + x]);
                }
            }

            #region CREATE TEXTURED TILE QUADS
            TextureCornersFactory floorTCF = new TextureCornersFactory();
            floorTCF.Add(TextureCorners.BuildProb(new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.5f), 0.25f));
            floorTCF.Add(TextureCorners.BuildProb(new Vector2(0.5f, 0.0f), new Vector2(1.0f, 0.5f), 0.25f));
            floorTCF.Add(TextureCorners.BuildProb(new Vector2(0.0f, 0.5f), new Vector2(0.5f, 1.0f), 0.25f));
            floorTCF.Add(TextureCorners.BuildProb(new Vector2(0.5f, 0.5f), new Vector2(1.0f, 1.0f), 0.25f));

            TextureCornersFactory wallTCF = new TextureCornersFactory();
            wallTCF.Add(TextureCorners.BuildProb(new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.5f), 0.85f));
            wallTCF.Add(TextureCorners.BuildProb(new Vector2(0.5f, 0.0f), new Vector2(1.0f, 0.5f), 0.05f));
            wallTCF.Add(TextureCorners.BuildProb(new Vector2(0.0f, 0.5f), new Vector2(0.5f, 1.0f), 0.05f));
            wallTCF.Add(TextureCorners.BuildProb(new Vector2(0.5f, 0.5f), new Vector2(1.0f, 1.0f), 0.05f));

            List<QuadDeclaration> walls = new List<QuadDeclaration>();
            List<QuadDeclaration> floors = new List<QuadDeclaration>();
            List<QuadDeclaration> blackquads = new List<QuadDeclaration>();


            for (int z = 0; z < tilesZ; z++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    Vector3 XZOrigin = this.origin + new Vector3(x,0,z) * TileSize;
                    // build quads based on tile
                    switch (tiles[z, x])
                    {
                        case Tile.TileType.Floor:
                            QuadDeclaration qd = WorldQuadBuilder.BuildFloorQuad(XZOrigin, TileSize);
                            qd.SetTextureCorners(floorTCF.Get());
                            floors.Add(qd);
                            break;
                        case Tile.TileType.Wall:

                            if (x > 0 && tiles[z, x - 1] == Tile.TileType.Floor)
                            {
                                QuadDeclaration qdE = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.East);
                                qdE.SetTextureCorners(wallTCF.Get());
                                walls.Add(qdE);
                            }
                            if (x < (tilesX - 1) && tiles[z, x + 1] == Tile.TileType.Floor)
                            {
                                QuadDeclaration qdW = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.West);
                                qdW.SetTextureCorners(wallTCF.Get());
                                walls.Add(qdW);
                            }
                            if (z > 0 && tiles[z - 1, x] == Tile.TileType.Floor)
                            {
                                QuadDeclaration qdN = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.North);
                                qdN.SetTextureCorners(wallTCF.Get());
                                walls.Add(qdN);
                            }
                            if (z < (tilesZ - 1) && tiles[z + 1, x] == Tile.TileType.Floor)
                            {
                                QuadDeclaration qdS = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.South);
                                qdS.SetTextureCorners(wallTCF.Get());
                                walls.Add(qdS);
                            }

                            // black walls
                            if ((x > 0 && tiles[z, x - 1] == Tile.TileType.Empty) || (x == 0))
                            {
                                QuadDeclaration qdE = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.East);
                                qdE.SetTextureCorners(wallTCF.Get());
                                blackquads.Add(qdE);
                            }
                            if ((x < (tilesX - 1) && tiles[z, x + 1] == Tile.TileType.Empty) || (x == tilesX-1))
                            {
                                QuadDeclaration qdW = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.West);
                                qdW.SetTextureCorners(wallTCF.Get());
                                blackquads.Add(qdW);
                            }
                            if ((z > 0 && tiles[z - 1, x] == Tile.TileType.Empty) || (z==0))
                            {
                                QuadDeclaration qdN = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.North);
                                qdN.SetTextureCorners(wallTCF.Get());
                                blackquads.Add(qdN);
                            }
                            if ((z < (tilesZ - 1) && tiles[z + 1, x] == Tile.TileType.Empty) || (z == tilesZ -1))
                            {
                                QuadDeclaration qdS = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.South);
                                qdS.SetTextureCorners(wallTCF.Get());
                                blackquads.Add(qdS);
                            }
                            
                            QuadDeclaration qdr = WorldQuadBuilder.BuildRoofQuad(XZOrigin, TileSize);
                            qdr.SetTextureCorners(TextureCorners.Build(new Vector2(0,0), new Vector2(1,1)));
                            blackquads.Add(qdr);                            
                            break;
                    }
                }
            }

            floorVIS = VertexIndiceSet.Build(floors);
            wallVIS = VertexIndiceSet.Build(walls);
            blackVIS = VertexIndiceSet.Build(blackquads);
            #endregion
        
            
        }





        public void Draw(Camera camera, BasicEffect quadEffect)
        {
            quadEffect.TextureEnabled = true;
            quadEffect.Texture = AssetLoader.tex_floors;
            foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Globals.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList, 
                    floorVIS.vertices, 
                    0,
                    floorVIS.vertices.Length, 
                    floorVIS.indices, 
                    0,
                    floorVIS.indices.Length / 3
                );
            }

            quadEffect.Texture = AssetLoader.tex_walls;
            foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Globals.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    wallVIS.vertices,
                    0,
                    wallVIS.vertices.Length,
                    wallVIS.indices,
                    0,
                    wallVIS.indices.Length / 3
                );
            }

            quadEffect.Texture = AssetLoader.tex_black;
            foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Globals.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    blackVIS.vertices,
                    0,
                    blackVIS.vertices.Length,
                    blackVIS.indices,
                    0,
                    blackVIS.indices.Length / 3
                );
            }
        }
    }
}
