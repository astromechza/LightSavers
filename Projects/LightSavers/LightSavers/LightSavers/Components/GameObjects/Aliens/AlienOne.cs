using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Components.Projectiles;
using LightSavers.Utils;
using LightSavers.Utils.Geometry;
using Microsoft.Xna.Framework;
using SkinnedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects.Aliens
{
    public class AlienOne : BaseAlien
    {
        public AlienOne() { }

        public AlienOne(Vector3 spawnPosition, BaseSpawner spawner)
        {
            Construct(spawnPosition, (float)Globals.random.NextDouble() * MathHelper.TwoPi, spawner);
        }

        public override void Construct(Vector3 spawnPosition, float rotation, BaseSpawner spawner)
        {
            base.Construct(spawnPosition, rotation, spawner);            

            this._health = 100;

            this._mesh = new SkinnedMesh();
            this._mesh.Model = AssetLoader.mdl_alien1;

            this._aplayer = new DurationBasedAnimator(_mesh.SkinningData, _mesh.SkinningData.AnimationClips["Take 001"], null);

            this._aplayer.AddAnimationPackage = AssetLoader.ani_alien1;
            this._aplayer.StartClip(2);

            this.VerticalOffset = new Vector3(0, 0.8f, 0);
            this.ScaleMatrix = Matrix.CreateScale(0.6f);

            UpdateAnimations(0);
            UpdateMajorTransform();

            Globals.gameInstance.sceneGraph.Setup(_mesh);
            this._modelReceipt = Globals.gameInstance.sceneGraph.Add(_mesh);

            this._targetPosition = new Vector3();
            AssignRandomTarget();

            this._collisionRectangle = new RectangleF(0, 0, 1.0f, 1.0f);
            RebuildCollisionRectangle(_position);
        }

        public override void Update(float ms)
        {
            #region TURN towards rotation target
            float deltarotation = _targetRotation - _rotation;

            // sanitise
            if (deltarotation > Math.PI) deltarotation -= MathHelper.TwoPi;
            if (deltarotation < -Math.PI) deltarotation += MathHelper.TwoPi;

            // add difference
            _rotation += (ms / 300) * deltarotation;
            #endregion

            #region CHECK distance to target, choose another if close
            float distance = (_targetPosition - _position).LengthSquared();
            if (distance < 0.4f)
            {
                AssignRandomTarget();
            }
            #endregion

            // If target is in front, then it can move
            if (deltarotation < 0.15f)
            {
                // calculate new position based on delta
                Vector3 newpos = _position + _positionDelta * (ms / 200);
                RebuildCollisionRectangle(newpos);

                //TODO: collision check here
                if (!Globals.gameInstance.cellCollider.RectangleCollides(_collisionRectangle))
                {
                    _position = newpos;
                }
                else
                {
                    // pick another target
                    AssignRandomTarget();
                }
            }

            UpdateAnimations(ms * 1.5f); // animations are accelerated a bit
            UpdateMajorTransform();
            _modelReceipt.graph.Renew(_modelReceipt);

            // check for collision with bullet
            IProjectile p = Globals.gameInstance.projectileManager.CheckHit(this);
            if (p != null)
            {
                p.PreDestroy();
                p.Destroy();
                this._health -= p.GetDamage();
                if (this._health < 0)
                {
                    this._mustBeDeleted = true;
                    this.DestroyReceipt();
                }
            }

        }


        private void RebuildCollisionRectangle(Vector3 o)
        {
            _collisionRectangle.Left = o.X - 0.5f;
            _collisionRectangle.Top = o.Z - 0.5f;
        }

        public void AssignRandomTarget()
        {
            _targetPosition.X = this._position.X + (float)Globals.random.NextDouble() * 10 - 5;
            _targetPosition.Z = this._position.Z + (float)Globals.random.NextDouble() * 10 - 5;
            _positionDelta = _targetPosition - _position;
            _positionDelta.Normalize();
            _targetRotation = (float)Math.Atan2(-_positionDelta.Z, _positionDelta.X) + MathHelper.PiOver2;
        }

        
    }
}
