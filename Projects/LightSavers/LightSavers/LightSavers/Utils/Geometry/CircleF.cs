using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Utils.Geometry
{
    public class CircleF
    {

        float _x, _y, _radius;

        public float X 
        { 
            get { return _x; }
            set { _x = value; }
        }
        public float Y 
        {
            get { return _y; }
            set { _y = value; }
        }
        public float Radius 
        {
            get { return _radius; }
            set { _radius = value; }
        }

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
    }
}
