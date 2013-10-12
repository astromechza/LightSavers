using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightSavers.Components;
using Microsoft.Xna.Framework;
using LightPrePassRenderer;
using Microsoft.Xna.Framework.Graphics;
using LightSavers.Utils;
using LightSavers.Components.GameObjects;

namespace LightSavers.WorldBuilding
{

    public class WorldBuilder
    {
        // extra colours
        private Color PureGreen = new Color(0, 255, 0);
        
        public Vector3 origin;

        public WorldBuilder(int size, Vector3 origin)
        {
            this.origin = origin;

            if (size < 2) size = 2;

            Vector3 corigin = origin;

            // add start
            BuildSection(0, corigin);
            corigin += Vector3.Right * 32;

            // add middles
            for (int i=0;i<size-2;i++)
            {
                int ri = 2 + Globals.random.Next(AssetLoader.mdl_section.Length-2);
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
            mesh.Transform = Matrix.CreateTranslation(corigin + new Vector3(16, 0, 16));
            Globals.gameInstance.sceneGraph.Setup(mesh);
            Globals.gameInstance.sceneGraph.Add(mesh);

            System.Diagnostics.Debug.WriteLine("Spawning Entities");
            SpawnEntities(index, corigin);
        }

        public void SpawnEntities(int index, Vector3 corigin)
        {

            Texture2D t = AssetLoader.tex_section_ent[index];
            Color[] colours = new Color[96*96];
            t.GetData<Color>(colours);

            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    int pixelX = x * 3;
                    int pixelY = y * 3;
                    int pixelIndex = pixelY * 96 + pixelX;


                    Color c = colours[pixelIndex];
                    Vector3 center = corigin + new Vector3(0.5f + x, 0, 0.5f + y);


                    if (c == Color.Black || c == Color.Blue)
                    {
                        Globals.gameInstance.cellCollider.SetCollision(center.X, center.Z, true);
                    }
                    else if (c == Color.Yellow)
                    {
                        SpawnOverheadLight(center);
                    }
                    else if (c == Color.Red)
                    {
                        SpawnFilingCabinet(center, colours, pixelX, pixelY);
                    }
                    else if (c == PureGreen)
                    {
                        int pi2 = (pixelY+1) * 96 + pixelX+1;

                        Color cc = colours[pi2];
                        if (cc == PureGreen)
                        {
                            Globals.gameInstance.campaignManager.AddDoor(new Door(center));
                        }
                    }
                    else if (c == Color.DarkMagenta)
                    {
                        int pi2 = (pixelY + 2) * 96 + pixelX + 1;

                        if (colours[pi2] != Color.DarkMagenta)
                        {
                            Mesh m = new Mesh();
                            m.Model = AssetLoader.mdl_desk;
                            m.SetInstancingEnabled(true);
                            m.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(0)) * Matrix.CreateTranslation(center + new Vector3(0,0,0.5f));

                            Globals.gameInstance.cellCollider.SetCollision(center.X, center.Z, true);
                            Globals.gameInstance.cellCollider.SetCollision(center.X, center.Z + 1, true);

                            Globals.gameInstance.sceneGraph.Setup(m);
                            Globals.gameInstance.sceneGraph.Add(m);
                        }

                        int pi3 = (pixelY + 1) * 96 + pixelX + 2;

                        if (colours[pi3] != Color.DarkMagenta)
                        {
                            Mesh m = new Mesh();
                            m.Model = AssetLoader.mdl_desk;
                            m.SetInstancingEnabled(true);
                            m.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(center + new Vector3(0.5f, 0, 0));

                            Globals.gameInstance.cellCollider.SetCollision(center.X, center.Z, true);
                            Globals.gameInstance.cellCollider.SetCollision(center.X, center.Z + 1, true);

                            Globals.gameInstance.sceneGraph.Setup(m);
                            Globals.gameInstance.sceneGraph.Add(m);
                        }

                    }
                    else if (c == Color.Turquoise)
                    {
                        float a = GetAngleToAWall(colours, pixelX, pixelY);
                        Mesh m = new Mesh();
                        m.Model = AssetLoader.mdl_pipe;
                        m.SetInstancingEnabled(true);
                        m.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(a)) * Matrix.CreateTranslation(center);

                        Globals.gameInstance.cellCollider.SetCollision(center.X, center.Z, true);

                        Globals.gameInstance.sceneGraph.Setup(m);
                        Globals.gameInstance.sceneGraph.Add(m);

                    }

                }
            }
        }

        private float GetAngleToAWall(Color[] data, int x, int y)
        {
            Color u = data[(y - 3) * 96 + x];
            Color d = data[(y + 3) * 96 + x];
            Color l = data[y * 96 + x - 3];
            Color r = data[y * 96 + x + 3];
            
            if (u == Color.Blue || u == Color.Black)
            {
                return 0;
            }
            else if (d == Color.Blue || d == Color.Black)
            {
                return 180;
            }
            else if (l == Color.Blue || l == Color.Black)
            {
                return 90;
            }
            else if (r == Color.Blue || r == Color.Black)
            {
                return -90;
            }
            return 0;
        }

        public void SpawnOverheadLight(Vector3 position)
        {
            Mesh m = new Mesh();
            m.Model = AssetLoader.mdl_ceilinglight;
            m.Transform = Matrix.CreateTranslation(position + Vector3.Up * 4);
            Globals.gameInstance.sceneGraph.Add(m);

            Light l = new Light();
            l.LightType = Light.Type.Point;
            l.Radius = 7;
            l.Intensity = 0.4f;

            
            l.Transform = Matrix.CreateTranslation(position + Vector3.Up * 4);
            Globals.gameInstance.sceneGraph.Add(l);
        }

        public void SpawnFilingCabinet(Vector3 center, Color[] data, int x, int y)
        {
            Globals.gameInstance.cellCollider.SetCollision(center.X, center.Z, true);

            Color u = data[y * 96 + x+1];
            Color d = data[(y + 2) * 96 + x+1];
            Color l = data[(y+1) * 96 + x];
            Color r = data[(y+1) * 96 + x + 2];

            float angle_d = 0;

            if (u != Color.Red)
            {
                angle_d = 0;
            }
            else if (d != Color.Red)
            {
                angle_d = 180;
            }
            else if (l != Color.Red)
            {
                angle_d = 90;
            }
            else if (r != Color.Red)
            {
                angle_d = -90;
            }

            Mesh m = new Mesh();
            m.Model = AssetLoader.mdl_filingcabinet;
            m.SetInstancingEnabled(true);
            m.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(angle_d)) * Matrix.CreateTranslation(center);
            Globals.gameInstance.sceneGraph.Setup(m);
            Globals.gameInstance.sceneGraph.Add(m);
        }
        


    }
}
