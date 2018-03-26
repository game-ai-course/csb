using System;
using System.Linq;

namespace CG.CodersStrikeBack
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var reader = new StateReader();
            var initData = reader.ReadInitData();
            var persister = new StatePersister();
            var evaluator = new StateEvaluator();
            var fastAi = new FastAi();
            var ai = new SingleTargetAi(10, evaluator, fastAi);
            while (true)
            {
                var state = reader.ReadState(initData, persister.IsInitialState);
                var countdown = new Countdown(140);
                persister.FillState(state);
                Console.Error.WriteLine("my checkpoints taken:  " + state.MyPods[0].CheckpointsTaken + " " + state.MyPods[1].CheckpointsTaken);
                Console.Error.WriteLine("his checkpoints taken: " + state.HisPods[0].CheckpointsTaken + " " + state.HisPods[1].CheckpointsTaken);
                var bestMoves = ai.GetMoves(state, countdown);
                persister.RegisterMoves(bestMoves);
                Console.Error.WriteLine(countdown);
                foreach (var t in bestMoves)
                    Console.WriteLine(t);
            }
        }
    }
}
