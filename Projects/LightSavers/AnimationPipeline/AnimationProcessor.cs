using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using AnimationAux;

///http://metlab.cse.msu.edu/betterskinned.html
///Bound Under MPL License

namespace AnimationPipeline
{
    [ContentProcessor(DisplayName = "Animation Processor")]
    public class AnimationProcessor : ModelProcessor
    {
        //The model that will be processed
        private ModelContent model;

        //Extra information on the model will be stored here... (animation clips, etc)
        private ModelExtra modelExtra = new ModelExtra();

        //Keeps track of which materials have already been swapped to a skinned materials
        private Dictionary<MaterialContent, SkinnedMaterialContent> toSkinnedMaterial = new Dictionary<MaterialContent, SkinnedMaterialContent>();

        //? not quite sure exactly what this does yet, will get back to it once I've gone over the functions lower down
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            //this uses a method defined below
            BoneContent skeleton = ProcessSkeleton(input);

            SwapSkinnedMaterial(input);

            model = base.Process(input, context);

            ProcessAnimations(model, input, context);

            model.Tag = modelExtra;

            return model;
        }

        #region skeleton Support
        //? not Idea what the hell Node Content is - this method implements skeletal animation support (processes the skeleton)
        private BoneContent ProcessSkeleton(NodeContent input)
        {
            BoneContent skeleton = MeshHelper.FindSkeleton(input);

            //If there is not skeleton, surrender to the powers that be!!! (return null)
            if (skeleton == null)
                return null;

            //NOTE: The rest of this method only works if there is a skeleton - this comment is a sanity check - don't judge me!!! >.<

            //? This method has not been defined at the time of writing, I have no idea what it's doing
            //On another note: his comments say that we don't want to worry about different parts of the model being in different local coordinate systems so he's just baking everything (stoner?) anyways, I will try to figure this out when I get there
            FlattenTransforms(input, skeleton);

            //? This removes "nubs" according to the other comments these are useless bones that 3D studio max apparently puts in. 
            TrimSkeleton(skeleton);

            //? Convert a heirarchy of nodes and bones into a list (what on earth are nodes?)
            List<NodeContent> nodes = FlattenHeirarchy(input);
            IList<BoneContent> bones = MeshHelper.FlattenSkeleton(skeleton);

            //?
            Dictionary<NodeContent, int> nodeToIndex = new Dictionary<NodeContent, int>();
            for (int i=0; i< nodes.Count; i++)
            {
                nodeToIndex[nodes[i]] = i;
            }

            return skeleton;
        }

        //? Convert a tree of nodes into a list of nodes in topological order - as of now I have no idea what this is for 2013/09/01
        private List<NodeContent> FlattenHeirarchy(NodeContent item)
        {
            List<NodeContent> nodes = new List<NodeContent>();
            nodes.Add(item);
            foreach (NodeContent child in item.Children)
            {
                FlattenHeirarchy(nodes, child);
            }

            return nodes;
        }

        private void FlattenHeirarchy(List<NodeContent> nodes, NodeContent item)
        {
            nodes.Add(item);
            foreach (NodeContent child in item.Children)
            {
                FlattenHeirarchy(nodes, child);
            }
        }

        //? Bakes unwanted transforms into the model geometry, so that all everything ends up in the same local coordinates
        void FlattenTransforms(NodeContent node, BoneContent skeleton)
        {
            foreach (NodeContent child in node.Children)
            {
                //? do not process the skeleton for some reason
                if (child == skeleton)
                    continue;

                //Do not bake in transforms unless it's part of a skinned mesh NB
                if (IsSkinned(child))
                {
                    FlattenAllTransforms(child);
                }

            }
        }

        //Recursively flatten all transforms from this node down
        void FlattenAllTransforms(NodeContent node)
        {
            //backe the local transforms into the actual geometry
            MeshHelper.TransformScene(node, node.Transform);

            //since it is now baked, local coordinate systems can be set to identity
            node.Transform = Matrix.Identity;

            foreach (NodeContent child in node.Children)
            {
                FlattenAllTransforms(child);
            }

        }

        //Remove some extras that are useless to us (put in by 3ds max)
        void TrimSkeleton(NodeContent skeleton)
        {
            //store a list of nodes to be deleted
            List<NodeContent> todelete = new List<NodeContent>();

            foreach(NodeContent child in skeleton.Children)
            {
                if (child.Name.EndsWith("Nub") || child.Name.EndsWith("Footsteps"))
                    todelete.Add(child);
                else
                    TrimSkeleton(child);
            }

            //delete the nubs
            foreach (NodeContent child in todelete)
            {
                skeleton.Children.Remove(child);
            }

        }

        #endregion

