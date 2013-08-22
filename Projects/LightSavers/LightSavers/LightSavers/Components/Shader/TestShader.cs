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

        public DirectionalLight DirectionalLight0;
        public DirectionalLight DirectionalLight1;
        public DirectionalLight DirectionalLight2;
        public DirectionalLight DirectionalLight3;

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

            DirectionalLight0 = new DirectionalLight(this, 0);
            DirectionalLight1 = new DirectionalLight(this, 1);
            DirectionalLight2 = new DirectionalLight(this, 2);
            DirectionalLight3 = new DirectionalLight(this, 3);

        }





    }
}
