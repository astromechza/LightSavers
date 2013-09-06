using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LightSavers.Components.WorldBuilding;
using Microsoft.Xna.Framework.Graphics;
using LightSavers.Components.Shader;

namespace LightSavers.Components
{
    /// <summary>
    /// Each WorldSection is a 32x32 'block' area that is drawn together. 
    /// WorldSections act as one level of detail in collision grid and also 
    /// make it very easy to test world collisions using a simple array lookup
    /// and can be treated as a sort of height map as well.
    /// 
    /// WorldSections are loaded from 32x32 pixel bitmap image files using the colours:
    /// BLACK ( 000000 ) : No tile, blank empty space
    /// WHITE ( FFFFFF ) : Floor tile, randomly chosen texture
    /// BLUE ( FF0000 ) : Wall tile, this corrospondes to a full height tile with intelligent wall quads.
    /// </summary>
    public class WorldSection
    {
        // Constants
        public const float TileSize = 1.0f;     // size of each tile
        public const float WallHeight = 2.0f;   // height of walls

        // Tile grid
        private Tile.TileType[,] tiles;
        private int tilesX;                     // number of tiles in the x direction
        private int tilesZ;                     // number of tiles in the y direction
        private Vector3 origin;                 // top left corner

        // Geometry
        private VertexIndiceSet floorVIS;       // floor vertex set
        private VertexIndiceSet wallVIS;        // wall vertex set
        private VertexIndiceSet blackVIS;       // black tile vertex set

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The 32x32 colour array from the WorldContainer Load method</param>
        /// <param name="tilesX">Number of tiles in the left-right direction. should be 32 </param>
        /// <param name="tilesZ">Number of tiles in the up-down direction. should be 32 </param>
        /// <param name="origin">Topleft corner of the section </param>
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

            // now create geometry based on tile colours
            CreateGeometry();       
            
        }


        private void CreateGeometry()
        {
            // = Generate some texture corners to make it easy to randomise floor and wall textures
            // Floors
            TextureCornersFactory floorTCF = new TextureCornersFactory();
            floorTCF.Add(TextureCorners.BuildProb(new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.5f), 0.25f));
            floorTCF.Add(TextureCorners.BuildProb(new Vector2(0.5f, 0.0f), new Vector2(1.0f, 0.5f), 0.25f));
            floorTCF.Add(TextureCorners.BuildProb(new Vector2(0.0f, 0.5f), new Vector2(0.5f, 1.0f), 0.25f));
            floorTCF.Add(TextureCorners.BuildProb(new Vector2(0.5f, 0.5f), new Vector2(1.0f, 1.0f), 0.25f));
            // Walls
            TextureCornersFactory wallTCF = new TextureCornersFactory();
            wallTCF.Add(TextureCorners.BuildProb(new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.5f), 0.85f));
            wallTCF.Add(TextureCorners.BuildProb(new Vector2(0.5f, 0.0f), new Vector2(1.0f, 0.5f), 0.05f));
            wallTCF.Add(TextureCorners.BuildProb(new Vector2(0.0f, 0.5f), new Vector2(0.5f, 1.0f), 0.05f));
            wallTCF.Add(TextureCorners.BuildProb(new Vector2(0.5f, 0.5f), new Vector2(1.0f, 1.0f), 0.05f));

            // empty geometry lists
            List<QuadDeclaration> walls = new List<QuadDeclaration>();
            List<QuadDeclaration> floors = new List<QuadDeclaration>();
            List<QuadDeclaration> blackquads = new List<QuadDeclaration>();

            // For each tile in the grid
            for (int z = 0; z < tilesZ; z++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    // calculate its topleft corner
                    Vector3 XZOrigin = new Vector3(x, 0, z) * TileSize;
                    // build quads based on tile
                    switch (tiles[z, x])
                    {
                        // Floor is each. Just a quad on the floor level
                        case Tile.TileType.Floor:
                        {
                            QuadDeclaration qd = WorldQuadBuilder.BuildFloorQuad(XZOrigin, TileSize);
                            qd.SetTextureCorners(floorTCF.Get());
                            floors.Add(qd);
                            break;
                        }
                        // Wall is a little harder
                        case Tile.TileType.Wall:
                        {
                            // Black quad at top level
                            QuadDeclaration qdr = WorldQuadBuilder.BuildRoofQuad(XZOrigin, TileSize);
                            qdr.SetTextureCorners(TextureCorners.Build(new Vector2(0, 0), new Vector2(1, 1)));
                            blackquads.Add(qdr);

                            // West East North and South walls
                            if (x > 0 && tiles[z, x - 1] == Tile.TileType.Floor)
                            {
                                QuadDeclaration qdW = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.West);
                                qdW.SetTextureCorners(wallTCF.Get());
                                walls.Add(qdW);
                            }
                            if (x < (tilesX - 1) && tiles[z, x + 1] == Tile.TileType.Floor)
                            {
                                QuadDeclaration qdE = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.East);
                                qdE.SetTextureCorners(wallTCF.Get());
                                walls.Add(qdE);
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

                            // black walls for edge tiles                            
                            if ((x < (tilesX - 1) && tiles[z, x + 1] == Tile.TileType.Empty) || (x == tilesX - 1))
                            {
                                QuadDeclaration qdE = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.East);
                                qdE.SetTextureCorners(wallTCF.Get());
                                blackquads.Add(qdE);
                            }
                            if ((x > 0 && tiles[z, x - 1] == Tile.TileType.Empty) || (x == 0))
                            {
                                QuadDeclaration qdW = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.West);
                                qdW.SetTextureCorners(wallTCF.Get());
                                blackquads.Add(qdW);
                            }
                            if ((z > 0 && tiles[z - 1, x] == Tile.TileType.Empty) || (z == 0))
                            {
                                QuadDeclaration qdN = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.North);
                                qdN.SetTextureCorners(wallTCF.Get());
                                blackquads.Add(qdN);
                            }
                            if ((z < (tilesZ - 1) && tiles[z + 1, x] == Tile.TileType.Empty) || (z == tilesZ - 1))
                            {
                                QuadDeclaration qdS = WorldQuadBuilder.BuildWallQuad(XZOrigin, TileSize, Orientation.South);
                                qdS.SetTextureCorners(wallTCF.Get());
                                blackquads.Add(qdS);
                            }

                            break;
                        }
                    }
                }
            }

            // Build geometry sets using quad declarations
            floorVIS = VertexIndiceSet.Build(floors);
            wallVIS = VertexIndiceSet.Build(walls);
            blackVIS = VertexIndiceSet.Build(blackquads);
        }

        public void Draw(Camera camera, TestShader shader)
        {
            // Set the world matrix
            shader.WorldMatrix.SetValue(Matrix.CreateTranslation(origin));

            // set the texture
            shader.CurrentTexture.SetValue(AssetLoader.tex_floors);

            // draw VIS sets
            // NO array length checking is done here as it is a critical error
            foreach (EffectPass pass in shader.Effect.CurrentTechnique.Passes)
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

            shader.CurrentTexture.SetValue(AssetLoader.tex_walls);
            foreach (EffectPass pass in shader.Effect.CurrentTechnique.Passes)
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

            shader.CurrentTexture.SetValue(AssetLoader.tex_black);
            foreach (EffectPass pass in shader.Effect.CurrentTechnique.Passes)
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
