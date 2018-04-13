using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace CG.CodersStrikeBack
{
    [TestFixture]
    public class CompareAis_Should
    {
        public IAi CreateFirstPlayer()
        {
            var fastAi = new FastAi();
            var stateEvaluator = new StateEvaluator();
            var trackLogger = new TrackLogger("orange");
            return new RandomPlanAi(trackLogger);
        }

        public IAi CreateSecondPlayer()
        {
            var fastAi = new FastAi();
            var stateEvaluator = new StateEvaluator();
            var trackLogger = new TrackLogger("red");
            return new SingleTargetAi(15, stateEvaluator, fastAi, trackLogger);
        }

        [Test]
        [Explicit]
        public void Visualize()
        {
            var myAi = CreateFirstPlayer();
            var hisAi = CreateSecondPlayer();
            var init = StateReader.ReadInitData("6|3|5031 5248|11466 6084|9107 1812|");
            var state = StateReader.ReadState(
                init, "5095 4752 0 0 -1 1|4967 5744 0 0 -1 1|5224 3761 0 0 -1 1|4838 6735 0 0 -1 1").WithSwappedPlayers();
            state = state.WithSwappedPlayers();
            RunToEnd(init, state, myAi, hisAi, true);
            int myScore = state.MyPods.Max(p => p.CheckpointsTaken);
            int hisScore = state.HisPods.Max(p => p.CheckpointsTaken);
            Console.WriteLine(myScore + " : " + hisScore);
        }

        [Test]
        [Explicit]
        public void Compare([Values(100)] int gamesCount)
        {
            string resultFileName = TestContext.CurrentContext.TestDirectory + "/win-statistics.txt";
            var myAiName = CreateFirstPlayer().ToString();
            var hisAiName = CreateSecondPlayer().ToString();
            File.WriteAllText(resultFileName, $"{myAiName}\t{hisAiName}\n{string.Join("\t", "score0", "score1", "delta", "win0", "win1")}\n");
            StatValue scoresDiff = new StatValue();
            StatValue wins = new StatValue();
            for (int i = 0; i < gamesCount; i++)
            {
                var myAi = CreateFirstPlayer();
                var hisAi = CreateSecondPlayer();
                var (init, state) = CreateState();
                if (i % 2 == 0)
                    state = state.WithSwappedPlayers();
                RunToEnd(init, state, myAi, hisAi, false);
                if (i % 2 == 0)
                    state = state.WithSwappedPlayers();
                int myScore = state.MyPods.Max(p => p.CheckpointsTaken);
                int hisScore = state.HisPods.Max(p => p.CheckpointsTaken);
                scoresDiff.Add(myScore - hisScore);
                wins.Add(
                    myScore > hisScore ? 1 :
                    myScore < hisScore ? -1 : 0);
                File.AppendAllText(
                    resultFileName,
                    string.Join("\t", myScore, hisScore, myScore - hisScore, myScore > hisScore ? 1 : 0, hisScore > myScore ? 1 : 0)
                    + "\n"
                    );
            }
            File.AppendAllText(
                resultFileName,
                string.Join("\n",
                    "",
                    "stats:",
                    "wins0 - wins1: " + wins.ToDetailedString(),
                    "score0 - score1: " + scoresDiff.ToDetailedString()
                )
            );
        }

        private static (InitData, State) CreateState()
        {
            var init = StateReader.ReadInitData("6|3|5031 5248|11466 6084|9107 1812|");
            var state = StateReader.ReadState(
                init, "5095 4752 0 0 -1 1|4967 5744 0 0 -1 1|5224 3761 0 0 -1 1|4838 6735 0 0 -1 1");
            return (init, state);
        }

        private static void RunToEnd(
            InitData init, State state, IAi myAi, IAi hisAi, bool saveLog)
        {
            var log = new GameLogJson();
            log.Checkpoints = state.Checkpoints;
            log.PushToTimeline(state, new[] { "", "" });
            while (!GameIsFinished(state, init))
            {
                var myMoves = GetMoves(myAi, state, saveLog);
                var hisMoves = GetMoves(hisAi, state.WithSwappedPlayers(), saveLog);
                state.Tick(myMoves.Item1, hisMoves.Item1);
                log.PushToTimeline(state, new[] { myMoves.Item2, hisMoves.Item2 });
            }

            if (saveLog)
            {
                string dir = TestContext.CurrentContext.TestDirectory;
                File.WriteAllText(Path.Combine(dir, "viz", "log.js"), "const log = " + log.SerializeToJson());
                Process.Start(Path.Combine(dir, "viz", "index.html"));
            }
        }

        private static bool GameIsFinished(State state, InitData init)
        {
            return (state.IsDead.Any(d => d)
                    || state.AllPods.Any(p => p.CheckpointsTaken >= init.LapsCount * init.Checkpoints.Length));
        }

        private static Tuple<PodMove[], string> GetMoves(IAi myAi, State state, bool saveLog)
        {
            if (!saveLog)
                return Tuple.Create(myAi.GetMoves(state, 45), string.Empty);
            var w = new StringWriter();
            var oldWriter = Console.Error;
            Console.SetError(w);
            try
            {
                var myMoves = myAi.GetMoves(state, 45);
                w.Flush();
                return Tuple.Create(myMoves, w.ToString());
            }
            finally
            {
                Console.SetError(oldWriter);
            }
        }
    }

    public class GameLogJson
    {
        public Vec[] Checkpoints;

        public List<PodJson[]> Timeline = new List<PodJson[]>();
        public List<string[]> OutputTimeline = new List<string[]>();

        public string SerializeToJson()
        {
            var settings = new JsonSerializerSettings();
            var resolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy(),
            };
            settings.ContractResolver = resolver;
            settings.Formatting = Formatting.Indented;
            return JsonConvert.SerializeObject(this, settings);
        }

        public void PushToTimeline(State state, string[] outputs)
        {
            var podJsons = state.MyPods.Select((pod, i) => new PodJson(pod, 0, i))
                .Concat(state.HisPods.Select((pod, i) => new PodJson(pod, 1, i)))
                .ToArray();
            Timeline.Add(podJsons);
            OutputTimeline.Add(outputs);
        }
    }

    public class PodJson
    {
        public PodJson(Pod pod, int team, int index)
            : this(team, index, (Vec)pod.Pos, (Vec)pod.V, pod.HeadingInDegrees, pod.ShieldTicksLeft > 0, pod.CheckpointsTaken)
        {
        }

        public PodJson(int team, int index, Vec pos, Vec v, int h, bool isShield, int checkpoints)
        {
            Pos = pos;
            V = v;
            Team = team;
            Index = index;
            H = h;
            Checkpoints = checkpoints;
            IsShield = isShield;
        }

        public int Team, Index, H;
        public Vec Pos;
        public Vec V;
        public bool IsShield;
        public int Checkpoints;
    }
}
