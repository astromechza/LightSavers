using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LightSavers.Components.Shader;

namespace LightSavers.Components
{
    public class WorldContainer
    {
        private List<GameObject> allObjects;
        private WorldSection[] sections;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level">
        /// A subdirectory in the Content/levels folder containing level .bmp's
        /// </param>
        public WorldContainer(String level)
        {
            allObjects = new List<GameObject>();
            Load(level);
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
            DirectoryInfo dir = new DirectoryInfo(Globals.content.RootDirectory+"\\levels\\"+level);
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
                sections[i] = new WorldSection("levels\\" + level + "\\" + filenames[i], origin);

                // Increment origin
                origin += Vector3.Right * 32;
            }

        }

        /// <summary>
        /// Draw the current world geometry and all of the objects in it
        /// </summary>
        /// <param name="camera"> Camera object, used for view and projection matrices </param>
        /// <param name="shader"> Shader object, used to set specific lighting and texturing values </param>
        public void Draw(Camera camera, TestShader shader)
        {
            DrawWallsAndFloors(camera, shader);

            // Draw objects here
            foreach(GameObject go in allObjects)
            {
                go.Draw();
            }

        }

        /// <summary>
        /// A simple optimisation is to just draw the Sections which are in 
        /// view of the camera at the current moment.
        /// </summary>
        /// <param name="camera"> Camera object, used for view and projection matrices </param>
        /// <param name="shader"> Shader object, used to set specific lighting and texturing values </param>
        private void DrawWallsAndFloors(Camera camera, TestShader shader)
        {
            // calculate which sections must be drawn based on the left and right edges of the camera
            int camleft = (int)(camera.GetLeftPoint().X / 32) - 1;
            int camright = (int)(camera.GetRightPoint().X / 32) + 1;

            // avoid index out of range issues
            camleft = (int)MathHelper.Clamp(camleft, 0, sections.Length - 1);
            camright = (int)MathHelper.Clamp(camright, 0, sections.Length - 1);

            // Draw the suckers
            for (int i = camleft; i <= camright; i++)
            {
                sections[i].Draw(camera, shader);
            } 
        }

        /// <summary>
        /// Update the World in general and all of the objects in it
        /// </summary>
        /// <param name="ms">Elapsed milliseconds since last update call</param>
        public void Update(float ms)
        {
            foreach (GameObject go in allObjects)
            {
                go.Update(ms);
            }
        }
    }


    

}
