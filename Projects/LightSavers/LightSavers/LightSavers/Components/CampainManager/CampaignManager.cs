using LightSavers.Components.GameObjects;
using LightSavers.Components.GameObjects.Aliens;
using LightSavers.Utils;
using LightSavers.Utils.Geometry;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
            Thread t = new Thread(new ThreadStart(sections[index].FillWithAliens));
            t.Start();
        }

        public bool ProjectileCollidesDoor(Projectiles.BaseBullet b)
        {

            // top door
            if (b.position.X > currentSection * 32 + 31f) return true;
            return false;
        }

        // when checking whether a rectangle collides with a door
        // there are only 2 doors at most: the currently opening one, 
        // and the closed one at the end of the next section
        public bool RectangleCollidesDoor(RectangleF collisionRectangle)
        {
            int ni = (int)(collisionRectangle.Right + collisionRectangle.Left)/2;

            int nearestIndex = (ni - 16) / 32;
            nearestIndex = (int)MathHelper.Clamp(nearestIndex, 0, sections.Count - 1);
            return sections[nearestIndex].RectangleCollidesDoor(collisionRectangle);

        }
    }
}
