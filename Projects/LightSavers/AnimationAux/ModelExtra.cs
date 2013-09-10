using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///http://metlab.cse.msu.edu/betterskinned.html
///Bound Under MPL License

namespace AnimationAux
{
    /// <summary>
    /// ? This class adds some aditional stuff to the model class apparently (shared with the run time - whatever that means)
    /// </summary>
    public class ModelExtra
    {

        #region Fields
        //bone indices associated with a skinned model
        private List<int> skeleton = new List<int>();

        //Any associated animation clips
        public List<AnimationClip> clips = new List<AnimationClip>();
        #endregion

        #region properties
        public List<int> Skeleton { get { return skeleton; } set { skeleton = value; } }

        public List<AnimationClip> Clips { get { return clips; } set { clips = value; } }
        #endregion
    }
}
