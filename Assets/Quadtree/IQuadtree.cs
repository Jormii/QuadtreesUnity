using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quadtree {

    /// <summary>
    /// Interface for any quadtree implementation.
    /// </summary>
    public interface IQuadtree<T> {

        /// <summary>
        /// Attemps to insert a point in the quadtree.
        /// </summary>
        /// <returns>True, if the point was inserted, False if the quadtree's region does not contain the point
        /// or if the quadtree already contains this point.</returns>
        /// <param name="point">The point to insert.</param>
        /// <param name="data">The data associated with this point.</param>
        bool InsertPoint (QVector2D point, T data);

        /// <summary>
        /// Checks if the quadtree already stores the given point.
        /// </summary>
        /// <returns>True, if the point was contained, False otherwise.</returns>
        /// <param name="point">The point to check its existence in the quadtree.</param>
        bool ContainsPoint (QVector2D point);

        /// <summary>
        /// Subdivides the quadtree in four new child quadtrees.
        /// </summary>
        void Subdivide ();

        /// <summary>
        /// Returns a child of this quadtree.
        /// </summary>
        /// <returns>The requested child.</returns>
        /// <param name="quadrant">The quadrant that corresponds to the desired child.</param>
        IQuadtree<T> GetChild (QQuadrant quadrant);

        /// <summary>
        /// Retrieves a list with every leaf node in the quadtree.
        /// </summary>
        /// <param name="outputList">A list that will contain the leaf nodes. This method does not clear the provided list.</param>
        void GetLeafNodes (List<IQuadtree<T>> outputList);

        /// <summary>
        /// Checks whether this quadtree is a leaf node. Read-only.
        /// </summary>
        /// <value>True, if is a leaf node; otherwise, False.</value>
        bool IsLeaf {
            get;
        }

        /// <summary>
        /// Returns the depth of this node in the quadtree. The value 0 corresponds to root's depth. Read-only.
        /// </summary>
        /// <value>The depth of this quadtree.</value>
        uint Depth {
            get;
        }

        /// <summary>
        /// Returns the maximum depth the quadtree may reach. Upon reaching this depth, the quadtree will no
        /// longer subdivide. Read-only.
        /// </summary>
        /// <value>The maximum depth the quadtree may reach.</value>
        uint MaximumDepth {
            get;
        }

        /// <summary>
        /// Gets the size of the quadtree's bucket. When the quadtree stores as many points as its bucket size,
        /// the quadtree will subdivide. Read-only.
        /// </summary>
        /// <value>The size of the quadtree's bucket.</value>
        uint BucketSize {
            get;
        }

        /// <summary>
        /// Gets the region associated with this quadtree. Read-only.
        /// </summary>
        /// <value>The quadtree's region.</value>
        QRegion Region {
            get;
        }

        /// <summary>
        /// Retrieves the points and their related data stored in this node. Read-only.
        /// </summary>
        /// <value>A read-only dictionary containing the data. Only leaf nodes may contain data.</value>
        ReadOnlyDictionary<QVector2D, T> Data {
            get;
        }
    }
}