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
            camera = new Camera(new Vector3(32,0,32));

            floorwalls = new FloorAndWallSet(Vector3.Zero,
                new int[,]{
                    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,0,0,0,0,1,1,1,1,1,1,0,0,0,0,1},
                    {1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0},
                    {1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0},
                    {1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0},
                    {1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0},
                    {1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
                },
            4);
            
            quadEffect = new BasicEffect(Globals.graphics.GraphicsDevice);


            quadEffect.PreferPerPixelLighting = true;
            quadEffect.LightingEnabled = true;
            quadEffect.DirectionalLight0.Enabled = false;

            quadEffect.AmbientLightColor = new Vector3(0.8f,0.8f,0.8f);
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

        public void Update()
        {
            if (Globals.inputController.isButtonReleased(Buttons.Back, null))
            {
                gameLayer.StartTransitionOff();
            }
        }

    }
}
