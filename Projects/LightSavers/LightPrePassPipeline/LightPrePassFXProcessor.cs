#region File Description
//-----------------------------------------------------------------------------
// LightPrePassFxProcessor.cs
//
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace LightPrePassProcessor
{
    /// <summary>
    /// This class takes the array of #defines from the material, and insert it into the fx file
    /// </summary>
    [ContentProcessor(DisplayName = "Light Pre-Pass FX Processor")]
    public class LightPrePassFXProcessor : EffectProcessor
    {
        public override CompiledEffectContent Process(Microsoft.Xna.Framework.Content.Pipeline.Graphics.EffectContent input, ContentProcessorContext context)
        {
            this.DebugMode = EffectProcessorDebugMode.Optimize;
            if (context.Parameters.ContainsKey("Defines"))
                this.Defines = context.Parameters["Defines"].ToString();
            return base.Process(input, context);
        }
    }
}
