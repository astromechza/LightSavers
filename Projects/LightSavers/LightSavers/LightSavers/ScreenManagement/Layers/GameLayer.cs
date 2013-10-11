﻿using System;
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
            //canvas.Draw(AssetLoader.shot_blue, new Rectangle(0, viewport.Bounds.Height - (467 / 4), 981 / 4, 467 / 4), Color.White);
            //canvas.Draw(AssetLoader.pistol_green, new Rectangle(viewport.Bounds.Width - (981 / 4), viewport.Bounds.Height - (467 / 4), 981 / 4, 467 / 4), Color.White);
            int blueWeapon = 0;
            int greenWeapon = 0;

            Texture2D BlueTex = AssetLoader.pistol_blue;
            Texture2D GreenTex = AssetLoader.pistol_green;

            //finding blue hud
            if (blueWeapon == 1)
            {
                BlueTex = AssetLoader.shot_blue;
            }
            else if (blueWeapon == 2)
            {
                BlueTex = AssetLoader.rifle_blue;
            }
            else if (blueWeapon == 3)
            {
                BlueTex = AssetLoader.sniper_blue;
            }
            else if (blueWeapon == 4)
            {
                BlueTex = AssetLoader.sword_blue;
            }

            //finding green hud
            if (greenWeapon == 1)
            {
                GreenTex = AssetLoader.shot_green;
            }
            else if (greenWeapon == 2)
            {
                GreenTex = AssetLoader.rifle_green;
            }
            else if (greenWeapon == 3)
            {
                GreenTex = AssetLoader.sniper_green;
            }
            else if (greenWeapon == 4)
            {
                GreenTex = AssetLoader.sword_green;
            }

            canvas.Draw(AssetLoader.ammo, new Rectangle(10, viewport.Bounds.Height - (140) - 60, 40, 80), Color.White);
            canvas.Draw(BlueTex, new Rectangle(0, viewport.Bounds.Height - (140), 249, 140), Color.White);
            canvas.Draw(GreenTex, new Rectangle(viewport.Bounds.Width - 249, viewport.Bounds.Height - (140), 249, 140), Color.White);
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
