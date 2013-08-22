using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    public class TestLight : GameObject
    {
        public TestLight(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override RectangleF GetBoundRect()
        {
            return new RectangleF(X - 0.01f, Y - 0.01f, 0.02f, 0.02f);
        }

        public override void Update()
        {
            // do nothing for now
        }

        public override void Draw()
        {
            // draw shit
        }


    }
}
