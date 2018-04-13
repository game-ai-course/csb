namespace CG.CodersStrikeBack
{
    public class FlyToTargetPodAi : IPodAi
    {
        private readonly Vec target;
        private readonly int thrust;
        private readonly bool boostIfCan;
        private readonly bool shieldOnCollision;

        public FlyToTargetPodAi(Vec target, int thrust, bool boostIfCan, bool shieldOnCollision)
        {
            this.target = target;
            this.thrust = thrust;
            this.boostIfCan = boostIfCan;
            this.shieldOnCollision = shieldOnCollision;
        }

        public PodMove GetMove(Pod pod, State state)
        {
            var moveType = GetMoveType(pod, state);
            return new PodMove(target, thrust, moveType);
        }

        private MoveType GetMoveType(Pod pod, State state)
        {
            //TODO: return right moveType!
            return MoveType.Thrust;
        }

        private bool CanCollideNextTick(Pod pod, State state)
        {
            // TODO: Predict possible collision with other pod
            return false;
        }

    }
}
