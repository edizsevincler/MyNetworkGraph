using System.IO;
using System.Linq;
using System.Text;

namespace MyNetworkGraph.Core.IO;

public static class AdjacencyExporter
{
    // Komşuluk listesi (txt)
    public static void ExportAdjacencyList(MyNetworkGraph.Core.Graph.Graph g, string path)
    {
        var sb = new StringBuilder();

        foreach (var id in g.Nodes.Keys.OrderBy(x => x))
        {
            var neighbors = g.EdgesOf(id)
                .Select(e => e.Other(id))
                .Distinct()
                .OrderBy(x => x);

            sb.Append(id).Append(": ");
            sb.AppendLine(string.Join(", ", neighbors));
        }

        File.WriteAllText(path, sb.ToString());
    }

    // Komşuluk matrisi (csv)
    public static void ExportAdjacencyMatrixCsv(MyNetworkGraph.Core.Graph.Graph g, string path)
    {
        var ids = g.Nodes.Keys.OrderBy(x => x).ToList();
        var index = ids.Select((id, i) => (id, i)).ToDictionary(x => x.id, x => x.i);

        int n = ids.Count;
        int[,] mat = new int[n, n];

        foreach (var u in ids)
        {
            foreach (var e in g.EdgesOf(u))
            {
                int v = e.Other(u);
                int i = index[u], j = index[v];
                mat[i, j] = 1;
                mat[j, i] = 1;
            }
        }

        var sb = new StringBuilder();
        sb.Append(";");
        sb.AppendLine(string.Join(";", ids));

        for (int i = 0; i < n; i++)
        {
            sb.Append(ids[i]).Append(";");
            for (int j = 0; j < n; j++)
            {
                sb.Append(mat[i, j]);
                if (j < n - 1) sb.Append(";");
            }
            sb.AppendLine();
        }

        File.WriteAllText(path, sb.ToString());
    }
}

