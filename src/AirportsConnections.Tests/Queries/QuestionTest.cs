using AirportsConnections.Library.Queries;
using NUnit.Framework;
using System;

using Query = AirportsConnections.Library.Queries;

namespace AirportsConnections.Tests.Queries
{
    [TestFixture]
    public class QuestionTest
    {
        public void IsQuestionEqual(String message, Query.Type type, String route, int number, int value, Question q)
        {
            Assert.AreEqual(type, q.Type, message);
            Assert.AreEqual(route, q.Route, message);
            Assert.AreEqual(number, q.Number, message);
            Assert.AreEqual(value, q.Value, message);
        }
        [Test]
        public void Should_parse_question()
        {
            Question question = new Question("#1: What is the price of the connection NUE-FRA-LHR?");
            IsQuestionEqual("Should parse question", Query.Type.Price, "NUE-FRA-LHR", 1, 0, question);
        }
        [Test]
        public void Should_parse_oversized_question()
        {
            var question = new Question("#1: What is the price of the connection NUE-FRA-LHR after December 1?");
            IsQuestionEqual("Should parse oversized question", Query.Type.Price, "NUE-FRA-LHR", 1, 1, question);
        }
        [Test]
        public void Should_fail_without_flight_path()
        {
            var question = new Question("What is the price of the connection");
            IsQuestionEqual("Should fail without flight path", Query.Type.Unknown, string.Empty, int.MaxValue, 0, question);
        }
        [Test]
        public void Should_fail_on_long_airport_codes()
        {
            var question = new Question("What is the price of the connection NUE-FRA-LHFFR");
            IsQuestionEqual("Should fail on long airport codes", Query.Type.Unknown, string.Empty, int.MaxValue, 0, question);
        }
        [Test]
        public void Should_fail_on_short_airport_codes()
        {
            var question = new Question("What is the price of the connection NU-FA-LR");
            IsQuestionEqual("Should fail on short airport codes", Query.Type.Unknown, string.Empty, int.MaxValue, 0, question);
        }
        [Test]
        public void Should_find_cheapest()
        {
            var question = new Question("#4: What is the cheapest connection from NUE to AMS?");
            IsQuestionEqual("Should find cheapest", Query.Type.Cheapest, "NUE-AMS", 4, 0, question);
        }
        [Test]
        public void Should_find_maximum_connections()
        {
            var question = new Question("#6: How many with maximum 3 stops exists between NUE and FRA?");
            IsQuestionEqual("Should find maximum connections", Query.Type.MaximumConnections, "NUE-FRA", 6, 3, question);
        }
        [Test]
        public void Should_find_minimum_connections()
        {
            var question = new Question("#7: How many with minimum 3 stops exists between NUE and FRA?");
            IsQuestionEqual("Should find minimum connections", Query.Type.MinimumConnections, "NUE-FRA", 7, 3, question);
        }
        [Test]
        public void Should_find_exact_connections()
        {
            var question = new Question("#8: How many with exactly 3 stops exists between NUE and FRA?");
            IsQuestionEqual("Should find exact connections", Query.Type.ExactConnections, "NUE-FRA", 8, 3, question);
        }
        [Test]
        public void Should_find_all_connections()
        {
            var question = new Question("#9: Find all conenctions from NUE to LHR below 170Euros!");
            IsQuestionEqual("Should find all connections", Query.Type.BelowPrice, "NUE-LHR", 9, 170, question);
        }
    }
}