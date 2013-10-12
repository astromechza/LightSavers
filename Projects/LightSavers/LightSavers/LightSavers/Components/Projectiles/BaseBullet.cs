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
    public abstract class BaseBullet : IProjectile, IPoolable
    {
        // orientation and movement
        internal Vector3 position;
        internal float rotation;
        internal Matrix rotationM;
        internal float speed;
        private Vector3 delta;
        

        // Mesh and mesh receipt
        internal Mesh mesh;
        internal MeshSceneGraphReceipt modelReceipt;

        // Aginst and state machine
        internal float ageMs;
        internal bool aging;
        internal bool mustBeDeleted;
        internal int maxAgeMs = 1000;

        // projectile state
        public int PoolIndex { get; set; }

        public virtual void Construct(Vector3 startPos, float rotation)
        {
            this.position = startPos;
            this.rotation = rotation;
            this.rotationM = Matrix.CreateRotationY(rotation);

            this.mustBeDeleted = false;
            this.aging = false;
            this.ageMs = 0;
            this.mesh.Transform = rotationM * Matrix.CreateTranslation(this.position);

            this.modelReceipt = Globals.gameInstance.sceneGraph.Add(mesh);

            this.delta = Vector3.Transform(new Vector3(speed, 0, 0), rotationM);
        }

        public void Update(float ms)
        {

            if (aging)
            {
                ageMs += ms;
                if (ageMs > maxAgeMs)
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

        public abstract int GetDamage();

    }
}
