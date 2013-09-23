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
using LightPrePassRenderer.partitioning;

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

        private List<SpotShadowMapEntry> _spotShadowMaps = new List<SpotShadowMapEntry>();
        private List<Mesh.SubMesh> _visibleMeshes = new List<Mesh.SubMesh>(100);
        private const int NUM_SPOT_SHADOWS = 4;
        private const int SPOT_SHADOW_RESOLUTION = 512;

        private int _currentFreeSpotShadowMap;

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
        }

        public void InitFrame()
        {
            _currentFreeSpotShadowMap = 0;
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
        /// Generate the shadow map for a given spot light
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="meshes"></param>
        /// <param name="light"></param>
        /// <param name="shadowMap"></param>
        public void GenerateShadowTextureSpotLight(Renderer renderer, BaseLightAndMeshContainer renderWorld, Light light, SpotShadowMapEntry shadowMap)
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

            renderer.InstancingGroupManager.Reset();
            for (int index = 0; index < _visibleMeshes.Count; index++)
            {
                Mesh.SubMesh subMesh = _visibleMeshes[index];
                //render it
                if (!subMesh.InstanceEnabled)
                    subMesh.RenderShadowMap(ref viewProj, renderer.GraphicsDevice);
                else
                    renderer.InstancingGroupManager.AddInstancedSubMesh(subMesh);
            }
            renderer.InstancingGroupManager.GenerateInstanceInfo(renderer.GraphicsDevice);
            renderer.InstancingGroupManager.RenderShadowMap(ref viewProj, renderer.GraphicsDevice);
        }

    }
}