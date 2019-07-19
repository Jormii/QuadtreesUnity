using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quadtree {

    public class PointQuadtree<T> : IQuadtree<T> {

        public bool InsertPoint (QVector2D point, T data) {
            return false;
        }

        public bool ContainsPoint (QVector2D point) {
            return false;
        }

        public void Subdivide () {

        }

        public IQuadtree<T> GetChild (QQuadrant quadrant) {
            return null;
        }

        public void GetLeafNodes (List<IQuadtree<T>> outputList) {

        }

        #region Properties

        public bool IsLeaf {
            get => false;
        }

        public uint Depth {
            get => 0;
        }

        public uint MaximumDepth {
            get => 0;
        }

        public uint BucketSize {
            get => 0;
        }

        public QRegion Region {
            get => new QRegion ();
        }

        public ReadOnlyDictionary<QVector2D, T> Data {
            get => new ReadOnlyDictionary<QVector2D, T> (new Dictionary<QVector2D, T> ());
        }

        #endregion

    }
}