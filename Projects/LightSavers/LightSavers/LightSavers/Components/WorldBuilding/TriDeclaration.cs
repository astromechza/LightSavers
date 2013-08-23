using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.WorldBuilding
{
    public class TriDeclaration
    {

        public VertexPositionNormalTexture[] vertices;

        public TriDeclaration()
        {
            vertices = new VertexPositionNormalTexture[3];
        }

        public void SetNormal(Vector3 n)
        {
            vertices[0].Normal = n;
            vertices[1].Normal = n;
            vertices[2].Normal = n;
        }

        public void SetPositions(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            vertices[0].Position = v1;
            vertices[1].Position = v2;
            vertices[2].Position = v3;
        }

    }
}
