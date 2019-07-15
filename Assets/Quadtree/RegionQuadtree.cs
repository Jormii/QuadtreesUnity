using System.Collections.Generic;

namespace Quadtree {
    public class RegionQuadtree<T> : IQuadtree<T> {

        private readonly uint depth;
        private readonly uint bucketSize;
        private readonly QuadtreeRegion region;
        private readonly RegionQuadtree<T>[] children;
        private readonly Dictionary<Vector2D, T> data;
        private bool hasChildren;

        public RegionQuadtree (uint bucketSize, QuadtreeRegion region) : this (0, bucketSize, region) { }

        private RegionQuadtree (uint depth, uint bucketSize, QuadtreeRegion region) {
            this.depth = depth;
            this.bucketSize = bucketSize;
            this.region = region;
            this.children = new RegionQuadtree<T>[(int) QuadtreeQuadrant.NumberOfQuadrants];
            this.data = new Dictionary<Vector2D, T> ();
            this.hasChildren = false;
        }

        public bool InsertPoint (Vector2D point, T pointData) {
            if (ContainsPoint (point)) {
                return false;
            }

            if (hasChildren) {
                return InsertPointToRespectiveChild (point, pointData);
            } else {
                data.Add (point, pointData);
                if (data.Count == bucketSize) {
                    Subdivide ();
                }
                return true;
            }

            throw new System.Exception ("Error inserting point to quadtree");
        }

        private bool InsertPointToRespectiveChild (Vector2D point, T pointData) {
            RegionQuadtree<T> childToInsert = CalculateRespectiveChild (point);
            if (childToInsert == null) {
                return false;
            }

            return childToInsert.InsertPoint (point, pointData);
        }

        private RegionQuadtree<T> CalculateRespectiveChild (Vector2D point) {
            foreach (RegionQuadtree<T> child in children) {
                if (child.Region.ContainsPoint (point)) {
                    return child;
                }
            }

            return null;
        }

        public bool ContainsPoint (Vector2D point) {
            if (!region.ContainsPoint (point)) {
                return false;
            }

            if (hasChildren) {
                foreach (RegionQuadtree<T> child in children) {
                    if (child.ContainsPoint (point)) {
                        return true;
                    }
                }
                return false;
            } else {
                return data.ContainsKey (point);
            }
        }

        public void Subdivide () {
            UnityEngine.Debug.Log ("Subdividing quadtree " + ToString ());

            hasChildren = true;

            for (QuadtreeQuadrant quadrant = QuadtreeQuadrant.NorthEast; quadrant < QuadtreeQuadrant.NumberOfQuadrants; ++quadrant) {
                QuadtreeRegion childRegion = CalculateChildRegion (quadrant);
                children[(int) quadrant] = new RegionQuadtree<T> (depth + 1, bucketSize, region);

                UnityEngine.Debug.LogFormat ("{0} children's quadtree is {1}", quadrant, children[(int) quadrant]);
            }

            foreach (KeyValuePair<Vector2D, T> entry in data) {
                InsertPointToRespectiveChild (entry.Key, entry.Value);
            }
            data.Clear ();
        }

        private QuadtreeRegion CalculateChildRegion (QuadtreeQuadrant quadrant) {
            Vector2D childHalfRegion = region.HalfRegionSize / 2;
            Vector2D childCenter = CalculateChildCenter (quadrant, childHalfRegion);

            return new QuadtreeRegion (childCenter, childHalfRegion);
        }

        private Vector2D CalculateChildCenter (QuadtreeQuadrant quadrant, Vector2D childHalfRegion) {
            int xSign = (quadrant.XComponentIsPositive ()) ? 1 : -1;
            int ySign = (quadrant.YComponentIsPositive ()) ? 1 : -1;
            float centerXComponent = region.Center.X + xSign * childHalfRegion.X;
            float centerYComponent = region.Center.Y + ySign * childHalfRegion.Y;
            return new Vector2D (centerXComponent, centerYComponent);
        }

        public List<IQuadtree<T>> GetLeafNodes () {
            // TODO
            throw new System.Exception ("Quadtree::RegionQuadtree<T>::GetLeafNodes() yet to implement");
        }

        public override string ToString () {
            return string.Format ("RQ. Depth: {0}. Region: [{1}]. Data: {2}. Children [{3}]",
                depth, region, PrintData (), PrintChildren ());
        }

        private string PrintData () {
            string str = "";
            if (!hasChildren) {
                foreach (KeyValuePair<Vector2D, T> entry in data) {
                    str += string.Format ("[{0} : {1}]\t", entry.Key, entry.Value);
                }
            }

            return str;
        }

        private string PrintChildren () {
            string str = "No children";
            if (hasChildren) {
                str = "";
                foreach (RegionQuadtree<T> child in children) {
                    str += string.Format ("\t{0}\n", child.ToString ());
                }
            }

            return str;
        }

        public override bool Equals (object obj) {
            if (obj == null) {
                return false;
            }

            if (!GetType ().Equals (obj.GetType ())) {
                return false;
            }

            RegionQuadtree<T> otherQuadtree = (RegionQuadtree<T>) obj;
            return bucketSize == otherQuadtree.bucketSize &&
                data.Equals (otherQuadtree.data) &&
                depth == otherQuadtree.depth &&
                hasChildren == otherQuadtree.hasChildren &&
                region.Equals (otherQuadtree.region);
        }

        public override int GetHashCode () {
            return (bucketSize, data, depth, hasChildren, region).GetHashCode ();
        }

        /*
        Properties
         */

        public bool HasChildren {
            get => hasChildren;
        }

        public uint Depth {
            get => depth;
        }

        public uint BucketSize {
            get => bucketSize;
        }

        public QuadtreeRegion Region {
            get => region;
        }

        public IQuadtree<T>[] Children {
            get => children;
        }

        public Dictionary<Vector2D, T> Data {
            get => data;
        }
    }
}