using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Shader
{
    public class PointLight
    {
        // Is the light enabled. False by default. True'd on creation.
        EffectParameter _enabled;
        public EffectParameter Enabled { get { return _enabled; } }

        // Light Diffuse Colour
        EffectParameter _diffuse;
        public EffectParameter Diffuse { get { return _diffuse; } }

        // World Position 
        EffectParameter _position;
        public EffectParameter Position { get { return _position; } }

        // Attenuation Range and Constants
        EffectParameter _attenuation;
        public EffectParameter Attenuation { get { return _attenuation; } }

        public PointLight(TestShader shader, int lightIndex)
        {
            _enabled = shader.Effect.Parameters[String.Format("PointLight{0}Enabled", lightIndex)];
            _diffuse = shader.Effect.Parameters[String.Format("PointLight{0}Diffuse", lightIndex)];
            _attenuation = shader.Effect.Parameters[String.Format("PointLight{0}Attenuation", lightIndex)];
            _position = shader.Effect.Parameters[String.Format("PointLight{0}Position", lightIndex)];
            _enabled.SetValue(true);
        }
    }
}
