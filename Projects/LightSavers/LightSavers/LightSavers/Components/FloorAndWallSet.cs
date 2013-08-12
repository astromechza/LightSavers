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


            List<VertexPositionNormalTexture> floorvertexlist = new List<VertexPositionNormalTexture>();
            List<short> indexlist = new List<short>();


            tiles = new TileType[tilesz, tilesx];
            for (int z = 0; z < tilesz; z++)
            {                
                for (int x = 0; x < tilesx; x++)
                {
                    tiles[z,x] = (TileType)arrangement[z,x];

                    if (tiles[z, x] == TileType.Floor)
                    {

                        // v1 v2
                        // v3 v4

                        VertexPositionNormalTexture v1 = new VertexPositionNormalTexture();
                        v1.Position = XZOrigin + (new Vector3(x, 0, z) * cellsize);
                        v1.Normal = Vector3.Up;
                        v1.TextureCoordinate = new Vector2(0.5f, 0.5f);

                        VertexPositionNormalTexture v2 = new VertexPositionNormalTexture();
                        v2.Position = XZOrigin + (new Vector3(x + 1, 0, z) * cellsize);
                        v2.Normal = Vector3.Up;
                        v2.TextureCoordinate = new Vector2(1, 0.5f);

                        VertexPositionNormalTexture v3 = new VertexPositionNormalTexture();
                        v3.Position = XZOrigin + (new Vector3(x, 0, z + 1) * cellsize);
                        v3.Normal = Vector3.Up;
                        v3.TextureCoordinate = new Vector2(0.5f, 1);

                        VertexPositionNormalTexture v4 = new VertexPositionNormalTexture();
                        v4.Position = XZOrigin + (new Vector3(x + 1, 0, z + 1) * cellsize);
                        v4.Normal = Vector3.Up;
                        v4.TextureCoordinate = new Vector2(1, 1);

                        int vind = floorvertexlist.Count;

                        floorvertexlist.Add(v1);
                        floorvertexlist.Add(v2);
                        floorvertexlist.Add(v3);
                        floorvertexlist.Add(v4);

                        indexlist.Add((short)(vind + 0));
                        indexlist.Add((short)(vind + 3));
                        indexlist.Add((short)(vind + 2));
                        indexlist.Add((short)(vind + 0));
                        indexlist.Add((short)(vind + 1));
                        indexlist.Add((short)(vind + 3));

                    }
                    else
                    {

                        VertexPositionNormalTexture v1 = new VertexPositionNormalTexture();
                        v1.Position = XZOrigin + (new Vector3(x, 0, z) * cellsize) + Vector3.Up*4;
                        v1.Normal = Vector3.Up;
                        v1.TextureCoordinate = new Vector2(0, 0);

                        VertexPositionNormalTexture v2 = new VertexPositionNormalTexture();
                        v2.Position = XZOrigin + (new Vector3(x + 1, 0, z) * cellsize) + Vector3.Up * 4;
                        v2.Normal = Vector3.Up;
                        v2.TextureCoordinate = new Vector2(0.5f, 0);

                        VertexPositionNormalTexture v3 = new VertexPositionNormalTexture();
                        v3.Position = XZOrigin + (new Vector3(x, 0, z + 1) * cellsize) + Vector3.Up * 4;
                        v3.Normal = Vector3.Up;
                        v3.TextureCoordinate = new Vector2(0, 0.5f);

                        VertexPositionNormalTexture v4 = new VertexPositionNormalTexture();
                        v4.Position = XZOrigin + (new Vector3(x + 1, 0, z + 1) * cellsize) + Vector3.Up * 4;
                        v4.Normal = Vector3.Up;
                        v4.TextureCoordinate = new Vector2(0.5f, 0.5f);

                        int vind = floorvertexlist.Count;

                        floorvertexlist.Add(v1);
                        floorvertexlist.Add(v2);
                        floorvertexlist.Add(v3);
                        floorvertexlist.Add(v4);

                        indexlist.Add((short)(vind + 0));
                        indexlist.Add((short)(vind + 3));
                        indexlist.Add((short)(vind + 2));
                        indexlist.Add((short)(vind + 0));
                        indexlist.Add((short)(vind + 1));
                        indexlist.Add((short)(vind + 3));


                    }
                }
            }

            vertices = floorvertexlist.ToArray();

            indices = indexlist.ToArray();
            

        }



        public class QuadDeclaration
        {
            VertexPositionNormalTexture[] vertices;

            public QuadDeclaration()
            {
                vertices = new VertexPositionNormalTexture[4];
            }

            


        }


    }

    
}
