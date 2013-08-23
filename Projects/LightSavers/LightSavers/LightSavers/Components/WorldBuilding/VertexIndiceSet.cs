using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightSavers.Components.WorldBuilding;
using Microsoft.Xna.Framework.Graphics;

namespace LightSavers.Components
{
    public class VertexIndiceSet
    {
        public VertexPositionNormalTexture[] vertices;
        public short[] indices;
        public int numtriangles;

        public static VertexIndiceSet Build(List<QuadDeclaration> quads)
        {
            VertexIndiceSet vis = new VertexIndiceSet();
            vis.vertices = new VertexPositionNormalTexture[quads.Count * 4];
            vis.indices = new short[quads.Count * 6];

            int vi = 0;
            int ii = 0;
            foreach (QuadDeclaration qd in quads)
            {
                vis.indices[ii++] = (short)(vi);
                vis.indices[ii++] = (short)(vi + 3);
                vis.indices[ii++] = (short)(vi + 2);
                vis.indices[ii++] = (short)(vi);
                vis.indices[ii++] = (short)(vi + 1);
                vis.indices[ii++] = (short)(vi + 3);

                vis.vertices[vi++] = qd.vertices[0];
                vis.vertices[vi++] = qd.vertices[1];
                vis.vertices[vi++] = qd.vertices[2];
                vis.vertices[vi++] = qd.vertices[3];
            }

            return vis; 
        }

        public static VertexIndiceSet Build(List<TriDeclaration> tris)
        {
            VertexIndiceSet vis = new VertexIndiceSet();
            vis.vertices = new VertexPositionNormalTexture[tris.Count * 3];
            vis.indices = new short[tris.Count * 3];

            int vi = 0;
            int ii = 0;
            foreach (TriDeclaration tri in tris)
            {
                vis.indices[ii++] = (short)(vi);
                vis.indices[ii++] = (short)(vi + 1);
                vis.indices[ii++] = (short)(vi + 2);

                vis.vertices[vi++] = tri.vertices[0];
                vis.vertices[vi++] = tri.vertices[1];
                vis.vertices[vi++] = tri.vertices[2];
            }

            return vis;
        }
    }
}
