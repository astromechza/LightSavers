//-----------------------------------------------------------------------------
// LightPrePass.cs
//
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//-----------------------------------------------------------------------------

namespace LightPrePassRenderer
{
    /// <summary>
    /// Class
    /// </summary>
    public class RenderStatistics
    {
        public void Reset()
        {
            VisibleParticleSystems = DrawCalls = VisibleLights = ShadowCasterMeshes = ShadowCasterLights = 0;
        }

        public int DrawCalls;
        public int VisibleLights;
        public int ShadowCasterLights;
        public int ShadowCasterMeshes;
        public int VisibleParticleSystems;
    }
}
