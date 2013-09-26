using LightPrePassRenderer;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components
{
    public class CameraController
    {
        private Camera camera;
        public Camera Camera { get { return camera; } }

        // == Constructors ===
        public CameraController(Viewport v)
        {
            camera = new Camera();
            camera.Aspect = v.AspectRatio;
            camera.NearClip = 0.1f;
            camera.FarClip = 1000;
            camera.Viewport = v;
            camera.Transform = Matrix.Identity;
        }

        public CameraController(Viewport v, Matrix initialTransform)
        {
            camera = new Camera();
            camera.Aspect = v.AspectRatio;
            camera.NearClip = 0.1f;
            camera.FarClip = 1000;
            camera.Viewport = v;

            camera.Transform = initialTransform;
        }

        public void Fit(List<Vector2> list)
        {
            float minx = 10000;
            float maxx = 0;
            float minz = 32;
            float maxz = 0;

            foreach( Vector2 p in list)
            {
                minx = Math.Min(p.X, minx);
                minz = Math.Min(p.Y, minz);
                maxx = Math.Max(p.X, maxx);
                maxz = Math.Max(p.Y, maxz);
            }

            float X = (minx + maxx) / 2;
            float Z = (minz + maxz) / 2;

            float dx = (X - minx);
            float dz = (Z - minz);

            if (dx > dz)
            {
                float ny = dx * 2 * (float)Math.Tan(MathHelper.ToRadians(50));

                float Y = MathHelper.Clamp(ny, 5, 128);
                camera.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(X, Y+3, Z);
            }
            else
            {
                float ny = dz * 2 * (float)Math.Tan(MathHelper.ToRadians(50));

                float Y = MathHelper.Clamp(ny, 5, 128);

                camera.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(X, Y+3, Z);
            }




        }
    }
}
