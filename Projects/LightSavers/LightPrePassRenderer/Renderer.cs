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
using LightPrePassRenderer.partitioning;

namespace LightPrePassRenderer
{
    /// <summary>
    /// This class is responsible for rendering a set of lights and meshes into a texture, using
    /// the Light Pre Pass technique. This technique can be found here
    /// http://diaryofagraphicsprogrammer.blogspot.com/2008/03/light-pre-pass-renderer.html
    /// You can use the output texture as input for your post-process chain, or just render it to
    /// the screen
    /// </summary>
    public class Renderer
    {
        #region Structs

        /// <summary>
        /// Struct to hold extra information about the current lights
        /// </summary>
        private struct LightEntry
        {
            public Light light;
            public ShadowRenderer.SpotShadowMapEntry spotShadowMap;
            public bool castShadows;

            public float sqrDistanceToCam;
            public float priority;
        }

        #endregion
        #region Fields
        /// <summary>
        /// Our main graphic device, created by XNA framework
        /// </summary>
        private GraphicsDevice _graphicsDevice;

        /// <summary>
        /// Content manager responsible for loading our shaders.
        /// It can be the same you use to load your assets, or a new one
        /// to be used only in the renderer. This is useful when you unload
        /// your assets when changing levels (or another situation), so you
        /// don't need to care about reloading this.
        /// </summary>
        private ContentManager _contentManager;

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
        /// This render target is a downsampled version of the main depth buffer
        /// </summary>
        private RenderTarget2D _halfDepth;


        /// <summary>
        /// Effect to downsample zbuffer
        /// </summary>
        private DownsampleDepthEffect _downsampleDepth;

        /// <summary>
        /// Keed track if we already downsampled the depth buffer fot this frame
        /// </summary>
        private bool _depthDownsampledThisFrame = false;

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
        /// This render target stores the specular component of the light buffer, the sum of all lights
        /// applied to our scene
        /// </summary>
        private RenderTarget2D _lightSpecularBuffer;

        /// <summary>
        /// This render target stores our final composition
        /// </summary>
        private RenderTarget2D _outputTexture;

        /// <summary>
        /// Scratch buffers that are half the resolution of the main one
        /// </summary>
        private RenderTarget2D _halfBuffer0;
        private RenderTarget2D _halfBuffer1;

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
        private Vector3[] _localFrustumCorners = new Vector3[4];

        /// <summary>
        /// Depth states to render our light volume meshes
        /// </summary>
        private DepthStencilState _ccwDepthState;
        private DepthStencilState _cwDepthState;
        private DepthStencilState _depthStateReconstructZ;
        private DepthStencilState _depthStateDrawLights;

        private BlendState _lightAddBlendState;

        private ShadowRenderer _shadowRenderer;

        private List<LightEntry> _lightEntries = new List<LightEntry>();
        private List<LightEntry> _lightShadowCasters = new List<LightEntry>();

        private List<Mesh.SubMesh>[] _visibleMeshes = new List<Mesh.SubMesh>[(int)(MeshMetadata.ERenderQueue.Count)];
        private List<Light> _visibleLights = new List<Light>(10);

        //store the bindings for avoiding useless reallocations
        private RenderTargetBinding[] _lightAccumBinding = new RenderTargetBinding[2];
        private RenderTargetBinding[] _gBufferBinding = new RenderTargetBinding[2];

        private Camera _currentCamera;

        private InstancingGroupManager _instancingGroupManager = new InstancingGroupManager();

        #endregion
        #region Properties
        /// <summary>
        /// This render target stores our zbuffer values
        /// </summary>
        public RenderTarget2D DepthBuffer
        {
            get { return _depthBuffer; }
        }

        /// <summary>
        /// This render target stores our normal + specular power values
        /// </summary>
        public RenderTarget2D NormalBuffer
        {
            get { return _normalBuffer; }
        }

        /// <summary>
        /// This render target stores the light buffer, the sum of all lights
        /// applied to our scene
        /// </summary>
        public RenderTarget2D LightBuffer
        {
            get { return _lightBuffer; }
        }

        /// <summary>
        /// Use screen-aligned quads for point lights
        /// </summary>
        public bool UseQuads
        {
            get { return _useQuads; }
            set { _useQuads = value; }
        }

