using System;

namespace CG.CodersStrikeBack
{
    public class Pod : Disk
    {
        public Pod(
            VecD pos, VecD v, double headingInRadians, int nextCheckpointId, int checkpointsTaken = 0,
            bool canBoost = true, int timeWithoutCheckpoint = 0, int shieldTicksLeft = 0, int mass = 1,
            bool checkedAtLastTurn = false) : base(pos, v, mass, Constants.PodRadius)
        {
            HeadingInRadians = headingInRadians;
            NextCheckpointId = nextCheckpointId;
            CheckpointsTaken = checkpointsTaken;
            TimeWithoutCheckpoint = timeWithoutCheckpoint;
            ShieldTicksLeft = shieldTicksLeft;
            CanBoost = canBoost;
            CheckedAtLastTurn = checkedAtLastTurn;
        }

        public double HeadingInRadians { get; private set; }
        public int HeadingInDegrees => (int) Math.Round(HeadingInRadians * 180 / Math.PI);
        public int ShieldTicksLeft { get; private set; }
        public int NextCheckpointId { get; private set; }
        public int CheckpointsTaken { get; private set; }
        public int TimeWithoutCheckpoint { get; private set; }
        public bool CanBoost { get; private set; }
        public bool CheckedAtLastTurn { get; }


        public override string ToString()
        {
            return $"{Pos} {V} {HeadingInDegrees} {NextCheckpointId}";
        }

        public Pod MakeCopy()
        {
            return new Pod(
                Pos, V, HeadingInRadians, NextCheckpointId, CheckpointsTaken, CanBoost, TimeWithoutCheckpoint,
                ShieldTicksLeft,
                1, CheckedAtLastTurn);
        }

        public void Stop()
        {
            V = VecD.Zero;
        }
    }
}
