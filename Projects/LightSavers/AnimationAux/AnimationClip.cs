using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

///http://metlab.cse.msu.edu/betterskinned.html
///Bound Under MPL License

namespace AnimationAux
{
    /// <summary>
    /// An animation clip is a set of keyframes with associated bones.
    /// </summary>
    public class AnimationClip
    {
        #region Keyframe and Bone nested class

        /// <summary>
        /// An Keyframe is a rotation and translation for a moment in time.
        /// It would be easy to extend this to include scaling as well.
        /// </summary>
        public class Keyframe
        {
            public double Time;             // The keyframe time
            public Quaternion Rotation;     // The rotation for the bone
            public Vector3 Translation;     // The translation for the bone

            public Matrix Transform
            {
                get
                {
                    return Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Translation);
                }
                set
                {
                    Matrix transform = value;
                    transform.Right = Vector3.Normalize(transform.Right);
                    transform.Up = Vector3.Normalize(transform.Up);
                    transform.Backward = Vector3.Normalize(transform.Backward);
                    Rotation = Quaternion.CreateFromRotationMatrix(transform);
                    Translation = transform.Translation;
                }
            }
        }

        /// <summary>
        /// Keyframes are grouped per bone for an animation clip
        /// </summary>
        public class Bone
        {
            /// <summary>
            /// Each bone has a name so we can associate it with a runtime model
            /// </summary>
            private string name = "";

            /// <summary>
            /// The keyframes for this bone
            /// </summary>
            private List<Keyframe> keyframes = new List<Keyframe>();

            /// <summary>
            /// The bone name for these keyframes
            /// </summary>
            public string Name { get { return name; } set { name = value; } }

            /// <summary>
            /// The keyframes for this bone
            /// </summary>
            public List<Keyframe> Keyframes { get { return keyframes; } set { keyframes = value; } }
        }

        #endregion
        #region other
        /// <summary>
        /// The bones for this animation
        /// </summary>
        private List<Bone> bones = new List<Bone>();

        /// <summary>
        /// Name of the animation clip
        /// </summary>
        public string Name;

        /// <summary>
        /// Duration of the animation clip
        /// </summary>
        public double Duration;

        /// <summary>
        /// The bones for this animation clip with their keyframes
        /// </summary>
        public List<Bone> Bones { get { return bones; } }
        #endregion

        public AnimationClip extractAclip(int clipBegin, int clipEnd, int totalkeyFrames, string Name)
        {
            AnimationClip newMiniClip = new AnimationClip();
            newMiniClip.Name = Name;
            double newDuration = Duration * ((double)(totalkeyFrames / (clipEnd - clipBegin)));
            newMiniClip.Duration = newDuration;

            List<Bone> newBoneList = new List<Bone>();

            foreach (Bone bone in bones)
            {
                Bone newBone = extractBoneClip(bone, clipBegin, clipEnd);
                newBoneList.Add(newBone);
            }

            return newMiniClip;
        }

        private Bone extractBoneClip(Bone bone, int begin, int end)
        {
            Bone newBone = new Bone();
            newBone.Name = bone.Name;

            List<Keyframe> newKeyframes = new List<Keyframe>();

            //for (int i = begin; i < end; ++i)
            //{
            //    if (end < bone.Keyframes.Count() || i==0 )
            //    {
            //        newKeyframes=bone.Keyframes;
            //    }else{
            //        continue;
            //    }

            //}

            return newBone;
        }

        public AnimationClip copyClip(int clipBegin, int clipEnd, int totalkeyFrames, string Name)
        {
            //Create a new clip
            AnimationClip newClip = new AnimationClip();
            //Calculate the length of the new animation
            double newDuration = ((double)(clipEnd - clipBegin) / (double)178) * Duration;
            newClip.Duration = newDuration;

            //Calculate the number of keyframes in new animation
            int noOfkeyFrames = (int)(newDuration * Duration);

            newClip.Name = Name;

            foreach (Bone oldBone in bones)
            {
                //Copy the bones
                newClip.bones.Add(copyBone(oldBone, clipBegin, clipEnd, noOfkeyFrames));
            }

            return newClip;

        }

        private Bone copyBone(Bone bone, int begin, int end, int noOfkeyFrames)
        {
            //create a new bone
            Bone newBone = new Bone();

            //set the new bones name
            string newName = bone.Name;
            newBone.Name = newName;
            //Count the number of keyframes associated with his bone
            int No = bone.Keyframes.Count() - 1;

            //Iterate over every keyFrame
            int keyFrame = 0;
            double timeThusfar = 0;
            for (int i = 0; i <= No; i++)
            {
                if (i > 9 && begin + i < end)
                {
                    if (timeThusfar == 0 && !(i >= 12))
                    {
                        timeThusfar = bone.Keyframes[begin - 1 + i].Time;
                    }
                    keyFrame++;
                    newBone.Keyframes.Add(copyKeyframe(bone.Keyframes[begin + i], keyFrame, timeThusfar, noOfkeyFrames));
                }
                else
                {
                    newBone.Keyframes.Add(copyKeyframe(bone.Keyframes[i], keyFrame, timeThusfar, noOfkeyFrames));
                }


            }

            //foreach (Keyframe oldKeyframe in bone.Keyframes)
            //{
            //    //Add keyframes to the new Bone
            //    newBone.Keyframes.Add(copyKeyframe(oldKeyframe, begin, end, noOfkeyFrames));
            //}

            return newBone;
        }

        //Copy keyframe
        private Keyframe copyKeyframe(Keyframe oldKeyframe, int keyFrame, double timeSofar, int noOfKeyFrames)
        {
            //create a new keyframe
            Keyframe newKeyframe = new Keyframe();
            double time = oldKeyframe.Time - timeSofar;
            //Set Keyframe attributes
            newKeyframe.Rotation = oldKeyframe.Rotation;
            newKeyframe.Time = time;
            newKeyframe.Translation = oldKeyframe.Translation;
            newKeyframe.Transform = oldKeyframe.Transform;

            return newKeyframe;
        }
    }
}
