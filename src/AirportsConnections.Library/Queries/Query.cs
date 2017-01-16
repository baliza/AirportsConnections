using AirportsConnections.Library.Answers;
using AirportsConnections.Library.Graphs;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AirportsConnections.Library.Queries
{
    public class Query
    {
        private readonly IList<string> _flights;
        private Graph _graph;

        /// <summary>
        /// Ask a query to ran against all the flight possibilities
        /// </summary>
        /// <param name="flights">Array list of flights obtained from Parser.getFlights()</param>
        public Query(IList<string> flights)
        {
            this._flights = flights;
        }

        public Query(IList<string> flights, Graph graph)
        {
            this._graph = graph;
            this._flights = flights;
        }

        /// <summary>
        /// Ask a query to determine a flight answer
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>string answer to flight query</returns>
        public string AskQuestion(string query)
        {
            _graph = new Graph(_flights);
            var question = new Question(query);

            switch (question.Type)
            {
                case Type.Price:
                case Type.Cheapest:
                    return Answer.Format(question.Number, WhatIsTheCheapestConnection(question.Route));

                case Type.BelowPrice:
                    return Answer.Format(question.Number, WhatAreConnectionsBelowPrice(question.Route, question.Value));

                case Type.MaximumConnections:
                    return Answer.Format(question.Number, HowManyConnectionWithStops(question.Route, 0, question.Value));

                case Type.ExactConnections:
                    return Answer.Format(question.Number, HowManyConnectionWithStops(question.Route, question.Value, question.Value));

                case Type.MinimumConnections:
                    return Answer.Format(question.Number, HowManyConnectionWithStops(question.Route, question.Value, 10));

                default:
                    return Answer.UnknownQuestion(query);
            }
        }

        /// <summary>
        /// Return a connection that satisfies a condition of min and max stop
        /// </summary>
        /// <param name="route"></param>
        /// <param name="minStops">Number of minimum stops</param>
        /// <param name="maxStops">Number of maximum stops</param>
        /// <returns>Number of stops with given stop parameters</returns>
        public string HowManyConnectionWithStops(string route, int minStops, int maxStops)
        {
            var edge = new Edge(route);

            var connections = _graph.Depth(edge.Start, maxStops);
            /*
             *  Pattern pattern = Pattern.compile(String.format("^%s(-[A-z]{3}){%d,%d}-%s",
                edge.getStart(), minStops, maxStops, edge.getEnd()));
             */
            var pattern = $"{edge.Start}(-[A-z]{{3}}){{{minStops},{maxStops}}}-{edge.End}";
            var regex = new Regex(pattern);

            var routes = new List<string>();
            foreach (var conn in connections)
            {
                var matcher = regex.Match(conn);
                if (!matcher.Success)
                    continue;
                var match = matcher.Groups[0].Value;
                if (!routes.Contains(match))
                    routes.Add(match);
            }

            return routes.Count.ToString();
        }

        /// <summary>
        /// Find connections below given price
        /// </summary>
        /// <param name="route"></param>
        /// <param name="price">Price to search under</param>
        /// <returns>List all paths below given price</returns>
        public string WhatAreConnectionsBelowPrice(string route, int price)
        {
            var path = new Edge(route);
            var connections = _graph.Depth(route, 30);
            //Pattern pattern = Pattern.compile(string.Format("^%s(-[A-z]{3}){%d,%d}-%s",
            //        path.Start, 0, int.MaxValue, path.End));
            //var pattern = string.Format("^%s(-[A-z]{3}){%d,%d}-%s",path.Start, 0, int.MaxValue, path.End);
            var pattern = $"{path.Start}(-[A-z]{{3}}){{{0},{int.MaxValue}}}-{path.End}";
            var regex = new Regex(pattern);
            var routes = new List<string>();

            foreach (var conn in connections)
            {
                var matcher = regex.Match(conn);
                if (!matcher.Success) continue;

                var match = matcher.Groups[0].Value;

                var priceOfConnection = WhatIsThePriceOfConnection(match);
                var routePrice = 0;
                if (!int.TryParse(priceOfConnection, out routePrice) || routePrice >= price) continue;

                var newRoute = $"{match}-{routePrice}";
                if (routes.IndexOf(newRoute) == -1)
                    routes.Add(newRoute);
            }

            var result = routes.Aggregate((current, s) => current + ", " + s);

            return result;
        }

        /// <summary>
        /// Determine the cheapest connections for the route
        /// </summary>
        /// <param name="route">Cheapest connection option</param>
        /// <returns></returns>
        public string WhatIsTheCheapestConnection(string route)
        {
            Edge org = new Edge(route), dest = new Edge(route);
            if (dest.Start == dest.End)
            {
                dest = new Edge("START-END");
                _graph.splitEdge(org.Start, dest);
            }

            var optimalRoute = _graph.Path(dest.Path);
            int cheapestCost = int.Parse(optimalRoute[optimalRoute.Count - 1]);

            if (cheapestCost == int.MaxValue)
                return string.Empty;

            optimalRoute[0] = org.Start;
            optimalRoute[optimalRoute.Count - 1] = org.End;

            var result = optimalRoute.Aggregate("", (current, s) => current + (s + "-"));
            return result + cheapestCost;
        }

        /// <summary>
        /// Determine the lower price given a set of nodes
        /// </summary>
        /// <param name="route">Route to validate or price check</param>
        /// <returns>string with price or no connections found</returns>
        public string WhatIsThePriceOfConnection(string route)
        {
            var edges = _graph.Traverse(route);
            if (edges == null)
                return "No such connection found!";
            var cost = edges.Sum(e => e.Weight);
            return cost.ToString();
        }
    }
}