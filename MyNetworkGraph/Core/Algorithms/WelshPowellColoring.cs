using System.Collections.Generic;
using System.Linq;

namespace MyNetworkGraph.Core.Algorithms;

public class WelshPowellColoring
{
    // Çıktı: nodeId -> colorIndex (0,1,2,...)
    public Dictionary<int, int> Color(
        MyNetworkGraph.Core.Graph.Graph g, List<int> communityNodeIds)
    {
        var nodes = communityNodeIds
            .OrderByDescending(id => g.Degree(id))
            .ThenBy(id => id)
            .ToList();

        var color = new Dictionary<int, int>();

        foreach (var u in nodes)
        {
            // u'nun komşularının kullandığı renkleri topla
            var used = new HashSet<int>();
            foreach (var e in g.EdgesOf(u))
            {
                int v = e.Other(u);
                if (color.TryGetValue(v, out int c))
                    used.Add(c);
            }

            // en küçük kullanılmayan rengi ver
            int cmin = 0;
            while (used.Contains(cmin)) cmin++;
            color[u] = cmin;
        }

        return color;
    }

    // Yardımcı: tüm grafı community'lere bölüp her biri için renklendirme
    public List<(List<int> Community, Dictionary<int, int> Coloring)> ColorAllCommunities(
        MyNetworkGraph.Core.Graph.Graph g)
    {
        var comps = new ConnectedComponents().Run(g);
        var result = new List<(List<int>, Dictionary<int, int>)>();

        foreach (var comp in comps)
        {
            var map = Color(g, comp);
            result.Add((comp, map));
        }

        return result;
    }
}

