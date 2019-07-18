using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Quadtree {
    public class RegionQuadtree<T> : IQuadtree<T> {

        private readonly uint depth;
        private readonly uint maximumDepth;
        private readonly uint bucketSize;
        private readonly QRegion region;
        private readonly RegionQuadtree<T>[] children = new RegionQuadtree<T>[(int) QQuadrant.NumberOfQuadrants];
        private readonly Dictionary<QVector2D, T> data = new Dictionary<QVector2D, T> ();

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public RegionQuadtree (uint maximumDepth, uint bucketSize, QRegion region) : this (0, maximumDepth, bucketSize, region) { }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private RegionQuadtree (uint depth, uint maximumDepth, uint bucketSize, QRegion region) {
            this.depth = depth;
            this.maximumDepth = maximumDepth;
            this.bucketSize = bucketSize;
            this.region = region;
        }

        public bool InsertPoint (QVector2D point, T pointData) {
            if (IsLeaf) {
                if (!region.ContainsPoint (point) || data.ContainsKey (point)) {
                    return false;
                }

                data.Add (point, pointData);
                if (data.Count == bucketSize && depth != maximumDepth) {
                    Subdivide ();
                }
                return true;
            }

            return InsertInChild (point, pointData);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private bool InsertInChild (QVector2D point, T pointData) {
            if (children[0].region.ContainsPoint (point)) {
                return children[0].InsertPoint (point, pointData);
            }
            if (children[1].region.ContainsPoint (point)) {
                return children[1].InsertPoint (point, pointData);
            }
            if (children[2].region.ContainsPoint (point)) {
                return children[2].InsertPoint (point, pointData);
            }
            if (children[3].region.ContainsPoint (point)) {
                return children[3].InsertPoint (point, pointData);
            }

            return false;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint (QVector2D point) {
            if (IsLeaf) {
                return region.ContainsPoint (point) && data.ContainsKey (point);
            }

            if (children[0].ContainsPoint (point)) {
                return true;
            }
            if (children[1].ContainsPoint (point)) {
                return true;
            }
            if (children[2].ContainsPoint (point)) {
                return true;
            }
            if (children[3].ContainsPoint (point)) {
                return true;
            }

            return false;
        }

        public void Subdivide () {
            children[0] = new RegionQuadtree<T> (
                depth + 1,
                maximumDepth,
                bucketSize,
                CalculateChildRegion ((QQuadrant) 0)
            );
            children[1] = new RegionQuadtree<T> (
                depth + 1,
                maximumDepth,
                bucketSize,
                CalculateChildRegion ((QQuadrant) 1)
            );
            children[2] = new RegionQuadtree<T> (
                depth + 1,
                maximumDepth,
                bucketSize,
                CalculateChildRegion ((QQuadrant) 2)
            );
            children[3] = new RegionQuadtree<T> (
                depth + 1,
                maximumDepth,
                bucketSize,
                CalculateChildRegion ((QQuadrant) 3)
            );

            foreach (KeyValuePair<QVector2D, T> entry in data) {
                InsertInChild (entry.Key, entry.Value);
            }
            data.Clear ();
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private QRegion CalculateChildRegion (QQuadrant quadrant) {
            QVector2D childHalfRegion = .5f * region.halfRegionSize;

            return new QRegion (
                CalculateChildCenter (quadrant, childHalfRegion),
                childHalfRegion
            );
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private QVector2D CalculateChildCenter (QQuadrant quadrant, QVector2D childHalfRegion) {
            int xSign = (quadrant.XComponentIsPositive ()) ? 1 : -1;
            int ySign = (quadrant.YComponentIsPositive ()) ? 1 : -1;
            return new QVector2D (
                region.center.x + xSign * childHalfRegion.x,
                region.center.y + ySign * childHalfRegion.y
            );
        }

        public IQuadtree<T> GetChild (QQuadrant quadrant) {
            if (quadrant == QQuadrant.NumberOfQuadrants) {
                throw new System.Exception ("Argument cannot be \"NumberOfRegions\"");
            }

            if (IsLeaf) {
                throw new System.Exception ("Quadtree is leaf");
            }

            return children[(int) quadrant];
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void GetLeafNodes (List<IQuadtree<T>> outputList) {
            if (IsLeaf) {
                outputList.Add (this);
            } else {
                children[0].GetLeafNodes (outputList);
                children[1].GetLeafNodes (outputList);
                children[2].GetLeafNodes (outputList);
                children[3].GetLeafNodes (outputList);
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
                foreach (KeyValuePair<QVector2D, T> entry in data) {
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
                    str += string.Format ("\n{0}", child);
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

        public QRegion Region {
            get => region;
        }

        public ReadOnlyDictionary<QVector2D, T> Data {
            get => new ReadOnlyDictionary<QVector2D, T> (data);
        }
    }
}