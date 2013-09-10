//-----------------------------------------------------------------------------
// ShadowRenderer.cs
//
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightPrePassRenderer
{
    internal class ShadowRenderer
    {
        /// <summary>
        /// This struct holds all information to render the shadows
        /// </summary>
        internal class SpotShadowMapEntry
        {
            public RenderTarget2D Texture;
            public Matrix LightViewProjection;
        }

        internal class CascadeShadowMapEntry
        {
            public RenderTarget2D Texture;
            public Matrix[] LightViewProjectionMatrices = new Matrix[NUM_CSM_SPLITS];
            public Vector2[] LightClipPlanes = new Vector2[NUM_CSM_SPLITS];
        }


#if XBOX360
        private const int CASCADE_SHADOW_RESOLUTION = 640;
#else
        private const int CASCADE_SHADOW_RESOLUTION = 1024;
#endif
        private const int NUM_CSM_SPLITS = 3;

        //temporary variables to help on cascade shadow maps
        float[] splitDepthsTmp = new float[NUM_CSM_SPLITS + 1];
        Vector3[] frustumCornersWS = new Vector3[8];
        Vector3[] frustumCornersVS = new Vector3[8];
        Vector3[] splitFrustumCornersVS = new Vector3[8];

        /// <summary>
        /// Store the frustum as planes, as we are tweaking the far plane
        /// </summary>
        Plane[] _directionalClippingPlanes = new Plane[6];
        /// <summary>
        /// Temporary frustum, to create the clipping planes above
        /// </summary>
        BoundingFrustum _tempFrustum = new BoundingFrustum(Matrix.Identity);

        private List<SpotShadowMapEntry> _spotShadowMaps = new List<SpotShadowMapEntry>();
        private List<CascadeShadowMapEntry> _cascadeShadowMaps = new List<CascadeShadowMapEntry>();
        private List<Mesh.SubMesh> _visibleMeshes = new List<Mesh.SubMesh>(100);
        private const int NUM_SPOT_SHADOWS = 4;
        private const int NUM_CSM_SHADOWS = 1;
        private const int SPOT_SHADOW_RESOLUTION = 512;
        
        private int _currentFreeSpotShadowMap;
        private int _currentFreeCascadeShadowMap;

        public ShadowRenderer(Renderer renderer)
        {
            //create the render targets
            for (int i = 0; i < NUM_SPOT_SHADOWS; i++)
            {
                SpotShadowMapEntry entry = new SpotShadowMapEntry();
                //we store the linear depth, in a float render target. We need also the HW zbuffer
                entry.Texture = new RenderTarget2D(renderer.GraphicsDevice, SPOT_SHADOW_RESOLUTION,
                                                   SPOT_SHADOW_RESOLUTION, false, SurfaceFormat.Single,
                                                   DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
                entry.LightViewProjection = Matrix.Identity;
                _spotShadowMaps.Add(entry);
            }
            for (int i=0; i< NUM_CSM_SHADOWS; i++)
            {
                CascadeShadowMapEntry entry = new CascadeShadowMapEntry();
                entry.Texture = new RenderTarget2D(renderer.GraphicsDevice, CASCADE_SHADOW_RESOLUTION * NUM_CSM_SPLITS,
                                                   CASCADE_SHADOW_RESOLUTION, false, SurfaceFormat.Single,
                                                   DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
                _cascadeShadowMaps.Add(entry);
            }
        }

        public void InitFrame()
        {
            _currentFreeSpotShadowMap = 0;
            _currentFreeCascadeShadowMap = 0;
        }

        /// <summary>
        /// Returns an unused shadow map, or null if we run out of SMs
        /// </summary>
        /// <returns></returns>
        internal SpotShadowMapEntry GetFreeSpotShadowMap()
        {
            if (_currentFreeSpotShadowMap < _spotShadowMaps.Count)
            {
               return _spotShadowMaps[_currentFreeSpotShadowMap++];
            }
            return null;
        }

        /// <summary>
        /// Returns an unused cascade shadow map, or null if we run out of SMs
        /// </summary>
        /// <returns></returns>
        internal CascadeShadowMapEntry GetFreeCascadeShadowMap()
        {
            if (_currentFreeCascadeShadowMap < _cascadeShadowMaps.Count)
            {
                return _cascadeShadowMaps[_currentFreeCascadeShadowMap++];
            }
            return null;
        }

        /// <summary>
        /// Generate the shadow map for a given spot light
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="meshes"></param>
        /// <param name="light"></param>
        /// <param name="shadowMap"></param>
        public void GenerateShadowTextureSpotLight(Renderer renderer, RenderWorld renderWorld, Light light, SpotShadowMapEntry shadowMap)
        {
            //bind the render target
            renderer.GraphicsDevice.SetRenderTarget(shadowMap.Texture);
            //clear it to white, ie, far far away
            renderer.GraphicsDevice.Clear(Color.White);
            renderer.GraphicsDevice.BlendState = BlendState.Opaque;
            renderer.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


            Matrix viewProj = light.ViewProjection;
            shadowMap.LightViewProjection = viewProj;

            BoundingFrustum frustum = light.Frustum;

            _visibleMeshes.Clear();
            //cull meshes outside the light volume
            renderWorld.GetShadowCasters(frustum, _visibleMeshes);

            for (int index = 0; index < _visibleMeshes.Count; index++)
            {
                Mesh.SubMesh subMesh = _visibleMeshes[index];
                //render it
                subMesh.RenderShadowMap(ref viewProj, renderer.RenderStatistics, renderer.GraphicsDevice);
                
            }
        }

        /// <summary>
        /// Generate the cascade shadow map for a given directional light
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="meshes"> All meshes in the world</param>
        /// <param name="light"></param>
        /// <param name="cascadeShadowMap"></param>
        /// <param name="camera"></param>
        public void GenerateShadowTextureDirectionalLight(Renderer renderer, RenderWorld renderWorld, Light light, CascadeShadowMapEntry cascadeShadowMap, Camera camera)
        {
            //bind the render target
            renderer.GraphicsDevice.SetRenderTarget(cascadeShadowMap.Texture);
            //clear it to white, ie, far far away
            renderer.GraphicsDevice.Clear(Color.White);
            renderer.GraphicsDevice.BlendState = BlendState.Opaque;
            renderer.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Get the corners of the frustum
            camera.Frustum.GetCorners(frustumCornersWS);
            Matrix eyeTransform = camera.EyeTransform;
            Vector3.Transform(frustumCornersWS, ref eyeTransform, frustumCornersVS);


            float near = camera.NearClip, far = MathHelper.Min(camera.FarClip, light.ShadowDistance);

            splitDepthsTmp[0] = near;
            splitDepthsTmp[NUM_CSM_SPLITS] = far;

            //compute each distance the way you like...
            for (int i = 1; i < splitDepthsTmp.Length - 1; i++)
                splitDepthsTmp[i] = near + (far - near) * (float)Math.Pow((i / (float)NUM_CSM_SPLITS), 1.6f);


            Viewport splitViewport = new Viewport();
            Vector3 lightDir = -Vector3.Normalize(light.Transform.Forward);


            PlaneIntersectionType intersectionType;

            
            for (int i = 0; i < NUM_CSM_SPLITS; i++)
            {
                cascadeShadowMap.LightClipPlanes[i].X = -splitDepthsTmp[i];
                cascadeShadowMap.LightClipPlanes[i].Y = -splitDepthsTmp[i + 1];

                cascadeShadowMap.LightViewProjectionMatrices[i] = CreateLightViewProjectionMatrix(lightDir, camera, splitDepthsTmp[i], splitDepthsTmp[i + 1], i);
                Matrix viewProj = cascadeShadowMap.LightViewProjectionMatrices[i];
                _tempFrustum.Matrix = viewProj;
                //we tweak the near plane, so keep all other planes
                _directionalClippingPlanes[0] = _tempFrustum.Left;
                _directionalClippingPlanes[1] = _tempFrustum.Right;
                _directionalClippingPlanes[2] = _tempFrustum.Bottom;
                _directionalClippingPlanes[3] = _tempFrustum.Top;
                _directionalClippingPlanes[4] = _tempFrustum.Far;
                //the near clipping plane is set inside the CreateLightViewProjectionMatrix method, keep it

                // Set the viewport for the current split     
                splitViewport.MinDepth = 0;
                splitViewport.MaxDepth = 1;
                splitViewport.Width = CASCADE_SHADOW_RESOLUTION;
                splitViewport.Height = CASCADE_SHADOW_RESOLUTION;
                splitViewport.X = i * CASCADE_SHADOW_RESOLUTION;
                splitViewport.Y = 0;
                renderer.GraphicsDevice.Viewport = splitViewport;

                _tempFrustum.Matrix = viewProj;

                _visibleMeshes.Clear();

                renderWorld.GetShadowCasters(_tempFrustum, _directionalClippingPlanes, _visibleMeshes);
                for (int index = 0; index < _visibleMeshes.Count; index++)
                {
                    Mesh.SubMesh subMesh = _visibleMeshes[index];
                    //render it
                    subMesh.RenderShadowMap(ref viewProj, renderer.RenderStatistics, renderer.GraphicsDevice);
                }
            }
        }

        /// <summary>
        /// Creates the WorldViewProjection matrix from the perspective of the 
        /// light using the cameras bounding frustum to determine what is visible 
        /// in the scene.
        /// </summary>
        /// <returns>The WorldViewProjection for the light</returns>
        Matrix CreateLightViewProjectionMatrix(Vector3 lightDir, Camera camera, float minZ, float maxZ, int index)
        {
            for (int i = 0; i < 4; i++)
                splitFrustumCornersVS[i] = frustumCornersVS[i + 4] * (minZ / camera.FarClip);

            for (int i = 4; i < 8; i++)
                splitFrustumCornersVS[i] = frustumCornersVS[i] * (maxZ / camera.FarClip);

            Matrix cameraMat = camera.Transform;
            Vector3.Transform(splitFrustumCornersVS, ref cameraMat, frustumCornersWS);

            // Matrix with that will rotate in points the direction of the light
            Vector3 cameraUpVector = Vector3.Up;
            if (Math.Abs(Vector3.Dot(cameraUpVector, lightDir)) > 0.9f)
                cameraUpVector = Vector3.Forward;

            Matrix lightRotation = Matrix.CreateLookAt(Vector3.Zero,
                                                       -lightDir,
                                                       cameraUpVector);


            // Transform the positions of the corners into the direction of the light
            for (int i = 0; i < frustumCornersWS.Length; i++)
            {
                frustumCornersWS[i] = Vector3.Transform(frustumCornersWS[i], lightRotation);
            }


            // Find the smallest box around the points
            Vector3 mins = frustumCornersWS[0], maxes = frustumCornersWS[0];
            for (int i = 1; i < frustumCornersWS.Length; i++)
            {
                Vector3 p = frustumCornersWS[i];
                if (p.X < mins.X) mins.X = p.X;
                if (p.Y < mins.Y) mins.Y = p.Y;
                if (p.Z < mins.Z) mins.Z = p.Z;
                if (p.X > maxes.X) maxes.X = p.X;
                if (p.Y > maxes.Y) maxes.Y = p.Y;
                if (p.Z > maxes.Z) maxes.Z = p.Z;
            }


            // Find the smallest box around the points in view space
            Vector3 minsVS = splitFrustumCornersVS[0], maxesVS = splitFrustumCornersVS[0];
            for (int i = 1; i < splitFrustumCornersVS.Length; i++)
            {
                Vector3 p = splitFrustumCornersVS[i];
                if (p.X < minsVS.X) minsVS.X = p.X;
                if (p.Y < minsVS.Y) minsVS.Y = p.Y;
                if (p.Z < minsVS.Z) minsVS.Z = p.Z;
                if (p.X > maxesVS.X) maxesVS.X = p.X;
                if (p.Y > maxesVS.Y) maxesVS.Y = p.Y;
                if (p.Z > maxesVS.Z) maxesVS.Z = p.Z;
            }
            BoundingBox _lightBox = new BoundingBox(mins, maxes);

            bool fixShadowJittering = true;
            if (fixShadowJittering)
            {
                // I borrowed this code from some forum that I don't remember anymore =/
                // We snap the camera to 1 pixel increments so that moving the camera does not cause the shadows to jitter.
                // This is a matter of integer dividing by the world space size of a texel
                float diagonalLength = (frustumCornersWS[0] - frustumCornersWS[6]).Length();
                diagonalLength += 2; //Without this, the shadow map isn't big enough in the world.
                float worldsUnitsPerTexel = diagonalLength/(float) CASCADE_SHADOW_RESOLUTION;

                Vector3 vBorderOffset = (new Vector3(diagonalLength, diagonalLength, diagonalLength) -
                                         (_lightBox.Max - _lightBox.Min))*0.5f;
                _lightBox.Max += vBorderOffset;
                _lightBox.Min -= vBorderOffset;

                _lightBox.Min /= worldsUnitsPerTexel;
                _lightBox.Min.X = (float) Math.Floor(_lightBox.Min.X);
                _lightBox.Min.Y = (float) Math.Floor(_lightBox.Min.Y);
                _lightBox.Min.Z = (float) Math.Floor(_lightBox.Min.Z);
                _lightBox.Min *= worldsUnitsPerTexel;

                _lightBox.Max /= worldsUnitsPerTexel;
                _lightBox.Max.X = (float) Math.Floor(_lightBox.Max.X);
                _lightBox.Max.Y = (float) Math.Floor(_lightBox.Max.Y);
                _lightBox.Max.Z = (float) Math.Floor(_lightBox.Max.Z);
                _lightBox.Max *= worldsUnitsPerTexel;
            }

            Vector3 boxSize = _lightBox.Max - _lightBox.Min;
            if (boxSize.X == 0 || boxSize.Y == 0 || boxSize.Z == 0)
                boxSize = Vector3.One;
            Vector3 halfBoxSize = boxSize * 0.5f;

            // The position of the light should be in the center of the back
            // pannel of the box. 
            Vector3 lightPosition = _lightBox.Min + halfBoxSize;
            lightPosition.Z = _lightBox.Min.Z;

            // We need the position back in world coordinates so we transform 
            // the light position by the inverse of the lights rotation
            lightPosition = Vector3.Transform(lightPosition,
                                              Matrix.Invert(lightRotation));

            // Create the view matrix for the light
            Matrix lightView = Matrix.CreateLookAt(lightPosition ,
                                                   lightPosition - lightDir,
                                                   cameraUpVector);

            // Create the projection matrix for the light
            // The projection is orthographic since we are using a directional light
            Matrix lightProjection = Matrix.CreateOrthographic(boxSize.X, boxSize.Y,
                                                               -boxSize.Z, 0);



            Vector3 lightDirVS = Vector3.TransformNormal(-lightDir, camera.EyeTransform);
            //check if the light is in the same direction as the camera
            if (lightDirVS.Z > 0)
            {
                //use the far point as clipping plane
                Plane p = new Plane(-Vector3.Forward, maxesVS.Z);
                Plane.Transform(ref p, ref cameraMat, out _directionalClippingPlanes[5]);
            }
            else//lightDirVS.Z < 0
            {
                //use the closest point as clipping plane
                Plane p = new Plane(Vector3.Forward, minsVS.Z);
                Plane.Transform(ref p, ref cameraMat, out _directionalClippingPlanes[5]);
            }

            return lightView * lightProjection;
        }
    }
}