        #region Skinned Support
        //? Determine if a node is a skinned node, meaning it has bone weights. Bone weights?
        bool IsSkinned(NodeContent node)
        {
            MeshContent mesh = node as MeshContent;
            if (mesh != null)
            {
                //? Find the vertex channel that has a bone weight collection
                foreach (GeometryContent geometry in mesh.Geometry)
                {
                    foreach (VertexChannel vchannel in geometry.Vertices.Channels)
                    {
                        if (vchannel is VertexChannel<BoneWeightCollection>)
                            return true;
                    }
                }
            }

            return false;
        }

        //? swaps in a skinned effect instead of a basic effect  by finding meshes that have bone weights associated with them. still not sure on this bone weigthting thing... skinned effect vs basic effect - I wonder if thats whats been making the model pop up as plane black
        //Recursively
        void SwapSkinnedMaterial(NodeContent node)
        {
            //convert node to a mesh content type thingy
            MeshContent mesh = node as MeshContent;

            if ( mesh != null)
            {
                //find the geometry in the vertex channel that has a bone weight collection
                foreach (GeometryContent geometry in mesh.Geometry)
                {
                    bool swap = false;
                    //? vertices channels?
                    foreach (VertexChannel vchannel in geometry.Vertices.Channels)
                    {
                        if ( vchannel is VertexChannel<BoneWeightCollection>)
                        {
                            swap = true;
                            break;
                        }
                    }

                    if (swap)
                    {
                        if (toSkinnedMaterial.ContainsKey(geometry.Material))
                        {
                            //? 
                            geometry.Material = toSkinnedMaterial[geometry.Material];
                        }
                        else
                        {
                            //? converts to a skinned material?
                            SkinnedMaterialContent smaterial = new SkinnedMaterialContent();
                            BasicMaterialContent bmaterial = geometry.Material as BasicMaterialContent;

                            smaterial.Alpha = bmaterial.Alpha;
                            smaterial.DiffuseColor = bmaterial.DiffuseColor;
                            smaterial.EmissiveColor = bmaterial.EmissiveColor;
                            smaterial.SpecularColor = bmaterial.SpecularColor;
                            smaterial.SpecularPower = bmaterial.SpecularPower;
                            smaterial.Texture = bmaterial.Texture;
                            //? I have no idea where the 4 comes from
                            smaterial.WeightsPerVertex = 4;

                            toSkinnedMaterial[geometry.Material] = smaterial;
                            geometry.Material = smaterial;

                        }
                    }

                }               
            }
            //recurse over the children
            foreach (NodeContent child in node.Children)
            {
                SwapSkinnedMaterial(child);
            }
        }
        #endregion

        #region Animation Support

        //Bone lookup table - converts bone names to indices
        private Dictionary<string, int> bones = new Dictionary<string, int>();

        //bone transformations for a base pose
        private Matrix[] boneTransforms;

        //Keep track of clips by name;
        private Dictionary<string, AnimationClip> clips = new Dictionary<string, AnimationClip>();

        //Entry point for Animation Processing
        public void ProcessAnimations(ModelContent model, NodeContent input, ContentProcessorContext context)
        { 
            for ( int  i = 0; i< model.Bones.Count; ++i)
            {
                //create a lookup table
                bones[model.Bones[i].Name] = i;
            }

            //for saving bone transforms
            boneTransforms = new Matrix[model.Bones.Count];

            //Collect all the animation data
            ProcessAnimationRecursive(input);

            //make sure there is always an animation clip, even if none is included in the FBX
            if (modelExtra.clips.Count == 0)
            {
                AnimationClip clip = new AnimationClip();
                modelExtra.Clips.Add(clip);

                //? not sure if there is something specific about the naming policy here:
                string clipName = "Take 001";

                clips[clipName] = clip;

                clip.Name = clipName;

                foreach (ModelBoneContent bone in model.Bones)
                {
                    AnimationClip.Bone clipBone = new AnimationClip.Bone();
                    clipBone.Name = bone.Name;

                    clip.Bones.Add(clipBone);
                }
            }

            //Insure all animation have a first key frame for every bone
            foreach (AnimationClip clip in modelExtra.Clips)
            {
                for (int b = 0; b < bones.Count; b++)
                {
                    List<AnimationClip.Keyframe> keyframes = new List<AnimationClip.Keyframe>();
                    if (keyframes.Count == 0 || keyframes[0].Time > 0)
                    {
                        AnimationClip.Keyframe keyframe = new AnimationClip.Keyframe();
                        keyframe.Time = 0;
                        keyframe.Transform = boneTransforms[b];
                        keyframes.Insert(0, keyframe);
                    }
                }
            }
        }

