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
        private GameLayer gameLayer;
        private WorldContainer world;
        private Camera camera;

        FloorAndWallSet floorwalls;

        BasicEffect quadEffect;

        public GameContainer(GameLayer layer)
        {
            gameLayer = layer;
            camera = new Camera(new Vector3(32.1f,0,32));

            floorwalls = new FloorAndWallSet(Vector3.Zero,
                new int[,]{
                    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1},
                    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,1},
                    {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,1},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,1},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,1},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
                },
            2);
            
            quadEffect = new BasicEffect(Globals.graphics.GraphicsDevice);


            quadEffect.PreferPerPixelLighting = true;
            quadEffect.LightingEnabled = true;
            //quadEffect.DirectionalLight0.Enabled = false;
            quadEffect.DirectionalLight0.Direction = new Vector3(-1, -3, -1);
            quadEffect.DirectionalLight0.DiffuseColor = new Vector3(0.2f,0.2f,0.2f);
            quadEffect.DirectionalLight0.SpecularColor = new Vector3(0, 0, 0);

            quadEffect.FogEnabled = true;
            quadEffect.FogColor = new Vector3(0, 0, 0);
            quadEffect.FogStart = 32;
            quadEffect.FogEnd = 48;

            quadEffect.AmbientLightColor = new Vector3(0.4f,0.4f,0.4f);
            quadEffect.SpecularPower = 50f;

            quadEffect.World = Matrix.Identity;
            quadEffect.TextureEnabled = true;
            quadEffect.Texture = AssetLoader.tex_floors;
            


        }

        public void DrawWorld()
        {


            quadEffect.View = camera.GetViewMatrix();
            quadEffect.Projection = camera.GetProjectionMatrix();

            foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Globals.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, floorwalls.vertices, 0, floorwalls.vertices.Length, floorwalls.indices, 0, floorwalls.indices.Length / 3);
            }

        }

        public void DrawHud(SpriteBatch canvas, Viewport viewport)
        {
            //throw new NotImplementedException();
        }

        public void Update(GameTime gametime)
        {
            if (Globals.inputController.isButtonReleased(Buttons.Back, null))
            {
                gameLayer.StartTransitionOff();
            }

            Vector2 v = Globals.inputController.getAnalogVector(AnalogStick.Left, null);

            if (v.Length() > 0.1f)
            {

                Vector3 v3 = new Vector3(v.X, 0, -v.Y);

                camera = new Camera(camera.GetFocus() + v3 * 0.1f);

            }

            Vector2 v2 = Globals.inputController.getAnalogVector(AnalogStick.Right, null);

            if (v2.Length() > 0.1f)
            {

                Vector3 v3 = new Vector3(v2.X, v2.Y, 0);

                camera = new Camera(camera.GetFocus() + v3 * 0.1f);

            }

        }

    }
}
