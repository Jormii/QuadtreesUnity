using System.Collections.Generic;

namespace Quadtree {
    public class RegionQuadtree<T> : IQuadtree<T> {

        private readonly uint depth;
        private readonly uint maximumDepth;
        private readonly uint bucketSize;
        private readonly QuadtreeRegion region;
        private readonly RegionQuadtree<T>[] children;
        private readonly Dictionary<Vector2D, T> data;
        private bool hasChildren;

        public RegionQuadtree (uint maximumDepth, uint bucketSize, QuadtreeRegion region) : this (0, maximumDepth, bucketSize, region) { }

        private RegionQuadtree (uint depth, uint maximumDepth, uint bucketSize, QuadtreeRegion region) {
            this.depth = depth;
            this.maximumDepth = maximumDepth;
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
                if (data.Count == bucketSize && depth != maximumDepth) {
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
            hasChildren = true;

            for (QuadtreeQuadrant quadrant = QuadtreeQuadrant.NorthEast; quadrant < QuadtreeQuadrant.NumberOfQuadrants; ++quadrant) {
                QuadtreeRegion childRegion = CalculateChildRegion (quadrant);
                children[(int) quadrant] = new RegionQuadtree<T> (depth + 1, maximumDepth, bucketSize, childRegion);
            }

            foreach (KeyValuePair<Vector2D, T> entry in data) {
                InsertPointToRespectiveChild (entry.Key, entry.Value);
            }
            data.Clear ();
        }

        private QuadtreeRegion CalculateChildRegion (QuadtreeQuadrant quadrant) {
            Vector2D childHalfRegion = region.halfRegionSize / 2f;
            Vector2D childCenter = CalculateChildCenter (quadrant, childHalfRegion);

            return new QuadtreeRegion (childCenter, childHalfRegion);
        }

        private Vector2D CalculateChildCenter (QuadtreeQuadrant quadrant, Vector2D childHalfRegion) {
            int xSign = (quadrant.XComponentIsPositive ()) ? 1 : -1;
            int ySign = (quadrant.YComponentIsPositive ()) ? 1 : -1;
            float centerXComponent = region.center.x + xSign * childHalfRegion.x;
            float centerYComponent = region.center.y + ySign * childHalfRegion.y;
            return new Vector2D (centerXComponent, centerYComponent);
        }

        public void GetLeafNodes (List<IQuadtree<T>> outputList) {
            if (!hasChildren) {
                outputList.Add (this);
            } else {
                foreach (IQuadtree<T> child in children) {
                    child.GetLeafNodes (outputList);
                }
            }
        }

        public override string ToString () {
            string tabs = PrintTabs ();
            return string.Format ("{0}RQ. Depth: {1}. Region: [{2}].\n{3}Data: {4}.\n{5}Children: [{6}\n",
                tabs, depth, region, tabs, PrintData (), tabs, PrintChildren ());
        }

        private string PrintTabs () {
            string str = "";
            for (int i = 0; i < depth; ++i) {
                str += "\t";
            }

            return str;
        }

        private string PrintData () {
            string str = "{ ";
            if (!hasChildren) {
                foreach (KeyValuePair<Vector2D, T> entry in data) {
                    str += string.Format ("[{0} => {1}]; ", entry.Key, entry.Value);
                }
            }

            return str + " }";
        }

        private string PrintChildren () {
            string str = "No children";
            if (hasChildren) {
                str = "";
                foreach (RegionQuadtree<T> child in children) {
                    str += string.Format ("\n{0}", child.ToString ());
                }
                str += PrintTabs ();
            }

            return str + "]";
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

        public uint MaximumDepth {
            get => maximumDepth;
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