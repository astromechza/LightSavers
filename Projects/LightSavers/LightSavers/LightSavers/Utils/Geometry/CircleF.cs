using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Utils.Geometry
{
    public class CircleF
    {

        float _x, _y, _radius;

        public float X { get { return _x; } }
        public float Y { get { return _y; } }
        public float Radius { get { return _radius; } }

        public CircleF()
        {
            _x = 0;
            _y = 0;
            _radius = 1;
        }

        public CircleF(float x, float y, float radius)
        {
            _x = x;
            _y = y;
            _radius = radius;
        }

        public void SetX(float x)
        {
            _x = x;
        }

        public void SetY(float y)
        {
            _y = y;
        }

        public void SetRadius(float r)
        {
            _radius = r;
        }
                

    }
}
