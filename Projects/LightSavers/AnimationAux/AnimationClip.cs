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
    //Animation Clips store a form of keyframes for use with associated bones
    
    public class AnimationClip
    {
        // Each keyframe stores a rotation and translation for a moment in time
        #region Keyframe Nested Class:
        public class Keyframe
        {
            //? keyframe time
            public double Time;
            public Quaternion Rotation;//rotation for the bone
            public Vector3 Translation;//translation for the bone

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
        #endregion

       //? Keyframes are grouped per bones (apparently?)
        #region Bones
        public class Bone
        {
            //? Each bone has a name that is associated witha  run time model - not sure why/ what this means now
            private string name = "";

            //The keyframes for this bone
            private List<Keyframe> keyframes = new List<Keyframe>();

            public string Name { get { return name; } set { name = value; } }

            public List<Keyframe> Keyframes { get { return keyframes; } }

        }
        #endregion

        //The bones for this animation
        private List<Bone> bones = new List<Bone>();

        //name of the animation clip
        public string Name;

        //Length of the animation clip
        public double Duration;

        //return them bones that be in this animation clip
        public List<Bone> Bones { get { return bones; } }

    }

}
