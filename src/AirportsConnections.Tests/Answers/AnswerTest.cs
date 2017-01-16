using AirportsConnections.Library.Answers;
using NUnit.Framework;

namespace AirportsConnections.Tests.Answers
{
    [TestFixture]
    public class AnswerTest
    {
        [Test]
        public void Format_processes_valid_input()
        {
            var r = Answer.Format(1, "This is a test");
            Assert.AreEqual("#1: This is a test", r, "Should be valid format");
        }

        [Test]
        public void Format_does_NOT_process_invalid_answer()
        {
            var r = Answer.Format(2, string.Empty);
            Assert.AreEqual("2: No such connection found!", r, "Should be no connections");
        }

        [Test]
        public void Format_does_NOT_process_min_int_number()
        {
            var r = Answer.Format(int.MinValue, "This is a test");
            Assert.AreEqual(int.MinValue.ToString(), r, "Should be no connections");
        }
    }
}