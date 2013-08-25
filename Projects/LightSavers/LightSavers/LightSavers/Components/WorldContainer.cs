using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LightSavers.Components.Shader;
using LightSavers.Components.GameObjects;

namespace LightSavers.Components
{
    public class WorldContainer
    {
        private String level;
        private List<GameObject> allObjects;

        private WorldSection[] sections;

        public WorldContainer()
        {
            allObjects = new List<GameObject>();
        }

        public void Load(String level)
        {
            this.level = level;

            DirectoryInfo dir = new DirectoryInfo(Globals.content.RootDirectory+"\\levels\\"+level);

            FileInfo[] files = dir.GetFiles("*.*");

            List<String> filenames = new List<String>();
            for (int f = 0; f < files.Length; f++)
            {
                filenames.Add(Path.GetFileNameWithoutExtension(files[f].Name));
            }
            filenames.Sort();

            sections = new WorldSection[files.Length];

            Vector3 origin = Vector3.Zero;

            for (int i = 0; i < filenames.Count; i++)
            {
                Texture2D t = Globals.content.Load<Texture2D>("levels\\" + level + "\\" + filenames[i]);
                Color[] data = new Color[t.Height * t.Width];                
                t.GetData<Color>(data);

                sections[i] = new WorldSection(data, t.Width, t.Height, origin);

                origin.X += t.Width * WorldSection.TileSize;
            }


            allObjects.Add(new TestLight(0,3, 2, 3, 50, new Vector4(0.0f,1.0f,0.0f,1.0f)));

            allObjects.Add(new TestLight(1, 3, 2, 3, 50, new Vector4(0.0f, 0.0f, 1.0f, 1.0f)));

            allObjects.Add(new TestLight(2, 3, 2, 3, 50, new Vector4(1.0f, 1.0f, 0.0f, 1.0f)));

        }

        public void Draw(float millis, Camera camera, TestShader shader)
        {
            DrawWallsAndFloors(camera, shader);

            // Draw objects here
            foreach(GameObject go in allObjects)
            {
                go.Draw(millis, shader);
            }

        }

        private void DrawWallsAndFloors(Camera camera, TestShader shader)
        {
            // calculate which sections must be drawn
            int camleft = (int)(camera.GetLeftPoint().X / 32) - 1;
            int camright = (int)(camera.GetRightPoint().X / 32) + 1;

            camleft = (int)MathHelper.Clamp(camleft, 0, sections.Length - 1);
            camright = (int)MathHelper.Clamp(camright, 0, sections.Length - 1);

            for (int i = camleft; i <= camright; i++)
            {
                sections[i].Draw(camera, shader);
            } 
        }

        public void Update(float millis)
        {
            foreach (GameObject go in allObjects)
            {
                go.Update(millis);
            }
        }

    }


    

}
