//-----------------------------------------------------------------------------
// Renderer.cs
//
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
// 
// Part of this code is derived from
// http://mynameismjp.wordpress.com/2009/03/10/reconstructing-position-from-depth/
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LightSavers.Components;
using LightSavers.Components.GameObjects;

namespace LightSavers.Rendering
{
    /// <summary>
    /// This class is responsible for rendering a set of lights and meshes into a texture, using
    /// the Light Pre Pass technique. This technique can be found here
    /// http://diaryofagraphicsprogrammer.blogspot.com/2008/03/light-pre-pass-renderer.html
    /// You can use the output texture as input for your post-process chain, or just render it into
    /// the screen
    /// </summary>
    public class Renderer
    {
        #region Structs

        /// <summary>
        /// Struct to hold extra information about the current lights
        /// </summary>
        private class LightEntry
        {
            public Light light;
            public ShadowRenderer.SpotShadowMapEntry shadowMap;
            public bool castShadows = false;

            public float sqrDistanceToCam = 1;
            public float priority = 1;
        }

        #endregion
        #region Fields

        /// <summary>
        /// GBuffer height
        /// </summary>
        private int _height;

        /// <summary>
        /// GBuffer width
        /// </summary>
        private int _width;

        /// <summary>
        /// This render target stores our zbuffer values
        /// </summary>
        private RenderTarget2D _depthBuffer;

        /// <summary>
        /// This render target stores our normal + specular power values
        /// </summary>
        private RenderTarget2D _normalBuffer;

        /// <summary>
        /// This render target stores the light buffer, the sum of all lights
        /// applied to our scene
        /// </summary>
        private RenderTarget2D _lightBuffer;

        /// <summary>
        /// This render target stores our final composition
        /// </summary>
        private RenderTarget2D _outputTexture;

        /// <summary>
        /// Effect to reconstruct Z buffer from linear depth buffer
        /// </summary>
        private Effect _reconstructZBuffer;

        /// <summary>
        /// Effect that clears our GBuffer
        /// </summary>
        private Effect _clearGBuffer;

        /// <summary>
        /// Effect that performs the lighting 
        /// </summary>
        private Effect _lighting;

        /// <summary>
        /// Use screen-aligned quads for point lights
        /// </summary>
        private bool _useQuads = true;
        /// <summary>
        /// Helper class to draw our 2D quads
        /// </summary>
        private QuadRenderer _quadRenderer;

        /// <summary>
        /// Helper class to draw our point lights
        /// </summary>
        private MeshRenderer _sphereRenderer;

        /// <summary>
        /// Helper class to draw our spot lights
        /// </summary>
        private MeshRenderer _spotRenderer;

        /// <summary>
        /// Our frustum corners in world space
        /// </summary>
        private Vector3[] _cornersWorldSpace = new Vector3[8];

        /// <summary>
        /// Our frustum corners in view space
        /// </summary>
        private Vector3[] _cornersViewSpace = new Vector3[8];

        /// <summary>
        /// Our final corners, the 4 farthest points on the view space frustum
        /// </summary>
        private Vector3[] _currentFrustumCorners = new Vector3[4];

        /// <summary>
        /// Depth states to render our light volume meshes
        /// </summary>
        private DepthStencilState _ccwDepthState;
        private DepthStencilState _cwDepthState;
        private DepthStencilState _directionalDepthState;

        private List<LightEntry> _lightEntries = new List<LightEntry>();
        private List<LightEntry> _lightShadowCasters = new List<LightEntry>();

        private ShadowRenderer _shadowRenderer;
        
        #endregion
        #region Properties

        public RenderTarget2D DepthBuffer { get { return _depthBuffer; } }

        public RenderTarget2D NormalBuffer { get { return _normalBuffer; } }

        public RenderTarget2D LightBuffer { get { return _lightBuffer; } }

        public bool UseQuads { get { return _useQuads; } set { _useQuads = value; } }
        
