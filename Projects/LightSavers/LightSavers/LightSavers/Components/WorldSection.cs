using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LightSavers.Components.GameObjects;

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

        // Geometry
        private MeshWrapper mesh;
        public MeshWrapper Mesh { get {return mesh;} }

        public WorldSection(string file, Vector3 origin)
        {
            mesh = new MeshWrapper(Globals.content.Load<Model>(file), Matrix.CreateTranslation(origin));
        }

        public void Draw(Camera camera)
        {
            mesh.RenderToGBuffer(camera);
        }

        
    }
}
