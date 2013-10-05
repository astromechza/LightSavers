using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Guns
{
    public class BaseGun
    {
        public Mesh mesh;
        public MeshSceneGraphReceipt receipt;
        public Matrix holdTransform;
        public Vector3 emmitterPosition;

        public BaseGun()
        {
            mesh = null;
            receipt = null;
            holdTransform = Matrix.Identity;
            emmitterPosition = Vector3.Zero;
        }

        public void RenewReceipt(AwesomeSceneGraph b)
        {
            if (receipt != null) receipt.parentlist.Remove(mesh);
            receipt = b.AddMesh(mesh);
        }

        public void SetModel(Model m)
        {
            mesh = new Mesh();
            mesh.Model = m;
        }

        public void SetTransform(Matrix m)
        {
            mesh.Transform = holdTransform * m;
            emmitterPosition = mesh.Transform.Translation;
        }

        public void SetHoldTransform(Matrix t)
        {
            holdTransform = t;
        }


    }
}
