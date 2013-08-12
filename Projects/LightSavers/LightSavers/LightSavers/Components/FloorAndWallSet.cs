using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightSavers.Components
{

    /*
     *      
     * 
     *  11111111111111111
     *  10000000000000000
     *  10000000000000000
     *  10000000000000000
     *  10000000000000000
     *  10000000011111111
     *  1000000001
     *  1000000001
     *  10000000011111111
     *  10000000000000000
     *  10000000000000000
     *  11000000000000000         this sort of thing but 64x64
     *  00000000000000000
     *  00000000000000000
     *  00000000000000000
     *  00000000000000000
     *  11000000000000000
     *  10000000000000000
     *  11111111111111111
     * 
     *     
    */

    public class FloorAndWallSet
    {

        public enum TileType
        {
            Floor,       // 0
            Wall       // 1
        }

        public TileType[,] tiles;

        public VertexPositionNormalTexture[] vertices;
        public short[] indices;

        public int tilesx;
        public int tilesz;

        public FloorAndWallSet(Vector3 XZOrigin, int [,] arrangement,  float cellsize)
        {
            tilesz = arrangement.GetLength(0);
            tilesx = arrangement.GetLength(1);


            List<QuadDeclaration> floorQuads = new List<QuadDeclaration>();
            List<QuadDeclaration> wallAndRoofQuads = new List<QuadDeclaration>();
            List<short> indexlist = new List<short>();

            VertexDeclaration[,] possiblevertices = new VertexDeclaration[tilesz + 1, tilesx + 1];
            for (int z = 0; z < tilesz + 1; z++)
            {
                for (int x = 0; x < tilesx + 1; x++)
                {
                    possiblevertices[z, x] = new VertexDeclaration();
                    possiblevertices[z, x].v.Position = XZOrigin + new Vector3(x, 0, z) * cellsize;
                    possiblevertices[z, x].v.Normal = Vector3.Up;
                    possiblevertices[z, x].v.TextureCoordinate = new Vector2(x % 2, z % 2);
                }
            }

            tiles = new TileType[tilesz, tilesx];
            for (int z = 0; z < tilesz; z++)
            {                
                for (int x = 0; x < tilesx; x++)
                {
                    tiles[z,x] = (TileType)arrangement[z,x];     
                }
            }

            for (int z = 0; z < tilesz; z++)
            {
                for (int x = 0; x < tilesx; x++)
                {
                    if (tiles[z, x] == TileType.Floor)
                    {

                        QuadDeclaration qd = new QuadDeclaration();

                        qd.vertices[0].Position = XZOrigin + (new Vector3(x, 0, z) * cellsize);
                        qd.vertices[0].Normal = Vector3.Up;
                        qd.vertices[0].TextureCoordinate = new Vector2(0.5f, 0.5f);

                        qd.vertices[1].Position = XZOrigin + (new Vector3(x + 1, 0, z) * cellsize);
                        qd.vertices[1].Normal = Vector3.Up;
                        qd.vertices[1].TextureCoordinate = new Vector2(1, 0.5f);

                        qd.vertices[2].Position = XZOrigin + (new Vector3(x, 0, z + 1) * cellsize);
                        qd.vertices[2].Normal = Vector3.Up;
                        qd.vertices[2].TextureCoordinate = new Vector2(0.5f, 1);

                        qd.vertices[3].Position = XZOrigin + (new Vector3(x + 1, 0, z + 1) * cellsize);
                        qd.vertices[3].Normal = Vector3.Up;
                        qd.vertices[3].TextureCoordinate = new Vector2(1, 1);

                        floorQuads.Add(qd);
                    }
                    else if (tiles[z, x] == TileType.Wall)
                    {

                        QuadDeclaration qdt = new QuadDeclaration();

                        qdt.vertices[0].Position = XZOrigin + (new Vector3(x, 1, z) * cellsize);
                        qdt.vertices[0].Normal = Vector3.Up;
                        qdt.vertices[0].TextureCoordinate = new Vector2(0.5f, 0.5f);

                        qdt.vertices[1].Position = XZOrigin + (new Vector3(x + 1, 1, z) * cellsize);
                        qdt.vertices[1].Normal = Vector3.Up;
                        qdt.vertices[1].TextureCoordinate = new Vector2(1, 0.5f);

                        qdt.vertices[2].Position = XZOrigin + (new Vector3(x, 1, z + 1) * cellsize);
                        qdt.vertices[2].Normal = Vector3.Up;
                        qdt.vertices[2].TextureCoordinate = new Vector2(0.5f, 1);

                        qdt.vertices[3].Position = XZOrigin + (new Vector3(x + 1, 1, z + 1) * cellsize);
                        qdt.vertices[3].Normal = Vector3.Up;
                        qdt.vertices[3].TextureCoordinate = new Vector2(1, 1);

                        wallAndRoofQuads.Add(qdt);





                        // must make south wall
                        if (z < (tilesz - 1) && tiles[z + 1, x] != TileType.Wall)
                        {

                            QuadDeclaration qd = new QuadDeclaration();

                            qd.vertices[0].Position = XZOrigin + (new Vector3(x, 1, z + 1) * cellsize);
                            qd.vertices[0].Normal = Vector3.Backward;
                            qd.vertices[0].TextureCoordinate = new Vector2(0.5f, 0.5f);

                            qd.vertices[1].Position = XZOrigin + (new Vector3(x + 1, 1, z + 1) * cellsize);
                            qd.vertices[1].Normal = Vector3.Backward;
                            qd.vertices[1].TextureCoordinate = new Vector2(1, 0.5f);

                            qd.vertices[2].Position = XZOrigin + (new Vector3(x, 0, z + 1) * cellsize);
                            qd.vertices[2].Normal = Vector3.Backward;
                            qd.vertices[2].TextureCoordinate = new Vector2(0.5f, 1);

                            qd.vertices[3].Position = XZOrigin + (new Vector3(x + 1, 0, z + 1) * cellsize);
                            qd.vertices[3].Normal = Vector3.Backward;
                            qd.vertices[3].TextureCoordinate = new Vector2(1, 1);

                            wallAndRoofQuads.Add(qd);
                        }

                        // must make west wall
                        if (x < (tilesx - 1) && tiles[z, x+1] != TileType.Wall)
                        {

                            QuadDeclaration qd = new QuadDeclaration();

                            qd.vertices[0].Position = XZOrigin + (new Vector3(x+1, 1, z+1) * cellsize);
                            qd.vertices[0].Normal = Vector3.Right;
                            qd.vertices[0].TextureCoordinate = new Vector2(0.5f, 0.5f);

                            qd.vertices[1].Position = XZOrigin + (new Vector3(x+1, 1, z) * cellsize);
                            qd.vertices[1].Normal = Vector3.Right;
                            qd.vertices[1].TextureCoordinate = new Vector2(1, 0.5f);

                            qd.vertices[2].Position = XZOrigin + (new Vector3(x+1, 0, z+1) * cellsize);
                            qd.vertices[2].Normal = Vector3.Right;
                            qd.vertices[2].TextureCoordinate = new Vector2(0.5f, 1);

                            qd.vertices[3].Position = XZOrigin + (new Vector3(x+1, 0, z) * cellsize);
                            qd.vertices[3].Normal = Vector3.Right;
                            qd.vertices[3].TextureCoordinate = new Vector2(1, 1);

                            wallAndRoofQuads.Add(qd);
                        }

                        // must make east wall
                        if (x > 0 && tiles[z, x - 1] != TileType.Wall)
                        {

                            QuadDeclaration qd = new QuadDeclaration();

                            qd.vertices[0].Position = XZOrigin + (new Vector3(x, 1, z) * cellsize);
                            qd.vertices[0].Normal = Vector3.Left;
                            qd.vertices[0].TextureCoordinate = new Vector2(0.5f, 0.5f);

                            qd.vertices[1].Position = XZOrigin + (new Vector3(x, 1, z+1) * cellsize);
                            qd.vertices[1].Normal = Vector3.Left;
                            qd.vertices[1].TextureCoordinate = new Vector2(1, 0.5f);

                            qd.vertices[2].Position = XZOrigin + (new Vector3(x, 0, z) * cellsize);
                            qd.vertices[2].Normal = Vector3.Left;
                            qd.vertices[2].TextureCoordinate = new Vector2(0.5f, 1);

                            qd.vertices[3].Position = XZOrigin + (new Vector3(x, 0, z+1) * cellsize);
                            qd.vertices[3].Normal = Vector3.Left;
                            qd.vertices[3].TextureCoordinate = new Vector2(1, 1);

                            wallAndRoofQuads.Add(qd);
                        }





                    }
                }
            }

            

            vertices = new VertexPositionNormalTexture[floorQuads.Count * 4 + wallAndRoofQuads.Count * 4];
            int vi = 0;
            foreach (QuadDeclaration qd in floorQuads)
            {
                vertices[vi++] = qd.vertices[0];
                vertices[vi++] = qd.vertices[1];
                vertices[vi++] = qd.vertices[2];
                vertices[vi++] = qd.vertices[3];
            }
            foreach (QuadDeclaration qd in wallAndRoofQuads)
            {
                vertices[vi++] = qd.vertices[0];
                vertices[vi++] = qd.vertices[1];
                vertices[vi++] = qd.vertices[2];
                vertices[vi++] = qd.vertices[3];
            }

            indices = new short[floorQuads.Count * 6 + wallAndRoofQuads.Count * 6];
            int ii = 0;
            int qi = 0;
            foreach (QuadDeclaration qd in floorQuads)
            {
                indices[ii++] = (short)qi;
                indices[ii++] = (short)(qi + 3);
                indices[ii++] = (short)(qi + 2);
                indices[ii++] = (short)qi;
                indices[ii++] = (short)(qi + 1);
                indices[ii++] = (short)(qi + 3);
                qi+=4;
            }

            foreach (QuadDeclaration qd in wallAndRoofQuads)
            {
                indices[ii++] = (short)qi;
                indices[ii++] = (short)(qi + 3);
                indices[ii++] = (short)(qi + 2);
                indices[ii++] = (short)qi;
                indices[ii++] = (short)(qi + 1);
                indices[ii++] = (short)(qi + 3);
                qi += 4;
            }

            

        }



        public class QuadDeclaration
        {
            public VertexPositionNormalTexture[] vertices;

            public QuadDeclaration()
            {
                vertices = new VertexPositionNormalTexture[4];
            }

            


        }

        public class VertexDeclaration
        {
            public VertexPositionNormalTexture v;
            public short assignedIndex;
            public bool used;

            public VertexDeclaration()
            {
                v = new VertexPositionNormalTexture();
            }
        }


    }

    
}
