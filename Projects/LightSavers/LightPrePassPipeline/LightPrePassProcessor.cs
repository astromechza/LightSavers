#region File Description
//-----------------------------------------------------------------------------
//Based on  
//NormalMappingModelProcessor.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
// http://create.msdn.com/en-US/education/catalog/sample/normal_mapping
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Changed by
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//-----------------------------------------------------------------------------
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using LightPrePassRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace LightPrePassProcessor
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// Its based heavily on the normal mapping sample available at
    /// http://create.msdn.com/en-US/education/catalog/sample/normal_mapping
    /// </summary>
    [ContentProcessor(DisplayName = "LightPrePass Model Processor")]
    public class LightPrePassProcessor : ModelProcessor
    {
        protected bool _isSkinned = false;
        // these constants determine where we will look for the default maps in the opaque
        // data dictionary.
        public const string NormalMapKey = "NormalMap";
        public const string DiffuseMapKey = "DiffuseMap";
        public const string SpecularMapKey = "SpecularMap";
        public const string EmissiveMapKey = "EmissiveMap";
        public const string SecondNormalMapKey = "SecondNormalMap";
        public const string SecondDiffuseMapKey = "SecondDiffuseMap";
        public const string SecondSpecularMapKey = "SecondSpecularMap";
        public const string ReflectionMapKey = "ReflectionMap";

        private string _customFx = "";

        private MeshMetadata.ERenderQueue _renderQueue = MeshMetadata.ERenderQueue.Default;

        public MeshMetadata.ERenderQueue RenderQueue
        {
            get { return _renderQueue; }
            set { _renderQueue = value; }
        }

        private bool _castShadows = true;

        public string CustomFx
        {
            get { return _customFx; }
            set { _customFx = value; }
        }

        public bool CastShadows
        {
            get { return _castShadows; }
            set { _castShadows = value; }
        }

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            //we always want to generate tangent frames, as we use tangent space normal mapping
            GenerateTangentFrames = true;

            //merge transforms
            MeshHelper.TransformScene(input, input.Transform);
            input.Transform = Matrix.Identity;

            if (!_isSkinned)
                MergeTransforms(input);

            ModelContent model = base.Process(input, context);
            //gather some information that will be useful in run time
            MeshMetadata metadata = new MeshMetadata();
            BoundingBox aabb = new BoundingBox();
            metadata.BoundingBox = ComputeBoundingBox(input, ref aabb, metadata);

            //assign it to our Tag
            model.Tag = metadata;
            return model;
        }

        private void MergeTransforms(NodeContent input)
        {
            if (input is MeshContent)
            {
                MeshContent mc = (MeshContent) input;
                MeshHelper.TransformScene(mc, mc.Transform);
                mc.Transform = Matrix.Identity;
                MeshHelper.OptimizeForCache(mc);
            }
            foreach (NodeContent c in input.Children)
            {
                MergeTransforms(c);
            }
        }

        private BoundingBox ComputeBoundingBox(NodeContent input, ref BoundingBox aabb, MeshMetadata metadata)
        {
            BoundingBox boundingBox;
            if (input is MeshContent)
            {
                MeshContent mc = (MeshContent)input;
                MeshHelper.TransformScene(mc, mc.Transform);
                mc.Transform = Matrix.Identity;

                boundingBox = BoundingBox.CreateFromPoints(mc.Positions);
                //create sub mesh information
                MeshMetadata.SubMeshMetadata subMeshMetadata = new MeshMetadata.SubMeshMetadata();
                subMeshMetadata.BoundingBox = boundingBox;
                subMeshMetadata.RenderQueue = _renderQueue;
                subMeshMetadata.CastShadows = CastShadows;
                metadata.AddSubMeshMetadata(subMeshMetadata);
                if (metadata.SubMeshesMetadata.Count > 1)
                    boundingBox = BoundingBox.CreateMerged(boundingBox, aabb);
            }
            else
            {
                boundingBox = aabb;
            }

            foreach (NodeContent c in input.Children)
            {
                boundingBox = BoundingBox.CreateMerged(boundingBox, ComputeBoundingBox(c, ref boundingBox, metadata));
            }
            return boundingBox;
        }

        protected override MaterialContent ConvertMaterial(MaterialContent material,
           ContentProcessorContext context)
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

            lppMaterial.Effect = new ExternalReference<EffectContent>(_customFx.Length == 0 ? "shaders/LPPMainEffect.fx" : _customFx);
            lppMaterial.CompiledEffect = context.BuildAsset<EffectContent, CompiledEffectContent>(lppMaterial.Effect, "EffectProcessor");

            // copy the textures in the original material to the new lpp
            // material
            ExtractTextures(lppMaterial, material);
            //extract the extra parameters
            ExtractDefines(lppMaterial, material, context);

            // and convert the material using the NormalMappingMaterialProcessor,
            // who has something special in store for the normal map.
            return context.Convert<MaterialContent, MaterialContent>
                (lppMaterial, typeof(LightPrePassMaterialProcessor).Name, processorParameters);
        }

        /// <summary>
        /// Extract any defines we need from the original material, like alphaMasked, fresnel, reflection, etc, and pass it into
        /// the opaque data
        /// </summary>
        /// <param name="lppMaterial"></param>
        /// <param name="material"></param>
        /// <param name="context"></param>
        private void ExtractDefines(EffectMaterialContent lppMaterial, MaterialContent material, ContentProcessorContext context)
        {
            string defines = "";

            if (material.OpaqueData.ContainsKey("alphaMasked") && material.OpaqueData["alphaMasked"].ToString() == "True")
            {
                context.Logger.LogMessage("Alpha masked material found");
                lppMaterial.OpaqueData.Add("AlphaReference", (float)material.OpaqueData["AlphaReference"]);
                defines += "ALPHA_MASKED;";
            }

            if (material.OpaqueData.ContainsKey("reflectionEnabled") && material.OpaqueData["reflectionEnabled"].ToString() == "True")
            {
                context.Logger.LogMessage("Reflection enabled");
                defines += "REFLECTION_ENABLED;";
            }

            if (_isSkinned)
            {
                context.Logger.LogMessage("Skinned mesh found");
                defines += "SKINNED_MESH;";
            }

            if (material.OpaqueData.ContainsKey("dualLayerEnabled") && material.OpaqueData["dualLayerEnabled"].ToString() == "True")
            {
                context.Logger.LogMessage("Dual layer material found");
                defines += "DUAL_LAYER;";
            }

            if (!String.IsNullOrEmpty(defines))
                lppMaterial.OpaqueData.Add("Defines", defines);

        }
        private void ExtractTextures(EffectMaterialContent lppMaterial, MaterialContent material)
        {
            foreach (KeyValuePair<String, ExternalReference<TextureContent>> texture
                in material.Textures)
            {
                if (texture.Key.ToLower().Equals("diffusemap")  || texture.Key.ToLower().Equals("texture"))
                    lppMaterial.Textures.Add(DiffuseMapKey, texture.Value);
                if (texture.Key.ToLower().Equals("normalmap"))
                    lppMaterial.Textures.Add(NormalMapKey, texture.Value);
                if (texture.Key.ToLower().Equals("specularmap"))
                    lppMaterial.Textures.Add(SpecularMapKey, texture.Value);
                if (texture.Key.ToLower().Equals("emissivemap") || texture.Key.ToLower().Equals("emissive"))
                    lppMaterial.Textures.Add(EmissiveMapKey, texture.Value);

                if (texture.Key.ToLower().Equals("seconddiffusemap"))
                    lppMaterial.Textures.Add(SecondDiffuseMapKey, texture.Value);
                if (texture.Key.ToLower().Equals("secondnormalmap"))
                    lppMaterial.Textures.Add(SecondNormalMapKey, texture.Value);
                if (texture.Key.ToLower().Equals("secondspecularmap"))
                    lppMaterial.Textures.Add(SecondSpecularMapKey, texture.Value);

                if (texture.Key.ToLower().Equals("reflectionmap"))
                    lppMaterial.Textures.Add(ReflectionMapKey, texture.Value);
            }
           
            ExternalReference<TextureContent> externalRef;
            if (!lppMaterial.Textures.TryGetValue(DiffuseMapKey, out externalRef))
            {
                lppMaterial.Textures[DiffuseMapKey] = new ExternalReference<TextureContent>("textures/default_diffuse.tga");
            }
            if (!lppMaterial.Textures.TryGetValue(NormalMapKey, out externalRef))
            {
                lppMaterial.Textures[NormalMapKey] = new ExternalReference<TextureContent>("textures/default_normal.tga");
            }
            if (!lppMaterial.Textures.TryGetValue(SpecularMapKey, out externalRef))
            {
                lppMaterial.Textures[SpecularMapKey] = new ExternalReference<TextureContent>("textures/default_specular.tga");
            }
            if (!lppMaterial.Textures.TryGetValue(EmissiveMapKey, out externalRef))
            {
                lppMaterial.Textures[EmissiveMapKey] = new ExternalReference<TextureContent>("textures/default_emissive.tga");
            }
        }
    }
}