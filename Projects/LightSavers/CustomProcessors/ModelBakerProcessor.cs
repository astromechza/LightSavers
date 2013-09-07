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

            lppMaterial.Effect = new ExternalReference<EffectContent>("shaders/LPPMainEffect.fx");
            lppMaterial.CompiledEffect = context.BuildAsset<EffectContent, CompiledEffectContent>(lppMaterial.Effect, typeof(FXBakerProcessor).Name);

            ExtractTextures(lppMaterial, material);

            ExtractDefines(lppMaterial, material, context);

            return context.Convert<MaterialContent, MaterialContent>(lppMaterial, typeof(MaterialBakerProcessor).Name, processorParameters);
        }

        /// <summary>
        /// Copy Textures from source effectmaterial into destination effectmaterial
        /// </summary>
        private void ExtractTextures(EffectMaterialContent destination, MaterialContent source)
        {
            // Copy known textures
            foreach (KeyValuePair<String, ExternalReference<TextureContent>> texture in source.Textures)
            {
                if (texture.Key.ToLower().Contains("diffuseMap".ToLower())) destination.Textures.Add(DiffuseMapKey, texture.Value);
                if (texture.Key.ToLower().Contains("normalMap".ToLower())) destination.Textures.Add(NormalMapKey, texture.Value);
                if (texture.Key.ToLower().Contains("specularMap".ToLower())) destination.Textures.Add(SpecularMapKey, texture.Value);
                if (texture.Key.ToLower().Contains("emissiveMap".ToLower())) destination.Textures.Add(EmissiveMapKey, texture.Value);
            }

            // If Textures don't exist, add default textures instead
            ExternalReference<TextureContent> externalRef;
            if (!destination.Textures.TryGetValue(DiffuseMapKey, out externalRef))
            {
                destination.Textures[DiffuseMapKey] = new ExternalReference<TextureContent>("textures/default_diffuse.tga");
            }
            if (!destination.Textures.TryGetValue(NormalMapKey, out externalRef))
            {
                destination.Textures[NormalMapKey] = new ExternalReference<TextureContent>("textures/default_normal.tga");
            }
            if (!destination.Textures.TryGetValue(SpecularMapKey, out externalRef))
            {
                destination.Textures[SpecularMapKey] = new ExternalReference<TextureContent>("textures/default_specular.tga");
            }
            if (!destination.Textures.TryGetValue(EmissiveMapKey, out externalRef))
            {
                destination.Textures[EmissiveMapKey] = new ExternalReference<TextureContent>("textures/default_emissive.tga");
            }
        }

        /// <summary>
        /// Extract the defines we need (Just alpha for the moment)
        /// </summary>
        private void ExtractDefines(EffectMaterialContent destination, MaterialContent source, ContentProcessorContext context)
        {
            if (source.OpaqueData.ContainsKey("alphaMasked") && source.OpaqueData["alphaMasked"].ToString() == "True")
            {
                context.Logger.LogMessage("Alpha masked material found");
                destination.OpaqueData.Add("AlphaReference", (float) source.OpaqueData["AlphaReference"]);
                destination.OpaqueData.Add("Defines", "ALPHA_MASKED;");
            }
        }

    }
}