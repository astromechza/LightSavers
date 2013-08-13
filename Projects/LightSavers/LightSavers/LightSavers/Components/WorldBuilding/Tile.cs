using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightSavers.Components
{
    public class Tile
    {
        public enum TileType
        {
            Empty,
            Floor,       
            Wall       
        }

        public static TileType GetTileForColor(Color c)
        {
            switch (c.PackedValue)
            {
                case 0xFFFFFFFF:
                    return TileType.Floor;
                case 0xFFFF0000:
                    return TileType.Wall;
                default:
                    return TileType.Empty;
            }
        }
    }
}