        //Recursively process Animations through the entire scene graph, collecting up all the animation data
        private void ProcessAnimationRecursive(NodeContent input)
        {
            //Look up the bone for this input channel
            int inputBoneIndex;
            if(bones.TryGetValue(input.Name, out inputBoneIndex))
            {
                //save the transform
                boneTransforms[inputBoneIndex] = input.Transform;
            }

            foreach (KeyValuePair<string, AnimationContent> animation in input.Animations)
            {
                AnimationClip clip;
                string clipName = animation.Key;

                if(!clips.TryGetValue(clipName, out clip))
                {
                    //New clip
                    clip= new AnimationClip();
                    modelExtra.Clips.Add(clip);

                    //retain by name
                    clips[clipName] = clip;

                    clip.Name = clipName;

                    foreach (ModelBoneContent bone in model.Bones)
                    {
                        AnimationClip.Bone clipBone = new AnimationClip.Bone();
                        clipBone.Name = bone.Name;
                        clip.Bones.Add(clipBone);

                    }
                }

                //Insure the duration is set correctly
                if (animation.Value.Duration.TotalSeconds > clip.Duration)
                    clip.Duration = animation.Value.Duration.TotalSeconds;

                //? For each channel (what is a channel?) determine the bone and then process all of the keyframes for that bone

                foreach (KeyValuePair<string, AnimationChannel> channel in animation.Value.Channels)
                {
                    int boneIndex;
                    if (!bones.TryGetValue(channel.Key, out boneIndex))
                        continue; //ignore if not a named bone

                    //remove animations associated with bones that are not assigned to any meshes
                    if (UselessAnimationTest(boneIndex))
                        continue;

                    //? create a linked list that will be used for removing redundant keyframes
                    LinkedList<AnimationClip.Keyframe> keyframes = new LinkedList<AnimationClip.Keyframe>();
                    foreach (AnimationKeyframe keyframe in channel.Value)
                    {
                        Matrix transform = keyframe.Transform;

                        AnimationClip.Keyframe newKeyframe = new AnimationClip.Keyframe();
                        newKeyframe.Time = keyframe.Time.TotalSeconds;
                        newKeyframe.Transform = transform;

                        keyframes.AddLast(newKeyframe);
                    }

                    //remove anything that can be linearly interpolated
                    LinearKeyframeReduction(keyframes);
                    foreach (AnimationClip.Keyframe keyframe in keyframes)
                    {
                        clip.Bones[boneIndex].Keyframes.Add(keyframe);
                    }
                }

            }

            //recurse over each of the nodes children
            foreach (NodeContent child in input.Children)
            {
                ProcessAnimationRecursive(child);
            }
        }

        //? not sure what these will be used for yet - first guess is lerping and or slerping
        private const float TineLength = 1e-8f;
        private const float TineCosAngle = 0.9999999f;

        //This filters out keyframes that can be approximated well with linear interpolation
        private void LinearKeyframeReduction(LinkedList<AnimationClip.Keyframe> keyframes)
        {
            if (keyframes.Count < 3)
                return;

            //? weird use of a for loop?
            for (LinkedListNode<AnimationClip.Keyframe> node = keyframes.First.Next; ; )
            {
                LinkedListNode<AnimationClip.Keyframe> next = node.Next;
                if (next == null)
                    break;

                //determine nodes before and after current
                AnimationClip.Keyframe a = node.Previous.Value;
                AnimationClip.Keyframe b = node.Value;
                AnimationClip.Keyframe c = next.Value;

                //Time between keyframes
                float t = (float)((node.Value.Time - node.Previous.Value.Time) / (next.Value.Time - node.Previous.Value.Time));

                Vector3 translation = Vector3.Lerp(a.Translation, c.Translation, t);
                Quaternion rotation = Quaternion.Slerp(a.Rotation, c.Rotation, t);

                if ((translation - b.Translation).LengthSquared() < TineLength && Quaternion.Dot(rotation, b.Rotation) > TineCosAngle)
                {
                    keyframes.Remove(node);
                }

                node = next;
            }
        }

        //Used to discard any animations that are not assigned to a mesh or skeleton
        bool UselessAnimationTest(int boneId)
        {
            // if any mesh is assigned to a bone then it is not useless!
            foreach (ModelMeshContent mesh in model.Meshes)
            {
                if (mesh.ParentBone.Index == boneId)
                    return false;
            }

            //If this bone is part of a skeleton, then it also is not useless!
            foreach (int b in modelExtra.Skeleton)
            {
                if (boneId == b)
                    return false;
            }

            //Otherwise it be useless!!!
            return true;
        }
        #endregion

        ///////////////////////////ADD MORE HERE:::

    }
}
