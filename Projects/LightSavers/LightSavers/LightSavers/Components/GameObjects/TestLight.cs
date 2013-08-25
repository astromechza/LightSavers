using LightSavers.Components.Shader;
using LightSavers.Components.WorldBuilding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    public class TestLight : GameObject
    {
        private const float radius = 0.2f;
        private const float bouncethresholdSquared = 0.5f;

        private VertexIndiceSet VIS;
        private int lightindex;

        float startX;
        float destinationX;
        float startZ;
        float destinationZ;

        private Vector4 attenuation;
        private Vector4 colour;

        public TestLight(int lightindex, float x, float y, float z, float range, Vector4 colour)
        {
            X = x;
            Y = y;
            Z = z;

            this.lightindex = lightindex;
            this.colour = colour;

            startX = X;
            destinationX = X;
            startZ = Z;
            destinationZ = Z;

            attenuation = new Vector4(range, 1.0f, 4.5f/range, 75.0f/(range*range));

            BuildGeometry();
        }

        public void BuildGeometry()
        {
            List<TriDeclaration> tris = new List<TriDeclaration>();

            Vector3 top = new Vector3(0, radius, 0);
            Vector3 bottom = new Vector3(0, -radius, 0);

            Vector3 forward = new Vector3(0, 0, -radius);
            Vector3 backward = new Vector3(0, 0, +radius);

            Vector3 left = new Vector3(-radius, 0, 0);
            Vector3 right = new Vector3(radius, 0, 0);

            TriDeclaration t1 = new TriDeclaration(); 
            t1.SetPositions(top, left, forward);
            t1.SetNormal(new Vector3(-1, 1, -1));
            
            TriDeclaration t2 = new TriDeclaration(); 
            t2.SetPositions(top, forward, right);
            t2.SetNormal(new Vector3(1, 1, -1));
            
            TriDeclaration t3 = new TriDeclaration(); 
            t3.SetPositions(top, right, backward);
            t3.SetNormal(new Vector3(1, 1, 1));
            
            TriDeclaration t4 = new TriDeclaration(); 
            t4.SetPositions(top, backward, left);
            t4.SetNormal(new Vector3(-1, 1, 1));

            tris.Add(t1);
            tris.Add(t2);
            tris.Add(t3);
            tris.Add(t4);

            VIS = VertexIndiceSet.Build(tris);
        }

        public override RectangleF GetBoundRect()
        {
            return new RectangleF(X - 0.01f, Y - 0.01f, 0.02f, 0.02f);
        }

        public override void Update(float millis)
        {
            float distanceToDestination = Vector2.DistanceSquared(new Vector2(X, Z), new Vector2(destinationX, destinationZ));
                        
            // do nothing for now
            if (distanceToDestination > bouncethresholdSquared)
            {

                X += (destinationX - X) * 0.05f;
                Z += (destinationZ - Z) * 0.05f;
            }
            else
            {
                destinationX = Globals.random.Next(32); 
                destinationZ = Globals.random.Next(32); 
            }

        }

        public override void Draw(float millis, TestShader shader)
        {
            shader.WorldMatrix.SetValue(Matrix.CreateTranslation(new Vector3(X, Y, Z)));
            shader.CurrentTexture.SetValue(AssetLoader.tex_white);

            shader.PointLight[lightindex].Enabled.SetValue(true);
            shader.PointLight[lightindex].Attenuation.SetValue(attenuation);
            shader.PointLight[lightindex].Position.SetValue(new Vector3(X, Y, Z));
            shader.PointLight[lightindex].Diffuse.SetValue(colour);

            foreach (EffectPass pass in shader.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Globals.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    VIS.vertices,
                    0,
                    VIS.vertices.Length,
                    VIS.indices,
                    0,
                    VIS.indices.Length / 3
                );
            }
        }


    }
}
