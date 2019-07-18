using System.Runtime.CompilerServices;

namespace Quadtree {

    /// <summary>
    /// Represents a vector in 2D.
    /// </summary>
    public struct QVector2D {

        /// <summary>
        /// X component of the vector. Read-only.
        /// </summary>
        public readonly float x;

        /// <summary>
        /// Y component of the vector. Read-only.
        /// </summary>
        public readonly float y;

        /// <summary>
        /// Initializes a new instance of the QVector2D struct.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public QVector2D (float x, float y) {
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

            QVector2D argPoint = (QVector2D) obj;
            return x.Equals (argPoint.x) && y.Equals (argPoint.y);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode () {
            return (x, y).GetHashCode ();
        }

        /*
        Operator overload
        */

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static QVector2D operator + (QVector2D aPoint, QVector2D anotherPoint) {
            return new QVector2D (aPoint.x + anotherPoint.x, aPoint.y + anotherPoint.y);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static QVector2D operator * (QVector2D point, float number) {
            return new QVector2D (point.x * number, point.y * number);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static QVector2D operator * (float number, QVector2D point) {
            return new QVector2D (point.x * number, point.y * number);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static QVector2D operator / (QVector2D point, float number) {
            return new QVector2D (point.x / number, point.y / number);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static QVector2D operator / (float number, QVector2D point) {
            return new QVector2D (point.x / number, point.y / number);
        }
    }
}