        #endregion
        /// <summary>
        /// Construct a new copy of our renderer
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="contentManager"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Renderer(int width, int height)
        {
            _width = width;
            _height = height;
            _quadRenderer = new QuadRenderer();
            _sphereRenderer = new MeshRenderer(Globals.content.Load<Model>("meshes/sphere"));
            _spotRenderer = new MeshRenderer(Globals.content.Load<Model>("meshes/cone"));

            _cwDepthState = new DepthStencilState();
            _cwDepthState.DepthBufferWriteEnable = false;
            _cwDepthState.DepthBufferFunction = CompareFunction.LessEqual;

            _ccwDepthState = new DepthStencilState();
            _ccwDepthState.DepthBufferWriteEnable = false;
            _ccwDepthState.DepthBufferFunction = CompareFunction.GreaterEqual;

            _directionalDepthState = new DepthStencilState(); ;
            _directionalDepthState.DepthBufferWriteEnable = false;
            _directionalDepthState.DepthBufferFunction = CompareFunction.Greater;

            _shadowRenderer = new ShadowRenderer(this);

            CreateGBuffer();
            LoadShaders();
        }


        private void CreateGBuffer()
        {
            //One of our premises is to do not use the PRESERVE CONTENTS flags, 
            //that is supposed to be more expensive than DISCARD CONTENT.
            //We use a floating point (32bit) buffer for Z values, although our HW use only 24bits.
            //We could use some packing and use a 24bit buffer too, but lets start simpler
            _depthBuffer = new RenderTarget2D(Globals.graphics.GraphicsDevice, _width, _height, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

            //Our normal buffer stores encoded view-space normal into RG (10bit each) and the specular power in B.
            //Some engines encode the specular power with some log or ln functions. We will output 
            //only the normal texture's alpha channel multiplied by a const value (100),
            //so we have specular power in the range [1..100].
            //Currently, A is not used (2bit).
            _normalBuffer = new RenderTarget2D(Globals.graphics.GraphicsDevice, _width, _height, false, SurfaceFormat.Rgba1010102, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);

            //This buffer stores all the "pure" lighting on the scene, no albedo applied to it. We use an floating
            //point format to allow us "overbright" some areas. Read the blog for more information. We use a depth buffer
            //to optimize light rendering.
            _lightBuffer = new RenderTarget2D(Globals.graphics.GraphicsDevice, _width, _height, false, SurfaceFormat.Rgba64, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);

            //We need another depth here because we need to render all objects again, to reconstruct their shading 
            //using our light texture.
            _outputTexture = new RenderTarget2D(Globals.graphics.GraphicsDevice, _width, _height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
        }

        /// <summary>
        /// Loads all the needed shaders
        /// </summary>
        private void LoadShaders()
        {
            _clearGBuffer = Globals.content.Load<Effect>("shaders/ClearGBuffer");
            _lighting = Globals.content.Load<Effect>("shaders/LightingLpp");
            _reconstructZBuffer = Globals.content.Load<Effect>("shaders/ReconstructDepth");
        }

        /// <summary>
        /// Render the current scene. The culling will be performed inside this method, 
        /// because we need all meshes here to compute the shadow maps.
        /// </summary>
        /// <param name="camera">Current camera</param>
        /// <param name="lights">Visible lights</param>
        /// <param name="meshes">All meshes</param>
        /// <returns></returns>
        public RenderTarget2D RenderScene(Camera camera, List<Light> visibleLights, List<MeshWrapper> meshes)
        {
            
            //compute the frustum corners for this camera
            ComputeFrustumCorners(camera);


            //this resets the free shadow maps
            _shadowRenderer.InitFrame();

            //sort lights, choose the shadow casters
            SortLights(visibleLights, camera);
            SelectShadowCasters();

            //generate all shadow maps
            GenerateShadows(camera, meshes);

            //first of all, we must bind our GBuffer and reset all states
            Globals.graphics.GraphicsDevice.SetRenderTargets(_normalBuffer, _depthBuffer);
            Globals.graphics.GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Black, 1.0f, 0);
            Globals.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            Globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            Globals.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //bind the effect that outputs the default GBuffer values
            _clearGBuffer.CurrentTechnique.Passes[0].Apply();
            //draw a full screen quad for clearing our GBuffer
            _quadRenderer.RenderQuad(Globals.graphics.GraphicsDevice, -Vector2.One, Vector2.One);

            Globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Globals.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            //now, render all our objects
            RenderToGbuffer(camera,meshes);

            //resolve our GBuffer and render the lights
            //clear the light buffer with black
            Globals.graphics.GraphicsDevice.SetRenderTarget(_lightBuffer);
            //dont be fooled by Color.Black, as its alpha is 255 (or 1.0f)
            Globals.graphics.GraphicsDevice.Clear(new Color(0, 0, 0, 0));

            //dont use depth/stencil test...we dont have a depth buffer, anyway
            Globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            Globals.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            //draw using additive blending. 
            //At first I was using BlendState.additive, but it seems to use alpha channel for modulation, 
            //and as we use alpha channel as the specular intensity, we have to create our own blend state here
            Globals.graphics.GraphicsDevice.BlendState = new BlendState()
            {
                AlphaSourceBlend = Blend.One,
                ColorSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.One,
                ColorDestinationBlend = Blend.One,
            };

            RenderLights(camera);

            //reconstruct each object shading, using the light texture as input (and another specific parameters too)
            Globals.graphics.GraphicsDevice.SetRenderTarget(_outputTexture);
            Globals.graphics.GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil | ClearOptions.Target, Color.Black, 1.0f, 0);
            Globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Globals.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            Globals.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            ReconstructShading(camera, meshes);

            //unbind our final buffer and return it
            Globals.graphics.GraphicsDevice.SetRenderTarget(null);

            return _outputTexture;
        }

