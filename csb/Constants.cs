using System;

namespace CG.CodersStrikeBack
{
    public static class Constants
    {
        public const int MaxThrust = 100;
        public const int TimeToLiveWithoutCheckpoints = 100;
        public const int CheckpointRadius = 600;
        public const int PodRadius = 400;
        public const double MaxAngularSpeed = Math.PI / 10;
        public const int Width = 16000;
        public const int Height = 9000;
        public const int Diameter = 18358;
        public const double Friction = 0.85;
        public const int MinImpulse = 120;
        public const int BoostThrust = 650;
        public const int ShieldMass = 10;
        public const int ShieldDuration = 4;
    }
}
