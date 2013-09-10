//-----------------------------------------------------------------------------
// MeshMetadata.cs
//
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SkinnedModel;

namespace LightPrePassRenderer
{
    /// <summary>
    /// This class holds information about a processed mesh
    /// </summary>
    public class MeshMetadata
    {
        public enum ERenderQueue
        {
            Default,    
            SkipGbuffer,
            Blend,
            Count
        }

        public class SubMeshMetadata
        {
            private BoundingBox _boundingBox;
            private bool _castShadows = true;
            private ERenderQueue _renderQueue = ERenderQueue.Default;


            public BoundingBox BoundingBox
            {
                get { return _boundingBox; }
                set { _boundingBox = value; }
            }

            public bool CastShadows
            {
                get { return _castShadows; }
                set { _castShadows = value; }
            }

            public ERenderQueue RenderQueue
            {
                get { return _renderQueue; }
                set { _renderQueue = value; }
            }
        } 
        
        private List<SubMeshMetadata> _subMeshesMetadata = new List<SubMeshMetadata>();
        private SkinningData _skinningData;
        private BoundingBox _boundingBox;
        
        public void AddSubMeshMetadata(SubMeshMetadata metadata)
        {
            _subMeshesMetadata.Add(metadata);
        }

        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
            set { _boundingBox = value; }
        }

        public SkinningData SkinningData
        {
            get { return _skinningData; }
            set { _skinningData = value; }
        }

        public List<SubMeshMetadata> SubMeshesMetadata
        {
            get { return _subMeshesMetadata; }
        }
    }
}
