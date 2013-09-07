using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LightSavers.Components.GameObjects;

namespace LightSavers.Components
{
    public class WorldContainer
    {
        private List<GameObject> allObjects;
        private WorldSection[] sections;

        private Light light1;
        private Light light2;
        private Light light3;
        private Light light4;

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

            light1 = new Light();
            light1.LightType = Light.Type.Point;
            light1.Radius = 4f;
            light1.Intensity = 0.7f;
            light1.Color = new Color(1.0f, 0.5f, 0.5f);
            light1.Transform = Matrix.CreateTranslation(new Vector3(4, 1f, 4));

            light2 = new Light();
            light2.LightType = Light.Type.Spot;
            light2.ShadowDepthBias = 0.001f;
            light2.Radius = 40;
            light2.SpotAngle = 20;
            light2.Intensity = 1.5f;
            light2.Color = new Color(1.0f, 0.5f, 0.5f);
            light2.CastShadows = true;
            light2.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(270)) * Matrix.CreateTranslation(new Vector3(4, 1f, 4));

            light3 = new Light();
            light3.LightType = Light.Type.Point;
            light3.Radius = 10f;
            light3.Intensity = 0.7f;
            light3.Color = new Color(1.0f, 1.0f, 1.0f);
            light3.Transform = Matrix.CreateTranslation(new Vector3(4, 1f, 20));

            light4 = new Light();
            light4.LightType = Light.Type.Spot;
            light4.ShadowDepthBias = 0.001f;
            light4.Radius = 40;
            light4.SpotAngle = 20;
            light4.Intensity = 1.5f;
            light4.Color = new Color(0.5f, 0.5f, 1.0f);
            light4.CastShadows = true;
            light4.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(-45)) * Matrix.CreateTranslation(new Vector3(4, 1f, 20));
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
            List<Light> lights = new List<Light>();
            lights.Add(light1);
            lights.Add(light2);
            lights.Add(light3);
            lights.Add(light4);
            return lights;
        }

        public List<MeshWrapper> GetVisibleMeshes()
        {
            List<MeshWrapper> meshes = new List<MeshWrapper>();
            foreach (WorldSection ws in sections)
            {
                meshes.Add(ws.Mesh);
            }
            return meshes;
        }




        /// <summary>
        /// Update the World in general and all of the objects in it
        /// </summary>
        /// <param name="ms">Elapsed milliseconds since last update call</param>
        public void Update(float ms)
        {

            light2.Transform = Matrix.CreateRotationY(0.03f) * light2.Transform;



            foreach (GameObject go in allObjects)
            {
                go.Update(ms);
            }
        }
    }


    

}
