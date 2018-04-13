using System;
using System.Linq;

namespace CG.CodersStrikeBack
{
    public class StateReader : BaseStateReader
    {
        public StateReader(string input) : base(input)
        {
        }

        public StateReader() : base(Console.ReadLine)
        {
        }

        public InitData ReadInitData()
        {
            var lapsCount = ReadInt();
            var checkpointsCount = ReadInt();
            var cps = checkpointsCount.Times(i => ReadVec()).ToArray();
            if (logToError)
                Console.Error.WriteLine();
            return new InitData(cps, lapsCount);
        }

        public static State Read(string init, string state)
        {
            var initData = ReadInitData(init);
            return ReadState(initData, state);
        }

        public static State ReadState(InitData init, string state)
        {
            return new StateReader(state).ReadState(init);
        }

        public static InitData ReadInitData(string init)
        {
            return new StateReader(init).ReadInitData();
        }

        public State ReadState(InitData init, bool isInitialState = false)
        {
            var myPods = new[] { ReadPod(), ReadPod() };
            var hisPods = new[] { ReadPod(), ReadPod() };
            if (logToError)
                Console.Error.WriteLine();
            return new State(init.Checkpoints, myPods, hisPods, isInitialState, new bool[2]);
        }

        private Pod ReadPod()
        {
            var line = readLine();
            return ReadPod(line);
        }

        public static Pod ReadPod(string line)
        {
            var ps = line.Split().Select(int.Parse).ToList();
            return new Pod(new Vec(ps[0], ps[1]), new Vec(ps[2], ps[3]), ps[4] * Math.PI / 180, ps[5]);
        }
    }
}