        /// <summary>
        /// Select which lights can actually cast shadows
        /// </summary>
        private void SelectShadowCasters()
        {
            _lightShadowCasters.Clear();

            for (int i = 0; i < _lightEntries.Count; i++)
            {
                LightEntry entry = _lightEntries[i];
                if (entry.light.CastShadows)
                {
                    //only spot lights can cast shadows right now
                    if (entry.light.LightType == Light.Type.Spot)
                    {
                        entry.shadowMap = _shadowRenderer.GetFreeSpotShadowMap();
                        entry.castShadows = entry.shadowMap != null;
                        //if we dont have that many shadow maps, it cannot cast shadows
                        if (entry.castShadows)
                        {
                            _lightShadowCasters.Add(entry);
                        }
                    }
                }
            }
        }

        private void SortLights(List<Light> visibleLights, Camera camera)
        {
            _lightEntries.Clear();

            Vector3 camPos = camera.Transform.Translation;
            for (int index = 0; index < visibleLights.Count; index++)
            {
                LightEntry lightEntry = new LightEntry();
                lightEntry.light = visibleLights[index];
                lightEntry.sqrDistanceToCam = Math.Max(1,Vector3.Distance(lightEntry.light.Transform.Translation,
                                                            camPos));
                //compute a value to determine light order 
                lightEntry.priority = 1000 * lightEntry.light.Radius / Math.Max(1, lightEntry.sqrDistanceToCam);
                _lightEntries.Add(lightEntry);
            }

            _lightEntries.Sort(delegate(LightEntry p1, LightEntry p2)
            {
                return (int)(p2.priority - p1.priority);
            });
        }

        /// <summary>
        /// Generate the shadow maps and matrixes for the visible lights. We should limit 
        /// our shadow-casters based on number of available shadow maps (we could use some
        /// performance-related heuristic here too)
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="meshes"></param>
        private void GenerateShadows(Camera camera, List<MeshWrapper> meshes)
        {
            for (int index = 0; index < _lightShadowCasters.Count; index++)
            {
                LightEntry light = _lightShadowCasters[index];
                //it cannot be anything different than spot at this point
                if (light.light.LightType == Light.Type.Spot)
                {
                    _shadowRenderer.GenerateShadowTextureSpotLight(this, meshes, light.light, light.shadowMap);
                }
            }
        }

