using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Projectiles
{
    public class PistolBullet : IProjectile, IPoolable
    {
        private Vector3 position;
        private float rotation;
        private Matrix rotationM;
        private Vector3 delta;

        private Mesh mesh;

        private float ageMs;
        private bool aging;
        private bool mustBeDeleted;


        private MeshSceneGraphReceipt modelReceipt;

        // projectile state
        public int PoolIndex { get; set; }

        public PistolBullet() 
        {
            this.mesh = new Mesh();
            this.mesh.Model = AssetLoader.mdl_pistolBullet;
            this.mesh.SetInstancingEnabled(true);
            this.mesh.SetCastShadows(false);
        }

        public void Construct(Vector3 startPos, float rotation)
        {
            this.position = startPos;
            this.rotation = rotation;
            this.rotationM = Matrix.CreateRotationY(rotation);
            this.delta = Vector3.Transform(new Vector3(0.4f, 0, 0), rotationM);

            this.mustBeDeleted = false;
            this.aging = false;
            this.ageMs = 0;
            this.mesh.Transform = rotationM * Matrix.CreateTranslation(this.position);

            this.modelReceipt = Globals.gameInstance.sceneGraph.Add(mesh);
        }

        public void Update(float ms)
        {
            if (aging)
            {
                ageMs += ms;
                if (ageMs > 1000)
                {
                    Destroy();
                } 
            }
            else
            {
                Vector3 newposition = new Vector3(position.X, position.Y, position.Z);

                newposition += delta * ms / 16;

                if (Globals.gameInstance.cellCollider.PointCollides(newposition.X, newposition.Z))
                {
                    aging = true;
                    PreDestroy();
                }

                if (position != newposition)
                {
                    // check if it has moved into another box
                    int oldx = (int)position.X / 32;
                    int newx = (int)newposition.X / 32;

                    position = newposition;
                    mesh.Transform = rotationM * Matrix.CreateTranslation(position);

                    if (oldx != newx)
                    {
                        modelReceipt.graph.Renew(modelReceipt);
                    }

                }
            }

        }

        public bool MustBeDeleted()
        {
            return mustBeDeleted;
        }

        public void Destroy()
        {
            this.mustBeDeleted = true;
            this.modelReceipt.graph.Remove(this.modelReceipt);
        }

        public void PreDestroy()
        {
            Globals.gameInstance.fragmentManager.SpawnX(this.position, 3);
        }

        public Vector3 GetCenter()
        {
            return position;
        }

        public int GetDamage()
        {
            return 30;
        }
    }
}
