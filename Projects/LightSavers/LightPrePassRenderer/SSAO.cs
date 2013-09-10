using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LightPrePassRenderer
{
    public class SSAO
    {
        private bool _enabled = true;
        private Effect _effect;
        protected QuadRenderer _quadRenderer;
        private float _randomTile = 100;
        private float _radius = 0.05f;
        private float _maxRadius = 0.5f;
        private float _bias = 0.00001f;
        private int _blurCount = 1;
        private Texture2D _randomTex;
        private float _intensity = 1.75f;

        private EffectParameter _parameterDepthBuffer;
        private EffectParameter _parameterNormalBuffer;
        private EffectParameter _parameterRandomMap;
        private EffectParameter _parameterRandomTile;
        private EffectParameter _parameterRadius;
        private EffectParameter _parameterBias;
        private EffectParameter _parameterTempBufferRes;
        private EffectParameter _parameterHalfBufferHalfPixel;
        private EffectParameter _parameteGBufferHalfPixel;
        private EffectParameter _parameterSSAOBuffer;
        private EffectParameter _parameterBlurDirection;
        private EffectParameter _parameterSSAORes;
        private EffectParameter _parameterSSAOIntensity;
        private EffectParameter _parameterFarClip;

        private BlendState _finalMixBlendState;
        public float RandomTile
        {
            get { return _randomTile; }
            set { _randomTile = value; }
        }

        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public float Bias
        {
            get { return _bias; }
            set { _bias = value; }
        }

        public int BlurCount
        {
            get { return _blurCount; }
            set { _blurCount = value; if (_blurCount < 0) _blurCount = 0; }
        }

        public float Intensity
        {
            get { return _intensity; }
            set
            {
                _intensity = value;
                _intensity = MathHelper.Clamp(_intensity, 0.1f, 2.0f);
            }
        }

        public float MaxRadius
        {
            get { return _maxRadius; }
            set { _maxRadius = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public SSAO()
        {
            _quadRenderer = new QuadRenderer();
        }

        public void Init(ContentManager contentManager)
        {
            _finalMixBlendState = new BlendState()
            {
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha,
                ColorSourceBlend = Blend.Zero,
                AlphaSourceBlend = Blend.Zero
            };
            try
            {
                _effect = contentManager.Load<Effect>("shaders/SSAO");
                _randomTex = contentManager.Load<Texture2D>("textures/random");
                _parameterDepthBuffer = _effect.Parameters["DepthBuffer"];
                _parameterNormalBuffer = _effect.Parameters["NormalBuffer"];
                _parameterRandomMap = _effect.Parameters["RandomMap"];
                _parameterRandomTile = _effect.Parameters["RandomTile"];
                _parameterRadius = _effect.Parameters["Radius"];
                _parameterBias = _effect.Parameters["Bias"];
                _parameterTempBufferRes = _effect.Parameters["TempBufferRes"];
                _parameterSSAOBuffer = _effect.Parameters["SSAOBuffer"];
                _parameterBlurDirection = _effect.Parameters["BlurDirection"];
                _parameterSSAORes = _effect.Parameters["SSAORes"];
                _parameterSSAOIntensity = _effect.Parameters["SSAOIntensity"];
                _parameterFarClip = _effect.Parameters["FarClip"];
                _parameterHalfBufferHalfPixel = _effect.Parameters["HalfBufferHalfPixel"];
                _parameteGBufferHalfPixel = _effect.Parameters["GBufferPixelSize"];
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to load SSAO shaders");
            }
        }

        public void ComputeSSAO(Renderer renderer, GraphicsDevice device)
        {
            RenderTarget2D depthBuffer = renderer.GetDownsampledDepth();
            RenderTarget2D normalBuffer = renderer.NormalBuffer;
            RenderTarget2D half0 = renderer.HalfBuffer0;
            RenderTarget2D half1 = renderer.HalfBuffer1;

            //render to a half-res buffer
            device.SetRenderTarget(half0);
            device.Clear(Color.Black);

            _effect.CurrentTechnique = _effect.Techniques[0];
            _parameterDepthBuffer.SetValue(depthBuffer);
            _parameterNormalBuffer.SetValue(normalBuffer);
            _parameterRandomMap.SetValue(_randomTex);
            _parameterRandomTile.SetValue(_randomTile);
            _parameterRadius.SetValue(new Vector2(_radius, _maxRadius));
            _parameterBias.SetValue(_bias);

            _parameterHalfBufferHalfPixel.SetValue(new Vector2(0.5f / depthBuffer.Width, 0.5f / depthBuffer.Height));
            _parameteGBufferHalfPixel.SetValue(new Vector2(0.5f / normalBuffer.Width, 0.5f / normalBuffer.Height));

            _parameterFarClip.SetValue(renderer.CurrentCamera.FarClip);
       
            _effect.CurrentTechnique.Passes[0].Apply();

            device.RasterizerState = RasterizerState.CullNone;
            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.None;
            

            _quadRenderer.RenderQuad(device, -Vector2.One, Vector2.One);

            if (_blurCount > 0)
            {
                Vector2 tempBufferRes = new Vector2(half1.Width, half1.Height);
                _parameterTempBufferRes.SetValue(tempBufferRes);
                _effect.CurrentTechnique = _effect.Techniques[1];
                device.BlendState = BlendState.Opaque;
                device.DepthStencilState = DepthStencilState.None;
                for (int i = 0; i < _blurCount; i++)
                {
                    device.SetRenderTarget(half1);
                    device.Clear(Color.Black);
                    _parameterSSAOBuffer.SetValue(half0);
                    _parameterBlurDirection.SetValue(Vector2.UnitX / half1.Width);
                    _effect.Techniques[1].Passes[0].Apply();
                    _quadRenderer.RenderQuad(device, -Vector2.One, Vector2.One);


                    device.SetRenderTarget(half0);
                    device.Clear(Color.Black);
                    _parameterSSAOBuffer.SetValue(half1);
                    _parameterBlurDirection.SetValue(Vector2.UnitY / half1.Height);
                    _effect.Techniques[1].Passes[0].Apply();
                    _quadRenderer.RenderQuad(device, -Vector2.One, Vector2.One);
                }
            }

            device.DepthStencilState = DepthStencilState.None;
          
            device.BlendState = BlendState.Opaque;

        }

        public void FinalMix(Renderer renderer, GraphicsDevice device)
        {
            RenderTarget2D half0 = renderer.HalfBuffer0;

            device.RasterizerState = RasterizerState.CullNone;
            device.DepthStencilState = DepthStencilState.None;

            device.BlendState = _finalMixBlendState;

            _effect.CurrentTechnique = _effect.Techniques[2];
            _parameterSSAOBuffer.SetValue(half0);
            Vector2 tempBufferRes = new Vector2(half0.Width, half0.Height);
            _parameterSSAORes.SetValue(tempBufferRes);
            _parameterSSAOIntensity.SetValue(Intensity);
            _effect.CurrentTechnique.Passes[0].Apply();
            _quadRenderer.RenderQuad(device, -Vector2.One, Vector2.One);
        }
    }
}
