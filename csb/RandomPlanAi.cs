using System;

namespace CG.CodersStrikeBack
{
    public class RandomPlanAi : IAi
    {
        private readonly TrackLogger trackLogger;

        public RandomPlanAi(TrackLogger trackLogger = null)
        {
            this.trackLogger = trackLogger;
        }

        public PodMove[] GetMoves(State state, Countdown countdown)
        {
            int podsCount = state.MyPods.Length;
            var moves = new PodMove[podsCount];
            long time = countdown.TimeLeftMs / podsCount;
            for (var podIndex = 0; podIndex < podsCount; podIndex++)
            {
                var plan = GetBestPlan(state, podIndex, podIndex == 0 ? time : countdown);
                moves[podIndex] = plan.Moves[0];
                trackLogger?.LogPlan(plan, podIndex); // For plan drawing in the visializer
            }
            return moves;
        }

        private PodPlan GetBestPlan(State state, int podIndex, Countdown countdown)
        {
            // While have time — generate new random plan and evaluate it. When time is up, return the best plan found.
            throw new NotImplementedException();
        }
    }
}
