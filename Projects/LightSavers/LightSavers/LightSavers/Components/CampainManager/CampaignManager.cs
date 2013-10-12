using LightSavers.Components.GameObjects;
using LightSavers.Components.GameObjects.Aliens;
using LightSavers.Utils;
using LightSavers.Utils.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.CampainManager
{
    public class CampaignManager
    {
        private int numberOfSections;

        // list of doors
        private List<Door> doors;

        // list of creep spawners
        private AlienSpawner<AlienOne> alienOneSpawner;

        // link to parent game
        public RealGame game;

        public CampaignManager(int numberOfSections)
        {
            this.numberOfSections = numberOfSections;
            this.doors = new List<Door>(numberOfSections);
            this.alienOneSpawner = new AlienSpawner<AlienOne>(10, 15, 2000, 1000);
        }

        public void InitialSpawn()
        {
            this.alienOneSpawner.InitialSpawn();
        }

        public void AddDoor(Door d)
        {
            this.doors.Add(d);
        }

        public void Update(float ms)
        {
            RectangleF territory = Globals.gameInstance.GetPlayerTerritory();

            for (int i = 0; i < this.doors.Count; i++) this.doors[i].Update(ms);

            alienOneSpawner.Update(ms);
        }
    }
}
