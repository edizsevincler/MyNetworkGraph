using System.Collections.Generic;

namespace MyNetworkGraph.Core.Algorithms;

public class AStar
{
    // Heuristic: node özellikleri arasındaki "fark" -> ağırlık formülündeki payda kısmı (yaklaşık)
    private static double Heuristic(MyNetworkGraph.Core.Graph.Graph g, int a, int b)
    {
        var na = g.Nodes[a];
        var nb = g.Nodes[b];

        double da = na.Active - nb.Active;
        double di = na.Interaction - nb.Interaction;
        double dd = na.DegreeHint - nb.DegreeHint;

        // daha küçük => daha iyi, 0 olamaz diye 1 ekliyoruz
        return 1.0 + da * da + di * di + dd * dd;
    }

    public (double Distance, List<int> Path) Run(
        MyNetworkGraph.Core.Graph.Graph g, int start, int goal)
    {
        if (!g.HasNode(start) || !g.HasNode(goal))
            return (double.PositiveInfinity, new List<int>());

        var gScore = new Dictionary<int, double>();
        var prev = new Dictionary<int, int?>();
        var open = new PriorityQueue<int, double>();

        foreach (var id in g.Nodes.Keys)
        {
            gScore[id] = double.PositiveInfinity;
            prev[id] = null;
        }

        gScore[start] = 0;
        open.Enqueue(start, Heuristic(g, start, goal)); // f = g + h

        var closed = new HashSet<int>();

        while (open.Count > 0)
        {
            int u = open.Dequeue();
            if (u == goal) break;
            if (!closed.Add(u)) continue;

            foreach (var e in g.EdgesOf(u))
            {
                int v = e.Other(u);
                if (closed.Contains(v)) continue;

                double tentative = gScore[u] + e.Weight;
                if (tentative < gScore[v])
                {
                    gScore[v] = tentative;
                    prev[v] = u;
                    double f = tentative + Heuristic(g, v, goal);
                    open.Enqueue(v, f);
                }
            }
        }

        if (double.IsInfinity(gScore[goal]))
            return (double.PositiveInfinity, new List<int>());

        var path = new List<int>();
        for (int? at = goal; at != null; at = prev[at.Value])
            path.Add(at.Value);

        path.Reverse();
        return (gScore[goal], path);
    }
}

