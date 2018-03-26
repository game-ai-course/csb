namespace CG.CodersStrikeBack
{
    public interface IStateEvaluator
    {
        ExplainedScore Evaluate(State state);
    }
}
