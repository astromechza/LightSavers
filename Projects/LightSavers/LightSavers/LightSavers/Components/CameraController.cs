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
        public const float MAX_DISTANCE_BETWEEN_PLAYERS = 40f;
        public const float FOV_OVER_TWO = 22.5f;
        public float SIN_FOV_OVER_TWO = (float)Math.Sin(MathHelper.ToRadians(FOV_OVER_TWO));
        public float MAX_HEIGHT = ((MAX_DISTANCE_BETWEEN_PLAYERS * 0.7f) / 2) / (float)Math.Tan(MathHelper.ToRadians(FOV_OVER_TWO));

        private Matrix CAMERA_PITCH = Matrix.CreateRotationX(MathHelper.ToRadians(15));
        private Matrix CAMERA_DOWN = Matrix.CreateRotationX(MathHelper.ToRadians(-90));

        private Camera camera;
        public Camera Camera { get { return camera; } }

        private Vector3 lastXYZ;
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
            lastXYZ = targetXYZ;
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
            lastXYZ = targetXYZ;
        }

        public void Update(float ms)
        {
            lastXYZ = Vector3.Lerp(lastXYZ, targetXYZ, ms / 1000);

            MoveToTarget();
        }

        public void Fit(List<Vector2> list)
        {
            
            float minx = 10000;
            float maxx = 0;
            float minz = 32;
            float maxz = 0;

            for(int i=0;i<list.Count;i++)
            {
                minx = Math.Min(list[i].X, minx);
                minz = Math.Min(list[i].Y, minz);
                maxx = Math.Max(list[i].X, maxx);
                maxz = Math.Max(list[i].Y, maxz);
            }

            //arb tweaks
            minz = minz - 2;
            maxz = maxz + 2;

            float X = (minx + maxx) / 2;
            float Z = (minz + maxz) / 2;

            float dx = (X - minx);
            float dz = (Z - minz);

            float r = Math.Max(dx, dz) + 1.5f;

            float Y = Math.Abs(r / SIN_FOV_OVER_TWO) + 2;

            if (Y > MAX_HEIGHT) Y = MAX_HEIGHT;

            targetXYZ = new Vector3(X, Y, Z);

        }

        public void MoveToTarget()
        {
            camera.Transform = CAMERA_DOWN * Matrix.CreateTranslation(targetXYZ);
        }
    }
}
