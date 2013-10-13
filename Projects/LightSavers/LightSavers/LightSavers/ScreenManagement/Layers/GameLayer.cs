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
            //second number is number of players
            Globals.gameInstance = new RealGame(10, 2, sceneGraph);

            cameraController = new CameraController(viewport, Matrix.Identity);
            cameraController.Fit(Globals.gameInstance.GetCriticalPoints());
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

            //SETTING UP HUDS
            int blueWeapon = Globals.gameInstance.players[0].currentWeapon;
            
            Texture2D BlueTex = AssetLoader.pistol_blue;

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
            String health = "" + Globals.gameInstance.players[0].health;

            Vector2 w = AssetLoader.fnt_healthgamescreen.MeasureString(health);

            canvas.Draw(BlueTex, new Rectangle(0, viewport.Bounds.Height - (104), 197, 104), Color.White);
            canvas.DrawString(AssetLoader.fnt_healthgamescreen, health, new Vector2((160 - w.X / 2), viewport.Bounds.Height - (78)), Color.White);

            

            if (Globals.gameInstance.players.Length == 2)
            {
                int greenWeapon = Globals.gameInstance.players[1].currentWeapon;
                Texture2D GreenTex = AssetLoader.pistol_green;

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
                //Globals.gameInstance.players[1].health = 20;
                health = "" + Globals.gameInstance.players[1].health;

                w = AssetLoader.fnt_healthgamescreen.MeasureString(health);

                canvas.Draw(GreenTex, new Rectangle(viewport.Bounds.Width - 197, viewport.Bounds.Height - (104), 197, 104), Color.White);
                canvas.DrawString(AssetLoader.fnt_healthgamescreen, health, new Vector2(viewport.Bounds.Width - 197 + (37 - w.X / 2), viewport.Bounds.Height - (78)), Color.White);
                
            }
            //FINISHED HUDS

            canvas.End();
            
        }

        public override void Update(float ms)
        {

            Globals.gameInstance.Update(ms);

            if (Globals.inputController.isButtonReleased(Microsoft.Xna.Framework.Input.Buttons.Back, null))
            {
                this.StartTransitionOff();
            }

            cameraController.Fit(Globals.gameInstance.GetCriticalPoints());
            cameraController.Update(ms);
            
            base.Update(ms);
        }


    }
}
