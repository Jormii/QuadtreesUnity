using System.Runtime.CompilerServices;

namespace Quadtree {

    /// <summary>
    /// Enumerator listing the quadrants a region may split up.
    /// </summary>
    public enum QQuadrant {
        NorthEast, // First quadrant (+, +)
        SouthEast, // Second (+, -)
        SouthWest, // Third (-, -)
        NorthWest, // Fourth (-, +)
        NumberOfQuadrants
    }

    /// <summary>
    /// Contains extension methods for the <see cref="T:Quadtree.QQuadrant"/> enumerator.
    /// </summary>
    public static class QuadtreeQuadrantExtensions {

        /// <summary>
        /// Checks if the X component of the quadrant is positive.
        /// </summary>
        /// <returns><c>true</c>, if its X component is positive, <c>false</c> otherwise.</returns>
        /// <param name="quadrant">A quadrant.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool XComponentIsPositive (this QQuadrant quadrant) {
            return quadrant <= QQuadrant.SouthEast;
        }

        /// <summary>
        /// Checks if the Y component of the quadrant is positive.
        /// </summary>
        /// <returns><c>true</c>, if its Y component is positive, <c>false</c> otherwise.</returns>
        /// <param name="quadrant">A quadrant.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool YComponentIsPositive (this QQuadrant quadrant) {
            return quadrant == QQuadrant.NorthEast || quadrant == QQuadrant.NorthWest;
        }
    }
}