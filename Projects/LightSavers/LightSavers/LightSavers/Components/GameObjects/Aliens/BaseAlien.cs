using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Utils.Geometry;
using Microsoft.Xna.Framework;
using SkinnedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects.Aliens
{
    public class BaseAlien : GameObject
    {
        // state machine
        public enum AlienState { ALIVE, DEAD, DYING };
        public AlienState _state;

        // mesh
        public SkinnedMesh _mesh;
        public MeshSceneGraphReceipt _modelReceipt;
        
        // animation
        public DurationBasedAnimator _aplayer;
        
        // movement
        public float _rotation;
        public float _targetRotation;
        public Vector3 _targetPosition;
        public Vector3 _positionDelta;
        public RectangleF _collisionRectangle;

        // transform vars
        private Vector3 _verticalOffset = Vector3.Zero;
        private Matrix _scaleTransform = Matrix.Identity;

        // spawning / dying / destruction
        public int _health;
        public BaseSpawner _spawner;
        public bool _mustBeDeleted;

        #region ACCESSORS

        public Vector3 VerticalOffset { get { return _verticalOffset;} set {_verticalOffset = value;} }

        public Matrix ScaleMatrix { get { return _scaleTransform; } set { _scaleTransform = value; } }

        #endregion


        public virtual void Construct(Vector3 spawnPosition, float rotation, BaseSpawner spawner)
        {
            this._mustBeDeleted = false;
            this._position = spawnPosition;
            this._rotation = rotation;
            this._spawner = spawner;
        }

        public void UpdateAnimations(float ms)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)ms);
            _aplayer.Update(ts, true, Matrix.Identity, Matrix.Identity);
            _mesh.BoneMatrixes = _aplayer.GetSkinTransforms();
        }

        public void UpdateMajorTransform()
        {
            _mesh.Transform = _scaleTransform * Matrix.CreateRotationY(_rotation + (float)Math.PI) * Matrix.CreateTranslation(_position + _verticalOffset);   
        }

        public override void Update(float millis) { }

        public RectangleF GetBoundRect()
        {
            return _collisionRectangle;
        }

        public void UpdateReceipt()
        {
            _modelReceipt.graph.Renew(_modelReceipt);
        }

        public void DestroyReceipt()
        {
            _modelReceipt.graph.Remove(_modelReceipt);
        }
    }
}
