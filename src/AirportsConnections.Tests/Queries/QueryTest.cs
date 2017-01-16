using System.Collections.Generic;
using AirportsConnections.Library.Graphs;
using AirportsConnections.Library.Queries;
using NUnit.Framework;

namespace AirportsConnections.Tests.Queries
{
    [TestFixture]
    public class QueryTest
    {
        private Graph g;
        private IList<string> _paths;
        private Query q;
      

        [SetUp]
        public void SetUp()
        {
            _paths = new List<string>
            {
                "aaa-bbb-1",
                "bbb-ccc-2",
                "ddd-eee-2",
                "eee-fff-2",
                "ddd-fff-3",
                "aaa-fff-5",
                "ddd-bbb-1",
                "fff-aaa-6",
                "ccc-fff-1"
            };
            g = new Graph(_paths);
            q = new Query(_paths, g);
        }
  [Test]
        public void HowManyConnectionWithStops_Should_find_1_connection_with_stops()
  {
      var r = q.HowManyConnectionWithStops("aaa-fff", 0, 3);
            Assert.AreEqual("1", r);
        }
        [Test]
        public void WhatAreConnectionsBelowPrice_Should_find_no_connections_below_price()
        {
            Assert.AreEqual(string.Empty, q.WhatAreConnectionsBelowPrice("aaa-fff", 10));
        }

        [Test]
        public void WhatIsThePriceOfConnection_Should_find_cheapest_connection()
        {
            var r = q.WhatIsThePriceOfConnection("aaa-bbb-ccc");
            Assert.AreEqual("3", r);
        }

        [Test]
        public void WhatIsThePriceOfConnection_Should_Not_find_Cheapest_Connection()
        {
            var r = q.WhatIsThePriceOfConnection("aaa-bbb-ccc-ddd");
            Assert.AreEqual("No such connection found!", r);
        }
    }
}