using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LightSavers.Components.GameObjects;
using LightPrePassRenderer;

namespace LightSavers.Components
{
    public class WorldContainer
    {
        private List<GameObject> allObjects;
        private WorldSection[] sections;

        private List<Light> visibleLights;
        private List<Mesh> visibleMeshes;

        private PlayerObject[] players;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level">
        /// A subdirectory in the Content/levels folder containing level .bmp's
        /// </param>
        public WorldContainer(String level)
        {
            allObjects = new List<GameObject>();

            visibleMeshes = new List<Mesh>();
            Load(level);

            foreach (WorldSection s in sections) visibleMeshes.Add(s.Mesh);

            visibleLights = new List<Light>();

            SetupLighting();

            players = new PlayerObject[2];
            players[0] = new PlayerObject(PlayerIndex.One, new Color(0.5f, 1.0f, 0.5f), new Vector3(4, 0, 4), (float)Math.PI * 1.2f);
            players[1] = new PlayerObject(PlayerIndex.Two, new Color(0.5f, 0.6f, 1.0f), new Vector3(4, 0, 10), (float)Math.PI * 1.8f);
            
            visibleLights.AddRange(players[0].GetLights());
            visibleLights.AddRange(players[1].GetLights());

            visibleMeshes.Add(players[0].GetMesh());
            visibleMeshes.Add(players[1].GetMesh());
        }


        /// <summary>
        /// Loads all of the sections of a level in alphabetical order into the array of WorldSections.
        /// Each image is analysed in the constructor of the WorldSection
        /// </summary>
        /// <param name="level">
        /// A subdirectory in the Content/levels folder containing level .bmp's
        /// </param>
        public void Load(String level)
        {
            // Get directory
            DirectoryInfo dir = new DirectoryInfo(Globals.content.RootDirectory+"/levels/"+level);
            //get list of files from directory
            FileInfo[] files = dir.GetFiles("*.*");

            // Convert FileInfo into asset filenames
            List<String> filenames = new List<String>();
            for (int f = 0; f < files.Length; f++)
            {
                filenames.Add(Path.GetFileNameWithoutExtension(files[f].Name));
            }
            // Must be in alphabetical order
            filenames.Sort();

            // Initialise the WorldSection array
            sections = new WorldSection[files.Length];

            // Topleft of section 1 is (0,0,0)
            Vector3 origin = Vector3.Zero;            
            for (int i = 0; i < filenames.Count; i++)
            {
                
                // Construct section using colour array
                sections[i] = new WorldSection("levels/" + level + "/" + filenames[i], origin);

                // Increment origin
                origin += Vector3.Right * 32;
            }

        }
        
        public List<Light> GetVisibleLights()
        {
            return visibleLights;
        }

        public List<Mesh> GetVisibleMeshes()
        {
            return visibleMeshes;
        }




        /// <summary>
        /// Update the World in general and all of the objects in it
        /// </summary>
        /// <param name="ms">Elapsed milliseconds since last update call</param>
        public void Update(float ms)
        {

            visibleLights[1].Transform = Matrix.CreateRotationY(0.03f) * visibleLights[1].Transform;

            players[0].Update(ms);

            foreach (GameObject go in allObjects)
            {
                go.Update(ms);
            }
        }

        public void SetupLighting()
        {
            /*
            // testing light performance
            Random r = new Random();
            for (int i = 0; i < 33; i++)
            {
                Light l = new Light();
                l.LightType = Light.Type.Point;
                l.Radius = 10f;
                l.Intensity = 0.4f;
                l.Color = new Color(1.0f, 1.0f, 1.0f);
                l.CastShadows = true;
                int x = r.Next(100);
                int y = r.Next(32);
                l.Transform = Matrix.CreateTranslation(new Vector3(x, 1f, y));
                visibleLights.Add(l);
            }*/

        }
    }


    

}
