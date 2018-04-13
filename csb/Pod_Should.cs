using System;
using NUnit.Framework;

namespace CG.CodersStrikeBack
{
    [TestFixture]
    public class Pod_Should
    {
        [TestCase("0 0 0 0 0 1", "1000 0 100", "100 0 85 0 0 1")]
        [TestCase("0 0 0 0 0 1", "0 1000 100", "95 31 80 26 18 1")]
        [TestCase("10844 2983 241 -222 247 2", "9400 3000 100", "11019 2686 149 -252 229 2")]
        [TestCase("10030 2317 7 128 35 2", "11400 3000 100", "10126 2490 82 146 26 2")]
        [TestCase("1264 3490 -147 -522 291 1", "2000 3000 100", "1180 2890 -71 -509 309 1")]
        public void SimulateMove_WithoutCollisions(string input, string command, string expectedResult)
        {
            var pod = StateReader.ReadPod(input);
            var move = PodMove.Parse(command);
            throw new NotImplementedException("Finish this test! Put here simulation of one tick of the pod move without collisions");
            Assert.AreEqual(expectedResult, pod.ToString());
        }
    }
}
