using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quadtree {

    public interface IQuadtree<T> {

        bool InsertPoint (Vector2D point, T data);
        bool ContainsPoint (Vector2D point);
        bool RegionContainsPoint (Vector2D point);
        void Subdivide ();
        IQuadtree<T> GetChild (QuadtreeQuadrant quadrant);
        void GetLeafNodes (List<IQuadtree<T>> outputList);

        bool IsLeaf {
            get;
        }

        uint Depth {
            get;
        }

        uint MaximumDepth {
            get;
        }

        uint BucketSize {
            get;
        }

        QuadtreeRegion Region {
            get;
        }

        ReadOnlyDictionary<Vector2D, T> Data {
            get;
        }
    }
}