using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    public class WeaponDepot
    {
        public Vector3 _position;
        private float _rotation;

        private Mesh mesh;
        private Light light;

        private int gunindex = 0;

        private Mesh gunmesh;
        private MeshSceneGraphReceipt gunmeshReceipt;


        private Matrix[] gunTransforms = {
                                             Matrix.CreateScale(0.4f)* Matrix.CreateRotationZ(0.5f),
                                             Matrix.CreateScale(0.8f) * Matrix.CreateTranslation(new Vector3(0.5f,0,0)),
                                             Matrix.CreateScale(0.7f) * Matrix.CreateTranslation(new Vector3(0.1f, 0, 0.1f)),
                                             Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(new Vector3(0.2f, 0, 0.1f))
                                         };



        public WeaponDepot(Vector3 center, int index)
        {
            Construct(center);
            SetGun(index);
        }

        public void Construct(Vector3 center)
        {
            this._position = center;
            this._rotation = (float)Globals.random.NextDouble();

            this.mesh = new Mesh();
            this.mesh.Model = AssetLoader.mdl_weapon_depot;
            this.mesh.Transform = Matrix.CreateTranslation(_position);

            this.gunindex = -1;
            this.gunmesh = null;
            this.gunmeshReceipt = null;

            Globals.gameInstance.sceneGraph.Setup(this.mesh);
            Globals.gameInstance.sceneGraph.Add(this.mesh);

            this.light = new Light();
            this.light.LightType = Light.Type.Point;
            this.light.Radius = 3;
            this.light.Intensity = 0.4f;
            this.light.Color = Color.White;
            this.light.Transform = Matrix.CreateTranslation(this._position + Vector3.Up * 2);
            this.light.Enabled = false;

            Globals.gameInstance.sceneGraph.Setup(this.light);
            Globals.gameInstance.sceneGraph.Add(this.light);

        }


        public void Update(float ms)
        {
            if (gunindex >= 0)
            {
                _rotation += ms / 300;
                this.gunmesh.Transform = gunTransforms[gunindex] * Matrix.CreateRotationY(_rotation) * Matrix.CreateTranslation(_position + Vector3.Up);
            }
        }

        public void SetGun(int index)
        {            
            this.gunindex = index;
            this.light.Enabled = true;
            this.gunmesh = new Mesh();
            this.gunmesh.Model = GetModelForIndex(gunindex);
            this.gunmesh.Transform = gunTransforms[gunindex] * Matrix.CreateTranslation(_position + Vector3.Up);

            Globals.gameInstance.sceneGraph.Setup(this.gunmesh);
            this.gunmeshReceipt =  Globals.gameInstance.sceneGraph.Add(this.gunmesh);
        }

        private Model GetModelForIndex(int i)
        {
            switch (i)
            {
                case 1:
                    return AssetLoader.mdl_shotgun;
                case 2:
                    return AssetLoader.mdl_assault_rifle;
                case 3:
                    return AssetLoader.mdl_sniper_rifle;
                case 4:
                    return AssetLoader.mdl_sword;
                default:
                    return AssetLoader.mdl_pistol;
            }
        }

        public int GetIndex()
        {
            return gunindex;
        }

        public void Deactivate()
        {
            gunindex = -1;
            Globals.gameInstance.sceneGraph.Remove(gunmeshReceipt);
            light.Enabled = false;
        }

        public bool Active()
        {
            return gunindex > -1;
        }
    }
}
