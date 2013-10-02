using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Collisions
{
    public class CellCollider
    {

        private bool[,] collision;
        float xmax;
        float ymax;

        public CellCollider(int height, int width)
        {
            collision = new bool[height, width];
            xmax = width;
            ymax = height;
        }

        public void SetCollision(float x, float y, bool v)
        {
            int xi = (int)x;
            int yi = (int)y;
            collision[yi, xi] = v;
        }

        public bool GetCollision(float x, float y)
        {
            if (x < 0) return true;
            if (y < 0) return true;
            if (x >= xmax) return true;
            if (y >= ymax) return true;

            int xi = (int)x;
            int yi = (int)y;

            return collision[yi, xi];
        }

    }
}
