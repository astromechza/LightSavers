using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Components.CampainManager;
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
    public class AlienTwo : BaseAlien
    {

        public AlienTwo() { }

        public AlienTwo(Vector3 spawnPosition, CampaignSection section)
        {
            Construct(spawnPosition, (float)Globals.random.NextDouble() * MathHelper.TwoPi, section);
        }

        public override void Construct(Vector3 spawnPosition, float rotation, CampaignSection section)
        {
            base.Construct(spawnPosition, rotation, section);

            this._state = AlienState.ALIVE;
            this._health = 150;

            this._mesh = new SkinnedMesh();
            this._mesh.Model = AssetLoader.mdl_alien2;

            this._aplayer = new DurationBasedAnimator(_mesh.SkinningData, _mesh.SkinningData.AnimationClips["Take 001"], null);

            this._aplayer.AddAnimationPackage = AssetLoader.ani_alien2;
            this._aplayer.StartClip(Animation_States.moving);

            this.VerticalOffset = new Vector3(0, 1f, 0);
            this.ScaleMatrix = Matrix.CreateScale(0.8f);

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
            if (_state == AlienState.ALIVE)
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
                if (Math.Abs(deltarotation) < 0.15f)
                {
                    // calculate new position based on delta
                    Vector3 newpos = _position + _velocity * (ms / 600);
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

                UpdateAnimations(ms); // animations are accelerated a bit
                UpdateMajorTransform();
                _modelReceipt.graph.Renew(_modelReceipt);

                // check for collision with bullet
                IProjectile p = Globals.gameInstance.projectileManager.CheckHit(this);
                if (p != null)
                {
                    p.PreDestroy();
                    p.Destroy();
                    this._health -= p.GetDamage();
                    if (this._health <= 0)
                    {
                        this._state = AlienState.DYING;
                        this._aplayer.StartClip(Animation_States.death);
                        Globals.audioManager.PlayGameSound("aliendeath1");
                    }
                }
            }
            else
            {
                UpdateAnimations(ms * 1.5f); // animations are accelerated a bit

                if (this._aplayer.GetLoopCount() > 0)
                {
                    this._state = AlienState.DEAD;
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
            _targetPosition.X = MathHelper.Clamp(this._position.X + (float)Globals.random.NextDouble() * 10 - 5, this._section.Index*32, this._section.Index*32 +32);
            _targetPosition.Z = this._position.Z + (float)Globals.random.NextDouble() * 10 - 5;
            _velocity = _targetPosition - _position;
            _velocity.Normalize();
            _targetRotation = (float)Math.Atan2(-_velocity.Z, _velocity.X) + MathHelper.PiOver2;
        }

        
    }
}
