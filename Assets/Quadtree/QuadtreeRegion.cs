namespace Quadtree {
    public struct QuadtreeRegion {
        public readonly Vector2D center;
        public readonly Vector2D halfRegionSize;

        public QuadtreeRegion (Vector2D center, Vector2D halfRegionSize) {
            QuadtreeTest.AddDebugMessage ("QuadtreeRegion::QuadtreeRegion");
            this.center = center;
            this.halfRegionSize = halfRegionSize;
        }

        public bool ContainsPoint (Vector2D point) {
            QuadtreeTest.AddDebugMessage ("QuadtreeRegion::ContainsPoint");
            bool containsXComponent = point.x >= LeftUpperCorner.x && point.x <= RightLowerCorner.x;
            bool containsYComponent = point.y <= LeftUpperCorner.y && point.y >= RightLowerCorner.y;

            return containsXComponent && containsYComponent;
        }

        public override bool Equals (object obj) {
            QuadtreeTest.AddDebugMessage ("QuadtreeRegion::Equals");
            if (obj == null) {
                return false;
            }

            if (!GetType ().Equals (obj.GetType ())) {
                return false;
            }

            QuadtreeRegion otherRegion = (QuadtreeRegion) obj;
            return center.Equals (otherRegion.center) && halfRegionSize.Equals (otherRegion.halfRegionSize);
        }

        public override int GetHashCode () {
            QuadtreeTest.AddDebugMessage ("QuadtreeRegion::GetHashCode");
            return (center, halfRegionSize, LeftUpperCorner, RightLowerCorner).GetHashCode ();
        }

        public override string ToString () {
            return string.Format ("O: {0}, HS: {1}", center.ToString (), halfRegionSize.ToString ());
        }

        /*
        Properties
         */

        public Vector2D LeftUpperCorner {
            get {
                QuadtreeTest.AddDebugMessage ("QuadtreeRegion::LeftUpperCorner");
                float xComponent = center.x - halfRegionSize.x;
                float yComponent = center.y + halfRegionSize.y;
                return new Vector2D (xComponent, yComponent);
            }
        }

        public Vector2D RightLowerCorner {
            get {
                QuadtreeTest.AddDebugMessage ("QuadtreeRegion::RightLowerCorner");
                float xComponent = center.x + halfRegionSize.x;
                float yComponent = center.y - halfRegionSize.y;
                return new Vector2D (xComponent, yComponent);
            }
        }
    }
}