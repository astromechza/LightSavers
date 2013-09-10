using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightSavers.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LightSavers.Rendering;

namespace LightSavers.ScreenManagement.Layers
{
    public class GameLayer : ScreenLayer
    {
        private Viewport viewport;
        private RenderTarget2D game3DLayer;
        private SpriteBatch canvas;

        private Renderer renderer;

        private WorldContainer world;       // the world : all objects and things

        private CameraController cameraController;

        public GameLayer() : base()
        {
           
            // Screen layer attributes
            isTransparent = false;
            transitionOnTime = TimeSpan.FromSeconds(0.6);
            transitionOffTime = TimeSpan.FromSeconds(0.5);

            // 3D view vars
            viewport = Globals.graphics.GraphicsDevice.Viewport;

            // layers
            canvas = new SpriteBatch(Globals.graphics.GraphicsDevice);
            game3DLayer = new RenderTarget2D(
                Globals.graphics.GraphicsDevice,
                viewport.Width,
                viewport.Height,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24,
                0,
                RenderTargetUsage.DiscardContents);

            world = new WorldContainer("level0");

            Matrix temp = Matrix.CreateRotationX(MathHelper.ToRadians(-75)) * Matrix.CreateTranslation(new Vector3(4, 16, 8));
            cameraController = new CameraController(viewport, temp);
            
            renderer = new Renderer(viewport.Width, viewport.Height);

        }

        public override void Draw()
        {

            // reset these because spritebatch can do nasty stuff
            Globals.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            Globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            RenderTarget2D o = renderer.RenderScene(cameraController.Camera, world.GetVisibleLights(), world.GetVisibleMeshes());
            
            // Now switch back to the main render device
            Globals.graphics.GraphicsDevice.SetRenderTarget(null);
            Globals.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            Globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Draw the layers
            canvas.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            // draw the 3d scene
            canvas.Draw(o, viewport.Bounds, Color.White);


            canvas.End();
            
        }

        public override void Update(float ms)
        {
            cameraController.HandleInput(ms);


            world.Update(ms);

            base.Update(ms);
        }


    }
}
