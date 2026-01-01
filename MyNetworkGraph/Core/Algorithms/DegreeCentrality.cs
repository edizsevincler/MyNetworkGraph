using System.Collections.Generic;
using System.Linq;

namespace MyNetworkGraph.Core.Algorithms;

public class DegreeCentrality
{
    public List<(int NodeId, int Degree)> TopK(
        MyNetworkGraph.Core.Graph.Graph g, int k = 5)
    {
        var list = new List<(int NodeId, int Degree)>();

        foreach (var id in g.Nodes.Keys)
        {
            int degree = g.Degree(id);
            list.Add((id, degree));
        }

        return list
            .OrderByDescending(x => x.Degree)
            .ThenBy(x => x.NodeId)
            .Take(k)
            .ToList();
    }
}

