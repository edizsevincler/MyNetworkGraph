using System;
using System.Collections.Generic;
using System.Text;

namespace MyNetworkGraph.Core.Model;

public class Edge
{
    public int U { get; }
    public int V { get; }
    public double Weight { get; set; }

    public Edge(int a, int b, double weight)
    {
        if (a == b)
            throw new ArgumentException("Self-loop yasak (u==v).");

        U = Math.Min(a, b);
        V = Math.Max(a, b);
        Weight = weight;
    }

    public bool Connects(int a, int b)
    {
        int x = Math.Min(a, b), y = Math.Max(a, b);
        return U == x && V == y;
    }

    public int Other(int nodeId)
    {
        if (nodeId == U) return V;
        if (nodeId == V) return U;
        throw new ArgumentException("Edge bu node'u içermiyor.");
    }
}
