using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.HitParticle
{
    public class DropFragment : IPoolable
    {
        // projectile state
        public int PoolIndex { get; set; }

        private Mesh mesh;
        private MeshSceneGraphReceipt receipt;
        private Vector3 position;
        private float rotation;
        private Matrix rotationM;
        private Vector3 delta;

        public bool mustBeDeleted;

        public DropFragment() 
        {
            this.mesh = new Mesh();
            this.mesh.Model = AssetLoader.mdl_dropfragment;
            this.mesh.SetInstancingEnabled(true);
            this.mesh.SetCastShadows(false);
        }

        public void Construct(Vector3 o)
        {
            this.position = o;
            this.rotation = (float)Globals.random.NextDouble() * MathHelper.TwoPi;
            this.mustBeDeleted = false;
            this.rotationM = Matrix.CreateRotationY(this.rotation);

            this.delta = Vector3.Transform(new Vector3(0, -1, 0.4f), rotationM);

            this.mesh.Transform = rotationM * rotationM * Matrix.CreateTranslation(this.position);

            if(this.receipt != null)this.receipt.graph.Remove(this.receipt);
            this.receipt = Globals.gameInstance.sceneGraph.Add(mesh);
        }

        public void Update(float ms)
        {
            this.position = this.position + this.delta * (ms / 200);
            if (this.position.Y < 0)
            {
                this.receipt.graph.Remove(this.receipt);
                this.mustBeDeleted = true;
            }
            else
            {
                this.mesh.Transform = rotationM * Matrix.CreateTranslation(this.position);
                this.receipt.graph.Renew(this.receipt);
            }
        }
    }
}
