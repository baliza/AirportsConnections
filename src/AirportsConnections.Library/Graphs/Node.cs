//https://bitbucket.org/boriscosic/backend-flitetrakr/src/0ee9ae830663fd24fd162986da57b7c931414817/src/main/java/com/boriscosic/flitetrakr/graph/Node.java?at=master&fileviewer=file-view-default

namespace AirportsConnections.Library.Graphs
{
    public class Node
    {
        public Node(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }

        public bool IsTraveled { get; set; }
        
    }
}
