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

        // link to parent game
        public RealGame game;

        public List<CampaignSection> sections;
        private int currentSection;

        #region
        public int CurrentSection { get { return currentSection; } }
        #endregion

        public CampaignManager(int numberOfSections)
        {
            this.numberOfSections = numberOfSections;
            this.sections = new List<CampaignSection>(numberOfSections);
            this.currentSection = 0;
        }

        public void Update(float ms)
        {
            UpdateAliens(ms);

            if (sections[currentSection].GetAlienCount() == 0)
            {
                sections[currentSection].Open();
                currentSection += 1;
                sections[currentSection].FillWithAliens();
            }

            UpdateDoors(ms);

        }

        // Update all the aliens
        public void UpdateAliens(float ms)
        {
            for (int i = 0; i < sections.Count; i++)
            {
                sections[i].UpdateAliens(ms);
            }
        }

        // Update all the doors
        public void UpdateDoors(float ms)
        {
            for (int i = 0; i < sections.Count; i++)
            {
                if (sections[i].HasDoor()) sections[i].UpdateDoor(ms);
            }
        }

        public void AddSection(CampaignSection csection)
        {
            sections.Add(csection);
        }

        public void SpawnAliensInSection(int index)
        {
            sections[index].FillWithAliens();
        }

        public bool ProjectileCollidesDoor(Projectiles.BaseBullet b)
        {

            // top door
            if (b.position.X > currentSection * 32 + 31f) return true;
            return false;
        }
    }
}
