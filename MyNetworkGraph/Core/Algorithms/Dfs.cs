using System.Collections.Generic;

namespace MyNetworkGraph.Core.Algorithms;

public class Dfs
{
    public List<int> Run(MyNetworkGraph.Core.Graph.Graph g, int start)
    {
        if (!g.HasNode(start)) return new();

        var order = new List<int>();
        var vis = new HashSet<int>();
        var stack = new Stack<int>();

        stack.Push(start);

        while (stack.Count > 0)
        {
            int u = stack.Pop();
            if (!vis.Add(u)) continue;

            order.Add(u);

            foreach (var e in g.EdgesOf(u))
            {
                int v = e.Other(u);
                if (!vis.Contains(v))
                    stack.Push(v);
            }
        }

        return order;
    }
}
