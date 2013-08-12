using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightSavers.Components
{
    public class Camera
    {
        private const float FOV = 45.0f;
        private const float NEARPLANE = 0.5f;
        private const float FARPLANE = 10000;

        private Vector3 position;
        private Vector3 focus;

        public Camera(Vector3 f)
        {
            focus = f;
            position = focus + new Vector3(0, 0.7f, 0.2f) * 24;
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.CreateLookAt(position, focus, Vector3.Up);
        }

        public Matrix GetProjectionMatrix()
        {
            return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV), Globals.viewport.AspectRatio, NEARPLANE, FARPLANE);
        }

        public Vector3 GetLeftPoint()
        {
            float ha = FOV / 2;
            float dx = (float)Math.Tan(ha) * 14.14f;
            return focus + new Vector3(-dx, 0, 0);
        }

        public Vector3 GetRightPoint()
        {
            float ha = FOV / 2;
            float dx = (float)Math.Tan(ha) * 14.14f;
            return focus + new Vector3(dx, 0, 0);
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public Vector3 GetFocus()
        {
            return focus;
        }

    }
}
