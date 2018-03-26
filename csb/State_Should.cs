using System;
using System.IO;
using FluentAssertions;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace CG.CodersStrikeBack
{
    [TestFixture]
    public class State_Should
    {
        [TestCase(
            "3|6|7814 833|7651 5956|3122 7541|9494 4357|14510 7784|6314 4297",
            "8314 849 0 0 186 1|7314 817 0 0 186 1|9313 881 0 0 185 1|6315 785 0 0 187 1",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "0 0 0",
            "8314 849 0 0 186 1|7314 817 0 0 186 1|9313 881 0 0 185 1|6315 785 0 0 187 1",
            TestName = "a = 0, V = 0")]
        [TestCase(
            "3|3|5024 5234|11475 6093|9103 1849|",
            "11850 5497 89 -149 217 2|14585 4679 75 -281 209 2|10671 2890 -317 -242 202 2|10891 5840 -182 -121 258 2|",
            "8836 2296 100",
            "8878 2692 100",
            "6292 6202 100",
            "9831 2333 100",
            "11870 5275 17 -188 227 2|14566 4365 -16 -266 199 2|10254 2640 -354 -212 184 2|10680 5623 -179 -184 253 2|",
            TestName = "SimpleMove")]
        [TestCase(
            "3|3|5024 5234|11475 6093|9103 1849|",
            "8216 1818 -404 -71 137 0|9856 1998 -473 -47 157 2|5625 4940 -134 386 58 0|7411 1310 -363 -97 134 0|",
            "6236 5447 100",
            "10522 1990 100",
            "12011 4549 0",
            "6476 5622 100",
            "7763 1834 -385 13 119 0|9307 2016 -466 15 139 0|5491 5326 -113 328 40 1|7004 1303 -345 -6 116 0|",
            TestName = "NextCheckpoint!")]
        [TestCase(
            "3|3|9076 1867|5007 5269|11472 6079",
            "8152 2269 -375 107 129 1|6163 2554 -235 234 106 1|7466 2849 118 -57 327 0|6180 6655 247 123 347 2|",
            "6132 4948 100",
            "5712 4567 100",
            "8604 2095 SHIELD",
            "10484 5587 100",
            "8356 1874 285 -437 127 1|5906 2886 -218 281 103 1|7520 2850 34 11 326 0|6524 6754 292 84 346 2|",
            TestName = "Collision with shield!")]
        [TestCase(
            "3|3|9076 1867|5007 5269|11472 6079",
            "6864 4666 -344 231 177 1|6150 4011 -322 316 127 1|5656 5577 -203 221 48 1|5916 3209 -212 347 101 1|",
            "6039 4576 100",
            "5973 4321 100",
            "5819 4385 0",
            "12320 4691 100",
            "6477 4927 -182 326 186 1|5740 4462 -483 334 120 1|5453 5798 -172 187 30 1|5698 3566 -196 247 83 1|",
            TestName = "Double collision without shield")]
        [TestCase(
            "3|3|9076 1867|5007 5269|11472 6079",
            "14703 6135 430 -149 289 0|11506 5900 474 -163 306 0|8811 5423 462 66 22 2|12015 5180 235 -235 248 0|",
            "7786 2314 100",
            "7654 2356 100",
            "9624 5815 100",
            "8136 2807 100",
            "15135 5886 367 -211 271 0|12002 5668 391 -102 288 0|9363 5532 469 93 26 2|12194 4842 182 -382 230 0|",
            TestName = "Single collision without shield")]
        public void TickCorrectly(
            string init, string state1, string move1, string move2, string move3, string move4, string state2)
        {
            var state = StateReader.Read(init, state1);
            var myMoves = new[] { PodMove.Parse(move1), PodMove.Parse(move2) };
            var hisMoves = new[] { PodMove.Parse(move3), PodMove.Parse(move4) };
            state.Tick(myMoves, hisMoves);
            var expectedState = StateReader.Read(init, state2);
            EnsureAlmostEqual(state, expectedState);
        }

        [TestCase("gamelog1.txt")]
        [TestCase("gamelog2.txt")]
        public void SimulateGameLogCorrectly(string filename)
        {
            var lines = File.ReadAllLines(Path.Combine(TestContext.CurrentContext.TestDirectory, filename));
            var iLine = 0;
            var initData = new StateReader(lines[iLine++]).ReadInitData();
            var persister = new StatePersister();
            var tick = 0;
            State prevStateAfterMovesApplied = null;
            while (iLine < lines.Length)
            {
                var state = new StateReader(lines[iLine++]).ReadState(initData, persister.IsInitialState);
                persister.FillState(state);
                if (prevStateAfterMovesApplied != null)
                {
                    try
                    {
                        EnsureAlmostEqual(prevStateAfterMovesApplied, state);
                    }
                    catch
                    {
                        Console.Error.WriteLine("    simulation result: ");
                        Console.Error.WriteLine(prevStateAfterMovesApplied);
                        Console.Error.WriteLine("    true result:");
                        Console.Error.WriteLine(state);
                        throw;
                    }
                }
                iLine++;
                // ReSharper disable once AccessToModifiedClosure
                var myMoves = 2.Times(i => PodMove.Parse(lines[iLine++])).ToArray();
                var hisMoves = 2.Times(i => PodMove.Parse(lines[iLine++])).ToArray();
                Console.Error.WriteLine($"=== TICK {tick} ===");
                Console.Error.WriteLine(state);
                Console.Error.WriteLine("myPod[0]:  " + myMoves[0]);
                Console.Error.WriteLine("myPod[1]:  " + myMoves[1]);
                Console.Error.WriteLine("hisPod[0]: " + hisMoves[0]);
                Console.Error.WriteLine("hisPod[1]: " + hisMoves[1]);
                state.Tick(myMoves, hisMoves);
                persister.RegisterMoves(myMoves, hisMoves);
                prevStateAfterMovesApplied = state;
                tick++;
            }
        }

        [Test]
        public void EliminatePlayer_WhenHeCantTakeCheckpointsTooLongTime()
        {
            var state = StateReader.Read(
                "3|4|8024 7881|13301 5550|9567 1403|3663 4439|",
                "7822 7424 0 0 -1 1|8226 8338 0 0 -1 1|7418 6509 0 0 -1 1|8630 9253 0 0 -1 1|");
            var ai = new FastAi();
            for(int time=0; time<101; time++)
            {
                Assert.That(state.IsDead, Does.Not.Contains(true));
                var moves = ai.GetMoves(state, 0);
                state.Tick(moves, new[]{new PodMove(VecD.Zero, 0), new PodMove(VecD.Zero, 0)});
            }
            Assert.That(state.HisPods[0].TimeWithoutCheckpoint, Is.GreaterThan(100));
            Assert.That(state.HisPods[1].TimeWithoutCheckpoint, Is.GreaterThan(100));
            Assert.That(state.IsDead[1], Is.True);
        }

        [Test]
        public void DoesNotEliminatePlayer_WhenOnlyOneHisPodCantTakeCheckpointsTooLongTime()
        {
            var state = StateReader.Read(
                "3|4|8024 7881|13301 5550|9567 1403|3663 4439|",
                "7822 7424 0 0 -1 1|8226 8338 0 0 -1 1|7418 6509 0 0 -1 1|8630 9253 0 0 -1 1|");
            var ai = new FastAi();
            for (int time = 0; time < 200; time++)
            {
                var myMoves = ai.GetMoves(state, 0);
                var hisMoves = ai.GetMoves(state.WithSwappedPlayers(), 0);
                hisMoves[0] = new PodMove(VecD.Zero, 0);
                state.Tick(myMoves, hisMoves);
            }
            Assert.That(state.HisPods[0].TimeWithoutCheckpoint, Is.GreaterThan(100));
            Assert.That(state.IsDead, Is.All.EqualTo(false));
        }


        private void EnsureAlmostEqual(State state, State expectedState)
        {
            state.AllPods.Should().HaveCount(expectedState.AllPods.Length);
            state.AllPods.Zip(expectedState.AllPods, EnsureAlmostEqual).ToList();
        }

        private bool EnsureAlmostEqual(Pod actual, Pod expected)
        {
            try
            {
                (actual.Pos - expected.Pos).Length().Should().BeLessOrEqualTo(1.5, "it is position diff");
                (actual.V - expected.V).Length().Should().BeLessOrEqualTo(1.5, "it is velocity dif");
                var angleDif = (actual.HeadingInDegrees - expected.HeadingInDegrees + 360) % 360;
                angleDif.Should().Be(0, "it is heading diff");
                actual.NextCheckpointId.Should().Be(expected.NextCheckpointId, "it is NextCheckpointId");
            }
            catch
            {
                Console.Error.WriteLine("    not equal pods:");
                Console.Error.WriteLine(actual);
                Console.Error.WriteLine(expected);
                throw;
            }
            return true;
        }
    }
}
