using NUnit.Framework;

namespace CG.CodersStrikeBack
{
    [TestFixture]
    public class StatePersister_Should
    {
        [Test]
        public void FillCheckpointsData()
        {
            var persister = new StatePersister();
            var s1 = StateReader.Read("1|2|1000 1000|9000 1000",
                "0 1000 0 0 186 1|7314 817 0 0 186 1|9313 881 0 0 185 1|6315 785 0 0 187 1");
            s1.IsInitialState = true;
            persister.FillState(s1);
            Assert.That(s1.MyPods[0].CheckpointsTaken, Is.EqualTo(0));
            var s2 = StateReader.Read("1|2|1000 1000|9000 1000",
                "2000 1000 0 0 186 2|7314 817 0 0 186 1|9313 881 0 0 185 1|6315 785 0 0 187 1");
            persister.FillState(s2);

            Assert.That(s2.MyPods[0].CheckpointsTaken, Is.EqualTo(1));
            Assert.That(s2.MyPods[1].CheckpointsTaken, Is.EqualTo(0));
            Assert.That(s2.HisPods[0].CheckpointsTaken, Is.EqualTo(0));
            Assert.That(s2.HisPods[1].CheckpointsTaken, Is.EqualTo(0));

            Assert.That(s2.MyPods[0].TimeWithoutCheckpoint, Is.EqualTo(0));
            Assert.That(s2.MyPods[1].TimeWithoutCheckpoint, Is.EqualTo(1));
            Assert.That(s2.HisPods[0].TimeWithoutCheckpoint, Is.EqualTo(1));
            Assert.That(s2.HisPods[1].TimeWithoutCheckpoint, Is.EqualTo(1));

        }

        [Test]
        public void FillBoostAbility()
        {
            var persister = new StatePersister();
            var s1 = StateReader.Read("1|2|1000 1000|9000 1000",
                "0 1000 0 0 186 1|7314 817 0 0 186 1|9313 881 0 0 185 1|6315 785 0 0 187 1");
            s1.IsInitialState = true;
            persister.FillState(s1);
            Assert.That(s1.MyPods[0].CanBoost, Is.True);
            Assert.That(s1.MyPods[1].CanBoost, Is.True);
            persister.RegisterMoves(new[]
            {
                new PodMove(new VecD(0, 0), 0, MoveType.Boost),
                new PodMove(new VecD(0, 0), Constants.MaxThrust)
            });
            var s2 = StateReader.Read("1|2|1000 1000|9000 1000",
                "2000 1000 0 0 186 2|7314 817 0 0 186 1|9313 881 0 0 185 1|6315 785 0 0 187 1");
            persister.FillState(s2);

            Assert.That(s2.MyPods[0].CanBoost, Is.False);
            Assert.That(s2.MyPods[1].CanBoost, Is.True);
        }

        [Test]
        public void FillShieldState()
        {
            var persister = new StatePersister();
            var s1 = StateReader.Read("1|2|1000 1000|9000 1000",
                "0 1000 0 0 186 1|7314 817 0 0 186 1|9313 881 0 0 185 1|6315 785 0 0 187 1");
            s1.IsInitialState = true;
            persister.FillState(s1);
            Assert.That(s1.MyPods[0].ShieldTicksLeft, Is.Zero);
            Assert.That(s1.MyPods[1].ShieldTicksLeft, Is.Zero);
            persister.RegisterMoves(new[] {
                new PodMove(new VecD(0, 0), 0, MoveType.Shield),
                new PodMove(new VecD(0, 0), Constants.MaxThrust)
            });
            var s2 = StateReader.Read("1|2|1000 1000|9000 1000",
                "2000 1000 0 0 186 2|7314 817 0 0 186 1|9313 881 0 0 185 1|6315 785 0 0 187 1");
            persister.FillState(s2);

            Assert.That(s2.MyPods[0].ShieldTicksLeft, Is.EqualTo(3));
            Assert.That(s2.MyPods[1].ShieldTicksLeft, Is.EqualTo(0));
        }
    }
}
