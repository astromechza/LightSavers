using LightPrePassRenderer;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    public class Teleporter
    {
        private Vector3 _position;
        public Vector3 Position { get { return _position; } }
        private Mesh padmesh;

        private Mesh beamMesh;

        private float progress;
        private bool inProgress;

        public bool Started { get { return inProgress; } }
        public bool Finished { get { return (progress > 100); } }
        public float Progress { get { return progress; } }

        public Teleporter(Vector3 pos)
        {
            this._position = pos;
            this.padmesh = new Mesh();
            this.padmesh.Model = AssetLoader.mdl_teleport_pad;
            this.padmesh.Transform = Matrix.CreateTranslation(pos + new Vector3(0,0.15f,0));
            this.padmesh.SetCastShadows(false);
            Globals.gameInstance.sceneGraph.Setup(this.padmesh);
            Globals.gameInstance.sceneGraph.Add(this.padmesh);

            this.beamMesh = new Mesh();
            this.beamMesh.Model = AssetLoader.mdl_teleport_beam;
            this.beamMesh.Transform = Matrix.CreateScale(1,1,1) * Matrix.CreateTranslation(pos);
            Globals.gameInstance.sceneGraph.Setup(this.beamMesh);
            Globals.gameInstance.sceneGraph.Add(this.beamMesh);
            this.inProgress = false;
            this.progress = 0;
        }

        public void Update(float ms)
        {
            if (inProgress)
            {
                progress += ms/80;
                this.beamMesh.Transform = Matrix.CreateScale(1, progress*10, 1) * Matrix.CreateTranslation(_position);
                if (progress > 100)
                {
                    inProgress = false;
                }
            }
        }

        public void Start()
        {
            progress = 0;
            inProgress = true;
        }
    }
}
