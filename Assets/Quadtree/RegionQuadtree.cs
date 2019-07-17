using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Quadtree {
    public class RegionQuadtree<T> : IQuadtree<T> {

        private readonly uint depth;
        private readonly uint maximumDepth;
        private readonly uint bucketSize;
        private readonly QuadtreeRegion region;
        private readonly RegionQuadtree<T>[] children = new RegionQuadtree<T>[(int) QuadtreeQuadrant.NumberOfQuadrants];
        private readonly Dictionary<Vector2D, T> data = new Dictionary<Vector2D, T> ();

        public RegionQuadtree (uint maximumDepth, uint bucketSize, QuadtreeRegion region) : this (0, maximumDepth, bucketSize, region) { }

        private RegionQuadtree (uint depth, uint maximumDepth, uint bucketSize, QuadtreeRegion region) {
            this.depth = depth;
            this.maximumDepth = maximumDepth;
            this.bucketSize = bucketSize;
            this.region = region;
        }

        public bool InsertPoint (Vector2D point, T pointData) {
            if (IsLeaf) {
                if (!region.ContainsPoint (point) && data.ContainsKey (point)) {
                    return false;
                }

                data.Add (point, pointData);
                if (data.Count == bucketSize && depth != maximumDepth) {
                    Subdivide ();
                }
                return true;
            } else {
                return InsertInChild (point, pointData);
            }
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private bool InsertInChild (Vector2D point, T pointData) {
            foreach (RegionQuadtree<T> child in children) {
                if (child.Region.ContainsPoint (point)) {
                    return child.InsertPoint (point, pointData);
                }
            }
            return false;
        }

        public bool ContainsPoint (Vector2D point) {
            if (!region.ContainsPoint (point)) {
                return false;
            }

            if (IsLeaf) {
                return data.ContainsKey (point);
            } else {
                foreach (RegionQuadtree<T> child in children) {
                    if (child.ContainsPoint (point)) {
                        return true;
                    }
                }
                return false;
            }
        }

        public void Subdivide () {
            for (QuadtreeQuadrant quadrant = QuadtreeQuadrant.NorthEast; quadrant < QuadtreeQuadrant.NumberOfQuadrants; ++quadrant) {
                QuadtreeRegion childRegion = CalculateChildRegion (quadrant);
                children[(int) quadrant] = new RegionQuadtree<T> (depth + 1, maximumDepth, bucketSize, childRegion);
            }

            foreach (KeyValuePair<Vector2D, T> entry in data) {
                InsertInChild (entry.Key, entry.Value);
            }
            data.Clear ();
        }

        private QuadtreeRegion CalculateChildRegion (QuadtreeQuadrant quadrant) {
            Vector2D childHalfRegion = .5f * region.halfRegionSize;
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

        public IQuadtree<T> GetChild (QuadtreeQuadrant quadrant) {
            if (quadrant == QuadtreeQuadrant.NumberOfQuadrants) {
                throw new System.Exception ("Argument cannot be \"NumberOfRegions\"");
            }

            if (IsLeaf) {
                throw new System.Exception ("Quadtree is leaf");
            }

            return children[(int) quadrant];
        }

        public void GetLeafNodes (List<IQuadtree<T>> outputList) {
            if (IsLeaf) {
                outputList.Add (this);
            } else {
                foreach (IQuadtree<T> child in children) {
                    child.GetLeafNodes (outputList);
                }
            }
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
                region.Equals (otherQuadtree.region);
        }

        public override int GetHashCode () {
            return (bucketSize, data, depth, IsLeaf, region).GetHashCode ();
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
            if (IsLeaf) {
                foreach (KeyValuePair<Vector2D, T> entry in data) {
                    str += string.Format ("[{0} => {1}]; ", entry.Key, entry.Value);
                }
            }

            return str + " }";
        }

        private string PrintChildren () {
            string str = "No children";
            if (!IsLeaf) {
                str = "";
                foreach (RegionQuadtree<T> child in children) {
                    str += string.Format ("\n{0}", child.ToString ());
                }
                str += PrintTabs ();
            }

            return str + "]";
        }

        /*
        Properties
         */

        public bool IsLeaf {
            get => children[0] == null;
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

        public ReadOnlyDictionary<Vector2D, T> Data {
            get => new ReadOnlyDictionary<Vector2D, T> (data);
        }
    }
}