        /// <summary>
        /// Our main graphic device, created by XNA framework
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        /// <summary>
        /// This render target stores the specular component of the light buffer, the sum of all lights
        /// applied to our scene
        /// </summary>
        public RenderTarget2D LightSpecularBuffer
        {
            get { return _lightSpecularBuffer; }
        }

        /// <summary>
        /// Scratch buffers that are half the resolution of the main one
        /// </summary>
        public RenderTarget2D HalfBuffer0
        {
            get { return _halfBuffer0; }
        }

        /// <summary>
        /// Scratch buffers that are half the resolution of the main one
        /// </summary>
        public RenderTarget2D HalfBuffer1
        {
            get { return _halfBuffer1; }
        }

        /// <summary>
        /// This render target is a downsampled version of the main depth buffer
        /// </summary>
        public RenderTarget2D HalfDepth
        {
            get { return _halfDepth; }
        }

        public Camera CurrentCamera
        {
            get { return _currentCamera; }
        }

        public InstancingGroupManager InstancingGroupManager
        {
            get { return _instancingGroupManager; }
        }

        #endregion
        /// <summary>
        /// Construct a new copy of our renderer
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="contentManager"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Renderer(GraphicsDevice graphicsDevice, ContentManager contentManager, int width, int height)
        {
            _width = width;
            _height = height;
            _graphicsDevice = graphicsDevice;
            _contentManager = contentManager;
            _quadRenderer = new QuadRenderer();
            _sphereRenderer = new MeshRenderer(contentManager.Load<Model>("meshes/sphere"));
            _spotRenderer = new MeshRenderer(contentManager.Load<Model>("meshes/cone"));

            _cwDepthState = new DepthStencilState();
            _cwDepthState.DepthBufferWriteEnable = false;
            _cwDepthState.DepthBufferFunction = CompareFunction.LessEqual;

            _ccwDepthState = new DepthStencilState();
            _ccwDepthState.DepthBufferWriteEnable = false;
            _ccwDepthState.DepthBufferFunction = CompareFunction.GreaterEqual;

            _depthStateDrawLights = new DepthStencilState();

            //we draw our volumes with front-face culling, so we have to use GreaterEqual here
            _depthStateDrawLights.DepthBufferFunction = CompareFunction.GreaterEqual;
            //with our z-buffer reconstructed we only need to read it
            _depthStateDrawLights.DepthBufferWriteEnable = false;

            _shadowRenderer = new ShadowRenderer(this);

            _depthStateReconstructZ = new DepthStencilState
            {
                DepthBufferEnable = true,
                DepthBufferWriteEnable = true,
                DepthBufferFunction = CompareFunction.Always
            };

            _lightAddBlendState = new BlendState()
            {
                AlphaSourceBlend = Blend.One,
                ColorSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.One,
                ColorDestinationBlend = Blend.One,
            };

            //create render queues
            for (int index = 0; index < _visibleMeshes.Length; index++)
            {
                _visibleMeshes[index] = new List<Mesh.SubMesh>();
            }


            CreateGBuffer();
            LoadShaders();

            _downsampleDepth = new DownsampleDepthEffect();
            _downsampleDepth.Init(contentManager, this);
        }


