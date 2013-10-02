using LightPrePassRenderer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    public class Door
    {
        public enum DoorState { OPEN, CLOSING, CLOSED, OPENING };
        private Matrix FLIP180 = Matrix.CreateRotationX((float)Math.PI);

        private const float MAXOPEN = 1.3f;
        private const float MINOPEN = 0.0f;
        private const float ALMOSTOPEN = MAXOPEN - 0.05f;
        private const float ALMOSTCLOSED = MINOPEN + 0.05f;
        private const float SPEED = 1.0f/2000; // float per second
        private const float DISTANCE = 6;

        // Meshes
        private Mesh doorBaseMesh;
        private Mesh doorLeftMesh;
        private Mesh doorRightMesh;

        private Light lightLeft;
        private Light lightRight;

        private RealGame game;

        private Vector3 position;

        private DoorState state;
        private float openPercent;
        

        public Door(RealGame game, Vector3 position)
        {
            this.game = game;
            this.position = position;

            doorBaseMesh = new Mesh();
            doorBaseMesh.Model = AssetLoader.mdl_doorBase;
            doorBaseMesh.Transform = Matrix.CreateTranslation(position);

            doorLeftMesh = new Mesh();
            doorLeftMesh.Model = AssetLoader.mdl_doorPanel;
            doorLeftMesh.Transform = Matrix.CreateTranslation(position);

            doorRightMesh = new Mesh();
            doorRightMesh.Model = AssetLoader.mdl_doorPanel;
            doorRightMesh.Transform = FLIP180 * Matrix.CreateTranslation(position + new Vector3(0, 2.5f, 0));

            game.sceneGraph.AddMesh(doorBaseMesh);
            game.sceneGraph.AddMesh(doorLeftMesh);
            game.sceneGraph.AddMesh(doorRightMesh);

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

            game.sceneGraph.AddLight(lightRight);
            game.sceneGraph.AddLight(lightLeft);

            state = DoorState.OPENING;
            openPercent = MINOPEN;
            UpdatePanelPositions();
        }

        public void Update(float ms)
        {
            // TODO: remove once we have events firing properly
            if (true)
            {
                PlayerObject p = game.GetClosestPlayer(position);
                float d = Vector3.DistanceSquared(p.Position, position);
                if (d < DISTANCE)
                {
                    state = DoorState.OPENING;
                    lightRight.Color = Color.Green;
                    lightLeft.Color = Color.Green;
                }
                else
                {
                    state = DoorState.CLOSING;
                    lightRight.Color = Color.Red;
                    lightLeft.Color = Color.Red;
                }
            }  

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
            doorLeftMesh.Transform = Matrix.CreateTranslation(position + new Vector3(0, 0, openPercent));
            doorRightMesh.Transform = FLIP180 * Matrix.CreateTranslation(position + new Vector3(0, 2.5f, -openPercent));

        }

        public void Open()
        {
            if (state == DoorState.CLOSED || state == DoorState.CLOSING)
            {
                state = DoorState.OPENING;
            }
        }

        public void Close()
        {
            if (state == DoorState.OPENING || state == DoorState.OPEN)
            {
                state = DoorState.CLOSING;
            }
        }

    }
}
