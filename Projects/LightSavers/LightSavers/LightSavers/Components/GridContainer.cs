using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components
{
    public class GridContainer
    {
        public const int HEIGHT = 6;

        /// <summary>
        /// level number,
        /// 0 - root,
        /// 1 - 4ths,
        /// 2 - 16ths
        /// </summary>
        private int level;

        /// <summary>
        /// min = bottom corner,
        /// cen = middle,
        /// max = top corner
        /// </summary>
        private Vector3 min, cen, max;

        /// <summary>
        /// All of the objects in this level that dont fit in a specific sub level
        /// </summary>
        private List<AGridContainable> contained;

        /// <summary>
        /// Is this container subdivided
        /// </summary>
        private bool hasSubcontainers;

        /// <summary>
        /// The 4 subcontainers
        /// </summary>
        private GridContainer[,] subcontainers;

        #region SECTION: Accesors
        public Vector3 Min { get { return min; } }
        public Vector3 Max { get { return max; } }
        public BoundingBox BoundingBox { get { return new BoundingBox(min, max); } }
        #endregion

        /// <summary>
        /// Construct a container for all objects that fit inside
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="level"></param>
        public GridContainer(Vector3 min, Vector3 max, int level)
        {
            this.min = min;
            this.max = max;

            // ensure these values
            this.min.Y = 0;
            this.max.Y = HEIGHT;

            this.cen = (min + max) / 2;
            this.contained = new List<AGridContainable>();
            this.level = level;
            this.hasSubcontainers = false;
            if (level > 0)
            {
                hasSubcontainers = true;
                subcontainers = new GridContainer[2, 2];
                subcontainers[0, 0] = new GridContainer(new Vector3(min.X, min.Y, min.Z), new Vector3(cen.X, max.Y, cen.Z), level - 1); 
                subcontainers[0, 1] = new GridContainer(new Vector3(cen.X, min.Y, min.Z), new Vector3(max.X, max.Y, cen.Z), level - 1);
                subcontainers[1, 0] = new GridContainer(new Vector3(min.X, min.Y, cen.Z), new Vector3(cen.X, max.Y, max.Z), level - 1); 
                subcontainers[1, 1] = new GridContainer(new Vector3(cen.X, min.Y, cen.Z), new Vector3(max.X, max.Y, max.Z), level - 1);
            }
        }

        /// <summary>
        /// Does the given GameObject actually fit inside the container
        /// </summary>
        /// <param name="go"></param>
        /// <returns>Does it fit</returns>
        public bool Fits(AGridContainable go)
        {
            BoundingBox bb = go.GetBoundingBox();
            if (bb.Min.X < this.min.X) return false;
            if (bb.Min.Y < this.min.Y) return false;
            if (bb.Max.X > this.max.X) return false;
            if (bb.Max.Y > this.max.Y) return false;
            return true;
        }

        /// <summary>
        /// Add the given GameObject to the container.
        /// </summary>
        /// <param name="go">the gameobject to add</param>
        public void Add(AGridContainable go)
        {
            if (!hasSubcontainers)
            {
                contained.Add(go);
                go.SetParentContainer(this);
            }
            else
            {
                if (subcontainers[0, 0].Fits(go))
                {
                    subcontainers[0, 0].Add(go);
                }
                else if (subcontainers[0, 1].Fits(go))
                {
                    subcontainers[0, 1].Add(go);
                }
                else if (subcontainers[1, 0].Fits(go))
                {
                    subcontainers[1, 0].Add(go);
                }
                else if (subcontainers[1, 1].Fits(go))
                {
                    subcontainers[1, 1].Add(go);
                }
                else
                {
                    contained.Add(go);
                    go.SetParentContainer(this);
                }
            }
        }

        /// <summary>
        /// Remove the given gameobject from the container
        /// </summary>
        /// <param name="go"></param>
        public void Remove(AGridContainable go)
        {
            if (go.GetParentContainer() == null) return;
                        
            go.GetParentContainer().contained.Remove(go);
            go.SetParentContainer(null);
        }
    
        public void BuildIntersectList(BoundingBox bb, List<AGridContainable> output)
        {
            foreach (AGridContainable a in contained)
            {
                if (a.GetBoundingBox().Intersects(bb))
                {
                    output.Add(a);
                }
            }

            if (subcontainers[0, 0].BoundingBox.Intersects(bb)) subcontainers[0, 0].BuildIntersectList(bb, output);
            if (subcontainers[0, 1].BoundingBox.Intersects(bb)) subcontainers[0, 1].BuildIntersectList(bb, output);
            if (subcontainers[1, 0].BoundingBox.Intersects(bb)) subcontainers[1, 0].BuildIntersectList(bb, output);
            if (subcontainers[1, 1].BoundingBox.Intersects(bb)) subcontainers[1, 1].BuildIntersectList(bb, output);
        }

        public void BuildIntersectList(BoundingFrustum bf, List<AGridContainable> output)
        {
            foreach (AGridContainable a in contained)
            {
                if (a.GetBoundingBox().Intersects(bf))
                {
                    output.Add(a);
                }
            }

            if (subcontainers[0, 0].BoundingBox.Intersects(bf)) subcontainers[0, 0].BuildIntersectList(bf, output);
            if (subcontainers[0, 1].BoundingBox.Intersects(bf)) subcontainers[0, 1].BuildIntersectList(bf, output);
            if (subcontainers[1, 0].BoundingBox.Intersects(bf)) subcontainers[1, 0].BuildIntersectList(bf, output);
            if (subcontainers[1, 1].BoundingBox.Intersects(bf)) subcontainers[1, 1].BuildIntersectList(bf, output);
        }

    
    }
}
