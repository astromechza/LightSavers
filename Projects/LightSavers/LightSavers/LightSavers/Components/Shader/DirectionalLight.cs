using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Shader
{
    public class DirectionalLight
    {
        EffectParameter _enabled;
        public EffectParameter Enabled { get { return _enabled; } }
        EffectParameter _colour;
        public EffectParameter Colour { get { return _colour; } }
        EffectParameter _specular;
        public EffectParameter SpecularColour { get { return _specular; } }
        EffectParameter _direction;
        public EffectParameter Direction { get { return _direction; } }

        public DirectionalLight(TestShader shader, int lightIndex)
        {
            _enabled = shader.Effect.Parameters[String.Format("Light{0}Enabled", lightIndex)];
            _colour = shader.Effect.Parameters[String.Format("Light{0}Colour", lightIndex)];
            _direction = shader.Effect.Parameters[String.Format("Light{0}Direction", lightIndex)];
            _specular = shader.Effect.Parameters[String.Format("Light{0}Specular", lightIndex)];

            _enabled.SetValue(false);
        }
    }
}
