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

        public RealGame(int numberOfSections, int numPlayers, AwesomeSceneGraph sg)
        {
            this.sceneGraph = sg;

            cellCollider = new CellCollider(32, numberOfSections * 32);

            worldBuilder = new WorldBuilder(this, numberOfSections, Vector3.Zero);
            
            players = new PlayerObject[numPlayers];

            Color[] playerColours = new Color[] {
                new Color(0.5f, 1.0f, 0.5f),
                new Color(0.5f, 0.6f, 1.0f)
            };

            Vector3[] spawns = new Vector3[] {
                new Vector3(4, 0, 4),
                new Vector3(4, 0, 10)
            };

            for (int i = 0; i < numPlayers; i++)
            {
                players[i] = new PlayerObject(this, (i==0) ? PlayerIndex.One : PlayerIndex.Two, playerColours[i], spawns[i], 0);
                players[i].AddToSG(sg);
            }

        }

        public void Update(float ms)
        {
            foreach (PlayerObject p in players) p.Update(ms);
        }

        public List<Vector2> GetCriticalPoints()
        {
            List<Vector2> o = new List<Vector2>(10);
            foreach (PlayerObject p in players) p.AddCriticalPoints(o);
            return o;
        }

    }
}
