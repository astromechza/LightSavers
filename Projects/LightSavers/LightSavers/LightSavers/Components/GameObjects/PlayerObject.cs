using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    public class PlayerObject : GameObject
    {

        private PlayerIndex playerIndex;
        private Color color;

        private MeshWrapper mesh;

        // Transform info
        private Vector3 position;
        private float rotation;

        // Lighting variables
        private Light arealight;
        private Light torchlight;
        private Light halolight;

        public PlayerObject(PlayerIndex playerIndex, Color color, Vector3 pos, float initialYRot)
        {
            this.playerIndex = playerIndex;
            this.color = color;

            // initial transform
            position = pos;
            rotation = initialYRot;

            mesh = new MeshWrapper(AssetLoader.mld_character);
            SetupLights();
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            mesh.Transform = Matrix.CreateScale(0.5f) * Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(position + new Vector3(0,0.7f,0));

            halolight.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(position + new Vector3(0, 5.0f, 0));
            torchlight.Transform = Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(position + new Vector3(0, 0.6f, 0));
        }

        public override void Update(float ms)
        {

            Vector2 v = Globals.inputController.getAnalogVector(AnalogStick.Left, playerIndex);
            if (v.Length() > 0.1f)
            {
                // modifies the horizantal direction
                position += new Vector3(v.X, 0, -v.Y) * ms / 100;

                


            }

            Vector2 v2 = Globals.inputController.getAnalogVector(AnalogStick.Right, playerIndex);
            if (v2.Length() > 0.1f)
            {

                float targetrotation = (float)Math.Atan2(v2.Y, v2.X) - MathHelper.PiOver2;

                float deltarotation = targetrotation - rotation;
                if (deltarotation > Math.PI)
                    deltarotation -= MathHelper.TwoPi;
                if (deltarotation < -Math.PI)
                    deltarotation += MathHelper.TwoPi;

                rotation += 0.1f * deltarotation;
            }

           


            UpdateTransform();
        }

        public void SetupLights()
        {

            torchlight = new Light();
            torchlight.LightType = Light.Type.Spot;
            torchlight.ShadowDepthBias = 0.01f;
            torchlight.Radius = 20;
            torchlight.SpotAngle = 25;
            torchlight.Intensity = 1.5f;
            torchlight.Color = color;
            torchlight.CastShadows = true;
            torchlight.Transform = Matrix.Identity;

            halolight = new Light();
            halolight.LightType = Light.Type.Spot;
            halolight.ShadowDepthBias = 0.001f;
            halolight.Radius = 6;
            halolight.SpotAngle = 30;
            halolight.SpotExponent = 1;
            halolight.Intensity = 0.8f;
            halolight.Color = color;
            halolight.CastShadows = true;
            halolight.Transform = Matrix.Identity;

        }


        public Light[] GetLights()
        {
            return new Light[] { torchlight, halolight };
        }
        public MeshWrapper GetMesh()
        {
            return mesh;
        }

        public override RectangleF GetBoundRect()
        {
            return new RectangleF();
        }
    }
}
