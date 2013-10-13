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
    public class AlienOne : BaseAlien
    {

        Vector3 _roamingTarget = new Vector3();
        PlayerObject _targetPlayer = null;
        LiveState _livestate;

        public AlienOne() { }

        public AlienOne(Vector3 spawnPosition, CampaignSection section)
        {
            Construct(spawnPosition, (float)Globals.random.NextDouble() * MathHelper.TwoPi, section);
        }

        public override void Construct(Vector3 spawnPosition, float rotation, CampaignSection section)
        {
            base.Construct(spawnPosition, rotation, section);

            this._state = AlienState.ALIVE;
            this._livestate = LiveState.ROAMING;
            this._health = 90;

            this._mesh = new SkinnedMesh();
            this._mesh.Model = AssetLoader.mdl_alien1;

            this._aplayer = new DurationBasedAnimator(_mesh.SkinningData, _mesh.SkinningData.AnimationClips["Take 001"], null);

            this._aplayer.AddAnimationPackage = AssetLoader.ani_alien1;
            this._aplayer.StartClip(Animation_States.moving);

            this.VerticalOffset = new Vector3(0, 0.8f, 0);
            this.ScaleMatrix = Matrix.CreateScale(0.6f);

            UpdateAnimations(0);
            UpdateMajorTransform();

            Globals.gameInstance.sceneGraph.Setup(_mesh);
            this._modelReceipt = Globals.gameInstance.sceneGraph.Add(_mesh);

            _collisionRectangle = new RectangleF(0,0,1.0f,1.0f);
        }

        public override void Update(float ms)
        {

            if (_state == AlienState.ALIVE)
            {
                // check bullet collision
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

                if (_livestate == LiveState.ROAMING)
                {
                    // check distance to target
                    if (Vector3.DistanceSquared(_targetPosition, _position) < 0.3f)
                    {
                        // new target
                        _targetPosition = new Vector3(
                            MathHelper.Clamp(
                                _position.X + (float)Globals.random.NextDouble() * 10 - 5, 
                                _section.Index * 32, 
                                _section.Index * 32 + 32
                            ),
                            _position.Y,
                            _position.Z + (float)Globals.random.NextDouble() * 10 - 5                            
                        );
                    }

                    // check player LOS
                    PlayerObject closestPlayer = Globals.gameInstance.GetClosestPlayer(_position);
                    if (Vector3.DistanceSquared(closestPlayer.Position, _position) <= 6*6)
                    {
                        //check LOS
                        _livestate = LiveState.CHASING;
                        _targetPlayer = closestPlayer;
                    }
                }

                if (_livestate == LiveState.CHASING)
                {
                    _targetPosition = _targetPlayer.Position;
                    float d = Vector3.DistanceSquared(_targetPosition, _position);
                    if (d > 6 * 6)
                    {
                        _livestate = LiveState.ROAMING;
                        _targetPosition = _position;
                    }
                    else 
                    {
                        //check LOS
                        bool LOS = true;
                        if (LOS)
                        {
                            if (d < 1.0f)
                            {
                                _livestate = LiveState.ATTACKING;
                                _aplayer.StartClip(Animation_States.attacking);
                            }
                        }
                        else
                        {
                            _livestate = LiveState.ROAMING;
                            _targetPosition = _position;
                        }
                    }               
                }
                if (_livestate == LiveState.ATTACKING)
                {
                    _targetPosition = _position;
                    RotateToFacePosition(_targetPlayer.Position);
                    if (_aplayer.GetLoopCount() > 0)
                    {
                        _livestate = LiveState.CHASING;
                        _aplayer.StartClip(Animation_States.moving);
                    }
                }
                
                if (_targetPosition != _position)
                {
                    _velocity = _targetPosition - _position;
                    _velocity.Normalize();
                    RotateToFacePosition(_velocity);
                    Vector3 newpos = _position + _velocity * (ms / 200);

                    RebuildCollisionRectangle(newpos);
                    if (Globals.gameInstance.cellCollider.RectangleCollides(_collisionRectangle))
                    {
                        _targetPosition = _position;
                    }
                    else if (Globals.gameInstance.campaignManager.CollideCurrentEntities(this))
                    {
                        _targetPosition = _position;
                    }
                    else if (Globals.gameInstance.CollidesPlayers(this))
                    {
                        _targetPosition = _position;
                    }
                    else
                    {
                        _position = newpos;
                    }

                }
                UpdateAnimations(ms * 1.5f);
                UpdateMajorTransform();
                _modelReceipt.graph.Renew(_modelReceipt);
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

        private void RotateToFacePosition(Vector3 o)
        {
            Vector3 t = _targetPosition - _position;
            t.Normalize();
            if (t.LengthSquared() > 0.01f) _rotation = (float)Math.Atan2(-t.Z, t.X) + MathHelper.PiOver2;

        }

        
    }
}
