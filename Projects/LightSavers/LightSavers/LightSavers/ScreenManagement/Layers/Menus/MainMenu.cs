using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightSavers.ScreenManagement.Layers.Menus
{
    public class MainMenu : ScreenLayer
    {
        private Viewport viewport;
        private RenderTarget2D menu3dscene;
        private SpriteBatch canvas;
        public Matrix viewMatrix;
        public Matrix projectionMatrix;
        Model model;


        public MainMenu() : base()
        {
           
            // Screen layer attributes
            isTransparent = false;
            transitionOnTime = TimeSpan.FromSeconds(0.6);
            transitionOffTime = TimeSpan.FromSeconds(0.5);

            // 3D view vars
            viewport = Globals.graphics.GraphicsDevice.Viewport;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), viewport.AspectRatio, 0.5f, 10000);
            viewMatrix = Matrix.CreateLookAt(new Vector3(0.8f, 0.4f, 0.8f), new Vector3(0, 0.3f, 0.2f), Vector3.Up);

            // layers
            canvas = new SpriteBatch(Globals.graphics.GraphicsDevice);
            menu3dscene = new RenderTarget2D(
                Globals.graphics.GraphicsDevice,
                viewport.Width,
                viewport.Height,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24,
                0,
                RenderTargetUsage.DiscardContents);

            // the actual 3d model
            model = AssetLoader.mdl_menuscene;

        }

        public override void Draw(GameTime gameTime)
        {
            // First we need to draw to a temporary buffer
            Globals.graphics.GraphicsDevice.SetRenderTarget(menu3dscene);

            // reset these because spritebatch can do nasty stuff
            Globals.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            Globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // draw the model!
            for (int i = 0; i < model.Meshes.Count; i++)
            {

                foreach (BasicEffect effect in model.Meshes[i].Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true; // remember DEFAULT lighting causes a big hit on fps
                    effect.World = model.Meshes[i].ParentBone.Transform * Matrix.CreateScale(0.2f); // respect different meshes transforms

                    effect.Projection = projectionMatrix;
                    effect.View = viewMatrix;
                }
                model.Meshes[i].Draw();
            }

            // Now switch back to the main render device
            Globals.graphics.GraphicsDevice.SetRenderTarget(null);

            // Draw the layers
            canvas.Begin();

            canvas.Draw(menu3dscene, viewport.Bounds, Color.White);

            if (state == ScreenState.TransitioningOff || state == ScreenState.TransitioningOn)
            {
                int trans = (int)((1 - transitionPercent) * 255.0f);
                canvas.Draw(AssetLoader.tex_black, viewport.Bounds, new Color(trans, trans, trans, trans));
            }

            canvas.End();
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

       

    }
}
