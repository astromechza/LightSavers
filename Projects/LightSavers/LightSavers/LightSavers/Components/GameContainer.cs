using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightSavers.ScreenManagement.Layers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LightSavers.Components.Shader;

namespace LightSavers.Components
{
    public class GameContainer
    {
        private GameLayer gameLayer; // parent layer

        private Camera camera;
        private WorldContainer world;

        TestShader shader;

        public GameContainer(GameLayer layer)
        {
            // set parent layer
            gameLayer = layer;

            // load level
            world = new WorldContainer();
            world.Load("level0");

            // set camera
            camera = new Camera(new Vector3(32,0,32));

            shader = new TestShader();

            shader.AmbientLightColour.SetValue(new Vector4(0.05f, 0.05f, 0.05f, 1.0f));
            shader.AmbientLightColour.SetValue(new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

            shader.WorldMatrix.SetValue(Matrix.Identity);

            shader.CurrentTexture.SetValue(AssetLoader.tex_white);

        }

        public void DrawWorld(float millis)
        {
            shader.ViewMatrix.SetValue(camera.GetViewMatrix());
            shader.ProjectionMatrix.SetValue(camera.GetProjectionMatrix());
            shader.CamPosition.SetValue(camera.GetPosition());
            world.Draw(millis, camera, shader);
        }

        public void DrawHud(SpriteBatch canvas, Viewport viewport)
        {
            //throw new NotImplementedException();
        }

        public void Update(float millis)
        {


            if (Globals.inputController.isButtonReleased(Buttons.Back, null))
            {
                gameLayer.StartTransitionOff();
            }

            Vector2 v = Globals.inputController.getAnalogVector(AnalogStick.Left, null);

            if (v.Length() > 0.1f)
            {

                Vector3 v3 = new Vector3(v.X, 0, -v.Y);

                camera = new Camera(camera.GetFocus() + v3 * millis / 20);

            }

            Vector2 v2 = Globals.inputController.getAnalogVector(AnalogStick.Right, null);

            if (v2.Length() > 0.1f)
            {

                Vector3 v3 = new Vector3(v2.X, v2.Y, 0);

                camera = new Camera(camera.GetFocus() + v3 * millis / 20);

            }

            world.Update(millis);

        }

    }
}
