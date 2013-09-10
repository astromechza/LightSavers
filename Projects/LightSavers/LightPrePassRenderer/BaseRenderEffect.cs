//-----------------------------------------------------------------------------
// BaseRenderEffect.cs
//
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//-----------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightPrePassRenderer
{
    public class BaseRenderEffect
    {
        public static float TotalTime = 0;

        private Effect _effect;

        private EffectParameter _worldParameter;
        private EffectParameter _viewParameter;
        private EffectParameter _projectionParameter;
        private EffectParameter _worldViewParameter;
        private EffectParameter _worldViewProjectionParameter;
        private EffectParameter _textureMatrixParameter;
        private EffectParameter _lightViewProjParameter;
        private EffectParameter _farClipParameter;
        private EffectParameter _lightBufferPixelSizeParameter;
        private EffectParameter _bonesParameter;
        private EffectParameter _normalBufferParameter;
        private EffectParameter _depthBufferParameter;
        private EffectParameter _lightBufferParameter;
        private EffectParameter _lightSpecularBufferParameter;
        private EffectParameter _colorBufferParameter;
        private EffectParameter _diffuseParameter;
        private EffectParameter _ambientParameter;
        private EffectParameter _ambientCubemapParameter;


        private EffectParameter _totalTimeParameter;


        public BaseRenderEffect(Effect effect)
        {
            SetEffect(effect);
        }

        public EffectParameter DiffuseMapParameter
        {
            get { return _diffuseParameter; }
        }

        public EffectParameter AmbientParameter
        {
            get { return _ambientParameter; }
        }

        public EffectParameter AmbientCubemapParameter
        {
            get { return _ambientCubemapParameter; }
        }

        public void SetEffect(Effect effect)
        {
            _effect = effect;
            ExtractParameters();
        }

        private void ExtractParameters()
        {
            _worldParameter = _effect.Parameters["World"];
            _viewParameter = _effect.Parameters["View"];
            _projectionParameter = _effect.Parameters["Projection"];
            _worldViewParameter = _effect.Parameters["WorldView"];
            _worldViewProjectionParameter = _effect.Parameters["WorldViewProjection"];

            _textureMatrixParameter = _effect.Parameters["TextureMatrix"];
            _lightViewProjParameter = _effect.Parameters["LightViewProj"];
            _farClipParameter = _effect.Parameters["FarClip"];
            _lightBufferPixelSizeParameter = _effect.Parameters["LightBufferPixelSize"];

            _bonesParameter = _effect.Parameters["Bones"];
            _normalBufferParameter = _effect.Parameters["NormalBuffer"];
            _lightBufferParameter = _effect.Parameters["LightBuffer"];
            _lightSpecularBufferParameter = _effect.Parameters["LightSpecularBuffer"];

            _depthBufferParameter = _effect.Parameters["DepthBuffer"];
            _colorBufferParameter = _effect.Parameters["ColorBuffer"];

            _ambientParameter = _effect.Parameters["AmbientColor"];
            _ambientCubemapParameter = _effect.Parameters["AmbientCubeMap"];

            _totalTimeParameter = _effect.Parameters["TotalTime"];

            _diffuseParameter = _effect.Parameters["DiffuseMap"];
            if (_diffuseParameter == null)
                _diffuseParameter = _effect.Parameters["Diffuse"];

            if (_textureMatrixParameter != null)
                _textureMatrixParameter.SetValue(Matrix.Identity);
        }

        public void SetMatrices(Matrix world, Matrix view, Matrix projection)
        {
            Matrix worldView, worldViewProj;
            Matrix.Multiply(ref world, ref view, out worldView);
            Matrix.Multiply(ref worldView, ref projection, out worldViewProj);

            if (_worldParameter != null)
                _worldParameter.SetValue(world);
            if (_viewParameter != null)
                _viewParameter.SetValue(view);
            if (_projectionParameter != null)
                _projectionParameter.SetValue(projection);
            if (_worldViewParameter != null)
                _worldViewParameter.SetValue(worldView);
            if (_worldViewProjectionParameter != null)
                _worldViewProjectionParameter.SetValue(worldViewProj);
        }

        public void SetTextureMatrix(Matrix texture)
        {
            _textureMatrixParameter.SetValue(texture);
        }

        public void SetLightViewProj(Matrix matrix)
        {
            _lightViewProjParameter.SetValue(matrix);
        }

        public void SetFarClip(float farClip)
        {
            if (_farClipParameter != null)
                _farClipParameter.SetValue(farClip);
        }

        public void SetLightBufferPixelSize(Vector2 pixelSize)
        {
            if (_lightBufferPixelSizeParameter != null)
                _lightBufferPixelSizeParameter.SetValue(pixelSize);
        }

        public void SetBones(Matrix[] bones)
        {
            _bonesParameter.SetValue(bones);
        }

        public void SetNormalBuffer(Texture normalTarget)
        {
            if (_normalBufferParameter != null)
                _normalBufferParameter.SetValue(normalTarget);
        }

        public void SetLightBuffer(Texture lightAccumBuffer, Texture lightSpecularAccumBuffer)
        {
            if (_lightBufferParameter != null)
                _lightBufferParameter.SetValue(lightAccumBuffer);
            if (_lightSpecularBufferParameter != null)
                _lightSpecularBufferParameter.SetValue(lightSpecularAccumBuffer);
        }

        public void SetWorld(Matrix globalTransform)
        {
            _worldParameter.SetValue(globalTransform);
        }

        public void SetWorldViewProjection(Matrix worldViewProj)
        {
            _worldViewProjectionParameter.SetValue(worldViewProj);
        }

        public void SetCurrentTechnique(int t)
        {
            _effect.CurrentTechnique = _effect.Techniques[t];
        }

        public void Apply()
        {
            if (_totalTimeParameter != null)
                _totalTimeParameter.SetValue(TotalTime);
            _effect.CurrentTechnique.Passes[0].Apply();
        }

        public EffectParameter GetParameter(string parameter)
        {
            return _effect.Parameters[parameter];
        }

        public void SetDepthBuffer(Texture2D depth)
        {
            if (_depthBufferParameter != null)
                _depthBufferParameter.SetValue(depth);
        }

        public void SetColorBuffer(Texture2D color)
        {
            if (_colorBufferParameter != null)
                _colorBufferParameter.SetValue(color);
        }

        public int GetNumTechniques()
        {
            if (_effect != null)
                return _effect.Techniques.Count;
            return 0;
        }
    }
}
