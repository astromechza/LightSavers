﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightSavers.Components
{
    public class WorldContainer
    {
        private String level;

        private WorldSection[] sections;

        public WorldContainer()
        {

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

                origin += Vector3.Right * t.Width * WorldSection.TileSize;
            }
        }

        public void Draw(Camera camera, BasicEffect quadEffect)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                sections[i].Draw(camera, quadEffect);
            }           
        }

    }


    

}
