using System;

namespace CG
{
    public class Disk
    {
        public Disk(VecD pos, VecD v, int mass, int radius)
        {
            Pos = pos;
            V = v;
            Mass = mass;
            Radius = radius;
        }

        public VecD Pos { get; protected set; }
        public VecD V { get; protected set; }
        public int Mass { get; protected set; }
        public int Radius { get; }

        public void Move(double time)
        {
            Pos += time * V;
        }

        public double GetCollisionTime(Disk other)
        {
            // TODO: ошибки преждевременного округления. 
            // Правильнее всё считать в double-ах, а окрглять в конце хода.
            // Сейчас на пару пикселей периодически расходятся симуляции с реальностью.
            var dr = other.Pos - Pos;
            var dv = other.V - V;
            var dvdr = dv.ScalarProd(dr);
            if (dvdr > 0) return double.PositiveInfinity;
            var dvdv = dv.ScalarProd(dv);
            if (dvdv == 0) return double.PositiveInfinity;
            var drdr = dr.ScalarProd(dr);
            var sigma = Radius + other.Radius;
            var d = dvdr * dvdr - dvdv * (drdr - sigma * sigma);
            if (d < 0) return double.PositiveInfinity;
            var collisionTime = -(dvdr + Math.Sqrt(d)) / dvdv;
            if (collisionTime < 0) return double.PositiveInfinity;
            return collisionTime;
        }

        public void BounceOff(Disk other)
        {
            var impulse = ComputeImpulse(Pos, V, Mass, other.Pos, other.V, other.Mass);
            var vs = BouncedSpeed(V, Mass, other.V, other.Mass, impulse);
            V = vs.Item1;
            other.V = vs.Item2;
        }

        public void BounceOffWithMinimumImpulse(Disk other, double minImpulse)
        {
            var impulse = ComputeImpulse(Pos, V, Mass, other.Pos, other.V, other.Mass);
            var impulseSize = Math.Max(impulse.Length(), impulse.Length() * 0.5 + minImpulse);
            var adjusted = (Pos - other.Pos).Resize(impulseSize);
            var vs = BouncedSpeed(V, Mass, other.V, other.Mass, adjusted);
            V = vs.Item1;
            other.V = vs.Item2;
        }

        private static VecD ComputeImpulse(VecD p1, VecD v1, int m1, VecD p2, VecD v2, int m2)
        {
            var dp = p2 - p1;
            var dv = v2 - v1;
            var drdr = dp.ScalarProd(dp);
            var dvdr = dp.ScalarProd(dv);
            var massCoefficient = (m1 + m2) / (double) (m1 * m2);
            return 2 * dvdr / (massCoefficient * drdr) * dp;
        }

        private static Tuple<VecD, VecD> BouncedSpeed(VecD v1, int m1, VecD v2, int m2, VecD impulse)
        {
            return Tuple.Create(v1 + impulse / m1, v2 - impulse / m2);
        }

        public override string ToString()
        {
            return $"{nameof(Pos)}: {Pos}, {nameof(V)}: {V}, {nameof(Mass)}: {Mass}, {nameof(Radius)}: {Radius}";
        }
    }
}
