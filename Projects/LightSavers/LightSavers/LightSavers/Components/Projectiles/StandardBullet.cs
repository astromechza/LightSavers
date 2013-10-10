using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using Microsoft.Xna.Framework;
using ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Projectiles
{
    public class StandardBullet : IProjectile, IPoolable
    {
        private RealGame game;
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

        public StandardBullet() 
        {
            this.mesh = new Mesh();
            this.mesh.Model = AssetLoader.mdl_bullet;
            this.mesh.SetInstancingEnabled(true);
            this.mesh.SetCastShadows(false);
        }

        public void Construct(RealGame game, Vector3 startPos, float rotation)
        {
            this.game = game;
            this.position = startPos;
            this.rotation = rotation;
            this.rotationM = Matrix.CreateRotationY(rotation);
            this.delta = Vector3.Transform(new Vector3(0.4f, 0, 0), rotationM);

            this.mustBeDeleted = false;
            this.aging = false;
            this.ageMs = 0;
            this.mesh.Transform = rotationM * Matrix.CreateTranslation(this.position);

            this.modelReceipt = game.sceneGraph.Add(mesh);
        }

        public void Update(float ms)
        {
            if (aging)
            {
                ageMs += ms;
                if (ageMs > 1000)
                {
                    mustBeDeleted = true;
                    modelReceipt.graph.Remove(modelReceipt);
                } 
            }
            else
            {
                Vector3 newposition = new Vector3(position.X, position.Y, position.Z);

                newposition += delta * ms / 16;

                if (game.cellCollider.PointCollides(newposition.X, newposition.Z))
                {
                    aging = true;
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
    }
}
