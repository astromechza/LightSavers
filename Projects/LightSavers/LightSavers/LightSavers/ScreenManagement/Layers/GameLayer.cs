using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LightPrePassRenderer;
using LightSavers.Components;
using LightPrePassRenderer.partitioning;
using LightSavers.Utils;

namespace LightSavers.ScreenManagement.Layers
{
    public class GameLayer : ScreenLayer
    {
        private Viewport viewport;
        private RenderTarget2D game3DLayer;
        private SpriteBatch canvas;
        private RealGame game;       // the world : all objects and things

        private Renderer renderer;
        private CameraController cameraController;
        private BlockBasedSceneGraph sceneGraph;

        public GameLayer() : base()
        {
           
            // Screen layer attributes
            isTransparent = false;
            transitionOnTime = TimeSpan.FromSeconds(0.6);
            transitionOffTime = TimeSpan.FromSeconds(0.5);

            // 3D view vars
            viewport = Globals.graphics.GraphicsDevice.Viewport;

            // drawable layers
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

            // Create the renderer, this renderer binds to the graphics device and with the given width and height, is used to 
            // draw everything on each frame
            renderer = new Renderer(Globals.graphics.GraphicsDevice, Globals.content, viewport.Width, viewport.Height);

            // The light and mesh container is used to store mesh and light obejcts. This is just for RENDERING. Not for DRAWING
            sceneGraph = new BlockBasedSceneGraph(10);
            sceneGraph.SetSubMeshDelegate(delegate(Mesh.SubMesh subMesh) 
            {                
                renderer.SetupSubMesh(subMesh);
                subMesh.RenderEffect.AmbientParameter.SetValue(Vector4.Zero);
            });
            sceneGraph.SetLightDelegate(delegate(Light l) { });

            // Load the Game
            game = new RealGame(10, 1, sceneGraph);

            cameraController = new CameraController(viewport, Matrix.Identity);
            cameraController.Fit(game.GetCriticalPoints());
            cameraController.MoveToTarget();
            
        }

        private RenderTarget2D temp;
        public override void Draw()
        {
            
            // reset these because spritebatch can do nasty stuff
            Globals.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            Globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            temp = renderer.RenderScene(cameraController.Camera, sceneGraph);
            
            // Now switch back to the main render device
            Globals.graphics.GraphicsDevice.SetRenderTarget(null);
            Globals.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            Globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            // Draw the layers
            canvas.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            // draw the 3d scene
            canvas.Draw(temp, viewport.Bounds, Color.White);

            canvas.End();
            
        }

        public override void Update(float ms)
        {
            
            game.Update(ms);

            if (Globals.inputController.isButtonReleased(Microsoft.Xna.Framework.Input.Buttons.Back, null))
            {
                this.StartTransitionOff();
            }

            cameraController.Fit(game.GetCriticalPoints());
            cameraController.Update(ms);
            
            base.Update(ms);
        }


    }
}
