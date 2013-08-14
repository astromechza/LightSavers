using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightSavers.Components.WorldBuilding
{
    public enum Orientation
    {
        North,
        South,
        East,
        West
    }

    public class WorldQuadBuilder
    {



        public static QuadDeclaration BuildFloorQuad(Vector3 XZOrigin, float TileSize)
        {
            QuadDeclaration qd = new QuadDeclaration();
            qd.SetNormal(Vector3.Up);
            qd.vertices[0].Position = XZOrigin;
            qd.vertices[1].Position = XZOrigin + (new Vector3(TileSize, 0, 0));
            qd.vertices[2].Position = XZOrigin + (new Vector3(0, 0, TileSize));
            qd.vertices[3].Position = XZOrigin + (new Vector3(TileSize, 0, TileSize));
            return qd;
        }



        public static QuadDeclaration BuildWallQuad(Vector3 XZOrigin, float TileSize, Orientation orientation)
        {
            QuadDeclaration qd = new QuadDeclaration();
            

            switch (orientation)
            {
                case Orientation.North:
                    qd.SetNormal(Vector3.Forward);
                    qd.vertices[0].Position = XZOrigin + (new Vector3(TileSize, WorldSection.WallHeight, 0));
                    qd.vertices[1].Position = XZOrigin + (new Vector3(0, WorldSection.WallHeight, 0));
                    qd.vertices[2].Position = XZOrigin + (new Vector3(TileSize, 0, 0));
                    qd.vertices[3].Position = XZOrigin + (new Vector3(0, 0, 0));
                    break;
                case Orientation.South:
                    qd.SetNormal(Vector3.Backward);
                    qd.vertices[0].Position = XZOrigin + (new Vector3(0, WorldSection.WallHeight, TileSize));
                    qd.vertices[1].Position = XZOrigin + (new Vector3(TileSize, WorldSection.WallHeight, TileSize));
                    qd.vertices[2].Position = XZOrigin + (new Vector3(0, 0, TileSize));
                    qd.vertices[3].Position = XZOrigin + (new Vector3(TileSize, 0, TileSize));
                    break;
                case Orientation.East:
                    qd.SetNormal(Vector3.Right);
                    qd.vertices[0].Position = XZOrigin + (new Vector3(0, WorldSection.WallHeight, 0));
                    qd.vertices[1].Position = XZOrigin + (new Vector3(0, WorldSection.WallHeight, TileSize));
                    qd.vertices[2].Position = XZOrigin + (new Vector3(0, 0, 0));
                    qd.vertices[3].Position = XZOrigin + (new Vector3(0, 0, TileSize));
                    break;
                case Orientation.West:
                    qd.SetNormal(Vector3.Left);
                    qd.vertices[0].Position = XZOrigin + (new Vector3(TileSize, WorldSection.WallHeight, TileSize));
                    qd.vertices[1].Position = XZOrigin + (new Vector3(TileSize, WorldSection.WallHeight, 0));
                    qd.vertices[2].Position = XZOrigin + (new Vector3(TileSize, 0, TileSize));
                    qd.vertices[3].Position = XZOrigin + (new Vector3(TileSize, 0, 0));
                    break;
            }

            return qd;
        }

        public static QuadDeclaration BuildRoofQuad(Vector3 XZOrigin, float TileSize)
        {
            QuadDeclaration qd = BuildFloorQuad(XZOrigin, TileSize);
            qd.vertices[0].Position += new Vector3(0, WorldSection.WallHeight, 0);
            qd.vertices[1].Position += new Vector3(0, WorldSection.WallHeight, 0);
            qd.vertices[2].Position += new Vector3(0, WorldSection.WallHeight, 0);
            qd.vertices[3].Position += new Vector3(0, WorldSection.WallHeight, 0);
            return qd;
        }
    }
}
