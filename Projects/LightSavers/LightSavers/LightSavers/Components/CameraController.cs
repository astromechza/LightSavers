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

        private Vector3 targetXYZ;

        // == Constructors ===
        public CameraController(Viewport v)
        {
            camera = new Camera();
            camera.Aspect = v.AspectRatio;
            camera.NearClip = 0.1f;
            camera.FarClip = 1000;
            camera.Viewport = v;
            camera.Transform = Matrix.Identity;
            targetXYZ = camera.Transform.Translation;
        }

        public CameraController(Viewport v, Matrix initialTransform)
        {
            camera = new Camera();
            camera.Aspect = v.AspectRatio;
            camera.NearClip = 0.1f;
            camera.FarClip = 1000;
            camera.Viewport = v;
            camera.Transform = initialTransform;
            targetXYZ = camera.Transform.Translation;
        }

        public void Update(float ms)
        {
            Matrix cam = camera.Transform;
            Vector3 to = targetXYZ;

            Vector3 too = Vector3.Lerp(cam.Translation, to, ms / 300);
            cam.Translation = too;

            camera.Transform = cam;
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

            float r = Math.Max(dx, dz) + 1.5f;

            float Y = Math.Abs(r / (float)Math.Sin(22.5f));

            targetXYZ = new Vector3(X, Y + 3, Z);

        }
    }
}
