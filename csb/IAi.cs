namespace CG.CodersStrikeBack
{
    public interface IAi
    {
        PodMove[] GetMoves(State state, Countdown countdown);
    }
}
