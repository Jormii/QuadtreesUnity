namespace Quadtree {
    public enum QuadtreeQuadrant {
        NorthEast, // First quadrant (+, +)
        SouthEast, // Second (+, -)
        SouthWest, // Third (-, -)
        NorthWest, // Fourth (-, +)
        NumberOfQuadrants
    }

    public static class QuadtreeQuadrantExtensions {

        public static bool XComponentIsPositive (this QuadtreeQuadrant quadrant) {
            return quadrant <= QuadtreeQuadrant.SouthEast;
        }

        public static bool YComponentIsPositive (this QuadtreeQuadrant quadrant) {
            return quadrant == QuadtreeQuadrant.NorthEast || quadrant == QuadtreeQuadrant.NorthWest;
        }

    }

}