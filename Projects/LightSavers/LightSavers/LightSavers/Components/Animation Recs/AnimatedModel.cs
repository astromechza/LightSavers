using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using AnimationAux;

///http://metlab.cse.msu.edu/betterskinned.html
///Bound Under MPL License

namespace LightSavers.Components.Animation_recs
{
    //This class encloses the xna model that will be used and includes support for bones, animation and some manipulations
    public class AnimatedModel
    {
        #region Fields
        //The actual model
        private Model model = null;

        //Extra data associated with the model
        //this stores the animation clips and skeleton indices
        private ModelExtra modelExtra = null;

        //model bones
        private List<Bone> bones = new List<Bone>();

        //? My own variable :D:D this should scale up the model - guess it's here more for debuggin than anything else
        private float modelScale=50f;

        //model Asset name
        private string assetName = "";

        //? assotiated animation clip player
        private AnimationPlayer player = null;

        #endregion

        #region Properties
        public Model Model
        {
            get { return model; }
        }

        public List<Bone> Bones { get { return bones; } }

        public List<AnimationClip> Clips { get { return modelExtra.Clips; } }

        /// <summary>
        /// Set a custom scale for the model
        /// </summary>
        public float ModelScale { get { return modelScale; } set { modelScale = value; } }

        #endregion

        #region Construction and Loading
        //constructor
        public AnimatedModel(string assetName)
        {
            this.assetName = assetName;
        }

        public void LoadContent(ContentManager content)
        {
            this.model = content.Load<Model>(assetName);
            //? Tag as?
            modelExtra = model.Tag as ModelExtra;            
            System.Diagnostics.Debug.Assert(modelExtra != null);

            ObtainBones();
        }

        #endregion

        #region Bones Management
        //Get the bones from the model and create a bone class object (the custom bone class defined in AnimationRecs) the custom bone class is used for the actual bone animation work
        private void ObtainBones()
        {
            bones.Clear();

            foreach (ModelBone bone in model.Bones)
            {
                //Create a new bone object and add it to the hierarchy of bones
                Bone newBone = new Bone(bone.Name, bone.Transform, bone.Parent != null ? bones[bone.Parent.Index] : null);

                //Add to the bones for this model
                bones.Add(newBone);
            }
        }

        //find a bone int his model by name
        public Bone FindBone(string name)
        {
            foreach (Bone bone in Bones)
            {
                if (bone.Name == name)
                    return bone;
            }

            return null;
        }

        #endregion

        #region AnimationManagement
        public AnimationPlayer PlayClip(AnimationClip clip)
        {
            // create a new clip player and assign it to this model
            //? Animation player still being constructed at this time
            player = new AnimationPlayer(clip, this);
            return player;
        }
        #endregion

        #region Drawing
        //? This class will be modified for our cause!

        public void Draw(GraphicsDevice graphics, Camera camera, Matrix world)
        {
            //no point in drawing a no-existent model
            if (model == null)
                return;

            //set the scale of the model!
            Matrix scale = Matrix.CreateScale(modelScale);
            Matrix[] boneTransforms = new Matrix[bones.Count];

            for (int i = 0; i < bones.Count; i++)
            {
                Bone bone = bones[i];
                bone.ComputeAbsoluteTransform();

                boneTransforms[i] = bone.AbsoluteTransform;
            }

            //? Determine the skin transforms from the skeleton _ I'm guessing skin is the texture?
            Matrix[] skeleton = new Matrix[modelExtra.Skeleton.Count];
            for (int s = 0; s < modelExtra.Skeleton.Count; s++)
            {
                Bone bone = bones[modelExtra.Skeleton[s]];
                skeleton[s] = bone.SkinTransform * bone.AbsoluteTransform * scale;
            }

            //draw the model
            foreach (ModelMesh modelMesh in model.Meshes)
            {
                foreach (Effect effect in modelMesh.Effects)
                {
                    if (effect is BasicEffect)
                    {
                        BasicEffect beffect = effect as BasicEffect;
                        beffect.World = boneTransforms[modelMesh.ParentBone.Index] * world;
                        beffect.View = camera.GetViewMatrix();
                        beffect.Projection = camera.GetProjectionMatrix();
                        beffect.EnableDefaultLighting();
                        beffect.PreferPerPixelLighting = true;
                    }

                    if (effect is SkinnedEffect)
                    {
                        SkinnedEffect seffect = effect as SkinnedEffect;
                        seffect.World = boneTransforms[modelMesh.ParentBone.Index] * world;
                        seffect.View = camera.GetViewMatrix();
                        seffect.Projection = camera.GetProjectionMatrix();
                        seffect.EnableDefaultLighting();
                        seffect.PreferPerPixelLighting = true;
                        seffect.SetBoneTransforms(skeleton);
                    }
                }
                modelMesh.Draw();
            }
        }
        #endregion

    }
}
