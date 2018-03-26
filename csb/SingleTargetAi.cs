using System;

namespace CG.CodersStrikeBack
{
    public class SingleTargetAi : IAi
    {
        private readonly IStateEvaluator evaluator;
        private readonly IAi fastEnemyAi;
        private readonly int timeToSimulate;

        public SingleTargetAi(int timeToSimulate, IStateEvaluator evaluator, IAi fastEnemyAi)
        {
            this.timeToSimulate = timeToSimulate;
            this.evaluator = evaluator;
            this.fastEnemyAi = fastEnemyAi;
        }

        public PodMove[] GetMoves(State state, Countdown countdown)
        {
            ExplainedScore bestScore = double.NegativeInfinity;
            PodMove[] bestMoves = null;
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
            return bestMoves;
        }
    }
}
