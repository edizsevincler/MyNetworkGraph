using System;
using System.Collections.Generic;
using System.Text;

namespace MyNetworkGraph.Core.Model;

public class Node
{
    public int Id { get; }
    public string Label { get; set; }

    // Dinamik ağırlık için özellikler
    public double Active { get; set; }
    public double Interaction { get; set; }
    public double DegreeHint { get; set; }

    // UI koordinatları
    public double X { get; set; }
    public double Y { get; set; }

    public Node(int id, string label, double active, double interaction, double degreeHint)
    {
        Id = id;
        Label = label;
        Active = active;
        Interaction = interaction;
        DegreeHint = degreeHint;
    }

    public override string ToString()
        => $"Node{{id={Id}, label='{Label}'}}";
}
