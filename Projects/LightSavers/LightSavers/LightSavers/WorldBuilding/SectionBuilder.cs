using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.WorldBuilding
{
    public class SectionBuilder
    {
        public static void Build(string filename, Vector3 origin, LightAndMeshContainer lamc)
        {

            // First add the environment mesh itself
            Mesh mesh = new Mesh();
            mesh.Model = Globals.content.Load<Model>(filename);
            mesh.Transform = Matrix.CreateTranslation(origin);
            lamc.AddMesh(mesh);




        }

    }
}
