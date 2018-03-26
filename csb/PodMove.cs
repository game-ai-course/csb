using System;

namespace CG.CodersStrikeBack
{
    public enum MoveType
    {
        Thrust,
        Boost,
        Shield
    }

    public class PodMove
    {
        public PodMove(VecD target, int thrust, MoveType moveType = MoveType.Thrust)
            : this((Vec)target.Round(), thrust, moveType)
        {

        }
        public PodMove(Vec target, int thrust, MoveType moveType = MoveType.Thrust)
        {
            Target = target;
            Thrust = thrust;
            MoveType = moveType;
        }

        public Vec Target { get; }
        public int Thrust { get; }
        public MoveType MoveType { get; }
        public bool Shield => MoveType == MoveType.Shield;
        public bool Boost => MoveType == MoveType.Boost;
        public string Message;

        public static PodMove Parse(string input)
        {
            var ps = input.Split();
            var target = new Vec(ps[0].ToInt(), ps[1].ToInt());
            var moveType = MoveType.Thrust;
            if (!int.TryParse(ps[2], out var thrust))
            {
                if (ps[2] == "BOOST") moveType = MoveType.Boost;
                else if (ps[2] == "SHIELD") moveType = MoveType.Shield;
                else throw new Exception($"{ps[2]} is unknown move type");
            }
            return new PodMove(target, thrust, moveType);
        }

        public override string ToString()
        {
            var thrust = Shield ? "SHIELD" : (Boost ? "BOOST" : Thrust.ToString());
            if (string.IsNullOrEmpty(Message)) return $"{Target} {thrust}";
            return $"{Target} {thrust} {Message}";
        }

        public double GetTurnAngle(Pod pod)
        {
            var toTargetAngle = (Target - pod.Pos).GetAngle();
            return (toTargetAngle - pod.HeadingInRadians).NormAngleInRadians();
        }
    }
}
