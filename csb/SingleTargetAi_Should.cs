using System;
using NUnit.Framework;

namespace CG.CodersStrikeBack
{
    [TestFixture]
    public class SingleTargetAi_Should
    {
        public class TestStateEvaluator : IStateEvaluator
        {
            public ExplainedScore Evaluate(State state)
            {
                return -state.MyPods[0].Pos.DistTo(state.Checkpoints[state.MyPods[0].NextCheckpointId]);
            }
        }

        [Test]
        public void MovePodToTarget([Range(1, 15, 1)]int simulationTime)
        {
            var state = StateReader.Read("3|2|10 000 2000|16000 9000",
                "8000 2000 0 0 0 0|0 200 0 0 0 1|0 400 0 0 0 1|0 6000 0 0 0 1");
            var fastAi = new FastAi();
            var evaluator = new TestStateEvaluator();
            var initialScore = evaluator.Evaluate(state);
            var moves = new SingleTargetAi(simulationTime, evaluator, fastAi).GetMoves(state, 40);
            state.Tick(moves, fastAi.GetMoves(state, 1));
            var finalScore = evaluator.Evaluate(state);
            Console.WriteLine($"MOVE [{moves[0]}] change score from {initialScore} to {finalScore}");
            Assert.That(finalScore, Is.GreaterThan(initialScore));
        }

        [Test]
        public void RotatePodOnPlaceToTarget([Range(1, 15, 1)]int simulationTime)
        {
            var state = StateReader.Read("3|2|10000 2000|16000 9000",
                "8000 2000 0 0 180 0|0 200 0 0 0 1|0 400 0 0 0 1|0 6000 0 0 0 1");
            var fastAi = new FastAi();
            var evaluator = new TestStateEvaluator();
            var moves = new SingleTargetAi(simulationTime, evaluator, fastAi).GetMoves(state, 40);
            state.Tick(moves, fastAi.GetMoves(state, 1));
            double podHeading = state.MyPods[0].HeadingInRadians.NormAngleInRadians();
            Console.WriteLine($"MOVE [{moves[0]}] turn pod0 to {podHeading}");
            Assert.That(Math.Abs(podHeading), Is.LessThan(3));
            Assert.That(state.MyPods[0].V, Is.EqualTo(VecD.Zero));
        }
    }
}
