using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Utils.Geometry
{
    public class Collider
    {

        public static bool Collide(CircleF c, RectangleF r)
        {
            // Find the closest point to the circle within the rectangle
            float closestX = MathHelper.Clamp(c.X, r.Left, r.Right);
            float closestY = MathHelper.Clamp(c.Y, r.Top, r.Bottom);

            // Calculate the distance between the circle's center and this closest point
            float distanceX = c.X - closestX;
            float distanceY = c.Y - closestY;

            // If the distance is less than the circle's radius, an intersection occurs
            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
            return distanceSquared < (c.Radius * c.Radius);
        }

        public static bool Collide(CircleF c1, CircleF c2)
        {
            float distanceX = c1.X - c2.X;
            float distanceY = c1.Y - c2.Y;

            float combinedradius = c1.Radius + c2.Radius;

            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
            return distanceSquared < (combinedradius * combinedradius);
        }

        public static bool Collide(RectangleF r1, RectangleF r2)
        {
            if (r1.Right < r2.Left) return false;
            if (r1.Bottom < r2.Top) return false;
            if (r1.Top > r2.Bottom) return false;
            if (r1.Left > r2.Right) return false;
            return true;
        }

    }
}
