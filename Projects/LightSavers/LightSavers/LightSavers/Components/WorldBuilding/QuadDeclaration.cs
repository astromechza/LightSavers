using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightSavers.Components.WorldBuilding
{
    public class QuadDeclaration
    {
        public VertexPositionNormalTexture[] vertices;
        public QuadDeclaration()
        {
            vertices = new VertexPositionNormalTexture[4];
        }

        public void SetTextureCorners(TextureCorners tc)
        {
            vertices[0].TextureCoordinate = tc.topleft;     vertices[1].TextureCoordinate = tc.topright;
            vertices[2].TextureCoordinate = tc.bottomleft;  vertices[3].TextureCoordinate = tc.bottomright;
        }

        public void SetNormal(Vector3 n)
        {
            vertices[0].Normal = n;
            vertices[1].Normal = n;
            vertices[2].Normal = n;
            vertices[3].Normal = n;
        }
    }
}
