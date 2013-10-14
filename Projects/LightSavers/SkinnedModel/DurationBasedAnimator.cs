using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkinnedModel
{
    public class DurationBasedAnimator
    {

        private SkinningData skindata;

        private AnimationClip fullclip;
        TimeSpan fullclipDuration;

        private Matrix[] boneTransforms;
        private Matrix[] worldTransforms;
        private Matrix[] skinTransforms;

        private Dictionary<int, DurationClip> durations;
        private Dictionary<int,int> validBones;

        DurationClip currentDurationClip;
        int currentKeyFrame;
        int currentLoopCount;
        TimeSpan startTimeValue, endTimeValue, currentTimeValue;

        /// <summary>
        /// Duration Based Animation constructor
        /// </summary>
        /// <param name="skin"></param>
        /// <param name="clip"></param>
        /// <param name="Dictionary of valid bones - null if all bones are valid"></param>
        public DurationBasedAnimator(SkinningData skin, AnimationClip clip, Dictionary<int, int> validBones)
        {
            // store skin data
            skindata = skin;

            // store the long clip
            fullclip = clip;
            fullclipDuration = fullclip.Duration;

            // set up empty matrices
            boneTransforms = new Matrix[skin.BindPose.Count];
            worldTransforms = new Matrix[skin.BindPose.Count];
            skinTransforms = new Matrix[skin.BindPose.Count];
            
            durations = new Dictionary<int, DurationClip>();

            this.validBones = validBones;
        }

        public void StartClip(int name)
        {
            currentDurationClip = durations[name];
            currentTimeValue = currentDurationClip.start;
            currentKeyFrame = currentDurationClip.startFrame;
            currentDurationClip.startPose.CopyTo(boneTransforms, 0);
            currentLoopCount = 0;
        }

        /// <summary>
        /// Updates the Animation
        /// </summary>
        /// <param name="time"></param>
        /// <param name="relativeToCurrentTime"></param>
        /// <param name="rootTransform"></param>
        /// <param name="worldTransform, set to identity if model is animatied with one animation player"></param>
        public void Update(TimeSpan time, bool relativeToCurrentTime, Matrix rootTransform, Matrix worldTransform)
        {
            UpdateBoneTransforms(time, relativeToCurrentTime);
            UpdateWorldTransforms(rootTransform, worldTransform);
            UpdateSkinTransforms();
        }

        public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
        {
            // Update the animation position.
            if (relativeToCurrentTime)
            {
                time += currentTimeValue;

                // If we reached the end, loop back to the start.
                while (time >= currentDurationClip.end)
                {
                    if (time >= fullclipDuration)
                        time = fullclipDuration;
                    else
                        time -= currentDurationClip.duration;
                    currentLoopCount++;
                } 
            }

            // If the position moved backwards, reset the keyframe index.
            if (time < currentTimeValue)
            {
                currentKeyFrame = currentDurationClip.startFrame;
                currentDurationClip.startPose.CopyTo(boneTransforms, 0);
            }

            currentTimeValue = time;

            // Read keyframe matrices.
            IList<Keyframe> keyframes = fullclip.Keyframes;

            while (currentKeyFrame < keyframes.Count)
            {
                Keyframe keyframe = keyframes[currentKeyFrame];

                // Stop when we've read up to the current time position.
                if (keyframe.Time > currentTimeValue)
                    break;

                boneTransforms[keyframe.Bone] = keyframe.Transform;

                currentKeyFrame++;
            }
        }


        /// <summary>
        /// Helper used by the Update method to refresh the WorldTransforms data.
        /// </summary>
        /// <param name="rootTransform"></param>
        /// <param name="worldTransform"></param>
        public void UpdateWorldTransforms(Matrix rootTransform, Matrix worldTransform)
        {
            // Root bone.
            if (validBones == null || validBones.ContainsKey(0))
                worldTransforms[0] = boneTransforms[0] * rootTransform;
            else
                    this.worldTransforms[0] = worldTransform;
            // Child bones.
            for (int bone = 1; bone < worldTransforms.Length; bone++)
            {
                int parentBone = skindata.SkeletonHierarchy[bone];

                if (validBones == null || validBones.ContainsKey(bone))
                    worldTransforms[bone] = boneTransforms[bone] * worldTransforms[parentBone];
            }
        }

        /// <summary>
        /// Helper used by the Update method to refresh the SkinTransforms data.
        /// </summary>
        public void UpdateSkinTransforms()
        {
            for (int bone = 0; bone < skinTransforms.Length; bone++)
            {
                if (validBones == null || validBones.ContainsKey(bone))
                skinTransforms[bone] = skindata.InverseBindPose[bone] * worldTransforms[bone];
            }
        }

        /// <summary>
        /// Gets the current bone transform matrices, relative to their parent bones.
        /// </summary>
        public Matrix[] GetBoneTransforms()
        {
            return boneTransforms;
        }

        /// <summary>
        /// Merge the skin transforms of the top and bottom animations
        /// </summary>
        /// <param name="Skinned transforms of the other animation player"></param>
        /// <returns></returns>
        public Matrix[] MergeTransforms( Matrix[] skinned1)
        {
            for (int i = 0; i < skinned1.Length; ++i)
            {
                if (validBones == null || !validBones.ContainsKey(i))                
                    skinTransforms[i] = skinned1[i];
                
            }
            return skinTransforms;
        }

        /// <summary>
        /// Gets the current bone transform matrices, in absolute format.
        /// </summary>
        public Matrix[] GetWorldTransforms()
        {
            return worldTransforms;
        }

        /// <summary>
        /// Gets the current bone transform matrices,
        /// relative to the skinning bind pose.
        /// </summary>
        public Matrix[] GetSkinTransforms()
        {
            return skinTransforms;
        }

        public int GetLoopCount()
        {
            return currentLoopCount;
        }

        public AnimationPackage AddAnimationPackage
        {
            set { durations = value.shareAnimation; }

        }

        public class DurationClip
        {
            public TimeSpan duration;
            public TimeSpan start;
            public int startFrame;
            public TimeSpan end;
            public int endFrame;
            public Matrix[] startPose;
        }

        /// <summary>
        /// Class used to contain the Animation subsets ( a package of animations )
        /// </summary>
        public class AnimationPackage
        {
            AnimationClip fullclip;
            float keyframes;
            int bpCount;
            int ms;
            float c;
            Dictionary<int, DurationClip> animations;

            public Dictionary<int, DurationClip> shareAnimation
            {
                get{return animations;}
            }

            public AnimationPackage(SkinningData skin, float keyframeCount)
            {
                keyframes = keyframeCount;
                fullclip = skin.AnimationClips["Take 001"];
                bpCount = skin.BindPose.Count;
                ms = (int)fullclip.Duration.TotalMilliseconds;
                c = ms / keyframeCount;
                animations = new Dictionary<int, DurationClip>();
            }

            public void AddDurationClipEasy(int name, int start, int end)
            {
                AddDurationClip(name, TimeSpan.FromMilliseconds(c * start), TimeSpan.FromMilliseconds(c * end));
            }

            private void AddDurationClip(int name, TimeSpan start, TimeSpan end)
            {

                DurationClip d = new DurationClip();
                d.start = start;
                d.startFrame = 0;
                d.end = end;
                d.endFrame = fullclip.Keyframes.Count - 1;
                d.duration = d.end - d.start;
                d.startPose = new Matrix[bpCount];

                bool searching = true;
                for (int i = 0; i < fullclip.Keyframes.Count; i++)
                {

                    if (searching && fullclip.Keyframes[i].Time >= d.start)
                    {
                        searching = false;
                        d.startFrame = i;
                    }
                    else if (fullclip.Keyframes[i].Time >= d.end)
                    {
                        d.endFrame = i;
                        break;
                    }
                    else
                    {
                        d.startPose[fullclip.Keyframes[i].Bone] = fullclip.Keyframes[i].Transform;
                    }

                }

                animations.Add(name, d);

                System.Diagnostics.Debug.WriteLine("clip: " + name + " " + d.startFrame + "->" + d.endFrame);
            }
        }
    }
}
