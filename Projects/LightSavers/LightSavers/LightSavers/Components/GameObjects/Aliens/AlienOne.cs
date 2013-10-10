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
        private Vector3 VERTICAL_OFFSET = new Vector3(0, 0.8f, 0);
        private Matrix SCALE = Matrix.CreateScale(0.6f);


        public AlienOne(RealGame game, Vector3 spawnPosition) : 
            base(game, spawnPosition, (float)Globals.random.NextDouble() * MathHelper.TwoPi)
        {

            this._mesh = new SkinnedMesh();
            this._mesh.Model = AssetLoader.mdl_alien1;

            this._aplayer = new DurationBasedAnimator(_mesh.SkinningData, _mesh.SkinningData.AnimationClips["Take 001"]);

            this._aplayer.AddAnimationPackage = AssetLoader.ani_alien1;
            this._aplayer.StartClip("moving");

            this.ScaleMatrix = SCALE;
            this.VerticalOffset = VERTICAL_OFFSET;

            UpdateAnimations(0);
            UpdateMajorTransform();

            this._game.sceneGraph.Setup(_mesh);
            this._modelReceipt = game.sceneGraph.Add(_mesh);

            this._targetPosition = new Vector3();
            AssignRandomTarget();

            this._collisionRectangle = new RectangleF(0,0,1.0f,1.0f);
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
                if (!_game.cellCollider.RectangleCollides(_collisionRectangle))
                {
                    _position = newpos;
                }
                else
                {
                    // pick another target
                    AssignRandomTarget();
                }
            }
            
            // check for collision with bullet
            IProjectile p = _game.projectileManager.CheckHit(this);
            if (p != null)
            {
                p.Destroy();
            }

            UpdateAnimations(ms * 1.5f); // animations are accelerated a bit
            UpdateMajorTransform();
        }


        private void RebuildCollisionRectangle(Vector3 o)
        {
            _collisionRectangle.Left = o.X - 0.5f;
            _collisionRectangle.Top = o.Z - 0.5f;
        }

        public void AssignRandomTarget()
        {
            _targetPosition.X = (float)Globals.random.NextDouble() * 15+1;
            _targetPosition.Z = (float)Globals.random.NextDouble() * 25+1;
            _positionDelta = _targetPosition - _position;
            _positionDelta.Normalize();
            _targetRotation = (float)Math.Atan2(-_positionDelta.Z, _positionDelta.X) + MathHelper.PiOver2;
        }

        
    }
}
