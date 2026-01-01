using System;
using System.Collections.Generic;
using System.Text;
using MyNetworkGraph.Core.Model;
namespace MyNetworkGraph.Core.Graph;

public class Graph
{
    public Dictionary<int, Node> Nodes { get; } = new();
    private readonly Dictionary<int, List<Edge>> _adj = new();
    private readonly HashSet<string> _edgeSet = new();

    public void Clear()
    {
        Nodes.Clear();
        _adj.Clear();
        _edgeSet.Clear();
    }

    public bool HasNode(int id) => Nodes.ContainsKey(id);

    public IReadOnlyList<Edge> EdgesOf(int id) =>
        _adj.TryGetValue(id, out var list) ? list : Array.Empty<Edge>();

    public void AddNode(Node n)
    {
        if (Nodes.ContainsKey(n.Id))
            throw new ArgumentException($"Aynı id ile node var: {n.Id}");

        Nodes[n.Id] = n;
        _adj[n.Id] = new List<Edge>();
    }

    public void RemoveNode(int id)
    {
        if (!Nodes.ContainsKey(id)) return;

        foreach (var e in EdgesOf(id).ToList())
            RemoveEdge(e.U, e.V);

        Nodes.Remove(id);
        _adj.Remove(id);
    }

    public void AddEdge(int a, int b)
    {
        if (a == b) throw new ArgumentException("Self-loop yasak.");
        if (!Nodes.ContainsKey(a) || !Nodes.ContainsKey(b))
            throw new ArgumentException("Node yok.");

        string key = Key(a, b);
        if (_edgeSet.Contains(key))
            throw new ArgumentException("Bu edge zaten var.");

        double w = WeightCalculator.Weight(Nodes[a], Nodes[b]);
        var e = new Edge(a, b, w);

        _adj[a].Add(e);
        _adj[b].Add(e);
        _edgeSet.Add(key);
    }

    public void RemoveEdge(int a, int b)
    {
        string key = Key(a, b);
        if (!_edgeSet.Contains(key)) return;

        if (_adj.TryGetValue(a, out var la))
            la.RemoveAll(e => e.Connects(a, b));

        if (_adj.TryGetValue(b, out var lb))
            lb.RemoveAll(e => e.Connects(a, b));

        _edgeSet.Remove(key);
    }

    public HashSet<Edge> GetAllEdges()
    {
        var set = new HashSet<Edge>();
        foreach (var list in _adj.Values)
            foreach (var e in list)
                set.Add(e);
        return set;
    }

    public int Degree(int id) => EdgesOf(id).Count;

    public void RecomputeDegreeHints()
    {
        foreach (var n in Nodes.Values)
            n.DegreeHint = Degree(n.Id);
    }

    public void RecalcAllWeights()
    {
        foreach (var e in GetAllEdges())
            e.Weight = WeightCalculator.Weight(Nodes[e.U], Nodes[e.V]);
    }

    private static string Key(int a, int b)
    {
        int x = Math.Min(a, b), y = Math.Max(a, b);
        return $"{x}-{y}";
    }
}
