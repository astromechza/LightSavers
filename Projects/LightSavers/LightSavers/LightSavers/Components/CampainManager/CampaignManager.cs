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
using LightSavers.ScreenManagement.Layers;

namespace LightSavers.Components.CampainManager
{
    public class CampaignManager
    {
        private int numberOfSections;

        public List<CampaignSection> sections;
        private int currentSection;

        public Teleporter endteleporter;

        #region
        public int CurrentSection { get { return currentSection; } }
        #endregion

        public CampaignManager(int numberOfSections)
        {
            this.numberOfSections = numberOfSections;
            this.sections = new List<CampaignSection>(numberOfSections);
            this.currentSection = 0;
            this.endteleporter = null;
        }

        public void Update(float ms)
        {
            UpdateAliens(ms);

            UpdateWeaponDepots(ms);
            if (currentSection < sections.Count - 1)
            {
                if (sections[currentSection].GetAlienCount() == 0)
                {
                    sections[currentSection].Open();
                    currentSection += 1;
                    sections[currentSection].FillWithAliens();
                }
            }
            else
            {
                // check if players are both on the portal
                bool allOnPortal = true;
                for (int i = 0; i < Globals.gameInstance.players.Length; i++)
                {
                    if (Vector3.DistanceSquared(endteleporter.Position, Globals.gameInstance.players[i].Position) > 1.5f)
                    {
                        allOnPortal = false;
                        break;
                    }
                }

                if (allOnPortal && !endteleporter.Started && !endteleporter.Finished)
                {
                    endteleporter.Start();
                }
                endteleporter.Update(ms);

                if (endteleporter.Progress > 30)
                {
                    // win
                    //Globals.gameInstance.parentLayer.
                    GameLayer game = (GameLayer)Globals.screenManager.GetTop();
                    game.fadeOutCompleteCallback = win;
                    game.StartTransitionOff();
                }

            }

            UpdateDoors(ms);

        }

        public bool win()
        {
            Globals.screenManager.Push(new EndScreen("won", true));
            return true;
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

        public void UpdateWeaponDepots(float ms)
        {
            for (int i = 0; i < sections.Count; i++)
            {
                sections[i].UpdateWeaponDepot(ms);
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

        public bool CollideCurrentEntities(BaseAlien alienOne)
        {
            return sections[currentSection].CollideAliens(alienOne);
        }

        public bool CollideCurrentEntities(PlayerObject playerObject)
        {
            return sections[currentSection].CollideAliens(playerObject);
        }

        public string GetCurrentTitle()
        {
            return sections[currentSection].GetCurrentTitle();
        }

        public float GetCurrentProgress()
        {
            return (float)(currentSection-1) / sections.Count;
        }

        public WeaponDepot GetNearestActiveDepot(Vector3 _position)
        {
            int ind = (int)_position.X / 32;

            return sections[ind].GetNearestActiveDepot(_position);
        }

        public void SetTeleport(Teleporter teleporter)
        {
            endteleporter = teleporter;
        }
    }
}
