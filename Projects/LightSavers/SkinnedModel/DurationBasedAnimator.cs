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

        private Matrix[] boneTransforms;
        private Matrix[] worldTransforms;
        private Matrix[] skinTransforms;

        private Dictionary<String, DurationClip> durations;

        DurationClip currentDurationClip;
        int currentKeyFrame;
        TimeSpan startTimeValue, endTimeValue, currentTimeValue;

        public DurationBasedAnimator(SkinningData skin, AnimationClip clip)
        {
            // store skin data
            skindata = skin;

            // store the long clip
            fullclip = clip;

            // set up empty matrices
            boneTransforms = new Matrix[skin.BindPose.Count];
            worldTransforms = new Matrix[skin.BindPose.Count];
            skinTransforms = new Matrix[skin.BindPose.Count];
            
            durations = new Dictionary<string, DurationClip>();

            //AddDurationClip("Take 001", TimeSpan.Zero, fullclip.Duration);
            //StartClip("Take 001");

        }

        

        public void StartClip(String name)
        {
            currentDurationClip = durations[name];
            currentTimeValue = currentDurationClip.start;
            currentKeyFrame = currentDurationClip.startFrame;
            currentDurationClip.startPose.CopyTo(boneTransforms, 0);
        }

        public void Update(TimeSpan time, bool relativeToCurrentTime, Matrix rootTransform)
        {
            UpdateBoneTransforms(time, relativeToCurrentTime);
            UpdateWorldTransforms(rootTransform);
            UpdateSkinTransforms();
        }

        public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
        {
            // Update the animation position.
            if (relativeToCurrentTime)
            {
                time += currentTimeValue;

                // If we reached the end, loop back to the start.
                while (time >= currentDurationClip.end) time -= currentDurationClip.duration;
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

                // Use this keyframe.
                boneTransforms[keyframe.Bone] = keyframe.Transform;

                currentKeyFrame++;
            }
        }


        /// <summary>
        /// Helper used by the Update method to refresh the WorldTransforms data.
        /// </summary>
        public void UpdateWorldTransforms(Matrix rootTransform)
        {
            // Root bone.
            worldTransforms[0] = boneTransforms[0] * rootTransform;

            // Child bones.
            for (int bone = 1; bone < worldTransforms.Length; bone++)
            {
                int parentBone = skindata.SkeletonHierarchy[bone];

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

        public class AnimationPackage
        {
            AnimationClip fullclip;
            float keyframes;
            int bpCount;
            int ms;
            float c;
            Dictionary<string, DurationClip> animations;

            public Dictionary<string, DurationClip> shareAnimation
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
                animations = new Dictionary<string, DurationClip>();
            }

            public void AddDurationClipEasy(string name, int start, int end)
            {
                AddDurationClip(name, TimeSpan.FromMilliseconds(c * start), TimeSpan.FromMilliseconds(c * end));
            }

            private void AddDurationClip(String name, TimeSpan start, TimeSpan end)
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
