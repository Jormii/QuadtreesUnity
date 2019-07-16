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
            QuadtreeTest.AddDebugMessage ("QuadtreeQuadrant::QuadtreeQuadrantExtensions::XComponentIsPositive");
            return quadrant <= QuadtreeQuadrant.SouthEast;
        }

        public static bool YComponentIsPositive (this QuadtreeQuadrant quadrant) {
            QuadtreeTest.AddDebugMessage ("QuadtreeQuadrant::QuadtreeQuadrantExtensions::YComponentIsPositive");
            return quadrant == QuadtreeQuadrant.NorthEast || quadrant == QuadtreeQuadrant.NorthWest;
        }

    }

}