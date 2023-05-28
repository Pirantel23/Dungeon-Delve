using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;


namespace Tests
{
    public class LevelGenerationTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void RoomEqualTest()
        {
            var r1 = new Room(4, 5);
            var r2 = new Room(4, 5);
            Assert.True(r1.Equals(r2));
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator LevelGenerationTestsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}


