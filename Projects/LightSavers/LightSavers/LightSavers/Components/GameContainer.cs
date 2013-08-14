using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightSavers.ScreenManagement.Layers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LightSavers.Components
{
    public class GameContainer
    {
        private GameLayer gameLayer; // parent layer

        private Camera camera;
        private WorldContainer world;

        BasicEffect quadEffect;

        public GameContainer(GameLayer layer)
        {
            // set parent layer
            gameLayer = layer;

            // load level
            world = new WorldContainer();
            world.Load("level0");

            // set camera
            camera = new Camera(new Vector3(32.1f,0,32));

            
            
            quadEffect = new BasicEffect(Globals.graphics.GraphicsDevice);


            quadEffect.PreferPerPixelLighting = true;
            
            quadEffect.LightingEnabled = true;
            quadEffect.DirectionalLight0.Enabled = true;
            quadEffect.DirectionalLight0.Direction = new Vector3(-1, -1, -1);
            quadEffect.DirectionalLight0.DiffuseColor = new Vector3(0.1f,0.1f,0.3f);
            quadEffect.DirectionalLight0.SpecularColor = new Vector3(0, 0, 0);

            quadEffect.FogEnabled = true;
            quadEffect.FogColor = new Vector3(0, 0, 0);
            quadEffect.FogStart = 32;
            quadEffect.FogEnd = 48;

            quadEffect.AmbientLightColor = new Vector3(0.1f,0.1f,0.1f);
            quadEffect.SpecularPower = 50f;

            quadEffect.World = Matrix.Identity;
            


        }

        public void DrawWorld()
        {

            quadEffect.View = camera.GetViewMatrix();
            quadEffect.Projection = camera.GetProjectionMatrix();
            world.Draw(camera, quadEffect);
        }

        public void DrawHud(SpriteBatch canvas, Viewport viewport)
        {
            //throw new NotImplementedException();
        }

        public void Update(GameTime gametime)
        {
            float ms = (float)gametime.ElapsedGameTime.TotalMilliseconds;


            if (Globals.inputController.isButtonReleased(Buttons.Back, null))
            {
                gameLayer.StartTransitionOff();
            }

            Vector2 v = Globals.inputController.getAnalogVector(AnalogStick.Left, null);

            if (v.Length() > 0.1f)
            {

                Vector3 v3 = new Vector3(v.X, 0, -v.Y);

                camera = new Camera(camera.GetFocus() + v3 * ms / 20);

            }

            Vector2 v2 = Globals.inputController.getAnalogVector(AnalogStick.Right, null);

            if (v2.Length() > 0.1f)
            {

                Vector3 v3 = new Vector3(v2.X, v2.Y, 0);

                camera = new Camera(camera.GetFocus() + v3 * ms / 20);

            }


        }

    }
}