        private void CreateGBuffer()
        {
            //One of our premises is to do not use the PRESERVE CONTENTS flags, 
            //that is supposed to be more expensive than DISCARD CONTENT.
            //We use a floating point (32bit) buffer for Z values, although our HW use only 24bits.
            //We could use some packing and use a 24bit buffer too, but lets start simpler
            _depthBuffer = new RenderTarget2D(GraphicsDevice, _width, _height, false, SurfaceFormat.Single,
                                              DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

            //the downsampled depth buffer must have the same format as the main one
            _halfDepth = new RenderTarget2D(GraphicsDevice, _width / 2, _height / 2, false,
                                              SurfaceFormat.Single, DepthFormat.None, 0,
                                              RenderTargetUsage.DiscardContents);

            //Our normal buffer stores encoded view-space normal into RG (10bit each) and the specular power in B.
            //Some engines encode the specular power with some log or ln functions. We will output 
            //only the normal texture's alpha channel multiplied by a const value (100),
            //so we have specular power in the range [1..100].
            //Currently, A is not used (2bit).
            _normalBuffer = new RenderTarget2D(GraphicsDevice, _width, _height, false, SurfaceFormat.Rgba1010102,
                                               DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);

            //This buffer stores all the "pure" lighting on the scene, no albedo applied to it. We use an floating
            //point format to allow us "overbright" some areas. Read the blog for more information. We use a depth buffer
            //to optimize light rendering.
            _lightBuffer = new RenderTarget2D(GraphicsDevice, _width, _height, false, SurfaceFormat.HdrBlendable,
                                              DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);

            //we need a separate texture for the specular, since the xbox doesnt allow a RGBA64 buffer
            _lightSpecularBuffer = new RenderTarget2D(GraphicsDevice, _width, _height, false,
                                             SurfaceFormat.HdrBlendable, DepthFormat.None, 0,
                                             RenderTargetUsage.DiscardContents);

            //We need another depth here because we need to render all objects again, to reconstruct their shading 
            //using our light texture.
            _outputTexture = new RenderTarget2D(GraphicsDevice, _width, _height, false, SurfaceFormat.Color,
                                                DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);

            int halfRes = 2;
            _halfBuffer0 = new RenderTarget2D(GraphicsDevice, _width / halfRes, _height / halfRes, false,
                                              SurfaceFormat.Color, DepthFormat.None, 0,
                                              RenderTargetUsage.DiscardContents);
            _halfBuffer1 = new RenderTarget2D(GraphicsDevice, _width / halfRes, _height / halfRes, false,
                                              SurfaceFormat.Color, DepthFormat.None, 0,
                                              RenderTargetUsage.DiscardContents);

            _gBufferBinding[0] = new RenderTargetBinding(_normalBuffer);
            _gBufferBinding[1] = new RenderTargetBinding(_depthBuffer);

            _lightAccumBinding[0] = new RenderTargetBinding(_lightBuffer);
            _lightAccumBinding[1] = new RenderTargetBinding(_lightSpecularBuffer);
        }

        /// <summary>
        /// Loads all the needed shaders
        /// </summary>
        private void LoadShaders()
        {
            _clearGBuffer = _contentManager.Load<Effect>("shaders/ClearGBuffer");
            _lighting = _contentManager.Load<Effect>("shaders/LightingLpp");
            _reconstructZBuffer = _contentManager.Load<Effect>("shaders/ReconstructDepth");
        }

        /// <summary>
        /// Attempt to get the downsampled depth buffer. If it's not generated for this frame,
        /// downsample it no
        /// </summary>
        /// <returns></returns>
        public RenderTarget2D GetDownsampledDepth()
        {
            if (!_depthDownsampledThisFrame)
                DownsampleDepthbuffer();
            return _halfDepth;
        }

        private void DownsampleDepthbuffer()
        {
            _downsampleDepth.RenderEffect(this, _graphicsDevice);
            _depthDownsampledThisFrame = true;
        }

        /// <summary>
        /// Render the current scene. The culling will be performed inside this method, 
        /// because we need all meshes here to compute the shadow maps.
        /// </summary>
        /// <param name="camera">Current camera</param>
        /// <param name="visibleLights"></param>
        /// <param name="meshes">All meshes</param>
        /// <param name="sceneGraph"></param>
        /// <param name="particleSystems"></param>
        /// <param name="gameTime"></param>
        /// <param name="lights">Visible lights</param>
        /// <returns></returns>
        public RenderTarget2D RenderScene(Camera camera, BaseSceneGraph sceneGraph, GameTime gameTime)
        {
            InstancingGroupManager.Reset();

            _depthDownsampledThisFrame = false;
            _currentCamera = camera;

            BaseRenderEffect.TotalTime = (float)gameTime.TotalGameTime.TotalSeconds;
            //compute the frustum corners for this camera
            ComputeFrustumCorners(camera);


            //this resets the free shadow maps
            _shadowRenderer.InitFrame();

            _visibleLights.Clear();
            sceneGraph.GetVisibleLights(camera.Frustum, _visibleLights);
            //sort lights, choose the shadow casters
            SortLights(camera);
            SelectShadowCasters();

            //generate all shadow maps
            GenerateShadows(camera, sceneGraph);

            //first of all, we must bind our GBuffer and reset all states
            GraphicsDevice.SetRenderTargets(_gBufferBinding);

            GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Black, 1.0f, 0);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //bind the effect that outputs the default GBuffer values
            _clearGBuffer.CurrentTechnique.Passes[0].Apply();
            //draw a full screen quad for clearing our GBuffer
            _quadRenderer.RenderQuad(GraphicsDevice, -Vector2.One, Vector2.One);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            //select the visible meshes
            CullVisibleMeshes(camera, sceneGraph);

            //now, render them to the G-Buffer
            RenderToGbuffer(camera);

            //resolve our GBuffer and render the lights
            //clear the light buffer with black
            GraphicsDevice.SetRenderTargets(_lightAccumBinding);
            //dont be fooled by Color.Black, as its alpha is 255 (or 1.0f)
            GraphicsDevice.Clear(new Color(0, 0, 0, 0));

            //dont use depth/stencil test...we dont have a depth buffer, anyway
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            //draw using additive blending. 
            //At first I was using BlendState.additive, but it seems to use alpha channel for modulation, 
            //and as we use alpha channel as the specular intensity, we have to create our own blend state here

            GraphicsDevice.BlendState = _lightAddBlendState;

            RenderLights(camera);

            //reconstruct each object shading, using the light texture as input (and another specific parameters too)
            GraphicsDevice.SetRenderTarget(_outputTexture);
            GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil | ClearOptions.Target, Color.Black, 1.0f, 0);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            //reconstruct the shading, using the already culled list
            ReconstructShading(camera);
            //render objects that doesn't need the lightbuffer information, such as skyboxes, pure reflective meshes, etc
            DrawOpaqueObjects(camera);
            //draw objects with transparency
            DrawBlendObjects(camera);

