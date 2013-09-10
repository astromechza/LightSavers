//-----------------------------------------------------------------------------
// BoundingBoxRenderer.cs
//
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//-----------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightPrePassRenderer
{
    public class BoundingBoxRenderer
    {
        private VertexPositionColor[] points = new VertexPositionColor[8];
        private static short[] index = new short[24];
        private VertexDeclaration declaration;
        private BasicEffect effect;

        public BoundingBoxRenderer()
        {
            index[0] = 0;
            index[1] = 1;
            index[2] = 1;
            index[3] = 2;
            index[4] = 2;
            index[5] = 3;
            index[6] = 3;
            index[7] = 0;

            index[8] = 4;
            index[9] = 5;
            index[10] = 5;
            index[11] = 6;
            index[12] = 6;
            index[13] = 7;
            index[14] = 7;
            index[15] = 4;

            index[16] = 0;
            index[17] = 4;
            index[18] = 1;
            index[19] = 5;
            index[20] = 2;
            index[21] = 6;
            index[22] = 3;
            index[23] = 7;

            InitBoundingBox();
        }

        private void InitBoundingBox()
        {
            BoundingBox boundingBox = new BoundingBox(-Vector3.One, Vector3.One);

            points[0].Position = new Vector3(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Min.Z);            
            points[1].Position = new Vector3(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Min.Z);
            points[2].Position = new Vector3(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Max.Z);
            points[3].Position = new Vector3(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Max.Z);

            points[4].Position = new Vector3(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Min.Z);
            points[5].Position = new Vector3(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Min.Z);
            points[6].Position = new Vector3(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Max.Z);
            points[7].Position = new Vector3(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Max.Z);

            for (int i=0;i<8;i++)
                points[i].Color = new Color(255, 255, 255, 255);
        }

        public void Draw(GraphicsDevice device, BoundingBox boundingBox, Camera camera, Color color)
        {
            if (declaration == null)
            {
                declaration = VertexPositionColor.VertexDeclaration;
            }

            if (effect == null)
            {
                effect = new BasicEffect(device);
            }
            effect.DiffuseColor = color.ToVector3();

            effect.View = camera.EyeTransform;
            effect.Projection = camera.ProjectionTransform;
            Vector3 size = boundingBox.Max - boundingBox.Min;
            effect.World = Matrix.CreateScale(size)*Matrix.CreateTranslation(boundingBox.Max - size*0.5f);
            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionColor>(
                                    PrimitiveType.LineList, points,
                                    0,
                                    8,
                                    index,
                                    0,
                                    12);
            }
        }
    }
}
