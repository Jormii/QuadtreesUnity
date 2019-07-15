namespace Quadtree {

    public struct Vector2D {
        private readonly float x;
        private readonly float y;

        public Vector2D (float x, float y) {
            this.x = x;
            this.y = y;
        }

        public override string ToString () {
            return string.Format ("({0:0.00}, {1:0.00})", x, y);
        }

        public override bool Equals (object obj) {
            if (obj == null) {
                return false;
            }

            if (!GetType ().Equals (obj.GetType ())) {
                return false;
            }

            Vector2D otherVector = (Vector2D) obj;
            return x.Equals (otherVector.x) && y.Equals (otherVector.y);
        }

        public override int GetHashCode () {
            return (x, y).GetHashCode ();
        }

        /*
        Properties
         */

        public float X {
            get => x;
        }

        public float Y {
            get => y;
        }

        /*
        Operators overloading
         */

        public static Vector2D operator / (Vector2D vector, float number) {
            return new Vector2D (vector.x / number, vector.y / number);
        }

    }
}