using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LightSavers.Components.Shader;

namespace LightSavers.Components
{
    /// <summary>
    /// Each WorldSection is a 32x32 'block' area that is drawn together. 
    /// WorldSections act as one level of detail in collision grid and also 
    /// make it very easy to test world collisions using a simple array lookup
    /// and can be treated as a sort of height map as well.
    /// 
    /// WorldSections are loaded from 32x32 pixel bitmap image files using the colours:
    /// BLACK ( 000000 ) : No tile, blank empty space
    /// WHITE ( FFFFFF ) : Floor tile, randomly chosen texture
    /// BLUE ( FF0000 ) : Wall tile, this corrospondes to a full height tile with intelligent wall quads.
    /// </summary>
    public class WorldSection
    {
        // Constants
        public const float TileSize = 1.0f;     // size of each tile
        public const float WallHeight = 2.0f;   // height of walls

        private Vector3 origin;                 // top left corner

        // Geometry
        private Model model;

        public WorldSection(string file, Vector3 origin)
        {
            model = Globals.content.Load<Model>(file);
            this.origin = origin;
        }

        public void Draw(Camera camera, TestShader shader)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateTranslation(origin);
                    effect.View = camera.GetViewMatrix();
                    effect.Projection = camera.GetProjectionMatrix();
                }
                mesh.Draw();
            }
        }
    }
}
