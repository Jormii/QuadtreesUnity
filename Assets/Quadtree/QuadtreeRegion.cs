namespace Quadtree {
    public struct QuadtreeRegion {
        private readonly Vector2D center;
        private readonly Vector2D halfRegionSize;

        public QuadtreeRegion (Vector2D center, Vector2D halfRegionSize) {
            this.center = center;
            this.halfRegionSize = halfRegionSize;
        }

        public bool ContainsPoint (Vector2D point) {
            bool containsXComponent = point.X >= LeftUpperCorner.X && point.X <= RightLowerCorner.X;
            bool containsYComponent = point.Y <= LeftUpperCorner.Y && point.Y >= RightLowerCorner.Y;

            return containsXComponent && containsYComponent;
        }

        public override bool Equals (object obj) {
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
            return (center, halfRegionSize, LeftUpperCorner, RightLowerCorner).GetHashCode ();
        }

        public override string ToString () {
            return string.Format ("O: {0}, HS: {1}", center.ToString (), halfRegionSize.ToString ());
        }

        /*
        Properties
         */

        public Vector2D Center {
            get => center;
        }

        public Vector2D HalfRegionSize {
            get => halfRegionSize;
        }

        public Vector2D LeftUpperCorner {
            get {
                float xComponent = center.X - halfRegionSize.X;
                float yComponent = center.Y + halfRegionSize.Y;
                return new Vector2D (xComponent, yComponent);
            }
        }

        public Vector2D RightLowerCorner {
            get {
                float xComponent = center.X + halfRegionSize.X;
                float yComponent = center.Y - halfRegionSize.Y;
                return new Vector2D (xComponent, yComponent);
            }
        }
    }
}