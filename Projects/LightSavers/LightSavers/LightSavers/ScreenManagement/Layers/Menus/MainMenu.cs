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

        Model model;

        // Bone lookup matrix
        ModelBone[] bones;

        // Store original transforms away from the origin
        Matrix[] boneOriginTransforms;

        // Keep last bone transforms
        Matrix[] currentBoneTransforms;
        bool currentTransformNeedsRebuild = true;

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

            viewMatrix = Matrix.CreateLookAt(new Vector3(-1.0f, 0.5f, 1.0f), Vector3.Zero, Vector3.Up);

            spin = 0.0f;

            model = AssetLoader.gun_mdl;

            // Store bones and original transform matrices
            bones = new ModelBone[model.Bones.Count];
            boneOriginTransforms = new Matrix[model.Bones.Count];
            for (int i = 0; i < model.Bones.Count; i++)
            {
                bones[i] = model.Bones[i];
                boneOriginTransforms[i] = model.Bones[i].Transform;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Globals.graphics.GraphicsDevice.SetRenderTarget(menu3dscene);

            Globals.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            Globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Globals.graphics.GraphicsDevice.Clear(Color.Gray);

            if (currentTransformNeedsRebuild)
            {
                currentBoneTransforms = new Matrix[AssetLoader.gun_mdl.Bones.Count];
                AssetLoader.gun_mdl.CopyAbsoluteBoneTransformsTo(currentBoneTransforms);
                currentTransformNeedsRebuild = false;
            }

            foreach (ModelMesh mesh in AssetLoader.gun_mdl.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = currentBoneTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(spin) * Matrix.CreateRotationX(spin/2);

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
            spin += (float)gameTime.ElapsedGameTime.TotalSeconds;
            bones[4].Transform = Matrix.CreateTranslation(0,0, spin/360.0f ) * boneOriginTransforms[4];
            currentTransformNeedsRebuild = true;
            base.Update(gameTime);
        }

       

    }
}
