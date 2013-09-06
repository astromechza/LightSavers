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
        private GameLayer gameLayer;        // parent layer

        private Camera camera;              // camera viewing the world
        private WorldContainer world;       // the world : all objects and things


        public GameContainer(GameLayer layer)
        {
            // set parent layer
            gameLayer = layer;

            // load level
            world = new WorldContainer("level0");

            // set camera
            camera = new Camera(new Vector3(32,0,32));


        }


        public void DrawWorld()
        {

            // draw the world
            world.Draw(camera);
        }

        public void DrawHud(SpriteBatch canvas, Viewport viewport)
        {
            //throw new NotImplementedException();
        }

        public void Update(float ms)
        {
            // if back is pressed. exit layer
            if (Globals.inputController.isButtonReleased(Buttons.Back, null)) gameLayer.StartTransitionOff();

            // control horizantal movement
            Vector2 v = Globals.inputController.getAnalogVector(AnalogStick.Left, null);
            if (v.Length() > 0.1f)
            {
                Vector3 v3 = new Vector3(v.X, 0, -v.Y);
                camera = new Camera(camera.GetFocus() + v3 * ms / 30);
            }

            // control vertical movement
            Vector2 v2 = Globals.inputController.getAnalogVector(AnalogStick.Right, null);
            if (v2.Length() > 0.1f)
            {
                Vector3 v3 = new Vector3(v2.X, v2.Y, 0);
                camera = new Camera(camera.GetFocus() + v3 * ms / 30);
            }

            world.Update(ms);

        }

    }
}