        private void ReconstructZBuffer(Camera camera)
        {
            //bind effect
            _reconstructZBuffer.Parameters["GBufferPixelSize"].SetValue(new Vector2(0.5f / _width, 0.5f / _height));
            _reconstructZBuffer.Parameters["DepthBuffer"].SetValue(_depthBuffer);
            _reconstructZBuffer.Parameters["FarClip"].SetValue(camera.FarClip);
            //our projection matrix is almost all 0s, we just need these 2 values to restoure our Z-buffer from our linear depth buffer
            _reconstructZBuffer.Parameters["ProjectionValues"].SetValue(new Vector2(camera.ProjectionMatrix.M33, camera.ProjectionMatrix.M43));
            _reconstructZBuffer.CurrentTechnique.Passes[0].Apply();

            //we need to always write to z-buffer
            DepthStencilState depthStencilState = new DepthStencilState {
                DepthBufferEnable = true,
                DepthBufferWriteEnable = true,
                DepthBufferFunction = CompareFunction.Always
            };

            //store previous state
            BlendState oldBlendState = Globals.graphics.GraphicsDevice.BlendState;
            BlendState blendState = new BlendState();
            //we dont need to write to color channels
            blendState.ColorWriteChannels = ColorWriteChannels.None;

            Globals.graphics.GraphicsDevice.DepthStencilState = depthStencilState;

            _quadRenderer.RenderQuad(Globals.graphics.GraphicsDevice, -Vector2.One, Vector2.One);

            DepthStencilState depthState = new DepthStencilState();

            //we draw our volumes with front-face culling, so we have to use GreaterEqual here
            depthState.DepthBufferFunction = CompareFunction.GreaterEqual;
            //with our z-buffer reconstructed we only need to read it
            depthState.DepthBufferWriteEnable = false;
            Globals.graphics.GraphicsDevice.DepthStencilState = depthState;
            Globals.graphics.GraphicsDevice.BlendState = oldBlendState;
        }

        private void ReconstructShading(Camera camera, List<MeshWrapper> meshes)
        {
            foreach (MeshWrapper mesh in meshes)
            {
                mesh.RenderReconstructedShading(camera, _lightBuffer);
            }
        }

