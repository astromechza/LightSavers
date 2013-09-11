using LightPrePassRenderer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    public class PlayerObject : GameObject
    {
        #region CONSTANTS
        // Player orientation
        float PLAYER_YORIGIN = 0.8f;
        float PLAYER_SCALE = 0.6f;

        // Light stuff
        float TORCH_HEIGHT = 0.6f;
        float HALO_HEIGHT = 6.0f;
        #endregion



        private PlayerIndex playerIndex;
        private Color color;

        private Mesh mesh;

        // Transform info
        private Vector3 position;

        private float rotation;

        // Lighting variables
        private Light torchlight;
        private Light halolight;

        public PlayerObject(PlayerIndex playerIndex, Color color, Vector3 pos, float initialYRot)
        {
            this.playerIndex = playerIndex;
            this.color = color;

            // initial transform
            position = pos;
            rotation = initialYRot;

            mesh = new Mesh();
            mesh.Model = AssetLoader.mld_character;
            
            SetupLights();
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            mesh.Transform = Matrix.CreateScale(PLAYER_SCALE) * Matrix.CreateRotationY(rotation+(float)Math.PI) * Matrix.CreateTranslation(position + new Vector3(0, PLAYER_YORIGIN, 0));
            halolight.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(position + new Vector3(0, HALO_HEIGHT, 0));
            torchlight.Transform = Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(position + new Vector3(0, TORCH_HEIGHT, 0));
        }

        public override void Update(float ms)
        {

            Vector2 v = Globals.inputController.getAnalogVector(AnalogStick.Left, playerIndex);
            if (v.Length() > 0.1f)
            {
                Vector3 pdelta = new Vector3(v.X, 0, -v.Y);
                pdelta.Normalize();
                // modifies the horizantal direction
                position += pdelta * ms / 100;

                


            }

            Vector2 v2 = Globals.inputController.getAnalogVector(AnalogStick.Right, playerIndex);
            if (v2.Length() > 0.01f)
            {
                // get target angle
                float targetrotation = (float)Math.Atan2(v2.Y, v2.X) - MathHelper.PiOver2;

                // get difference
                float deltarotation = targetrotation - rotation;

                // sanitise
                if (deltarotation > Math.PI)
                    deltarotation -= MathHelper.TwoPi;
                if (deltarotation < -Math.PI)
                    deltarotation += MathHelper.TwoPi;

                // add difference
                rotation += 0.1f * deltarotation;

                // sanitise rotation
                if (rotation > MathHelper.TwoPi) rotation -= MathHelper.TwoPi;
                if (rotation < -MathHelper.TwoPi) rotation += MathHelper.TwoPi;
            }

           


            UpdateTransform();
        }

        public void SetupLights()
        {

            torchlight = new Light();
            torchlight.LightType = Light.Type.Spot;
            torchlight.ShadowDepthBias = 0.005f;
            torchlight.Radius = 20;
            torchlight.SpotAngle = 25;
            torchlight.Intensity = 1.5f;
            torchlight.Color = color;
            torchlight.CastShadows = true;
            torchlight.Transform = Matrix.Identity;

            halolight = new Light();
            halolight.LightType = Light.Type.Spot;
            halolight.ShadowDepthBias = 0.001f;
            halolight.Radius = 8;
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
        public Mesh GetMesh()
        {
            return mesh;
        }

        public override RectangleF GetBoundRect()
        {
            return new RectangleF();
        }
    }
}
