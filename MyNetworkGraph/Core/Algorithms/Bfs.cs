using System.Collections.Generic;
using MyNetworkGraph.Core.Graph;

namespace MyNetworkGraph.Core.Algorithms;

public class Bfs
{
    public List<int> Run(MyNetworkGraph.Core.Graph.Graph g, int start)
    {
        if (!g.HasNode(start)) return new();

        var order = new List<int>();
        var vis = new HashSet<int>();
        var q = new Queue<int>();

        vis.Add(start);
        q.Enqueue(start);

        while (q.Count > 0)
        {
            int u = q.Dequeue();
            order.Add(u);

            foreach (var e in g.EdgesOf(u))
            {
                int v = e.Other(u);
                if (vis.Add(v)) q.Enqueue(v);
            }
        }

        return order;
    }
}

