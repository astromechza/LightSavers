using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Components.GameObjects;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LightSavers.Components
{
    public class RealGame
    {
        private PlayerObject[] players;

        public RealGame(int numberOfSections, LightAndMeshContainer lightAndMeshContainer)
        {
            #region LOAD LEVEL
            Vector3 origin = Vector3.Zero;

            Mesh mesh = new Mesh();
            mesh.Model = Globals.content.Load<Model>("levels/start");
            mesh.Transform = Matrix.CreateTranslation(origin);
            lightAndMeshContainer.AddMesh(mesh);

            origin += Vector3.Right * 32;

            DirectoryInfo dir = new DirectoryInfo(Globals.content.RootDirectory + "/levels/parts");
            FileInfo[] files = dir.GetFiles("*.xnb");

            // Convert FileInfo into asset filenames
            List<String> filenames = new List<String>();
            for (int f = 0; f < files.Length; f++)
            {
                filenames.Add(Path.GetFileNameWithoutExtension(files[f].Name));
            }

            Random r = new Random();
            for (int i = 0; i < numberOfSections - 2; i++)
            {
                string rfile = filenames[r.Next(filenames.Count)];
                mesh = new Mesh();
                mesh.Model = Globals.content.Load<Model>("levels/parts/" + rfile);
                mesh.Transform = Matrix.CreateTranslation(origin);
                lightAndMeshContainer.AddMesh(mesh);

                origin += Vector3.Right * 32;
            }

            mesh = new Mesh();
            mesh.Model = Globals.content.Load<Model>("levels/end");
            mesh.Transform = Matrix.CreateTranslation(origin);
            lightAndMeshContainer.AddMesh(mesh);
            #endregion

            #region LOAD PLAYERS
            players = new PlayerObject[2];
            players[0] = new PlayerObject(PlayerIndex.One, new Color(0.5f, 1.0f, 0.5f), new Vector3(4, 0, 4), (float)Math.PI * 1.2f);
            players[1] = new PlayerObject(PlayerIndex.Two, new Color(0.5f, 0.6f, 1.0f), new Vector3(4, 0, 10), (float)Math.PI * 1.8f);

            lightAndMeshContainer.AddMesh(players[0].GetMesh());
            lightAndMeshContainer.AddMesh(players[1].GetMesh());

            foreach (Light l in players[0].GetLights()) lightAndMeshContainer.AddLight(l);
            foreach (Light l in players[1].GetLights()) lightAndMeshContainer.AddLight(l);            
            #endregion

            #region LOAD ARB TEST OBJECTS

            for (int i = 0; i < 500; i++)
            {
                int x = r.Next(64) + 6;
                int z = r.Next(20) + 6;
                float y = (float)(r.NextDouble() * 1.0f + 0.2f);

                Mesh m = new Mesh();
                m.Model = AssetLoader.mdl_sphere;
                m.Transform = Matrix.CreateTranslation(x, y, z);
                lightAndMeshContainer.AddMesh(m);
            }

            #endregion

        }

        public void Update(float ms)
        {
            players[0].Update(ms);
        }
    }
}
