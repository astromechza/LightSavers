//-----------------------------------------------------------------------------
// Light.cs
//                                              
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;

namespace LightPrePassRenderer
{
    /// <summary>
    /// Class that holds basic information about scene lights
    /// </summary>
    public class Light
    {
        public enum Type
        {
            Point,
            Spot        
        } ;

        private Type _lightType = Type.Point;
        private Matrix _transform = Matrix.Identity;
        private Matrix _invTransform = Matrix.Identity;
        //for point lights, we use a bounding sphere
        private BoundingSphere _boundingSphere;
        //for spot lights, we can use a frustum
        private BoundingFrustum _frustum;
        private float _radius = 1;
        private float _intensity = 1;
        private float _specularIntensity = 1;
        private Color _color = Color.White;
        private float _spotAngle = 45;
        private float _spotExponent = 10;
        private Matrix _projection = Matrix.Identity;
        private Matrix _viewProjection = Matrix.Identity;
        private bool _castShadows;
        private float _shadowDepthBias = 0.0005f;
        private float _shadowDistance = 50;
        private bool _enabled = true;

        /// <summary>
        /// Light radius
        /// </summary>
        public float Radius
        {
            get { return _radius; }
            set 
            {
                _radius = value;
                _boundingSphere.Radius = _radius;
                if (_lightType == Type.Spot)
                    UpdateSpotValues();
            }
        }

        /// <summary>
        /// Light color. We can have lights brighter than our Color.White
        /// if we increase the Intensity parameter
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        /// <summary>
        /// Stores the light's world tranform. For directional and spot lights,
        /// one should use the FORWARD axis to find its direction, and for point
        /// and spot, the TRANSLATION component stores its position
        /// </summary>
        public Matrix Transform
        {
            get { return _transform; }
            set 
            { 
                _transform = value;
                Matrix.Invert(ref _transform, out _invTransform);
                _boundingSphere.Center = _transform.Translation;
                if (_lightType == Type.Spot)
                    UpdateSpotValues();
            }
        }

        /// <summary>
        /// Stores the light type: point, directional or spot.
        /// </summary>
        public Type LightType
        {
            get { return _lightType; }
            set 
            { 
                _lightType = value;
                if (_lightType == Type.Spot)
                    UpdateSpotValues();
            }
        }

        /// <summary>
        /// Our light's color is multiplied by this value before going into
        /// the pipeline, so it allows us colors brighter than Color.White
        /// </summary>
        public float Intensity
        {
            get { return _intensity; }
            set { _intensity = value; }
        }

        /// <summary>
        /// Returns the current bounding sphere that fits this light
        /// </summary>
        public BoundingSphere BoundingSphere
        {
            get { return _boundingSphere; }
        }

        /// <summary>
        /// Defines the spotlight cone angle, in degrees
        /// </summary>
        public float SpotAngle
        {
            get { return _spotAngle; }
            set
            {
                _spotAngle = value;
                if (_lightType == Type.Spot)
                    UpdateSpotValues();
            }
        }

        /// <summary>
        /// Value the defines the sharpness of spotlight border. 
        /// Its not physically based, play around with some values.
        /// </summary>
        public float SpotExponent
        {
            get { return _spotExponent; }
            set { _spotExponent = value; }
        }

        public Matrix Projection
        {
            get { return _projection; }
        }

        public BoundingFrustum Frustum
        {
            get { return _frustum; }
        }

        public bool CastShadows
        {
            get {
                return _castShadows;
            }
            set {
                _castShadows = value;
            }
        }

        public Matrix ViewProjection
        {
            get 
            {
                return _viewProjection;
            }
        }

        public float ShadowDepthBias
        {
            get {
                return _shadowDepthBias;
            }
            set {
                _shadowDepthBias = value;
            }
        }

        public float ShadowDistance
        {
            get { return _shadowDistance; }
            set { _shadowDistance = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public float SpecularIntensity
        {
            get {
                return _specularIntensity;
            }
            set {
                _specularIntensity = value;
            }
        }


        public Light()
        {
            _boundingSphere = new BoundingSphere(_transform.Translation, _radius);
            _frustum = new BoundingFrustum(Matrix.Identity);
            UpdateSpotValues();
        }

        protected void UpdateSpotValues()
        {
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_spotAngle * 2), 1, 0.01f * _radius, _radius);

            Matrix.Multiply(ref _invTransform, ref _projection, out _viewProjection);
            Frustum.Matrix = _viewProjection;
        }
    }
}
