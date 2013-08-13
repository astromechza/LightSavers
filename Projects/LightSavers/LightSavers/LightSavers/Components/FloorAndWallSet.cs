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

        public VertexPositionNormalTexture[] floorvertices;
        public short[] floorindices;

        public VertexPositionNormalTexture[] wallvertices;
        public short[] wallindices;

        public VertexPositionNormalTexture[] roofvertices;
        public short[] roofindices;

        public int tilesx;
        public int tilesz;

        private static int wallhigh = 2;


        public FloorAndWallSet(Vector3 XZOrigin, int [,] arrangement,  float cellsize)
        {
            tilesz = arrangement.GetLength(0);
            tilesx = arrangement.GetLength(1);


            List<QuadDeclaration> floorQuads = new List<QuadDeclaration>();
            List<QuadDeclaration> wallQuads = new List<QuadDeclaration>();
            List<QuadDeclaration> roofQuads = new List<QuadDeclaration>();
            List<short> indexlist = new List<short>();

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

                        qdt.vertices[0].Position = XZOrigin + (new Vector3(x, wallhigh, z) * cellsize);
                        qdt.vertices[0].Normal = Vector3.Up;
                        qdt.vertices[0].TextureCoordinate = new Vector2(0.5f, 0.5f);

                        qdt.vertices[1].Position = XZOrigin + (new Vector3(x + 1, wallhigh, z) * cellsize);
                        qdt.vertices[1].Normal = Vector3.Up;
                        qdt.vertices[1].TextureCoordinate = new Vector2(1, 0.5f);

                        qdt.vertices[2].Position = XZOrigin + (new Vector3(x, wallhigh, z + 1) * cellsize);
                        qdt.vertices[2].Normal = Vector3.Up;
                        qdt.vertices[2].TextureCoordinate = new Vector2(0.5f, 1);

                        qdt.vertices[3].Position = XZOrigin + (new Vector3(x + 1, wallhigh, z + 1) * cellsize);
                        qdt.vertices[3].Normal = Vector3.Up;
                        qdt.vertices[3].TextureCoordinate = new Vector2(1, 1);

                        roofQuads.Add(qdt);

                        // must make south wall
                        if (z > 1 && tiles[z - 1, x] != TileType.Wall)
                        {

                            QuadDeclaration qd = new QuadDeclaration();

                            qd.vertices[0].Position = XZOrigin + (new Vector3(x + 1, wallhigh, z) * cellsize);
                            qd.vertices[0].Normal = Vector3.Backward;
                            qd.vertices[0].TextureCoordinate = new Vector2(0.5f, 0.5f);

                            qd.vertices[1].Position = XZOrigin + (new Vector3(x, wallhigh, z) * cellsize);
                            qd.vertices[1].Normal = Vector3.Backward;
                            qd.vertices[1].TextureCoordinate = new Vector2(1, 0.5f);

                            qd.vertices[2].Position = XZOrigin + (new Vector3(x+1, 0, z) * cellsize);
                            qd.vertices[2].Normal = Vector3.Backward;
                            qd.vertices[2].TextureCoordinate = new Vector2(0.5f, 1);

                            qd.vertices[3].Position = XZOrigin + (new Vector3(x, 0, z) * cellsize);
                            qd.vertices[3].Normal = Vector3.Backward;
                            qd.vertices[3].TextureCoordinate = new Vector2(1, 1);

                            wallQuads.Add(qd);
                        }

                        // must make south wall
                        if (z < (tilesz - 1) && tiles[z + 1, x] != TileType.Wall)
                        {

                            QuadDeclaration qd = new QuadDeclaration();

                            qd.vertices[0].Position = XZOrigin + (new Vector3(x, wallhigh, z + 1) * cellsize);
                            qd.vertices[0].Normal = Vector3.Backward;
                            qd.vertices[0].TextureCoordinate = new Vector2(0.5f, 0.5f);

                            qd.vertices[1].Position = XZOrigin + (new Vector3(x + 1, wallhigh, z + 1) * cellsize);
                            qd.vertices[1].Normal = Vector3.Backward;
                            qd.vertices[1].TextureCoordinate = new Vector2(1, 0.5f);

                            qd.vertices[2].Position = XZOrigin + (new Vector3(x, 0, z + 1) * cellsize);
                            qd.vertices[2].Normal = Vector3.Backward;
                            qd.vertices[2].TextureCoordinate = new Vector2(0.5f, 1);

                            qd.vertices[3].Position = XZOrigin + (new Vector3(x + 1, 0, z + 1) * cellsize);
                            qd.vertices[3].Normal = Vector3.Backward;
                            qd.vertices[3].TextureCoordinate = new Vector2(1, 1);

                            wallQuads.Add(qd);
                        }

                        // must make west wall
                        if (x < (tilesx - 1) && tiles[z, x+1] != TileType.Wall)
                        {

                            QuadDeclaration qd = new QuadDeclaration();

                            qd.vertices[0].Position = XZOrigin + (new Vector3(x + 1, wallhigh, z + 1) * cellsize);
                            qd.vertices[0].Normal = Vector3.Right;
                            qd.vertices[0].TextureCoordinate = new Vector2(0.5f, 0.5f);

                            qd.vertices[1].Position = XZOrigin + (new Vector3(x + 1, wallhigh, z) * cellsize);
                            qd.vertices[1].Normal = Vector3.Right;
                            qd.vertices[1].TextureCoordinate = new Vector2(1, 0.5f);

                            qd.vertices[2].Position = XZOrigin + (new Vector3(x+1, 0, z+1) * cellsize);
                            qd.vertices[2].Normal = Vector3.Right;
                            qd.vertices[2].TextureCoordinate = new Vector2(0.5f, 1);

                            qd.vertices[3].Position = XZOrigin + (new Vector3(x+1, 0, z) * cellsize);
                            qd.vertices[3].Normal = Vector3.Right;
                            qd.vertices[3].TextureCoordinate = new Vector2(1, 1);

                            wallQuads.Add(qd);
                        }

                        // must make east wall
                        if (x > 0 && tiles[z, x - 1] != TileType.Wall)
                        {

                            QuadDeclaration qd = new QuadDeclaration();

                            qd.vertices[0].Position = XZOrigin + (new Vector3(x, wallhigh, z) * cellsize);
                            qd.vertices[0].Normal = Vector3.Left;
                            qd.vertices[0].TextureCoordinate = new Vector2(0.5f, 0.5f);

                            qd.vertices[1].Position = XZOrigin + (new Vector3(x, wallhigh, z + 1) * cellsize);
                            qd.vertices[1].Normal = Vector3.Left;
                            qd.vertices[1].TextureCoordinate = new Vector2(1, 0.5f);

                            qd.vertices[2].Position = XZOrigin + (new Vector3(x, 0, z) * cellsize);
                            qd.vertices[2].Normal = Vector3.Left;
                            qd.vertices[2].TextureCoordinate = new Vector2(0.5f, 1);

                            qd.vertices[3].Position = XZOrigin + (new Vector3(x, 0, z+1) * cellsize);
                            qd.vertices[3].Normal = Vector3.Left;
                            qd.vertices[3].TextureCoordinate = new Vector2(1, 1);

                            wallQuads.Add(qd);
                        }





                    }
                }
            }

            floorvertices = new VertexPositionNormalTexture[floorQuads.Count * 4];
            floorindices = new short[floorQuads.Count * 6];
            int vi = 0;
            int ii = 0;
            foreach (QuadDeclaration qd in floorQuads)
            {
                floorindices[ii++] = (short)(vi);
                floorindices[ii++] = (short)(vi + 3);
                floorindices[ii++] = (short)(vi + 2);
                floorindices[ii++] = (short)(vi);
                floorindices[ii++] = (short)(vi + 1);
                floorindices[ii++] = (short)(vi + 3);

                floorvertices[vi++] = qd.vertices[0];
                floorvertices[vi++] = qd.vertices[1];
                floorvertices[vi++] = qd.vertices[2];
                floorvertices[vi++] = qd.vertices[3];
            }

            wallvertices = new VertexPositionNormalTexture[wallQuads.Count * 4];
            wallindices = new short[wallQuads.Count * 6];
            vi = 0;
            ii = 0;
            foreach (QuadDeclaration qd in wallQuads)
            {
                wallindices[ii++] = (short)(vi);
                wallindices[ii++] = (short)(vi + 3);
                wallindices[ii++] = (short)(vi + 2);
                wallindices[ii++] = (short)(vi);
                wallindices[ii++] = (short)(vi + 1);
                wallindices[ii++] = (short)(vi + 3);

                wallvertices[vi++] = qd.vertices[0];
                wallvertices[vi++] = qd.vertices[1];
                wallvertices[vi++] = qd.vertices[2];
                wallvertices[vi++] = qd.vertices[3];
            }

            roofvertices = new VertexPositionNormalTexture[roofQuads.Count * 4];
            roofindices = new short[roofQuads.Count * 6];
            vi = 0;
            ii = 0;
            foreach (QuadDeclaration qd in roofQuads)
            {
                roofindices[ii++] = (short)(vi);
                roofindices[ii++] = (short)(vi + 3);
                roofindices[ii++] = (short)(vi + 2);
                roofindices[ii++] = (short)(vi);
                roofindices[ii++] = (short)(vi + 1);
                roofindices[ii++] = (short)(vi + 3);

                roofvertices[vi++] = qd.vertices[0];
                roofvertices[vi++] = qd.vertices[1];
                roofvertices[vi++] = qd.vertices[2];
                roofvertices[vi++] = qd.vertices[3];
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
