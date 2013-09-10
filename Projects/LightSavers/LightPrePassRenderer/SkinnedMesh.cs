//-----------------------------------------------------------------------------
// SkinnedMesh.cs
//
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;

namespace LightPrePassRenderer
{
    public class SkinnedMesh :  Mesh
    {
        private Matrix[] _boneMatrixes;
        private SkinningData _skinningData;
 
        public override Model Model
        {
            set
            {
                base.Model = value;
                MeshMetadata metadata = _model.Tag as MeshMetadata;
                _skinningData = metadata.SkinningData;
                _boneMatrixes = _skinningData.BindPose.ToArray();
            }
        }
        
        public SkinningData SkinningData
        {
            get { return _skinningData; }
        }

        public override Matrix[] BoneMatrixes
        {
            get { return _boneMatrixes; }
            set { _boneMatrixes = value; }
        }

    }
}
