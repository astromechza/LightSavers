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

        public SimpleLightAndMeshContainer lightAndMeshContainer;

        public RealGame(int numberOfSections, SimpleLightAndMeshContainer lightAndMeshContainer)
        {
            this.lightAndMeshContainer = lightAndMeshContainer;

            #region LOAD LEVEL

            WorldBuilder wb = new WorldBuilder(this, numberOfSections, Vector3.Zero);            
           
            #endregion

            #region LOAD PLAYERS
            players = new PlayerObject[2];
            players[0] = new PlayerObject(PlayerIndex.One, new Color(0.5f, 1.0f, 0.5f), new Vector3(4, 0, 4), (float)Math.PI * 1.2f);
            players[1] = new PlayerObject(PlayerIndex.Two, new Color(0.5f, 0.6f, 1.0f), new Vector3(4, 0, 10), (float)Math.PI * 1.8f);

            lightAndMeshContainer.AddMesh(players[0].GetMesh());
            lightAndMeshContainer.AddMesh(players[1].GetMesh());

            foreach (Light l in players[0].GetLights()) lightAndMeshContainer.AddLight(l);
            foreach (Light l in players[1].GetLights()) lightAndMeshContainer.AddLight(l);            
            #endregion
           

        }

        public void Update(float ms)
        {
            players[0].Update(ms);
        }
    }
}