        private void RenderLights(Camera camera)
        {
            _lighting.Parameters["GBufferPixelSize"].SetValue(new Vector2(0.5f / _width, 0.5f / _height));
            _lighting.Parameters["DepthBuffer"].SetValue(_depthBuffer);
            _lighting.Parameters["NormalBuffer"].SetValue(_normalBuffer);
            
            //just comment this line if you dont want to reconstruct the zbuffer
            ReconstructZBuffer(camera);

            _lighting.Parameters["TanAspect"].SetValue(new Vector2(camera.TanFovy * camera.Aspect, -camera.TanFovy));

            for (int i = 0; i < _lightEntries.Count; i++)
            {
                LightEntry lightEntry = _lightEntries[i];
                Light light = lightEntry.light;

                 //convert light position into viewspace
                Vector3 viewSpaceLPos = Vector3.Transform(light.Transform.Translation, camera.ViewMatrix);
                Vector3 viewSpaceLDir = Vector3.TransformNormal(Vector3.Normalize(light.Transform.Backward), camera.ViewMatrix);
                _lighting.Parameters["LightPosition"].SetValue(viewSpaceLPos);
                _lighting.Parameters["LightDir"].SetValue(viewSpaceLDir);
                Vector3 lightColor = light.Color.ToVector3() * light.Intensity;
                _lighting.Parameters["LightColor"].SetValue(lightColor);
                float invRadiusSqr = 1.0f / (light.Radius * light.Radius);
                _lighting.Parameters["InvLightRadiusSqr"].SetValue(invRadiusSqr);
                _lighting.Parameters["FarClip"].SetValue(camera.FarClip);
                       
                switch (light.LightType)
                {
                    case Light.Type.Point:
                    case Light.Type.Spot:
                        if (light.LightType == Light.Type.Point)
                        {

                            //check if the light touches the near plane
                            BoundingSphere boundingSphereExpanded = light.BoundingSphere;
                            boundingSphereExpanded.Radius *= 1.375f; //expand it a little, because our mesh is not a perfect sphere
                            PlaneIntersectionType planeIntersectionType;
                            camera.Frustum.Near.Intersects(ref boundingSphereExpanded, out planeIntersectionType);
                            if (planeIntersectionType != PlaneIntersectionType.Back)
                            {
                                Globals.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                                Globals.graphics.GraphicsDevice.DepthStencilState = _ccwDepthState;

                            }
                            else
                            {
                                Globals.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                                Globals.graphics.GraphicsDevice.DepthStencilState = _cwDepthState;
                            }

                            Matrix lightMatrix = Matrix.CreateScale(light.Radius);
                            lightMatrix.Translation = light.BoundingSphere.Center;

                            _lighting.Parameters["WorldViewProjection"].SetValue(lightMatrix* camera.ViewProjectionMatrix);
                          
                            _lighting.CurrentTechnique = _lighting.Techniques[1];
                            _lighting.CurrentTechnique.Passes[0].Apply();

                            _sphereRenderer.BindMesh(Globals.graphics.GraphicsDevice);
                            _sphereRenderer.RenderMesh(Globals.graphics.GraphicsDevice);
                        }
                        else
                        {
                            //check if the light touches the far plane

                            Plane near = camera.Frustum.Near;
                            near.D += 3; //give some room because we dont use a perfect-fit mesh for the spot light
                            PlaneIntersectionType planeIntersectionType = near.Intersects(light.Frustum);
                            if (planeIntersectionType != PlaneIntersectionType.Back)
                            {
                                Globals.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                                Globals.graphics.GraphicsDevice.DepthStencilState = _ccwDepthState;

                            }
                            else
                            {
                                Globals.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                                Globals.graphics.GraphicsDevice.DepthStencilState = _cwDepthState;
                            }

                            float tan = (float) Math.Tan(MathHelper.ToRadians(light.SpotAngle));
                            Matrix lightMatrix = Matrix.CreateScale(light.Radius * tan, light.Radius * tan, light.Radius);

                            lightMatrix = lightMatrix * light.Transform;

                            _lighting.Parameters["WorldViewProjection"].SetValue(lightMatrix * camera.ViewProjectionMatrix);
                            float cosSpotAngle = (float)Math.Cos(MathHelper.ToRadians(light.SpotAngle));
                            _lighting.Parameters["SpotAngle"].SetValue(cosSpotAngle);
                            _lighting.Parameters["SpotExponent"].SetValue(light.SpotExponent / (1 - cosSpotAngle));
                            if (lightEntry.castShadows)
                            {
                                _lighting.CurrentTechnique = _lighting.Techniques[4];
                                _lighting.Parameters["MatLightViewProjSpot"].SetValue(lightEntry.shadowMap.LightViewProjection);
                                _lighting.Parameters["DepthBias"].SetValue(light.ShadowDepthBias);
                                Vector2 shadowMapPixelSize = new Vector2(0.5f / lightEntry.shadowMap.Texture.Width, 0.5f / lightEntry.shadowMap.Texture.Height);
                                _lighting.Parameters["ShadowMapPixelSize"].SetValue(shadowMapPixelSize);
                                _lighting.Parameters["ShadowMapSize"].SetValue(new Vector2(lightEntry.shadowMap.Texture.Width, lightEntry.shadowMap.Texture.Height));
                                _lighting.Parameters["ShadowMap"].SetValue(lightEntry.shadowMap.Texture);
                                _lighting.Parameters["CameraTransform"].SetValue(camera.Transform);
                            }
                            else
                            {
                                _lighting.CurrentTechnique = _lighting.Techniques[3];
                            }

                            _lighting.CurrentTechnique.Passes[0].Apply();

                            _spotRenderer.BindMesh(Globals.graphics.GraphicsDevice);
                            _spotRenderer.RenderMesh(Globals.graphics.GraphicsDevice);
                           
                        }

                        break;
                    case Light.Type.Directional:

                        Globals.graphics.GraphicsDevice.DepthStencilState = _directionalDepthState;
                        Globals.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                        ApplyFrustumCorners(_lighting, -Vector2.One, Vector2.One);
                        _lighting.CurrentTechnique = _lighting.Techniques[2];
                        _lighting.CurrentTechnique.Passes[0].Apply();
                        _quadRenderer.RenderQuad(Globals.graphics.GraphicsDevice, -Vector2.One, Vector2.One);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
        }

        private void RenderToGbuffer(Camera camera,List<MeshWrapper> meshes)
        {
            foreach (MeshWrapper mesh in meshes)
            {
                mesh.RenderToGBuffer(camera);
            }
        }

        /// <summary>
        /// Compute the frustum corners for a camera.
        /// Its used to reconstruct the pixel position using only the depth value.
        /// Read here for more information
        /// http://mynameismjp.wordpress.com/2009/03/10/reconstructing-position-from-depth/
        /// </summary>
        /// <param name="camera"> Current rendering camera </param>
        private void ComputeFrustumCorners(Camera camera)
        {
            camera.Frustum.GetCorners(_cornersWorldSpace);
            Matrix matView = camera.ViewMatrix; //this is the inverse of our camera transform
            Vector3.Transform(_cornersWorldSpace, ref matView, _cornersViewSpace); //put the frustum into view space
            for (int i = 0; i < 4; i++) //take only the 4 farthest points
            {
                _currentFrustumCorners[i] = _cornersViewSpace[i + 4];
            }
            Vector3 temp = _currentFrustumCorners[3];
            _currentFrustumCorners[3] = _currentFrustumCorners[2];
            _currentFrustumCorners[2] = temp;
        }

        /// <summary>
        /// This method computes the frustum corners applied to a quad that can be smaller than
        /// our screen. This is useful because instead of drawing a full-screen quad for each
        /// point light, we can draw smaller quads that fit the light's bounding sphere in screen-space,
        /// avoiding unecessary pixel shader operations
        /// </summary>
        /// <param name="effect">The effect we want to apply those corners</param>
        /// <param name="topLeftVertex"> The top left vertex, in screen space [-1..1]</param>
        /// <param name="bottomRightVertex">The bottom right vertex, in screen space [-1..1]</param>
        private void ApplyFrustumCorners(Effect effect, Vector2 topLeftVertex, Vector2 bottomRightVertex)
        {
            float dx = _currentFrustumCorners[1].X - _currentFrustumCorners[0].X;
            float dy = _currentFrustumCorners[0].Y - _currentFrustumCorners[2].Y;

            Vector3[] _localFrustumCorners = new Vector3[4];
            _localFrustumCorners[0] = _currentFrustumCorners[2];
            _localFrustumCorners[0].X += dx * (topLeftVertex.X * 0.5f + 0.5f);
            _localFrustumCorners[0].Y += dy * (bottomRightVertex.Y * 0.5f + 0.5f);

            _localFrustumCorners[1] = _currentFrustumCorners[2];
            _localFrustumCorners[1].X += dx * (bottomRightVertex.X * 0.5f + 0.5f);
            _localFrustumCorners[1].Y += dy * (bottomRightVertex.Y * 0.5f + 0.5f);

            _localFrustumCorners[2] = _currentFrustumCorners[2];
            _localFrustumCorners[2].X += dx * (topLeftVertex.X * 0.5f + 0.5f);
            _localFrustumCorners[2].Y += dy * (topLeftVertex.Y * 0.5f + 0.5f);

            _localFrustumCorners[3] = _currentFrustumCorners[2];
            _localFrustumCorners[3].X += dx * (bottomRightVertex.X * 0.5f + 0.5f);
            _localFrustumCorners[3].Y += dy * (topLeftVertex.Y * 0.5f + 0.5f);

            effect.Parameters["FrustumCorners"].SetValue(_localFrustumCorners);
        }

        public Texture2D GetShadowMap(int i)
        {
            if (i < _lightShadowCasters.Count ) return _lightShadowCasters[i].shadowMap.Texture;
            return null;
        }
    }
}