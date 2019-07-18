using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quadtree {

    /// <summary>
    /// Interface for any quadtree implementation.
    /// </summary>
    public interface IQuadtree<T> {

        /// <summary>
        /// Inserts a point in the quadtree.
        /// </summary>
        /// <returns><c>true</c>, if the point was inserted, <c>false</c> if the point does not belong to this region
        /// or if the quadtree already contains this point.</returns>
        /// <param name="point">A point.</param>
        /// <param name="data">The data associated with this point.</param>
        bool InsertPoint (QVector2D point, T data);

        /// <summary>
        /// Checks if the quadtree already stores the given point.
        /// </summary>
        /// <returns><c>true</c>, if point was contained, <c>false</c> otherwise.</returns>
        /// <param name="point">A point.</param>
        bool ContainsPoint (QVector2D point);

        /// <summary>
        /// Subdivides the quadtree in four new child quadtrees.
        /// </summary>
        void Subdivide ();

        /// <summary>
        /// Returns a child of this quadtree.
        /// </summary>
        /// <returns>The requested child.</returns>
        /// <param name="quadrant">A quadrant.</param>
        IQuadtree<T> GetChild (QQuadrant quadrant);
        void GetLeafNodes (List<IQuadtree<T>> outputList);

        /// <summary>
        /// Check whether this <see cref="T:Quadtree.IQuadtree`1"/> is a leaf node.
        /// </summary>
        /// <value><c>true</c> if is leaf node; otherwise, <c>false</c>.</value>
        bool IsLeaf {
            get;
        }

        /// <summary>
        /// Returns the depth of this node. Depth 0 corresponds to root's depth.
        /// </summary>
        /// <value>The depth of this quadtree.</value>
        uint Depth {
            get;
        }

        /// <summary>
        /// Returns the maximum depth the quadtree may reach. Upon reaching this depth, the quadtree will no
        /// longer subdivide.
        /// </summary>
        /// <value>The maximum depth of the quadtree.</value>
        uint MaximumDepth {
            get;
        }

        /// <summary>
        /// Gets the size of the quadtree's bucket. Once the quadtree stores as many points as its bucket size,
        /// the quadtree will subdivide.
        /// </summary>
        /// <value>The size of the quadtree's bucket.</value>
        uint BucketSize {
            get;
        }

        /// <summary>
        /// Gets the region associated with this quadtree.
        /// </summary>
        /// <value>The quadtree's region.</value>
        QRegion Region {
            get;
        }

        /// <summary>
        /// Retrieves the points and their related data.
        /// </summary>
        /// <value>A read-only dictionary containing the data. Only leaf nodes may contain data.</value>
        ReadOnlyDictionary<QVector2D, T> Data {
            get;
        }
    }
}