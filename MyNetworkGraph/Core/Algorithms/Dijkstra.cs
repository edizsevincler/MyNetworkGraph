using System.Collections.Generic;

namespace MyNetworkGraph.Core.Algorithms;

public class Dijkstra
{
    public (double Distance, List<int> Path) Run(
        MyNetworkGraph.Core.Graph.Graph g, int start, int goal)
    {
        var dist = new Dictionary<int, double>();
        var prev = new Dictionary<int, int?>();
        var pq = new PriorityQueue<int, double>();

        foreach (var id in g.Nodes.Keys)
        {
            dist[id] = double.PositiveInfinity;
            prev[id] = null;
        }

        if (!g.HasNode(start) || !g.HasNode(goal))
            return (double.PositiveInfinity, new List<int>());

        dist[start] = 0;
        pq.Enqueue(start, 0);

        while (pq.Count > 0)
        {
            int u = pq.Dequeue();
            if (u == goal) break;

            foreach (var e in g.EdgesOf(u))
            {
                int v = e.Other(u);
                double alt = dist[u] + e.Weight;

                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                    pq.Enqueue(v, alt);
                }
            }
        }

        if (double.IsInfinity(dist[goal]))
            return (double.PositiveInfinity, new List<int>());

        // Path oluştur
        var path = new List<int>();
        for (int? at = goal; at != null; at = prev[at.Value])
            path.Add(at.Value);

        path.Reverse();
        return (dist[goal], path);
    }
}

