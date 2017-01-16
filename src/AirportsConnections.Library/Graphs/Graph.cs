using System.Collections.Generic;
using System.Linq;
//https://bitbucket.org/boriscosic/backend-flitetrakr/src/0ee9ae830663fd24fd162986da57b7c931414817/src/main/java/com/boriscosic/flitetrakr/graph/Graph.java?at=master&fileviewer=file-view-default
namespace AirportsConnections.Library.Graphs
{
    public class Graph
    {
        private readonly IList<Node> _nodes;
        private readonly IList<Edge> _edges;
        private IDictionary<string, int> _distance;
        private IDictionary<string, string> _previous;
        private IList<string> _traversed;

        public Graph(IEnumerable<string> flights)
        {
            _nodes = new List<Node>();
            _edges = new List<Edge>();
            _traversed = new List<string>();
            foreach (var flight in flights)
                ParseNodeAndEdge(flight);
        }

        /// <summary>
        /// Traverse a given set of edges to determine cost
        /// </summary>
        /// <param name="route">string in XXX-YYY-123 format</param>
        /// <returns></returns>
        public IList<Edge> Traverse(string route)
        {
            var foundEdges = new List<Edge>();
            var routes = route.Split('-');
            for (var i = 0; i < routes.Length; i++)
            {
                if (i == routes.Length - 1)
                    break;

                var from = routes[i];
                var to = routes[i + 1];

                var edge = FindEdge($"{from}-{to}");
                if (edge == null)
                    return null;
                foundEdges.Add(edge);
            }
            return foundEdges;
        }

        /// <summary>
        /// Search all connecting nodes to determine viable paths
        /// </summary>
        /// <param name="route">string in format XXX-YYY</param>
        /// <param name="iterations">How many times to iterate</param>
        /// <returns>Return list of paths that were iterated.</returns>
        public IList<string> Depth(string route, int iterations)
        {            
            _traversed = new List<string>();
            var start = route.Split('-');
            var queue = new Queue<string>();            
            queue.Enqueue(start[0]);
            Iterate(queue, string.Empty, iterations * _edges.Count);
            return _traversed;
        }

        /// <summary>
        /// Greedy function that search in circles until iterations are exceeded or no neighboors are found.
        /// </summary>
        /// <param name="queue">Queue of node names to visit</param>
        /// <param name="path">Path visit so far</param>
        /// <param name="iterations">Maximum number of iterations</param>
        /// <returns></returns>
        private bool Iterate(Queue<string> queue, string path, int iterations)
        {
            var node = queue.Dequeue();
            path = path + node + "-";

            var neighbours = FindNeighbours(FindNode(node));
            if (path.Split('-').Length > iterations)
            {
                _traversed.Add(path);
                return true;
            }
            foreach (var e in neighbours)
            {
                Node neighbour = FindNode(e.End);
                if (queue.Count == 0 ||
                    neighbour != null && !queue.Contains(neighbour.Name))
                    queue.Enqueue(e.End);
                return Iterate(queue, path, iterations);
            }

            return true;
        }

        /// <summary>
        /// Create split edges and vertices when start is the same as end.
        /// </summary>
        /// <param name="node">Node name that has the same start and end</param>
        /// <param name="dest">Edge containing new path when start is the same as end</param>
        public void splitEdge(string node, Edge dest)
        {
            AddNode(dest.Start);
            AddNode(dest.End);
            var newEdges = new List<Edge>();

            foreach (var e in _edges)
            {
                if (e.Start == node)
                    newEdges.Add(new Edge(dest.Start + "-" + e.End, e.Weight));
                else if (e.End == node)
                    newEdges.Add(new Edge(e.Start + "-" + dest.End, e.Weight));
            }

            foreach (var e in newEdges)
                _edges.Add(e);
        }

        public IList<string> Path(string route)
        {
            var path = route.Split('-');
            _distance = new Dictionary<string, int>();
            _previous = new Dictionary<string, string>();

            foreach (var n in _nodes)
            {
                _distance.Add(n.Name, int.MaxValue);
                _previous.Add(n.Name, null);
            }

            _distance.Add(path[0], 0);

            var quest = new List<string> {path[0]};

            while (quest.Count > 0)
            {
                var n = FindMinNode();
                quest.Remove(n.Name);
                var neighbours = FindNeighbours(n);

                foreach (var e in neighbours)
                {
                    var nbWeight = e.Weight + _distance[n.Name];

                    var neighbour = FindNode(e.End);
                    if (neighbour != null && quest.IndexOf(neighbour.Name) == -1 && !neighbour.IsTraveled)
                        quest.Add(neighbour.Name);

                    if (neighbour != null)
                        if ((_distance[neighbour.Name] > nbWeight))
                        {
                            _distance.Add(neighbour.Name, nbWeight);
                            _previous.Add(neighbour.Name, n.Name);
                        }
                }
            }

            return findRoute(path[1]);
        }

        /// <summary>
        ///  Look at connecting neighbours of a node and return them in a list
        /// </summary>
        /// <param name="node"> Node to look at</param>
        /// <returns>List of edges that lead away from the node</returns>
        private IEnumerable<Edge> FindNeighbours(Node node)
        {
            var neighbours = new List<Edge>();

            foreach (var e in _edges.Where(x => x.Start == node.Name))
            {
                neighbours.Add(e);
            }
            return neighbours;
        }

        /// <summary>
        /// Find the minimum path node for Dijkstra
        /// </summary>
        /// <returns>Node with minimum distance</returns>
        private Node FindMinNode()
        {
            Node minNode = null;
            var minVal = int.MaxValue; //warning: why?
            foreach (var o in _distance)
            {
                var n = FindNode(o.Key);
                if ((n != null) && (o.Value <= minVal) && !n.IsTraveled)
                {
                    minVal = o.Value;
                    minNode = n;
                }
            }

            if (minNode != null)
                minNode.IsTraveled = true;

            return minNode;
        }

        /// <summary>
        /// Determine the cost and shortest route by reversing visited routes.
        /// </summary>
        /// <param name="string">string containing target to trace back from</param>
        /// <returns></returns>
        private IList<string> findRoute(string target)
        {
            var route = new List<string>();
            var last = _previous[target];
            while (last != null)
            {
                route.Add(last);
                last = _previous[last];
            }
            route.Reverse();
            route.Add(_distance[target].ToString());
            return route;
        }

        private void ParseNodeAndEdge(string path)
        {
            var edge = new Edge(path);
            AddNode(edge.Start);
            AddNode(edge.End);
            AddEdge(edge.Path, edge.Weight);
        }

        private void AddEdge(string path, int weight)
        {
            if (FindEdge(path) == null)
                _edges.Add(new Edge(path, weight));
        }

        private void AddNode(string name)
        {
            if (FindNode(name) == null)
                _nodes.Add(new Node(name));
        }

        private Edge FindEdge(string path)
        {
            var e = _edges.FirstOrDefault(x => x.Path == path);

            return e;
        }

        private Node FindNode(string name)
        {
            var n = _nodes.FirstOrDefault(x => x.Name == name);
            return n;
        }
    }
}