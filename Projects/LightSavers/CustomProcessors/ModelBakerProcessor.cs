using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;


namespace CustomProcessors
{
    /// <summary>
    /// MaterialBakerProcessor is designed to take in a model with attached material definitions and to 
    /// back these definitions into the attached material effect file. This helps to avoid additional processing
    /// later and gives the shader more data to use.
    /// 
    /// Heavily based on the LightPrePassProcessor Jorge Adriano Luna 2011 http://jcoluna.wordpress.com
    /// 
    /// </summary>
    [ContentProcessor(DisplayName = "ModelBakerProcessor")]
    public class ModelBakerProcessor : ModelProcessor
    {
        #region CONSTANTS
        // these constants determine where we will look for the default maps in the opaque data dictionary.
        public const string NormalMapKey = "NormalMap";
        public const string DiffuseMapKey = "DiffuseMap";
        public const string SpecularMapKey = "SpecularMap";
        public const string EmissiveMapKey = "EmissiveMap";

        #endregion

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            // Should Binormals and tangents be generated if none exist
            GenerateTangentFrames = true;

            // Merge the transform settings into the model mesh
            MeshHelper.TransformScene(input, input.Transform);
            input.Transform = Matrix.Identity;  // Reset transfrom
            MergeTransforms(input);

            return base.Process(input, context);
        }

        // Merge transforms into mesh
        private void MergeTransforms(NodeContent input)
        {
            if (input is MeshContent)
            {
                MeshContent mc = (MeshContent)input;
                MeshHelper.TransformScene(mc, mc.Transform);
                mc.Transform = Matrix.Identity;
                MeshHelper.OptimizeForCache(mc);
            }
            foreach (NodeContent c in input.Children)
            {
                MergeTransforms(c);
            }
        }

        // Bake material defines
        protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
        {
            EffectMaterialContent lppMaterial = new EffectMaterialContent();

            OpaqueDataDictionary processorParameters = new OpaqueDataDictionary();
            processorParameters["ColorKeyColor"] = this.ColorKeyColor;
            processorParameters["ColorKeyEnabled"] = false;
            processorParameters["TextureFormat"] = this.TextureFormat;
            processorParameters["GenerateMipmaps"] = this.GenerateMipmaps;
            processorParameters["ResizeTexturesToPowerOfTwo"] = this.ResizeTexturesToPowerOfTwo;
            processorParameters["PremultiplyTextureAlpha"] = false;
            processorParameters["ColorKeyEnabled"] = false;



            return base.ConvertMaterial(material, context);
        }
    
    }
}