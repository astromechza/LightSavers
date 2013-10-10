using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Utils.Geometry
{
    public class RectangleF
    {
        float _x, _y, _width, _length;


        public float Left 
        { 
            get { return _x; }
            set { _x = value; }
        }
        public float Top 
        { 
            get { return _y; }
            set { _y = value; }
        }
        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public float Length
        {
            get { return _length; }
            set { _length = value; }
        }
        public float Right 
        { 
            get { return _x + _width; } 
        }
        public float Bottom 
        { 
            get { return _y + _length; } 
        }


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

    }
}
