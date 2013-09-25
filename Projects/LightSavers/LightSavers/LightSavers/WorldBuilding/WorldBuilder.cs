using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightSavers.Components;
using Microsoft.Xna.Framework;
using LightPrePassRenderer;
using Microsoft.Xna.Framework.Graphics;

namespace LightSavers.WorldBuilding
{

    public class WorldBuilder
    {
        
        public RealGame game;
        public Vector3 origin;

        public WorldBuilder(RealGame game, int size, Vector3 origin)
        {
            this.game = game;
            this.origin = origin;

            if (size < 2) size = 2;

            Vector3 corigin = origin;

            // add start
            BuildSection(0, corigin);
            corigin += Vector3.Right * 32;

            // add middles
            Random r = new Random();
            for (int i=0;i<size-2;i++)
            {
                int ri = 2 + r.Next(AssetLoader.mdl_section.Length-2);
                BuildSection(ri, corigin);
                corigin += Vector3.Right * 32;
            }            

            // add end
            BuildSection(1, corigin);

        }

        public void BuildSection(int index, Vector3 corigin)
        {
            System.Diagnostics.Debug.WriteLine("Building Section @ " + corigin.ToString());
            Mesh mesh = new Mesh();
            mesh.Model = AssetLoader.mdl_section[index];
            mesh.Transform = Matrix.CreateTranslation(corigin);
            this.game.sceneGraph.AddMesh(mesh);

            System.Diagnostics.Debug.WriteLine("Spawning Entities");
            SpawnEntities(index, corigin);
        }

        public void SpawnEntities(int index, Vector3 corigin)
        {

            Texture2D t = AssetLoader.tex_section_ent[index];
            Color[] colours = new Color[32*32];
            t.GetData<Color>(colours);

            Random ra = new Random();

            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    Color c = colours[y * 32 + x];
                    Vector3 center = corigin + new Vector3(0.5f + x, 0, 0.5f + y);
                    if (c == Color.Yellow)
                    {
                        SpawnOverheadLight(center);
                    }

                    if (c == Color.White)
                    {

                        if (ra.Next(5) == 1)
                        {

                            if (x > 0 && y > 0)
                            {
                                if (x < 31 && y < 31)
                                {
                                    Color u = colours[(y - 1) * 32 + x];
                                    Color d = colours[(y + 1) * 32 + x];
                                    Color l = colours[(y) * 32 + x - 1];
                                    Color r = colours[(y) * 32 + x + 1];
                                    if (u == Color.Black)
                                        SpawnFilingCabinet(center, 180);
                                    else if (d == Color.Black)
                                        SpawnFilingCabinet(center, 0);
                                    else if (l == Color.Black)
                                        SpawnFilingCabinet(center, -90);
                                    else if (r == Color.Black)
                                        SpawnFilingCabinet(center, 90);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SpawnOverheadLight(Vector3 position)
        {
            Mesh m = new Mesh();
            m.Model = AssetLoader.mdl_ceilinglight;
            m.Transform = Matrix.CreateTranslation(position + Vector3.Up * 4);
            game.sceneGraph.AddMesh(m);

            Light l = new Light();
            l.LightType = Light.Type.Point;
            l.Radius = 7;
            l.Intensity = 0.5f;

            
            l.Transform = Matrix.CreateTranslation(position + Vector3.Up * 4);
            game.sceneGraph.AddLight(l);
        }

        public void SpawnFilingCabinet(Vector3 position, int angle_d)
        {
            Mesh m = new Mesh();
            m.Model = AssetLoader.mdl_filingcabinet;
            m.SetInstancingEnabled(true);
            m.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(angle_d)) * Matrix.CreateTranslation(position);
            game.sceneGraph.AddMesh(m);
        }
        


    }
}
