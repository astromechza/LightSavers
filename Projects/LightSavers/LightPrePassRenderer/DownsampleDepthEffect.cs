using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LightPrePassRenderer
{
    public class DownsampleDepthEffect
    {
        private Effect _effect;
        protected QuadRenderer _quadRenderer;

        public EffectParameter PixelSize;
        public EffectParameter DepthBuffer;
        public EffectParameter HalfPixel;

        public DownsampleDepthEffect()
        { 
            _quadRenderer = new QuadRenderer();
        }
        private void Apply()
        {
            _effect.CurrentTechnique.Passes[0].Apply();
        }

        public void ExtractParameters()
        {
            PixelSize = _effect.Parameters["PixelSize"];
            DepthBuffer = _effect.Parameters["DepthBuffer"];
            HalfPixel = _effect.Parameters["HalfPixel"];
        }

        public void Init(ContentManager contentManager, Renderer renderer)
        {
            try
            {
                _effect = contentManager.Load<Effect>("shaders/DownsampleDepth");
                ExtractParameters();
                Vector2 pixelSize = new Vector2(1.0f / (float)renderer.DepthBuffer.Width, 1.0f / (float)renderer.DepthBuffer.Height);
                PixelSize.SetValue(pixelSize);
                HalfPixel.SetValue(pixelSize * 0.5f);
                DepthBuffer.SetValue(renderer.DepthBuffer);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading downsample depth efect: " + ex.ToString());
            }
        }

        public void RenderEffect(Renderer renderer, GraphicsDevice device)
        {
            RenderTarget2D half0 = renderer.HalfDepth;
            //render to a half-res buffer
            device.SetRenderTarget(half0);

            Apply();

            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.None;
            _quadRenderer.RenderQuad(device, -Vector2.One, Vector2.One);

        }
    }
}