            //draw SSAO texture. It's not correct to do it here, because ideally the SSAO should affect only 
            //the ambient light, but it looks good this way

            //unbind our final buffer and return it
            GraphicsDevice.SetRenderTarget(null);

            return _outputTexture;
        }

        private void DrawOpaqueObjects(Camera camera)
        {
            List<Mesh.SubMesh> meshes = _visibleMeshes[(int)MeshMetadata.ERenderQueue.SkipGbuffer];
            for (int index = 0; index < meshes.Count; index++)
            {
                Mesh.SubMesh visibleMesh = meshes[index];
                visibleMesh.GenericRender(camera, GraphicsDevice);
            }
        }

        private void DrawBlendObjects(Camera camera)
        {
            _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            _graphicsDevice.BlendState = BlendState.Additive;

            List<Mesh.SubMesh> meshes = _visibleMeshes[(int)MeshMetadata.ERenderQueue.Blend];
            for (int index = 0; index < meshes.Count; index++)
            {
                Mesh.SubMesh visibleMesh = meshes[index];
                visibleMesh.GenericRender(camera, GraphicsDevice);
            }

            //reset states, since the custom shaders could have overriden them
            _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            _graphicsDevice.BlendState = BlendState.Additive;
            _graphicsDevice.BlendFactor = Color.White;
            _graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            Matrix projectionTransform = camera.ProjectionTransform;
            Matrix eyeTransform = camera.EyeTransform;
            

        }

