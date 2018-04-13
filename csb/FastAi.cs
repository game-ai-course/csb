using System.Linq;

namespace CG.CodersStrikeBack
{
    public class FastAi : IPodAi, IAi
    {
        public PodMove[] GetMoves(State state, Countdown countdown)
        {
            return state.MyPods.Select(p => GetMove(p, state)).ToArray();
        }

        public PodMove GetMove(Pod pod, State state)
        {
            var cp = state.Checkpoints[pod.NextCheckpointId];
            var target = cp - 3 * pod.V;
            double cosDAngle = VecD.FromPolar(1, pod.HeadingInRadians).ScalarProd((target - pod.Pos).Normalize());
            if (pod.CanBoost && cosDAngle > 0.85 && target.DistTo(pod.Pos) > Constants.BoostThrust * 6)
                return new PodMove(target, 0, MoveType.Boost);
            return new PodMove(target, (int)(Constants.MaxThrust * cosDAngle.BoundTo(0, 1)));
        }
    }
}
