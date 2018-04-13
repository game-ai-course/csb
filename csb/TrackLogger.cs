using System;
using System.Collections.Generic;
using System.Linq;

namespace CG.CodersStrikeBack
{
    public class TrackLogger
    {
        private readonly string color;

        public TrackLogger(string color)
        {
            this.color = color;
        }

        public void LogPlan(IEnumerable<Pod> podStates, int podIndex)
        {
            Console.Error.WriteLine($"Track {podIndex} {color} {string.Join(" ", podStates.Select(p => p.Pos.X + ";" + p.Pos.Y))}");
        }

        public void LogPlan(PodPlan plan, int podIndex)
        {
            Console.Error.WriteLine($"Track {podIndex} {color} {string.Join(" ", plan.PodStates.Select(p => p.Pos.X + ";" + p.Pos.Y))}");
            for (var index = 0; index < plan.Moves.Count; index++)
            {
                var move = plan.Moves[index];
                var prevPos = plan.PodStates[index].Pos;
                var dp = (move.Target - prevPos).Resize(move.Thrust);
                var nextPos = (Vec)(prevPos + dp);
                Console.Error.WriteLine($"Track {podIndex} black {prevPos.X};{prevPos.Y} {nextPos.X};{nextPos.Y}");
            }
        }
    }
}