        /// <summary>
        /// Do a frustum culling test on each sub mesh, adding the visible ones to our list
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="meshes"></param>
        private void CullVisibleMeshes(Camera camera, BaseSceneGraph sceneGraph)
        {
            for (int index = 0; index < _visibleMeshes.Length; index++)
            {
                List<Mesh.SubMesh> visibleMesh = _visibleMeshes[index];
                visibleMesh.Clear();
            }

            sceneGraph.GetVisibleMeshes(camera.Frustum, _visibleMeshes);

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
                if (_lightEntries[i].light.CastShadows)
                {
                    //only spot and directional lights can cast shadows right now
                    if (entry.light.LightType == Light.Type.Spot)
                    {
                        entry.spotShadowMap = _shadowRenderer.GetFreeSpotShadowMap();
                        entry.castShadows = entry.spotShadowMap != null;
                        //if we dont have that many shadow maps, it cannot cast shadows
                        if (entry.castShadows)
                        {
                            _lightShadowCasters.Add(entry);
                        }
                    }
                }
                //assign it back, since it's a struct
                _lightEntries[i] = entry;
            }
        }

        private void SortLights(Camera camera)
        {
            _lightEntries.Clear();

            Vector3 camPos = camera.Transform.Translation;
            for (int index = 0; index < _visibleLights.Count; index++)
            {
                LightEntry lightEntry = new LightEntry();
                lightEntry.light = _visibleLights[index];
                lightEntry.sqrDistanceToCam = Math.Max(1, Vector3.Distance(lightEntry.light.Transform.Translation,
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
        /// <param name="renderWorld"></param>
        private void GenerateShadows(Camera camera, BaseSceneGraph sceneGraph)
        {
            for (int index = 0; index < _lightShadowCasters.Count; index++)
            {
                LightEntry light = _lightShadowCasters[index];
                //only spot
                if (light.light.LightType == Light.Type.Spot)
                {
                    _shadowRenderer.GenerateShadowTextureSpotLight(this, sceneGraph, light.light, light.spotShadowMap);
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
            _reconstructZBuffer.Parameters["ProjectionValues"].SetValue(new Vector2(camera.ProjectionTransform.M33, camera.ProjectionTransform.M43));
            _reconstructZBuffer.CurrentTechnique.Passes[0].Apply();

            //we need to always write to z-buffer

            //store previous state
            BlendState oldBlendState = GraphicsDevice.BlendState;

            GraphicsDevice.DepthStencilState = _depthStateReconstructZ;

            _quadRenderer.RenderQuad(GraphicsDevice, -Vector2.One, Vector2.One);

            GraphicsDevice.DepthStencilState = _depthStateDrawLights;
            GraphicsDevice.BlendState = oldBlendState;
        }

        private void ReconstructShading(Camera camera)
        {
            List<Mesh.SubMesh> meshes = _visibleMeshes[(int)MeshMetadata.ERenderQueue.Default];
            for (int index = 0; index < meshes.Count; index++)
            {
                Mesh.SubMesh visibleMesh = meshes[index];

                if (!visibleMesh.InstanceEnabled)
                {
                    visibleMesh.ReconstructShading(camera, GraphicsDevice);
                }
            }
            //reuse the instance groups
            InstancingGroupManager.ReconstructShading(camera, GraphicsDevice);
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
                Vector3 viewSpaceLPos = Vector3.Transform(light.Transform.Translation, camera.EyeTransform);
                Vector3 viewSpaceLDir = Vector3.TransformNormal(Vector3.Normalize(light.Transform.Backward), camera.EyeTransform);
                _lighting.Parameters["LightPosition"].SetValue(viewSpaceLPos);
                _lighting.Parameters["LightDir"].SetValue(viewSpaceLDir);
                Vector4 lightColor = light.Color.ToVector4() * light.Intensity;
                lightColor.W = light.SpecularIntensity;
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
                                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                                GraphicsDevice.DepthStencilState = _ccwDepthState;

                            }
                            else
                            {
                                GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                                GraphicsDevice.DepthStencilState = _cwDepthState;
                            }

                            Matrix lightMatrix = Matrix.CreateScale(light.Radius);
                            lightMatrix.Translation = light.BoundingSphere.Center;

                            _lighting.Parameters["WorldViewProjection"].SetValue(lightMatrix *
                                                                                 camera.EyeProjectionTransform);

                            _lighting.CurrentTechnique = _lighting.Techniques[1];
                            _lighting.CurrentTechnique.Passes[0].Apply();

                            _sphereRenderer.BindMesh(GraphicsDevice);
                            _sphereRenderer.RenderMesh(GraphicsDevice);
                        }
                        else
                        {
                            //check if the light touches the far plane

                            Plane near = camera.Frustum.Near;
                            near.D += 3; //give some room because we dont use a perfect-fit mesh for the spot light
                            PlaneIntersectionType planeIntersectionType = near.Intersects(light.Frustum);
                            if (planeIntersectionType != PlaneIntersectionType.Back)
                            {
                                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                                GraphicsDevice.DepthStencilState = _ccwDepthState;

                            }
                            else
                            {
                                GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                                GraphicsDevice.DepthStencilState = _cwDepthState;
                            }

                            float tan = (float)Math.Tan(MathHelper.ToRadians(light.SpotAngle));
                            Matrix lightMatrix = Matrix.CreateScale(light.Radius * tan, light.Radius * tan, light.Radius);

                            lightMatrix = lightMatrix * light.Transform;

                            _lighting.Parameters["WorldViewProjection"].SetValue(lightMatrix *
                                                                                 camera.EyeProjectionTransform);
                            float cosSpotAngle = (float)Math.Cos(MathHelper.ToRadians(light.SpotAngle));
                            _lighting.Parameters["SpotAngle"].SetValue(cosSpotAngle);
                            _lighting.Parameters["SpotExponent"].SetValue(light.SpotExponent / (1 - cosSpotAngle));
                            if (lightEntry.castShadows)
                            {
                                _lighting.CurrentTechnique = _lighting.Techniques[4];
                                _lighting.Parameters["MatLightViewProjSpot"].SetValue(lightEntry.spotShadowMap.LightViewProjection);
                                _lighting.Parameters["DepthBias"].SetValue(light.ShadowDepthBias);
                                Vector2 shadowMapPixelSize = new Vector2(0.5f / lightEntry.spotShadowMap.Texture.Width, 0.5f / lightEntry.spotShadowMap.Texture.Height);
                                _lighting.Parameters["ShadowMapPixelSize"].SetValue(shadowMapPixelSize);
                                _lighting.Parameters["ShadowMapSize"].SetValue(new Vector2(lightEntry.spotShadowMap.Texture.Width, lightEntry.spotShadowMap.Texture.Height));
                                _lighting.Parameters["ShadowMap"].SetValue(lightEntry.spotShadowMap.Texture);
                                _lighting.Parameters["CameraTransform"].SetValue(camera.Transform);
                            }
                            else
                            {
                                _lighting.CurrentTechnique = _lighting.Techniques[3];
                            }

                            _lighting.CurrentTechnique.Passes[0].Apply();

                            _spotRenderer.BindMesh(GraphicsDevice);
                            _spotRenderer.RenderMesh(GraphicsDevice);

                        }

                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
            _graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        private void RenderToGbuffer(Camera camera)
        {
            List<Mesh.SubMesh> meshes = _visibleMeshes[(int)MeshMetadata.ERenderQueue.Default];
            for (int index = 0; index < meshes.Count; index++)
            {
                Mesh.SubMesh mesh = meshes[index];
                if (!mesh.InstanceEnabled)
                {
                    mesh.RenderToGBuffer(camera, GraphicsDevice);
                }
                else
                {
                    InstancingGroupManager.AddInstancedSubMesh(mesh);
                }
            }
            InstancingGroupManager.GenerateInstanceInfo(GraphicsDevice);
            InstancingGroupManager.RenderToGBuffer(camera, GraphicsDevice);
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
            Matrix matView = camera.EyeTransform; //this is the inverse of our camera transform
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
        public void ApplyFrustumCorners(Effect effect, Vector2 topLeftVertex, Vector2 bottomRightVertex)
        {
            ApplyFrustumCorners(effect.Parameters["FrustumCorners"], topLeftVertex, bottomRightVertex);
        }
        public void ApplyFrustumCorners(EffectParameter frustumCorners, Vector2 topLeftVertex, Vector2 bottomRightVertex)
        {
            float dx = _currentFrustumCorners[1].X - _currentFrustumCorners[0].X;
            float dy = _currentFrustumCorners[0].Y - _currentFrustumCorners[2].Y;

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

            frustumCorners.SetValue(_localFrustumCorners);
        }

        public Texture2D GetShadowMap(int i)
        {
            if (i < _lightShadowCasters.Count)
                return _lightShadowCasters[i].spotShadowMap.Texture;
            return null;
        }

        /// <summary>
        /// Call this for every mesh you create, so it sets the GBuffer textures only once
        /// </summary>
        /// <param name="mesh"></param>
        public void SetupSubMesh(Mesh.SubMesh subMesh)
        {
            subMesh.RenderEffect.SetLightBuffer(_lightBuffer, _lightSpecularBuffer);

            subMesh.RenderEffect.SetLightBufferPixelSize(new Vector2(0.5f / _lightBuffer.Width, 0.5f / _lightBuffer.Height));
            subMesh.RenderEffect.SetDepthBuffer(_depthBuffer);
            subMesh.RenderEffect.SetNormalBuffer(_normalBuffer);
        }
    }
}