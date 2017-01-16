namespace AirportsConnections.Library.Graphs
{
    public class Edge
    {
        /// <summary>
        /// Path from one vertex to another
        /// </summary>
        /// <param name="path"></param>
        public Edge(string path) : this(path, 0)
        {
            var edges = path.Split('-');
            Weight = edges.Length == 3 ? int.Parse(edges[2]) : 0;
        }

        /// <summary>
        /// Path from one vertex to another
        /// </summary>
        /// <param name="path"></param>
        /// <param name="weight">Weight of connecting edge</param>
        public Edge(string path, int weight)
        {
            var edges = path.Split('-');
            Start = edges[0];
            End = edges[1];
            Weight = weight;
        }

        public string End { get; }
        public string Path => Start + "-" + End;
        public string Start { get; }
        public int Weight { get; private set; }
    }
}