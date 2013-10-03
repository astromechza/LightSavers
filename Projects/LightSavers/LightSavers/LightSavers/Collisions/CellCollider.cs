using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Collisions
{
    /// <summary>
    /// This is a basic cell collider class. Divides floating point coordinates into 2D bins by flooring them
    /// eg: 0.5, 0.5 -> 0,0
    /// 
    /// This is used as a very fast environmental collision system for player movement and Alien A*
    /// 
    /// Use SetCollision to set a particular cell to be collidable
    /// Use GetCollision to query a point
    /// 
    /// </summary>
    public class CellCollider
    {
        
        private bool[,] collision;
        private float xmax;
        private float ymax;

        public CellCollider(int height, int width)
        {
            collision = new bool[height, width];
            xmax = width;
            ymax = height;
        }

        /// <summary>
        /// Set the cell containing the XY coordinate to be collidable or not collidable
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate in 2D space, Z coordinate when using 3D</param>
        /// <param name="v">The value to set the cell</param>
        public void SetCollision(float x, float y, bool v)
        {
            int xi = (int)x;
            int yi = (int)y;
            collision[yi, xi] = v;
        }

        /// <summary>
        /// Check for a collision in the given cell.
        /// Coordinates outside of the world space always return true.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate in 2D space, Z coordinate when using 3D</param>
        /// <returns>True if there is a collision here</returns>
        public bool GetCollision(float x, float y)
        {
            // check range
            if (x < 0) return true;
            if (y < 0) return true;
            if (x >= xmax) return true;
            if (y >= ymax) return true;

            // get bin
            int xi = (int)x;
            int yi = (int)y;

            // return
            return collision[yi, xi];
        }

    }
}
