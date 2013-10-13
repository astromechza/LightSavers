using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Collisions;
using LightSavers.Components.CampainManager;
using LightSavers.Components.GameObjects;
using LightSavers.Components.GameObjects.Aliens;
using LightSavers.Components.HitParticle;
using LightSavers.Components.Projectiles;
using LightSavers.Utils;
using LightSavers.Utils.Geometry;
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
    /// <summary>
    /// RealGame is the class controlling the actual game, its entities and inter-entity operations
    /// </summary>
    public class RealGame
    {
        public PlayerObject[] players;
        public ProjectileManager projectileManager = new ProjectileManager();
        public DropFragmentManager fragmentManager = new DropFragmentManager();
        public BlockBasedSceneGraph sceneGraph;
        public WorldBuilder worldBuilder;
        public CellCollider cellCollider;

        public CampaignManager campaignManager;

        private static Color[] playerColours = new Color[] {
                new Color(0.5f, 1.0f, 0.5f),
                new Color(0.5f, 0.6f, 1.0f)
        };

        private static Vector3[] spawns = new Vector3[] {
                new Vector3(22, 0, 14),
                new Vector3(22, 0, 20)
        };

        private float MaxProgess = 0;

        private List<Vector2> criticalPoints = new List<Vector2>(8);

        public RealGame(int numberOfSections, int numPlayers, BlockBasedSceneGraph sceneGraph)
        {
            // setup
            Globals.gameInstance = this;
            this.sceneGraph = sceneGraph;

            // object to store collisions (the size of the world)
            cellCollider = new CellCollider(32, numberOfSections * 32);

            // holds links between current spawn populations, doors and lights
            campaignManager = new CampaignManager(numberOfSections);

            // now build all the shit
            WorldBuilder.Build(numberOfSections, Vector3.Zero);
            
            // now spawn all player
            players = new PlayerObject[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {                
                players[i] = new PlayerObject((i==0) ? PlayerIndex.One : PlayerIndex.Two, playerColours[i], spawns[i], MathHelper.ToRadians(-90));
            }

            Globals.audioManager.SwitchToGame();
            //Globals.audioManager.PlayGameSound("start_game");
            Globals.audioManager.PlayGameSound("music");

        }

        public void Update(float ms)
        {
            for (int i = 0; i < players.Length; i++) players[i].Update(ms);
            projectileManager.Update(ms);
            
            campaignManager.Update(ms);

            fragmentManager.Update(ms);
        }

        public List<Vector2> GetCriticalPoints()
        {
            criticalPoints.Clear();
            for (int i = 0; i < players.Length; i++)
            {
                players[i].AddCriticalPoints(criticalPoints);
                MaxProgess = Math.Max(MaxProgess, players[i].Position.X);
            }
            return criticalPoints;
        }


        public PlayerObject GetClosestPlayer(Vector3 position)
        {
            float d = Vector3.DistanceSquared(position, players[0].Position);
            if (players.Length > 1)
            {
                float d2 = Vector3.DistanceSquared(position, players[1].Position);
                if (d2 < d) return players[1];
            }
            return players[0];
        }

        public RectangleF GetPlayerTerritory()
        {
            float left = players[0].Position.X;
            float right = players[0].Position.X;
            float top = players[0].Position.Y;
            float bottom = players[0].Position.Y;

            if (players.Length > 1)
            {
                left = Math.Min(left, players[1].Position.X);
                right = Math.Max(right, players[1].Position.X);
                top = Math.Min(top, players[1].Position.Y);
                bottom = Math.Max(bottom, players[1].Position.Y);
            }

            return new RectangleF(left, right, right - left, bottom - left);
        }

        public bool CollidesPlayers(AlienOne alienOne)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (Collider.Collide(alienOne._collisionRectangle, players[i].collisionRectangle)) return true;
            }

            return false;
        }

        public float GetMaxProgess()
        {
            return MaxProgess;
        }
    }
}
