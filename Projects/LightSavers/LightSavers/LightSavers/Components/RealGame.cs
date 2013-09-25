using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
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

        WorldBuilder worldBuilder;

        public RealGame(int numberOfSections, AwesomeSceneGraph sg)
        {
            this.sceneGraph = sg;


            worldBuilder = new WorldBuilder(this, numberOfSections, Vector3.Zero);            
           
            players = new PlayerObject[2];
            players[0] = new PlayerObject(PlayerIndex.One, new Color(0.5f, 1.0f, 0.5f), new Vector3(4, 0, 4), (float)Math.PI * 1.2f);
            players[1] = new PlayerObject(PlayerIndex.Two, new Color(0.5f, 0.6f, 1.0f), new Vector3(4, 0, 10), (float)Math.PI * 1.8f);

            players[0].AddToSG(sg);
            players[1].AddToSG(sg);
           
           

        }

        public void Update(float ms)
        {
            players[0].Update(ms);
        }

    }
}
