using System.Collections.Generic;

namespace MyNetworkGraph.Core.Algorithms;

public class ConnectedComponents
{
    public List<List<int>> Run(MyNetworkGraph.Core.Graph.Graph g)
    {
        var components = new List<List<int>>();
        var visited = new HashSet<int>();

        foreach (var start in g.Nodes.Keys)
        {
            if (visited.Contains(start)) continue;

            var comp = new List<int>();
            var queue = new Queue<int>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                int u = queue.Dequeue();
                comp.Add(u);

                foreach (var e in g.EdgesOf(u))
                {
                    int v = e.Other(u);
                    if (visited.Add(v))
                        queue.Enqueue(v);
                }
            }

            components.Add(comp);
        }

        return components;
    }
}
