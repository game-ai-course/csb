using System;
using System.Linq;

namespace CG.CodersStrikeBack
{
    public class State
    {
        public readonly Vec[] Checkpoints;
        public readonly Pod[] AllPods;
        public readonly Pod[] HisPods;
        public bool IsInitialState;
        public readonly Pod[] MyPods;
        public readonly bool[] IsDead;

        public State(Vec[] checkpoints, Pod[] myPods, Pod[] hisPods, bool isInitialState, bool[] isDead)
        {
            Checkpoints = checkpoints;
            MyPods = myPods;
            HisPods = hisPods;
            AllPods = new [] { myPods[0], myPods[1], hisPods[0], hisPods[1] };
            IsDead = isDead;
            IsInitialState = isInitialState;
        }

        public void Tick(PodMove[] myMoves, PodMove[] hisMoves)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Join("\n", MyPods.Concat(HisPods));
        }

        public State MakeCopy()
        {
            return new State(
                Checkpoints,
                new[] { MyPods[0].MakeCopy(), MyPods[1].MakeCopy() },
                new[] { HisPods[0].MakeCopy(), HisPods[1].MakeCopy() },
                IsInitialState, IsDead.ToArray());
        }

        public State WithSwappedPlayers()
        {
            return new State(Checkpoints, HisPods, MyPods, IsInitialState, IsDead.Reverse().ToArray());
        }
    }
}
