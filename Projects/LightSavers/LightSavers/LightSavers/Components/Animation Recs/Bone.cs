using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

///http://metlab.cse.msu.edu/betterskinned.html
///Bound Under MPL License

namespace LightSavers.Components.Animation_recs
{
    /// <summary>
    /// This bone class allows more detail to be associated with typical bones
    /// Reference : http://metlab.cse.msu.edu/betterskinned.html
    /// </summary>
    public class Bone
    {
        #region Fields

        //Stores the bones parent
        private Bone parent = null;

        //a list of this bones children
        private List<Bone> children = new List<Bone>();

        //This transform stores the original transform from the default (T pose) of the model - used as a point of reference
        private Matrix bindTransform = Matrix.Identity;

        //Bind scaling component that will be extracted from the bind transform
        //This means there is no support for animating a limb expanding (although I believe we could do that ourselves outside of things - should be fairly easy to implement)
        private Vector3 bindScale = Vector3.One;

        //stores the translation of the bone
        private Vector3 translation = Vector3.Zero;

        //Store the rotation of the bone as a Quartoniun - (Gimbal lock go diaf!)
        private Quaternion rotation = Quaternion.Identity;

        //Scaling applied to the bone - I'm not sure how this compares to bind scale
        private Vector3 scale = Vector3.One;

        #endregion

        #region Properties

        //Bones name
        public string Name = "";

        public Matrix BindTransform { get { return bindTransform; } }

        //?
        //His code mentions that this is the inverse of absolute bind transform for skinning - whatever that means
        public Matrix SkinTransform{get; set; }

        public Quaternion Rotation { get { return rotation; } set { rotation = value; } }

        public Vector3 Translation { get { return translation; } set { translation = value; } }

        public Vector3 Scale { get { return scale; } set { scale = value; } }

        public Bone Parent { get { return parent; } }

        public List<Bone> Children { get { return children; } }

        //?
        //Note Quite sure what this is used for I think this may have to do with how the parent has been transformed in global space ( positioned in the world )
        public Matrix AbsoluteTransform = Matrix.Identity;

        #endregion

        //Constructors etc:
        #region Operations
        public Bone(string name, Matrix bindTransform, Bone parent)
        {
            this.Name = name; this.parent = parent;

            //Add this Bone to its parent's children
            if (parent != null)
            {
                parent.children.Add(this);
            }

            //Remove Scaling from the animation!
            this.bindScale = new Vector3(bindTransform.Right.Length(),
            bindTransform.Up.Length(), bindTransform.Backward.Length());
            bindTransform.Right = bindTransform.Right / bindScale.X;
            bindTransform.Up = bindTransform.Up / bindScale.Y;
            bindTransform.Backward = bindTransform.Backward / bindScale.Y;
            this.bindTransform = bindTransform;

            //?
            // Set the skinning bind transform
            // That is the inverse of the absolute transform in the bind pose
            ComputeAbsoluteTransform();
            SkinTransform = Matrix.Invert(AbsoluteTransform);
        }

        //Computes the absolute transform - this works out where the bone is and what scale it is etc:
        //eg: it calculates the matrix that will the scale the bone set by (scale and the bindscale), the rotation and translation relative to the Bind Transform
        public void ComputeAbsoluteTransform()
        {
          Matrix transform=  Matrix.CreateScale(scale * bindScale) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Translation) * BindTransform;

          if (Parent != null)
          {
              //If this bone has a parent (eg if this a hand connected to an arm, it must be moved as such), thus it will be concatenated onto the parent bones transform
              AbsoluteTransform = transform * Parent.AbsoluteTransform;
          }
          else
          {   //if this is the root node, it's transform is the absolute
              AbsoluteTransform = transform;
          }
        }

        /// <summary>
        ///? - used for animation
        ///Apparently sets the rotation and translation such that the rotation times the translation times the bind after set equals this matrix - used to set animation values apparently?
        /// </summary>
        /// <param name="m">A amtrix that includes a translation and a rotation</param>
        public void SetCompleteTransform(Matrix m)
        {

            Matrix setTo = m * Matrix.Invert(BindTransform);

            Translation = setTo.Translation;
            Rotation = Quaternion.CreateFromRotationMatrix(setTo);
        }

        #endregion


    }
}
