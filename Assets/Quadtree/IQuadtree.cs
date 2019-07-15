using System.Collections.Generic;

namespace Quadtree {
    public interface IQuadtree<T> {
        bool InsertPoint (Vector2D point, T data);
        bool ContainsPoint (Vector2D point);
        void Subdivide ();
        List<IQuadtree<T>> GetLeafNodes ();

        bool HasChildren {
            get;
        }

        uint Depth {
            get;
        }

        uint BucketSize {
            get;
        }

        QuadtreeRegion Region {
            get;
        }

        IQuadtree<T>[] Children {
            get;
        }

        Dictionary<Vector2D, T> Data {
            get;
        }
    }
}