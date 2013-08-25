using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Shader
{
    public class TestShader
    {
        // SHADER PROPERTY AND ACCESSOR
        private Effect _effect;
        public Effect Effect
        {
            get { return _effect; }
        }

        // PARAMETER MODIFIERS
        EffectParameter _worldMatrix;
        public EffectParameter WorldMatrix { get { return _worldMatrix; } }
        
        EffectParameter _viewMatrix;
        public EffectParameter ViewMatrix { get { return _viewMatrix; } }
        
        EffectParameter _projectionMatrix;
        public EffectParameter ProjectionMatrix { get { return _projectionMatrix; } }

        EffectParameter _camPosition;
        public EffectParameter CamPosition { get { return _camPosition; } }
        
        EffectParameter _ambientLightColour;
        public EffectParameter AmbientLightColour { get { return _ambientLightColour; } }

        EffectParameter _currentTexture;
        public EffectParameter CurrentTexture { get { return _currentTexture; } }

        public PointLight[] PointLight;

        public TestShader()
        {
            _effect = Globals.content.Load<Effect>("shaders/test1");
            _effect.CurrentTechnique = _effect.Techniques[0];

            _worldMatrix = _effect.Parameters["World"];
            _viewMatrix = _effect.Parameters["View"];
            _projectionMatrix = _effect.Parameters["Projection"];
            _camPosition = _effect.Parameters["CameraPosition"];
            _ambientLightColour = _effect.Parameters["AmbientLightColour"];

            _currentTexture = _effect.Parameters["CurrentTexture"];

            PointLight = new PointLight[3];
            PointLight[0] = new Shader.PointLight(this, 0);
            PointLight[1] = new Shader.PointLight(this, 1);
            PointLight[2] = new Shader.PointLight(this, 2);
        }





    }
}
