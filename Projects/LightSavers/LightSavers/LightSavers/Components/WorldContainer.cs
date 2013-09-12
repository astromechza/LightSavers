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
        public WorldContainer(String level, int levelSize)
        {
            allObjects = new List<GameObject>();

            visibleMeshes = new List<Mesh>();
            Load(level,10);

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

            Random r = new Random();

            for (int i = 0; i < 200; i++)
            {
                int x = r.Next(64) + 16;
                int z = r.Next(20) + 6;
                float y = (float)(r.NextDouble() * 1.0f + 0.2f);

                Mesh m = new Mesh();
                m.Model = AssetLoader.mdl_sphere;
                m.Transform = Matrix.CreateTranslation(x, y, z);
                visibleMeshes.Add(m);
            }

        }


        /// <summary>
        /// Loads all of the sections of a level in alphabetical order into the array of WorldSections.
        /// Each image is analysed in the constructor of the WorldSection
        /// </summary>
        /// <param name="level">
        /// A subdirectory in the Content/levels folder containing level .bmp's
        /// </param>
        public void Load(String level, int levelSize)
        {
            // Get directory
            DirectoryInfo dir = new DirectoryInfo(Globals.content.RootDirectory+"/levels/"+level);
            //get list of files from directory
            FileInfo[] files = dir.GetFiles("*.xnb");

            // Convert FileInfo into asset filenames
            List<String> filenames = new List<String>();
            for (int f = 0; f < files.Length; f++)
            {
                filenames.Add(Path.GetFileNameWithoutExtension(files[f].Name));
            }
            // Must be in alphabetical order
            filenames.Sort();

            //if levelSize is bigger than number of levels, lock to num of levels
            if (levelSize > filenames.Count())
            {
                levelSize = filenames.Count();
            }

            // Initialise the WorldSection array
            sections = new WorldSection[levelSize];
            
            // Topleft of section 1 is (0,0,0)
            Vector3 origin = Vector3.Zero;

            //first and last tiles and remove them
            sections[0] = new WorldSection("levels/" + level + "/" + filenames[0], origin);
            filenames.RemoveAt(0);
            origin += Vector3.Right * 32;

            sections[levelSize - 1] = new WorldSection("levels/" + level + "/" + filenames[filenames.Count() - 1], Vector3.Right * (levelSize- 1) * 32);
            filenames.RemoveAt(filenames.Count() - 1);


            //random integer instance
            Random rnd = new Random();
            int current_section = 1;

            //looping thru sizes
            while (filenames.Count() > 0 && current_section < levelSize-1)
            {
                int index = rnd.Next(0, filenames.Count());
                sections[current_section] = new WorldSection("levels/" + level + "/" + filenames[index], origin);
                filenames.RemoveAt(index);
             
             // Increment origin
                origin += Vector3.Right * 32;
             
                current_section++;
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

        }
    }


    

}
