using System.Runtime.CompilerServices;

namespace Quadtree {

    /// <summary>
    /// Enumerator listing the quadrants a region may divide into.
    /// </summary>
    public enum QQuadrant {
        NorthEast, // First quadrant (+x, +y)
        SouthEast, // Second (+x, -y)
        SouthWest, // Third (-x, -y)
        NorthWest, // Fourth (-x, +y)
        NumberOfQuadrants
    }

    /// <summary>
    /// Contains extension methods for the QQuadrant enumerator.
    /// </summary>
    public static class QuadtreeQuadrantExtensions {

        /// <summary>
        /// Checks if the X component of the quadrant is positive.
        /// </summary>
        /// <returns>True, if its X component is positive, False otherwise.</returns>
        /// <param name="quadrant">The quadrant to check.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool XComponentIsPositive (this QQuadrant quadrant) {
            return quadrant <= QQuadrant.SouthEast;
        }

        /// <summary>
        /// Checks if the Y component of the quadrant is positive.
        /// </summary>
        /// <returns>True, if its Y component is positive, False otherwise.</returns>
        /// <param name="quadrant">The quadrant to check.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool YComponentIsPositive (this QQuadrant quadrant) {
            return quadrant == QQuadrant.NorthEast || quadrant == QQuadrant.NorthWest;
        }
    }
}