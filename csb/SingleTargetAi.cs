using System;
using System.Collections.Generic;
using System.Linq;

namespace CG.CodersStrikeBack
{
    public class SingleTargetAi : IAi
    {
        private readonly IStateEvaluator evaluator;
        private readonly IAi fastEnemyAi;
        private readonly TrackLogger trackLogger;
        private readonly int timeToSimulate;

        public SingleTargetAi(int timeToSimulate, IStateEvaluator evaluator, IAi fastEnemyAi, TrackLogger trackLogger = null)
        {
            this.timeToSimulate = timeToSimulate;
            this.evaluator = evaluator;
            this.fastEnemyAi = fastEnemyAi;
            this.trackLogger = trackLogger;
        }

        public PodMove[] GetMoves(State state, Countdown countdown)
        {
            ExplainedScore bestScore = double.NegativeInfinity;
            PodMove[] bestMoves = null;
            List<Pod[]> bestPodStates = null;
            var possibleTargets = state.Checkpoints;
            var possibleThrusts = new[] { Constants.MaxThrust, 0 };
            foreach (var target0 in possibleTargets)
                foreach (var target1 in possibleTargets)
                    foreach (int thrust0 in possibleThrusts)
                        foreach (int thrust1 in possibleThrusts)
                        {
                            /*
                             * Simulate timeToSimulate ticks, with i-th pod targets to target_i with thrust_i.
                             * Select the best option.
                             *
                             * Explore the different meanings of the «best». Compare different meanings with the test SingleTargetAi_Should.MovePodToTarget.
                             */
                        }
            if (bestPodStates != null)
                for (int i = 0; i < 2; i++)
                    trackLogger?.LogPlan(bestPodStates.Select(p => p[i]), i);
            return bestMoves;
        }
    }
}
