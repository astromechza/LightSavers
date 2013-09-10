using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

///http://metlab.cse.msu.edu/betterskinned.html
///Bound Under MPL License

namespace AnimationAux
{

    public class AnimationClipReader : ContentTypeReader<AnimationClip>
    {
        protected override AnimationClip Read(ContentReader input, AnimationClip existingInstance)
        {
            //create a new clip,
            AnimationClip clip = new AnimationClip();

            //set some clip attributes
            clip.Name = input.ReadString();
            
            //? still not too sure how duration is handled
            clip.Duration = input.ReadDouble();

            //iterate over every bone
            int boneCnt = input.ReadInt32();
            for (int i = 0; i < boneCnt; ++i)
            {
                AnimationClip.Bone bone = new AnimationClip.Bone();
                clip.Bones.Add(bone);
                bone.Name = input.ReadString();

                //? iterate over the number of keyframes?
                int cnt = input.ReadInt32();

                for (int j = 0; j < cnt; j++)
                {
                    //store a new keyframe object?
                    AnimationClip.Keyframe keyframe = new AnimationClip.Keyframe();
                    //? not sure as to what this time represents
                    keyframe.Time = input.ReadDouble();
                    keyframe.Rotation=input.ReadQuaternion();
                    keyframe.Translation = input.ReadVector3();
                    bone.Keyframes.Add(keyframe);

                }
            }
            return clip;
        }
    }
}
