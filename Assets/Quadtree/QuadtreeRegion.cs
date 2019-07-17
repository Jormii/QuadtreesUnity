using System.Runtime.CompilerServices;

namespace Quadtree {

    public struct QuadtreeRegion {

        public readonly Vector2D center;
        public readonly Vector2D halfRegionSize;
        public readonly Vector2D leftUpperCorner;
        public readonly Vector2D rightLowerCorner;

        public QuadtreeRegion (Vector2D center, Vector2D halfRegionSize) {
            this.center = center;
            this.halfRegionSize = halfRegionSize;

            this.leftUpperCorner = new Vector2D (
                center.x - halfRegionSize.x,
                center.y + halfRegionSize.y
            );

            this.rightLowerCorner = new Vector2D (
                center.x + halfRegionSize.x,
                center.y - halfRegionSize.y
            );
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint (Vector2D point) {
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

            QuadtreeRegion otherRegion = (QuadtreeRegion) obj;
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