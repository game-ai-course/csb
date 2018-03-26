using System;

namespace CG.CodersStrikeBack
{
    public class StateEvaluator : IStateEvaluator
    {
        public ExplainedScore Evaluate(State state)
        {
            var my = EvaluateForMe(state);
            var his = EvaluateForMe(state.WithSwappedPlayers());
            return my - his.Value*0.8;
        }

        private ExplainedScore EvaluateForMe(State state)
        {
            var pods = state.MyPods;
            var scores = new[]
            {
                DistanceScore(pods[0], state.Checkpoints),
                DistanceScore(pods[1], state.Checkpoints)
            };
            var leaderIndex = scores[0] >= scores[1] ? 0 : 1;
            var blockerIndex = 1 - leaderIndex;
            return scores[leaderIndex] + 0.1 * scores[blockerIndex];
        }

        public static double DistanceScore(Pod pod, Vec[] checkpoints)
        {
            var podCheckpoint = checkpoints[pod.NextCheckpointId] - pod.Pos;
            var posDif = Math.Max(0, (podCheckpoint - pod.V).Length() - 0.95 * Constants.CheckpointRadius) / Constants.Diameter;
            return pod.CheckpointsTaken - posDif;
        }
    }
}
