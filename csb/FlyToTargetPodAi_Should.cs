using System;
using NUnit.Framework;

namespace CG.CodersStrikeBack
{
    [TestFixture]
    public class FlyToTargetPodAi_Should
    {
        [Test]
        public void FlyToTarget([Range(0, 1000, 200)] int initialX, [Range(0, 4000, 400)] int initialY)
        {
            // My[0] approaches to target!
            var state = StateReader.Read(
                "3|2|10000 2000|16000 9000",
                $"{initialX} {initialY} 0 0 0 0|100000 2000 0 0 0 1|100000 0 0 0 0 1|100000 1000 0 0 0 1");
            var ai = new FlyToTargetPodAi(new Vec(10000, 0), Constants.MaxThrust, false, false);
            double dist = state.MyPods[0].Pos.DistTo(new Vec(8000, 2000));
            for (int i = 0; i < 10; i++)
            {
                var podMove = ai.GetMove(state.MyPods[0], state);
                var wait = new PodMove(VecD.Zero, 0);
                state.Tick(new[] { podMove, wait }, new[] { wait, wait });
                double newDist = state.MyPods[0].Pos.DistTo(new Vec(8000, 2000));
                Console.WriteLine(i + " pos: " + state.MyPods[0].Pos);
                Assert.That(newDist, Is.LessThan(dist));
                dist = newDist;
            }
        }

        [Test]
        public void DontUseBoost_WhenItIsWastedAlready()
        {
            var state = StateReader.Read(
                "3|2|10000 2000|16000 9000",
                "0 0 0 0 0 0|100000 2000 0 0 0 1|100000 0 0 0 0 1|100000 1000 0 0 0 1");
            var boost = new PodMove(VecD.Zero, 0, MoveType.Boost);
            // waste boost!
            state.Tick(new[]{boost, boost}, new[] { boost, boost });
            var ai = new FlyToTargetPodAi(new Vec(10000, 0), Constants.MaxThrust, true, false);
            var podMove = ai.GetMove(state.MyPods[0], state);
            Assert.That(podMove.MoveType, Is.EqualTo(MoveType.Thrust));
        }

        [Test]
        public void DontShield_IfNoCollisionPredicted()
        {
            //My[0] is close to His[0] but no collision possible
            var state = StateReader.Read(
                "3|2|10000 2000|16000 9000",
                "1000 0 0 0 180 0|100000 2000 0 0 0 1|0 0 -2000 0 0 1|100000 1000 0 0 0 1");
            var ai = new FlyToTargetPodAi(new Vec(0, 0), Constants.MaxThrust, false, true);
            var podMove = ai.GetMove(state.MyPods[0], state);
            Assert.That(podMove.MoveType, Is.EqualTo(MoveType.Thrust));
        }

        [Test]
        public void Shield_IfCollisionPredicted()
        {
            //My[0] collide His[0]
            var state = StateReader.Read(
                "3|2|10000 2000|16000 9000",
                "1000 0 0 0 180 0|100000 2000 0 0 0 1|0 0 2000 0 0 1|100000 1000 0 0 0 1");
            var ai = new FlyToTargetPodAi(new Vec(0, 0), Constants.MaxThrust, false, true);
            var podMove = ai.GetMove(state.MyPods[0], state);
            Assert.That(podMove.MoveType, Is.EqualTo(MoveType.Shield));
        }

        [Test]
        public void DontShield_IfShieldOnCollisionDisabled()
        {
            //My[0] collide His[0], but ShieldOnCollision=false
            var state = StateReader.Read(
                "3|2|10000 2000|16000 9000",
                "1000 0 0 0 180 0|100000 2000 0 0 0 1|0 0 2000 0 0 1|100000 1000 0 0 0 1");
            var ai = new FlyToTargetPodAi(new Vec(0, 0), Constants.MaxThrust, false, false);
            var podMove = ai.GetMove(state.MyPods[0], state);
            Assert.That(podMove.MoveType, Is.EqualTo(MoveType.Thrust));
        }

        [Test]
        public void UseBoost()
        {
            var state = StateReader.Read(
                "3|2|10000 2000|16000 9000",
                "0 0 0 0 0 0|100000 2000 0 0 0 1|100000 0 0 0 0 1|100000 1000 0 0 0 1");
            var ai = new FlyToTargetPodAi(new Vec(10000, 0), Constants.MaxThrust, true, false);
            var podMove = ai.GetMove(state.MyPods[0], state);
            Assert.That(podMove.MoveType, Is.EqualTo(MoveType.Boost));
        }
    }
}
