using System.Collections.Generic;

namespace CG.CodersStrikeBack
{
    public class PodPlan
    {
        public PodPlan(Pod initialPodState)
        {
            podStates.Add(initialPodState.MakeCopy());
        }

        public void AddItem(IPodAi podAi, PodMove move, Pod podStateAfterMove)
        {
            ais.Add(podAi);
            moves.Add(move);
            podStates.Add(podStateAfterMove.MakeCopy());
        }

        public IReadOnlyList<IPodAi> Ais => ais;
        public IReadOnlyList<PodMove> Moves => moves;
        public IReadOnlyList<Pod> PodStates => podStates;

        private readonly List<IPodAi> ais = new List<IPodAi>();
        private readonly List<PodMove> moves = new List<PodMove>();
        private readonly List<Pod> podStates = new List<Pod>();
        public ExplainedScore FinalScore = double.NegativeInfinity;
    }
}
