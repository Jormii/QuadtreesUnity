using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Quadtree {

    public class RegionQuadtree<T> : IQuadtree<T> {

        private static QVector2D subdivisionPoint;
        private static T subdivisionPointData;

        private readonly uint depth;
        private readonly uint maximumDepth;
        private readonly QRegion region;
        private readonly RegionQuadtree<T>[] children = new RegionQuadtree<T>[4];
        private readonly HashSet<QVector2D> points = new HashSet<QVector2D> ();
        private readonly T data;

        public RegionQuadtree (uint maximumDepth, QRegion region, T regionData) : this (0, maximumDepth, region, regionData) { }

        private RegionQuadtree (uint depth, uint maximumDepth, QRegion region, T regionData) {
            this.depth = depth;
            this.maximumDepth = maximumDepth;
            this.region = region;
            this.data = regionData;
        }

        public bool InsertPoint (QVector2D point, T pointData) {
            if (IsLeaf) {
                if (!region.ContainsPoint (point) || points.Contains (point)) {
                    return false;
                }

                if (!data.Equals (pointData) && depth != maximumDepth) {
                    subdivisionPoint = point;
                    subdivisionPointData = pointData;
                    Subdivide ();
                } else {
                    points.Add (point);
                }
                return true;
            }

            return InsertInChild (point, pointData);
        }

        /// <summary>
        /// Inserts the given point in the corresponding child.
        /// </summary>
        /// <returns>True, if the point was inserted, False otherwise.</returns>
        /// <param name="point">The point to insert.</param>
        /// <param name="pointData">The data associated with this point.</param>
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
                return points.Contains (point);
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
            QRegion child0Region = CalculateChildRegion ((QQuadrant) 0);
            children[0] = new RegionQuadtree<T> (
                depth + 1,
                maximumDepth,
                child0Region,
                CalculateChildData (child0Region)
            );
            QRegion child1Region = CalculateChildRegion ((QQuadrant) 1);
            children[1] = new RegionQuadtree<T> (
                depth + 1,
                maximumDepth,
                child1Region,
                CalculateChildData (child1Region)
            );
            QRegion child2Region = CalculateChildRegion ((QQuadrant) 2);
            children[2] = new RegionQuadtree<T> (
                depth + 1,
                maximumDepth,
                child2Region,
                CalculateChildData (child2Region)
            );
            QRegion child3Region = CalculateChildRegion ((QQuadrant) 3);
            children[3] = new RegionQuadtree<T> (
                depth + 1,
                maximumDepth,
                child3Region,
                CalculateChildData (child3Region)
            );

            foreach (QVector2D point in points) {
                InsertInChild (point, data);
            }
            points.Clear ();
            InsertInChild (subdivisionPoint, subdivisionPointData);
        }

        /// <summary>
        /// Calculates the region of the child associated to the given quadrant.
        /// </summary>
        /// <returns>The child region.</returns>
        /// <param name="quadrant">The quadrant of the child.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private QRegion CalculateChildRegion (QQuadrant quadrant) {
            QVector2D childHalfRegion = .5f * region.halfRegionSize;

            return new QRegion (
                CalculateChildCenter (quadrant, childHalfRegion),
                childHalfRegion
            );
        }

        /// <summary>
        /// Calculates the center of the child's region.
        /// </summary>
        /// <returns>The child's region center.</returns>
        /// <param name="quadrant">The quadrant of the child.</param>
        /// <param name="childHalfRegion">Child's region's half size.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private QVector2D CalculateChildCenter (QQuadrant quadrant, QVector2D childHalfRegion) {
            float xSign = (quadrant.XComponentIsPositive ()) ? 1 : -1;
            float ySign = (quadrant.YComponentIsPositive ()) ? 1 : -1;
            return new QVector2D (
                region.center.x + xSign * childHalfRegion.x,
                region.center.y + ySign * childHalfRegion.y
            );
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private T CalculateChildData (QRegion childRegion) {
            return (childRegion.ContainsPoint (subdivisionPoint)) ? subdivisionPointData : data;
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
            return points.Equals (otherQuadtree.points) &&
                data.Equals (otherQuadtree.data) &&
                depth == otherQuadtree.depth &&
                region.Equals (otherQuadtree.region);
        }

        public override int GetHashCode () {
            return (data, points, depth, IsLeaf, region).GetHashCode ();
        }

        public override string ToString () {
            string tabs = PrintTabs ();
            return string.Format ("{0}RQ. Depth: {1}. Region: [{2}].\n{3}Data: {4}.\n{5}Children: {6}\n",
                tabs, depth, region, tabs, PrintData (), tabs, PrintChildren ());
        }

        private string PrintTabs () {
            StringBuilder tabs = new StringBuilder ();
            for (int i = 0; i < depth; ++i) {
                tabs.Append ('\t');
            }

            return tabs.ToString ();
        }

        private string PrintData () {
            StringBuilder dataString = new StringBuilder ("{ ");
            dataString.AppendFormat ("{0}. Points: [ ", data);
            foreach (QVector2D point in points) {
                dataString.AppendFormat ("{0}, ", point);
            }
            dataString.Append ("] }");

            return dataString.ToString ();
        }

        private string PrintChildren () {
            if (!IsLeaf) {
                StringBuilder childrenString = new StringBuilder ("[");
                foreach (RegionQuadtree<T> child in children) {
                    childrenString.AppendFormat ("\n{0}", child);
                }
                childrenString.Append (PrintTabs ());
                childrenString.Append ("]");
                return childrenString.ToString ();
            }

            return "No children";
        }

        #region Properties

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
            get =>
                throw new System.Exception ("Point quadtrees dont have a bucket size property");
        }

        public QRegion Region {
            get => region;
        }

        public ReadOnlyDictionary<QVector2D, T> Data {
            get {
                Dictionary<QVector2D, T> dictionaryData = new Dictionary<QVector2D, T> ();
                foreach (QVector2D point in points) {
                    dictionaryData.Add (point, data);
                }
                return new ReadOnlyDictionary<QVector2D, T> (dictionaryData);
            }
        }

        #endregion

    }

}