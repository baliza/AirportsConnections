using AirportsConnections.Library.Answers;
using NUnit.Framework;

namespace AirportsConnections.Tests.Answers
{
    [TestFixture]
    public class AnswerTest
    {
        [Test]
        public void TestFormat()
        {
            var r1 = Answer.Format(1, "This is a test");
            Assert.AreEqual("#1: This is a test", r1, "Should be valid format");
            var r2 = Answer.Format(2, "");
            Assert.AreEqual("2: No such connection found!", r2, "Should be no connections");
        }
    }
}