using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using MyNetworkGraph.Core.Model;

namespace MyNetworkGraph.Core.IO;

public static class CsvGraphLoader
{
    // CSV format:
    // DugumId, Ozellik_I, Ozellik_II, Ozellik_III, Komsular
    // 1,0.8,12,3,"2,4,5"
    public static MyNetworkGraph.Core.Graph.Graph Load(string path)
    {
        var g = new MyNetworkGraph.Core.Graph.Graph();

        var lines = File.ReadAllLines(path);
        if (lines.Length == 0) return g;

        // 1) Node'ları ekle
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = SplitCsvLine(line);
            if (parts.Count < 4) continue;

            int id = int.Parse(parts[0], CultureInfo.InvariantCulture);
            double active = double.Parse(parts[1], CultureInfo.InvariantCulture);
            double interaction = double.Parse(parts[2], CultureInfo.InvariantCulture);
            double degreeHint = double.Parse(parts[3], CultureInfo.InvariantCulture);

            var node = new Node(id, $"User {id}", active, interaction, degreeHint);
            g.AddNode(node);
        }

        // 2) Edge'leri ekle (komşular)
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = SplitCsvLine(line);
            if (parts.Count < 5) continue;

            int id = int.Parse(parts[0], CultureInfo.InvariantCulture);
            string neighborsRaw = parts[4].Trim().Trim('"');

            if (string.IsNullOrWhiteSpace(neighborsRaw)) continue;

            var neigh = neighborsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var s in neigh)
            {
                if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int nb))
                    continue;

                if (g.HasNode(id) && g.HasNode(nb))
                {
                    try { g.AddEdge(id, nb); }
                    catch { /* duplicate vb. ignore */ }
                }
            }
        }

        g.RecomputeDegreeHints();
        g.RecalcAllWeights();
        return g;
    }

    private static List<string> SplitCsvLine(string line)
    {
        var res = new List<string>();
        bool inQuotes = false;
        var cur = new StringBuilder();

        foreach (char ch in line)
        {
            if (ch == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (ch == ',' && !inQuotes)
            {
                res.Add(cur.ToString().Trim());
                cur.Clear();
            }
            else
            {
                cur.Append(ch);
            }
        }

        res.Add(cur.ToString().Trim());
        return res;
    }
}


