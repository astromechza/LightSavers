using LightPrePassRenderer;
using LightSavers.Components.GameObjects;
using LightSavers.Components.GameObjects.Aliens;
using LightSavers.Utils.Geometry;
using Microsoft.Xna.Framework;
using System;
using LightSavers.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.CampainManager
{
    public class CampaignSection
    {
        private int _index;
        private Vector3 _origin;
        private Door _door;
        private List<Light> _overheadLights;
        private List<BaseAlien> _population;

        #region

        public int Index { get { return _index; } }

        #endregion

        public CampaignSection(int i, Vector3 o)
        {
            _index = i;
            _origin = o;
            _door = null;
            _overheadLights = new List<Light>();
            _population = new List<BaseAlien>();
        }

        public bool HasDoor()
        {
            return _door != null;
        }

        public void AddOverheadLight(Light o)
        {
            _overheadLights.Add(o);
        }

        public void SetDoor(Door door)
        {
            _door = door;
        }

        public void UpdateDoor(float ms)
        {
            _door.Update(ms);
        }

        public void FillWithAliens()
        {
            _population.Clear();

            AlienSpawner2<AlienOne>.Spawn(7, this);
            AlienSpawner2<AlienTwo>.Spawn(4, this);
        }

        public void AddAlienToPopulation(BaseAlien ba)
        {
            _population.Add(ba);
        }

        public void UpdateAliens(float ms)
        {
            for (int p = 0; p < _population.Count; p++)
            {
                _population[p].Update(ms);
                if (_population[p]._mustBeDeleted)
                {
                    _population.RemoveAt(p);
                    p--;
                }
            }
        }

        public int GetAlienCount()
        {
            return _population.Count;
        }

        public void Open()
        {
            Globals.audioManager.PlayGameSound("start_game");

            if (_door != null) _door.Open();
            for (int i = 0; i < _overheadLights.Count; i++) _overheadLights[i].Enabled = true;
        }

        public bool RectangleCollidesDoor(Utils.Geometry.RectangleF collisionRectangle)
        {
            if (_door != null)
            {
                if (Collider.Collide(collisionRectangle, _door.GetDoorLBB())) return true;
                if (Collider.Collide(collisionRectangle, _door.GetDoorRBB())) return true;
            }
            return false;
        }
    }
}
