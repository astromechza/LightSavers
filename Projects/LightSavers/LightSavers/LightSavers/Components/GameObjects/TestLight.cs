using LightSavers.Components.WorldBuilding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    public class TestLight : GameObject
    {
        private const float radius = 0.01f;

        private VertexIndiceSet VIS;

        public TestLight(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;

            BuildGeometry(X, Y, Z);
        }

        public void BuildGeometry(float x, float y, float z)
        {
            List<TriDeclaration> tris = new List<TriDeclaration>();

            Vector3 top = new Vector3(x, y + radius, z);
            Vector3 bottom = new Vector3(x, y - radius, z);

            Vector3 forward = new Vector3(x, y, z-radius);
            Vector3 backward = new Vector3(x, y, z+radius);

            Vector3 left = new Vector3(x - radius, y, z);
            Vector3 right = new Vector3(x + radius, y, z);

            TriDeclaration t1 = new TriDeclaration(); t1.SetPositions(top, left, forward);
            TriDeclaration t2 = new TriDeclaration(); t2.SetPositions(top, forward, right);
            TriDeclaration t3 = new TriDeclaration(); t3.SetPositions(top, right, backward);
            TriDeclaration t4 = new TriDeclaration(); t4.SetPositions(top, backward, left);

            tris.Add(t1);
            tris.Add(t2);
            tris.Add(t3);
            tris.Add(t4);

            VIS = VertexIndiceSet.Build(tris);
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
