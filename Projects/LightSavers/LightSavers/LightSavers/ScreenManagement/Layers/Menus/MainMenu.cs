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
        private RenderTarget2D menu3dscene;
        private SpriteBatch canvas;
        private Viewport viewport;
        private SpriteFont spriteFont;

        public Matrix viewMatrix;
        public Matrix projectionMatrix;

        public float spin;

        public MainMenu()
            : base()
        {
           
            isTransparent = false;

            canvas = new SpriteBatch(Globals.graphics.GraphicsDevice);
            viewport = Globals.graphics.GraphicsDevice.Viewport;
            menu3dscene = new RenderTarget2D(
                Globals.graphics.GraphicsDevice, 
                viewport.Width, 
                viewport.Height, 
                false, 
                SurfaceFormat.Color, 
                DepthFormat.Depth24, 
                0, 
                RenderTargetUsage.DiscardContents);

            spriteFont = Globals.content.Load<SpriteFont>("SpriteFont1");

            this.transitionOnTime = TimeSpan.FromSeconds(0.6);
            this.transitionOffTime = TimeSpan.FromSeconds(0.5);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), viewport.AspectRatio, 1f, 10000);

            viewMatrix = Matrix.CreateLookAt(new Vector3(1.0f, 0.5f, 1.0f), Vector3.Zero, Vector3.Up);

            spin = 0.0f;
        }

        public override void Draw(GameTime gameTime)
        {
            Globals.graphics.GraphicsDevice.SetRenderTarget(menu3dscene);

            Globals.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            Globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Globals.graphics.GraphicsDevice.Clear(Color.Gray);
            

            foreach (ModelMesh mesh in AssetLoader.cube_mdl.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = Matrix.CreateScale(0.3f) * Matrix.CreateRotationY(spin);

                    effect.Projection = projectionMatrix;
                    effect.View = viewMatrix;
                }
                mesh.Draw();
            }

            Globals.graphics.GraphicsDevice.SetRenderTarget(null);


            canvas.Begin();
            
            canvas.Draw(menu3dscene, viewport.Bounds, Color.White);

            canvas.DrawString(spriteFont, "MAIN MENU SCREEN", new Vector2(300, 300), Color.Blue);

            if (state == ScreenState.TransitioningOff || state == ScreenState.TransitioningOn)
            {
                int trans = (int)((1 - transitionPercent) * 255.0f);
                canvas.Draw(AssetLoader.black_tex, viewport.Bounds, new Color(trans, trans, trans, trans));
            }
            
            canvas.End();
        }

        public override void Update(GameTime gameTime)
        {
            spin += 0.01f;
            base.Update(gameTime);
        }

       

    }
}
