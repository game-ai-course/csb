namespace CG.CodersStrikeBack
{
    public interface IPodAi
    {
        PodMove GetMove(Pod pod, State state);
    }
}
