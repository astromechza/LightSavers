using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace CustomProcessors
{
    [ContentProcessor(DisplayName = "Light Pre-Pass FX Processor")]
    public class FXBakerProcessor : EffectProcessor
    {
        public override CompiledEffectContent Process(Microsoft.Xna.Framework.Content.Pipeline.Graphics.EffectContent input, ContentProcessorContext context)
        {
            this.DebugMode = EffectProcessorDebugMode.Optimize;
            if (context.Parameters.ContainsKey("Defines")) this.Defines = context.Parameters["Defines"].ToString();
            return base.Process(input, context);
        }
    }
}
