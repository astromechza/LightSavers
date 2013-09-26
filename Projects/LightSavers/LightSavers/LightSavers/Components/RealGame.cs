using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Collisions;
using LightSavers.Components.GameObjects;
using LightSavers.Utils;
using LightSavers.WorldBuilding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LightSavers.Components
{
    public class RealGame
    {
        private PlayerObject[] players;

        public AwesomeSceneGraph sceneGraph;

        public WorldBuilder worldBuilder;

        public CellCollider cellCollider;

        public RealGame(int numberOfSections, AwesomeSceneGraph sg)
        {
            this.sceneGraph = sg;

            cellCollider = new CellCollider(32, numberOfSections * 32);

            worldBuilder = new WorldBuilder(this, numberOfSections, Vector3.Zero);


            players = new PlayerObject[2];
            players[0] = new PlayerObject(this, PlayerIndex.One, new Color(0.5f, 1.0f, 0.5f), new Vector3(4, 0, 4), (float)Math.PI * 1.2f);
            players[1] = new PlayerObject(this, PlayerIndex.Two, new Color(0.5f, 0.6f, 1.0f), new Vector3(4, 0, 10), (float)Math.PI * 1.8f);

            players[0].AddToSG(sg);
            players[1].AddToSG(sg);
           
           

        }

        public void Update(float ms)
        {
            players[0].Update(ms);


            

        }

        public List<Vector2> GetCriticalPoints()
        {
            List<Vector2> o = new List<Vector2>(10);
            players[0].AddCriticalPoints(o);
            players[1].AddCriticalPoints(o);
            return o;
        }

    }
}
