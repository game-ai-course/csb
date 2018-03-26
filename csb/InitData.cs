namespace CG.CodersStrikeBack
{
    public class InitData
    {
        public readonly Vec[] Checkpoints;
        public readonly int LapsCount;

        public InitData(Vec[] checkpoints, int lapsCount)
        {
            Checkpoints = checkpoints;
            LapsCount = lapsCount;
        }
    }
}
