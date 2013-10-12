using LightPrePassRenderer;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    public class Door
    {
        #region CONSTANTS AND ENUMS
        public enum DoorState { OPEN, CLOSING, CLOSED, OPENING };
        private Matrix FLIP180X = Matrix.CreateRotationX((float)Math.PI);
        private Matrix FLIP90Y = Matrix.CreateRotationY(MathHelper.PiOver2);
        private const float MAXOPEN = 1.3f;
        private const float MINOPEN = 0.0f;
        private const float ALMOSTOPEN = MAXOPEN - 0.05f;
        private const float ALMOSTCLOSED = MINOPEN + 0.05f;
        private const float SPEED = 1.0f/1500; // float per second
        private const float DISTANCE = 8;
        #endregion

        // Meshes
        private Mesh doorBaseMesh;
        private Mesh doorLeftMesh;
        private Mesh doorRightMesh;

        private Light lightLeft;
        private Light lightRight;

        private Vector3 position;
        public Vector3 Position { get { return position; } }

        private DoorState state;
        private float openPercent;
        

        public Door(Vector3 position)
        {
            this.position = position;

            doorBaseMesh = new Mesh();
            doorBaseMesh.Model = AssetLoader.mdl_doorBase;
            doorBaseMesh.Transform = FLIP90Y * Matrix.CreateTranslation(position);

            doorLeftMesh = new Mesh();
            doorLeftMesh.Model = AssetLoader.mdl_doorPanel;
            doorLeftMesh.Transform = FLIP90Y * Matrix.CreateTranslation(position);

            doorRightMesh = new Mesh();
            doorRightMesh.Model = AssetLoader.mdl_doorPanel;
            doorRightMesh.Transform = FLIP180X * FLIP90Y * Matrix.CreateTranslation(position + new Vector3(0, 2.5f, 0));

            Globals.gameInstance.sceneGraph.Setup(doorBaseMesh);
            Globals.gameInstance.sceneGraph.Add(doorBaseMesh);
            Globals.gameInstance.sceneGraph.Setup(doorLeftMesh);
            Globals.gameInstance.sceneGraph.Add(doorLeftMesh);
            Globals.gameInstance.sceneGraph.Setup(doorRightMesh);
            Globals.gameInstance.sceneGraph.Add(doorRightMesh);

            lightLeft = new Light();
            lightLeft.LightType = Light.Type.Point;
            lightLeft.Color = Color.Red;
            lightLeft.Radius = 2;
            lightLeft.Transform = Matrix.CreateTranslation(position + new Vector3(-0.6f, 1.5f, -1.4f));

            lightRight = new Light();
            lightRight.LightType = Light.Type.Point;
            lightRight.Color = Color.Red;
            lightRight.Radius = 2;
            lightRight.Transform = Matrix.CreateTranslation(position + new Vector3(-0.6f, 1.5f, 1.4f));

            Globals.gameInstance.sceneGraph.Add(lightRight);
            Globals.gameInstance.sceneGraph.Add(lightLeft);

            state = DoorState.CLOSED;
            openPercent = MINOPEN;
            UpdatePanelPositions();
        }

        public void Update(float ms)
        {

            if (state == DoorState.CLOSING)
            {
                openPercent -= ms * SPEED;
                if (openPercent < ALMOSTCLOSED) { state = DoorState.CLOSED; openPercent = MINOPEN; };
                UpdatePanelPositions();
            }
            else if (state == DoorState.OPENING)
            {
                openPercent += ms * SPEED;
                if (openPercent > ALMOSTOPEN) { state = DoorState.OPEN; openPercent = MAXOPEN; };
                UpdatePanelPositions();
            }
        }

        private void UpdatePanelPositions()
        {
            doorLeftMesh.Transform = FLIP90Y * Matrix.CreateTranslation(position + new Vector3(0, 0, openPercent));
            doorRightMesh.Transform = FLIP90Y * FLIP180X * Matrix.CreateTranslation(position + new Vector3(0, 2.5f, -openPercent));

        }

        public void Open()
        {
            if (state == DoorState.CLOSED || state == DoorState.CLOSING)
            {
                state = DoorState.OPENING;
                lightRight.Color = Color.Green;
                lightLeft.Color = Color.Green;
            }
        }

        public void Close()
        {
            if (state == DoorState.OPENING || state == DoorState.OPEN)
            {
                state = DoorState.CLOSING;
                lightRight.Color = Color.Red;
                lightLeft.Color = Color.Red;
            }
        }

    }
}
