using System.Runtime.CompilerServices;

namespace Quadtree {

    public enum QuadtreeQuadrant {
        NorthEast, // First quadrant (+, +)
        SouthEast, // Second (+, -)
        SouthWest, // Third (-, -)
        NorthWest, // Fourth (-, +)
        NumberOfQuadrants
    }

    public static class QuadtreeQuadrantExtensions {

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool XComponentIsPositive (this QuadtreeQuadrant quadrant) {
            return quadrant <= QuadtreeQuadrant.SouthEast;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool YComponentIsPositive (this QuadtreeQuadrant quadrant) {
            return quadrant == QuadtreeQuadrant.NorthEast || quadrant == QuadtreeQuadrant.NorthWest;
        }
    }
}