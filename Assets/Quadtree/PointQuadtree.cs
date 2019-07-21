using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Quadtree {

    public class PointQuadtree<T> : IQuadtree<T> {

        private static QVector2D lastPointInserted;
        private static bool subdividing;
        
        private readonly uint depth;
        private readonly uint maximumDepth;
        private readonly QRegion region;
        private readonly PointQuadtree<T>[] children = new PointQuadtree<T>[4];
        private readonly Dictionary<QVector2D, T> data = new Dictionary<QVector2D, T> ();

        public PointQuadtree (uint maximumDepth, QRegion region) : this (0, maximumDepth, region) { }

        private PointQuadtree (uint depth, uint maximumDepth, QRegion region) {
            this.depth = depth;
            this.maximumDepth = maximumDepth;
            this.region = region;
        }

        public bool InsertPoint (QVector2D point, T pointData) {
            if (IsLeaf) {
                if (!region.ContainsPoint (point) || data.ContainsKey (point)) {
                    return false;
                }

                data.Add (point, pointData);
                lastPointInserted = point;
                if (depth != maximumDepth && !subdividing) {
                    Subdivide ();
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

        [MethodImpl (MethodImplOptions.AggressiveInlining)] public bool ContainsPoint (QVector2D point) {
            if (IsLeaf) {
                return data.ContainsKey (point);
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
            subdividing = true;

            children[0] = new PointQuadtree<T> (
                depth + 1,
                maximumDepth,
                CalculateChildRegion ((QQuadrant) 0)
            );
            children[1] = new PointQuadtree<T> (
                depth + 1,
                maximumDepth,
                CalculateChildRegion ((QQuadrant) 1)
            );
            children[2] = new PointQuadtree<T> (
                depth + 1,
                maximumDepth,
                CalculateChildRegion ((QQuadrant) 2)
            );
            children[3] = new PointQuadtree<T> (
                depth + 1,
                maximumDepth,
                CalculateChildRegion ((QQuadrant) 3)
            );

            foreach (KeyValuePair<QVector2D, T> entry in data) {
                InsertInChild (entry.Key, entry.Value);
            }
            data.Clear ();

            subdividing = false;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)] private QRegion CalculateChildRegion (QQuadrant quadrant) {
            QVector2D childHalfRegion = CalculateChildHalfSize (quadrant);

            return new QRegion (
                CalculateChildCenter (quadrant, childHalfRegion),
                childHalfRegion
            );
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)] private QVector2D CalculateChildHalfSize (QQuadrant quadrant) {
            QVector2D quadrantsCorner = GetQuadrantsCorner (quadrant);

            float xComponent = .5f * (Math.Max (lastPointInserted.x, quadrantsCorner.x) -
                Math.Min (lastPointInserted.x, quadrantsCorner.x));
            float yComponent = .5f * (Math.Max (lastPointInserted.y, quadrantsCorner.y) -
                Math.Min (lastPointInserted.y, quadrantsCorner.y));

            return new QVector2D (xComponent, yComponent);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)] private QVector2D GetQuadrantsCorner (QQuadrant quadrant) {
            switch (quadrant) {
                case QQuadrant.NorthEast:
                    return region.rightUpperCorner;
                case QQuadrant.SouthEast:
                    return region.rightLowerCorner;
                case QQuadrant.SouthWest:
                    return region.leftLowerCorner;
                case QQuadrant.NorthWest:
                    return region.leftUpperCorner;
                default:
                    throw new System.Exception ("This point should not be reached");
            }
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)] private QVector2D CalculateChildCenter (QQuadrant quadrant, QVector2D childHalfRegion) {
            float xSign = (quadrant.XComponentIsPositive ()) ? 1 : -1;
            float ySign = (quadrant.YComponentIsPositive ()) ? 1 : -1;
            return new QVector2D (
                lastPointInserted.x + xSign * childHalfRegion.x,
                lastPointInserted.y + ySign * childHalfRegion.y
            );
        }

        public IQuadtree<T> GetChild (QQuadrant quadrant) {
            if (quadrant == QQuadrant.NumberOfQuadrants) {
                throw new System.Exception ("Argument cannot be \"NumberOfRegions\"");
            }

            if (IsLeaf) {
                throw new System.Exception ("Quadtree is leaf");
            }

            return children[(int) quadrant];;
        }

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

            PointQuadtree<T> otherQuadtree = (PointQuadtree<T>) obj;
            return data.Equals (otherQuadtree.data) &&
                depth == otherQuadtree.depth &&
                region.Equals (otherQuadtree.region);
        }

        public override int GetHashCode () {
            return (data, depth, IsLeaf, region).GetHashCode ();
        }

        public override string ToString () {
            string tabs = PrintTabs ();
            return string.Format ("{0}PQ. Depth: {1}. Region: [{2}].\n{3}Data: {4}.\n{5}Children: {6}\n",
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
            if (IsLeaf) {
                foreach (KeyValuePair<QVector2D, T> entry in data) {
                    dataString.AppendFormat ("[{0} => {1}]; ", entry.Key, entry.Value);
                }
            }
            dataString.Append (" }");

            return dataString.ToString ();
        }

        private string PrintChildren () {
            if (!IsLeaf) {
                StringBuilder childrenString = new StringBuilder ("[");
                foreach (IQuadtree<T> child in children) {
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
            get => new ReadOnlyDictionary<QVector2D, T> (data);
        }

        #endregion

    }
}