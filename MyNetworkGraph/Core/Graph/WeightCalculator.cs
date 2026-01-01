using System;
using System.Collections.Generic;
using System.Text;
using MyNetworkGraph.Core.Model;

namespace MyNetworkGraph.Core.Graph;

public static class WeightCalculator
{
    // weight(i,j) = 1 / (1 + (Ai-Aj)^2 + (Ei-Ej)^2 + (Bi-Bj)^2)
    public static double Weight(Node a, Node b)
    {
        double da = a.Active - b.Active;
        double di = a.Interaction - b.Interaction;
        double dd = a.DegreeHint - b.DegreeHint;

        double denom = 1.0 + da * da + di * di + dd * dd;
        return 1.0 / denom;
    }
}
