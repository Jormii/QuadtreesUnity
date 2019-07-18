using System.Runtime.CompilerServices;

namespace Quadtree {

    /// <summary>
    /// Represents a region of the quadtree. A region is represented by its center and the half of its size.
    /// </summary>
    public struct QRegion {

        /// <summary>
        /// The center of the region. Read-only.
        /// </summary>
        public readonly QVector2D center;

        /// <summary>
        /// The half of the size of the region. This is equivalent to the full size of a region's child. Read-only.
        /// </summary>
        public readonly QVector2D halfRegionSize;

        /// <summary>
        /// The upper left corner of the region. Read-only.
        /// </summary>
        public readonly QVector2D leftUpperCorner;

        /// <summary>
        /// The right lower corner of the region. Read-only.
        /// </summary>
        public readonly QVector2D rightLowerCorner;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Quadtree.QRegion"/> struct.
        /// </summary>
        /// <param name="center">The center of the region.</param>
        /// <param name="halfRegionSize">The half size of the region.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public QRegion (QVector2D center, QVector2D halfRegionSize) {
            this.center = center;
            this.halfRegionSize = halfRegionSize;

            this.leftUpperCorner = new QVector2D (
                center.x - halfRegionSize.x,
                center.y + halfRegionSize.y
            );

            this.rightLowerCorner = new QVector2D (
                center.x + halfRegionSize.x,
                center.y - halfRegionSize.y
            );
        }

        /// <summary>
        /// Checks if the given point is contained in the region.
        /// </summary>
        /// <returns><c>true</c>, if point was contained, <c>false</c> otherwise.</returns>
        /// <param name="point">A point.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint (QVector2D point) {
            return point.x >= leftUpperCorner.x && point.x <= rightLowerCorner.x &&
                point.y <= leftUpperCorner.y && point.y >= rightLowerCorner.y;
        }

        public override bool Equals (object obj) {
            if (obj == null) {
                return false;
            }

            if (!GetType ().Equals (obj.GetType ())) {
                return false;
            }

            QRegion otherRegion = (QRegion) obj;
            return center.Equals (otherRegion.center) && halfRegionSize.Equals (otherRegion.halfRegionSize);
        }

        public override int GetHashCode () {
            return (center, halfRegionSize).GetHashCode ();
        }

        public override string ToString () {
            return string.Format ("O: {0}, HS: {1}, ULC: {2}, LRC: {3}",
                center, halfRegionSize, leftUpperCorner, rightLowerCorner);
        }
    }
}