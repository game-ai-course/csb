using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace CG
{
    public class VecD : IEquatable<VecD>, IFormattable
    {
        public readonly double X, Y;

        public static readonly VecD Zero = new VecD(0, 0);

        public VecD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator VecD(Vec v) => new VecD(v.X, v.Y);
        public static explicit operator Vec(VecD v) => new Vec((int)v.X, (int)v.Y);

        public double this[int dimension] => dimension == 0 ? X : Y;

        [Pure]
        public bool Equals(VecD other)
        {
            return other != null && X.Equals(other.X) && Y.Equals(other.Y);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{X.ToString(format, formatProvider)} {Y.ToString(format, formatProvider)}";
        }

        public static VecD FromPolar(double len, double angle)
        {
            return new VecD(len * Math.Cos(angle), len * Math.Sin(angle));
        }

        public static VecD Parse(string s)
        {
            var parts = s.Split();
            if (parts.Length != 2) throw new FormatException(s);
            return new VecD(double.Parse(parts[0], CultureInfo.InvariantCulture), double.Parse(parts[1], CultureInfo.InvariantCulture));
        }

        public bool InArea(double size)
        {
            return X >= 0 && X < size && Y >= 0 && Y < size;
        }

        public bool InArea(double w, double h)
        {
            return X.InRange(0, w - 1) && Y.InRange(0, h - 1);
        }

        public VecD BoundTo(double minX, double minY, double maxX, double maxY)
        {
            return new VecD(X.BoundTo(minX, maxX), Y.BoundTo(minY, maxY));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is VecD && Equals((VecD) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{X} {Y}";
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InRadiusTo(VecD b, double radius)
        {
            return SquaredDistTo(b) <= radius * radius;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double DistTo(VecD b)
        {
            return (b - this).Length();
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double SquaredDistTo(VecD b)
        {
            var dx = X - b.X;
            var dy = Y - b.Y;
            return dx * dx + dy * dy;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double LengthSquared()
        {
            return X * X + Y * Y;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VecD operator -(VecD a, VecD b)
        {
            return new VecD(a.X - b.X, a.Y - b.Y);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VecD operator -(VecD a)
        {
            return new VecD(-a.X, -a.Y);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VecD operator +(VecD v, VecD b)
        {
            return new VecD(v.X + b.X, v.Y + b.Y);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VecD operator *(VecD a, double k)
        {
            return new VecD(a.X * k, a.Y * k);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VecD operator /(VecD a, double k)
        {
            return new VecD(a.X / k, a.Y / k);
        }


        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VecD operator *(double k, VecD a)
        {
            return new VecD(a.X * k, a.Y * k);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ScalarProd(VecD p2)
        {
            return X * p2.X + Y * p2.Y;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ScalarProd(Vec p2)
        {
            return X * p2.X + Y * p2.Y;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double VecDtorProdLength(VecD p2)
        {
            return X * p2.Y - p2.X * Y;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double VecDtorProdLength(Vec p2)
        {
            return X * p2.Y - p2.X * Y;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VecD Translate(double shiftX, double shiftY)
        {
            return new VecD(X + shiftX, Y + shiftY);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VecD MoveTowards(VecD target, double distance)
        {
            var d = target - this;
            var difLen = d.Length();
            if (difLen < distance) return target;
            var k = distance / difLen;
            return new VecD(X + k * d.X, Y + k * d.Y);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VecD Rotate90CW()
        {
            return new VecD(Y, -X);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VecD Rotate90CCW()
        {
            return new VecD(-Y, X);
        }

        public VecD Resize(double newLen)
        {
            return newLen / Length() * this;
        }

        /// <returns>angle in (-Pi..Pi]</returns>
        public double GetAngle()
        {
            return Math.Atan2(Y, X);
        }

        public VecD Round()
        {
            return new VecD(Math.Round(X), Math.Round(Y));
        }

        public VecD Normalize()
        {
            return this / Length();
        }
    }
}
