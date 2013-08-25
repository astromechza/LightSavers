using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightSavers.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightSavers.ScreenManagement.Layers
{
    public class GameLayer : ScreenLayer
    {
        private Viewport viewport;
        private RenderTarget2D game3DLayer;
        private SpriteBatch canvas;

        public GameContainer gameContainer;

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

            gameContainer = new GameContainer(this);

        }

        public override void Draw(float millis)
        {
            // First we need to draw to a temporary buffer
            Globals.graphics.GraphicsDevice.SetRenderTarget(game3DLayer);
            Globals.graphics.GraphicsDevice.Clear(Color.Black);
            // reset these because spritebatch can do nasty stuff
            Globals.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            /*/*/// Draw everything here
            gameContainer.DrawWorld(millis);

            // Now switch back to the main render device
            Globals.graphics.GraphicsDevice.SetRenderTarget(null);

            // Draw the layers
            canvas.Begin();

            // draw the 3d scene
            canvas.Draw(game3DLayer, viewport.Bounds, Color.White);

            gameContainer.DrawHud(canvas, viewport);

            // draw the black transparent thing
            if (state == ScreenState.TransitioningOff || state == ScreenState.TransitioningOn)
            {
                int trans = (int)((1 - transitionPercent) * 255.0f);
                canvas.Draw(AssetLoader.tex_black, viewport.Bounds, new Color(trans, trans, trans, trans));
            }

            canvas.End();
            
        }

        public override void Update(float millis)
        {

            gameContainer.Update(millis);

            base.Update(millis);
        }


    }
}
