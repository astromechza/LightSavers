using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.MenuObjects
{
    public class MenuBackground
    {
        private Viewport viewport;

        private Renderer renderer;
        private SimpleSceneGraph lightAndMeshContainer;
        private Camera camera;

        //construct
        public MenuBackground()
        {
            viewport = Globals.graphics.GraphicsDevice.Viewport;

            // Create the renderer, this renderer binds to the graphics device and with the given width and height, is used to 
            // draw everything on each frame
            renderer = new Renderer(Globals.graphics.GraphicsDevice, Globals.content, viewport.Width, viewport.Height);

            // The light and mesh container is used to store mesh and light obejcts. This is just for RENDERING. Not for DRAWING
            lightAndMeshContainer = new SimpleSceneGraph(); 
            lightAndMeshContainer.SetSubMeshDelegate(delegate(Mesh.SubMesh subMesh)
            {
                renderer.SetupSubMesh(subMesh);
                subMesh.RenderEffect.AmbientParameter.SetValue(Vector4.Zero);
            });
            lightAndMeshContainer.SetLightDelegate(delegate(Light l) { });

            camera = new Camera();
            camera.Aspect = viewport.AspectRatio;
            camera.NearClip = 0.1f;
            camera.FarClip = 1000;
            camera.Viewport = viewport;
            camera.Transform = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(170),-0.3f,0) * Matrix.CreateTranslation(2,5f,-4);

            Mesh m = new Mesh();
            m.Model = AssetLoader.mdl_menuscene;
            m.Transform = Matrix.Identity;
            lightAndMeshContainer.AddMesh(m);

            Light mainlight = new Light();
            mainlight.LightType = Light.Type.Spot;
            mainlight.Color = Color.AntiqueWhite;
            mainlight.Transform = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(170),-0.7f,0) * Matrix.CreateTranslation(0, 5f, -4);
            mainlight.SpotAngle = 50;
            mainlight.SpotExponent = 1;
            mainlight.Radius = 13;
            mainlight.Intensity = 2f;
            lightAndMeshContainer.AddLight(mainlight);
        }

        //draw
        public void Draw(SpriteBatch canvas)
        {

            RenderTarget2D o = renderer.RenderScene(camera, lightAndMeshContainer, new GameTime());
            
            canvas.Draw(o, viewport.Bounds, Color.White);
        }

        //camera modification

    }
}
