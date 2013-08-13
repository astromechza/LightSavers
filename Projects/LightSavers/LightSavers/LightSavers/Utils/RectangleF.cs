using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers
{
    public class RectangleF
    {
        float _x, _y, _width, _length;

        public float x { get { return _x; } }
        public float y { get { return _y; } }
        public float width { get { return _width; } }
        public float length { get { return _length; } }

        public float x2 { get { return _x + _width; } }
        public float y2 { get { return _y + _length; } }


        public RectangleF()
        {
            _x = 0;
            _y = 0;
            _width = 0;
            _length = 0;
        }

        public RectangleF(float x, float y, float width, float length)
        {
            _x = x;
            _y = y;
            _width = width;
            _length = length;
        }

        public void setX(float x)
        {
            _x = x;
        }

        public void setY(float y)
        {
            _y = y;
        }


    }
}
