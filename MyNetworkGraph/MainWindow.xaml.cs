using Microsoft.Win32;
using MyNetworkGraph.Core.IO;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using MyNetworkGraph.Core.Algorithms;
using System.Linq;


namespace MyNetworkGraph;

public partial class MainWindow : Window
{
    private MyNetworkGraph.Core.Graph.Graph _graph = new();
    private System.Collections.Generic.Dictionary<int, int>? _nodeColors;
    private System.Collections.Generic.HashSet<string>? _highlightEdges;


    public MainWindow()
    {


        InitializeComponent();
        Log("Hazır. CSV Yükle ile başlayabilirsin.");
    }
    private string GetOutputFolder()
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string folder = System.IO.Path.Combine(desktop, "MyNetworkGraph_Output");
        System.IO.Directory.CreateDirectory(folder);
        return folder;
    }

    private bool TryGetStartGoal(out int start, out int goal)
    {
        start = 0; goal = 0;

        if (!int.TryParse(TxtStart.Text, out start) || !int.TryParse(TxtGoal.Text, out goal))
        {
            Log("Start/Goal sayısal olmalı.");
            return false;
        }

        if (_graph.Nodes.Count == 0)
        {
            Log("Önce CSV yükle.");
            return false;
        }

        if (!_graph.HasNode(start) || !_graph.HasNode(goal))
        {
            Log($"Geçersiz start/goal. Start={start} Goal={goal} graf içinde yok.");
            return false;
        }

        return true;
    }

    // ================= CSV =================

    private void BtnLoadCsv_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv",
            Title = "CSV seç"
        };

        if (dlg.ShowDialog() != true) return;

        _graph = CsvGraphLoader.Load(dlg.FileName);
        Log($"CSV yüklendi: {System.IO.Path.GetFileName(dlg.FileName)}");
        Log($"Node: {_graph.Nodes.Count}, Edge: {_graph.GetAllEdges().Count}");

        AutoLayout();
        DrawGraph();
    }

    private void BtnExportAdjList_Click(object sender, RoutedEventArgs e)
    {
        if (_graph.Nodes.Count == 0)
        {
            Log("Önce CSV yükle.");
            return;
        }

        try
        {
            string folder = GetOutputFolder();
            string path = System.IO.Path.Combine(folder, "adjacency_list.txt");


            MyNetworkGraph.Core.IO.AdjacencyExporter.ExportAdjacencyList(_graph, path);

            Log($"Komşuluk listesi kaydedildi: {path}");
        }
        catch (Exception ex)
        {
            Log("HATA (AdjList): " + ex.Message);
        }
    }
    private void BtnExportLog_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string path = System.IO.Path.Combine(desktop, "log.txt");

            System.IO.File.WriteAllText(path, TxtLog.Text);
            Log($"Log kaydedildi: {path}");
        }
        catch (Exception ex)
        {
            Log("HATA (Log export): " + ex.Message);
        }
    }
    private void BtnSaveCanvasPng_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string folder = GetOutputFolder();
            string fileName = $"canvas_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string path = System.IO.Path.Combine(folder, fileName);

            var rtb = new RenderTargetBitmap(
                (int)GraphCanvas.ActualWidth,
                (int)GraphCanvas.ActualHeight,
                96, 96,
                System.Windows.Media.PixelFormats.Pbgra32);

            rtb.Render(GraphCanvas);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Create))
            {
                encoder.Save(fs);
            }

            Log($"Canvas PNG kaydedildi: {path}");
        }
        catch (Exception ex)
        {
            Log("HATA (Canvas PNG): " + ex.Message);
        }
    }


    private void BtnExportAdjMatrix_Click(object sender, RoutedEventArgs e)
    {
        if (_graph.Nodes.Count == 0)
        {
            Log("Önce CSV yükle.");
            return;
        }

        try
        {
            string folder = GetOutputFolder();
            string path = System.IO.Path.Combine(folder, "adjacency_matrix.csv");


            MyNetworkGraph.Core.IO.AdjacencyExporter.ExportAdjacencyMatrixCsv(_graph, path);

            Log($"Komşuluk matrisi kaydedildi: {path}");
        }
        catch (Exception ex)
        {
            Log("HATA (AdjMatrix): " + ex.Message);
        }
    }


    // ================= BFS / DFS =================

    private void BtnBfs_Click(object sender, RoutedEventArgs e)
    {
        if (_graph.Nodes.Count == 0) { Log("Önce CSV yükle."); return; }

        int start = 1;
        var sw = Stopwatch.StartNew();
        var bfs = new Bfs();
        var order = bfs.Run(_graph, start);
        sw.Stop();

        Log($"BFS({start}) [{sw.ElapsedMilliseconds} ms]: " + string.Join(" -> ", order));
    }

    private void BtnDfs_Click(object sender, RoutedEventArgs e)
    {
        if (_graph.Nodes.Count == 0) { Log("Önce CSV yükle."); return; }

        int start = 1;
        var dfs = new MyNetworkGraph.Core.Algorithms.Dfs();
        var order = dfs.Run(_graph, start);

        Log($"DFS({start}) sırası: " + string.Join(" -> ", order));
    }

    // ================= DIJKSTRA / A* =================

    private void BtnDijkstra_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetStartGoal(out int start, out int goal)) return;

        var dij = new MyNetworkGraph.Core.Algorithms.Dijkstra();
        var result = dij.Run(_graph, start, goal);

        if (double.IsInfinity(result.Distance) || result.Path.Count == 0)
        {
            Log($"Dijkstra {start}→{goal}: yol yok.");
            return;
        }

        _highlightEdges = BuildEdgeSetFromPath(result.Path);
        DrawGraph();

        Log($"Dijkstra {start}→{goal}: Mesafe={result.Distance:F4} | Yol: " +
            string.Join(" -> ", result.Path));
    }

    private void BtnAStar_Click(object sender, RoutedEventArgs e)
    {
        if (!TryGetStartGoal(out int start, out int goal)) return;

        var astar = new MyNetworkGraph.Core.Algorithms.AStar();
        var result = astar.Run(_graph, start, goal);

        if (double.IsInfinity(result.Distance) || result.Path.Count == 0)
        {
            Log($"A* {start}→{goal}: yol yok.");
            return;
        }

        _highlightEdges = BuildEdgeSetFromPath(result.Path);
        DrawGraph();

        Log($"A* {start}→{goal}: Mesafe={result.Distance:F4} | Yol: " +
            string.Join(" -> ", result.Path));
    }
    private void BtnWelshPowell_Click(object sender, RoutedEventArgs e)
    {
        if (_graph.Nodes.Count == 0) { Log("Önce CSV yükle."); return; }

        var wp = new MyNetworkGraph.Core.Algorithms.WelshPowellColoring();
        var coloredComps = wp.ColorAllCommunities(_graph);

        // nodeId -> globalColorIndex
        _nodeColors = new System.Collections.Generic.Dictionary<int, int>();

        int offset = 0;
        foreach (var (community, coloring) in coloredComps)
        {
            int usedInThisCommunity = coloring.Values.DefaultIfEmpty(-1).Max() + 1;

            foreach (var kv in coloring)
            {
                int nodeId = kv.Key;
                int localColor = kv.Value;
                _nodeColors[nodeId] = offset + localColor;
            }

            Log($"Topluluk: {community.Count} düğüm | Renk sayısı: {usedInThisCommunity}");
            offset += usedInThisCommunity; // diğer toplulukların renkleri çakışmasın
        }

        DrawGraph();
        Log($"Welsh–Powell bitti. Toplam renk: {offset} | Topluluk sayısı: {coloredComps.Count}");
    }
    private void BtnDegree_Click(object sender, RoutedEventArgs e)
    {
        if (_graph.Nodes.Count == 0)
        {
            Log("Önce CSV yükle.");
            return;
        }

        var list = _graph.Nodes.Values
            .Select(n => new
            {
                NodeId = n.Id,
                Degree = _graph.Degree(n.Id)
            })
            .OrderByDescending(x => x.Degree)
            .Take(5)
            .ToList();

        LvDegree.ItemsSource = list;
        Log("Degree Centrality hesaplandı (Top 5).");
    }



    // ================= DRAW =================
    private void BtnTop5_Click(object sender, RoutedEventArgs e)
    {
        if (_graph.Nodes.Count == 0) { Log("Önce CSV yükle."); return; }

        var dc = new MyNetworkGraph.Core.Algorithms.DegreeCentrality();
        var top = dc.TopK(_graph, 5);

        Log("Top-5 Degree Centrality:");
        int rank = 1;
        foreach (var item in top)
        {
            Log($"{rank}) Node {item.NodeId}  Degree={item.Degree}");
            rank++;
        }
    }
    private void BtnColoring_Click(object sender, RoutedEventArgs e)
    {
        if (_graph.Nodes.Count == 0) { Log("Önce CSV yükle."); return; }

        var wp = new MyNetworkGraph.Core.Algorithms.WelshPowellColoring();
        var colored = wp.ColorAllCommunities(_graph);

        // tüm toplulukların renklerini tek dictionary’de birleştir
        _nodeColors = new System.Collections.Generic.Dictionary<int, int>();

        int communityIndex = 1;
        foreach (var (community, coloring) in colored)
        {
            Log($"Topluluk {communityIndex}: " + string.Join(", ", community.OrderBy(x => x)));
            Log($"  Kullanılan renk sayısı: {coloring.Values.Distinct().Count()}");

            foreach (var kv in coloring)
                _nodeColors[kv.Key] = kv.Value;

            communityIndex++;
        }

        DrawGraph(); // yeniden çiz → node’lar renklenmiş olacak
    }
    private void BtnScreenshot_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string folder = GetOutputFolder();
            string path = System.IO.Path.Combine(folder, $"canvas_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            var width = (int)GraphCanvas.ActualWidth;
            var height = (int)GraphCanvas.ActualHeight;

            if (width <= 0 || height <= 0)
            {
                Log("Canvas boyutu 0 görünüyor. Önce graf çizili olsun.");
                return;
            }

            var rtb = new System.Windows.Media.Imaging.RenderTargetBitmap(
                width, height, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);

            rtb.Render(GraphCanvas);

            var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(rtb));

            using (var fs = System.IO.File.Create(path))
            {
                encoder.Save(fs);
            }

            Log($"Screenshot kaydedildi: {path}");
        }
        catch (Exception ex)
        {
            Log("HATA (Screenshot): " + ex.Message);
        }
    }


    private void AutoLayout()
    {
        double w = GraphCanvas.ActualWidth;
        double h = GraphCanvas.ActualHeight;
        if (w < 10) w = 700;
        if (h < 10) h = 600;

        double cx = w / 2;
        double cy = h / 2;
        double r = Math.Min(w, h) * 0.35;

        var ids = _graph.Nodes.Keys.OrderBy(x => x).ToList();
        int n = ids.Count;

        for (int i = 0; i < n; i++)
        {
            double ang = 2 * Math.PI * i / n;
            int id = ids[i];
            _graph.Nodes[id].X = cx + r * Math.Cos(ang);
            _graph.Nodes[id].Y = cy + r * Math.Sin(ang);
        }
    }

    private void DrawGraph()
    {
        GraphCanvas.Children.Clear();

        foreach (var e in _graph.GetAllEdges())
        {
            var a = _graph.Nodes[e.U];
            var b = _graph.Nodes[e.V];

            bool highlighted = IsEdgeHighlighted(e.U, e.V);

            GraphCanvas.Children.Add(new Line
            {
                X1 = a.X,
                Y1 = a.Y,
                X2 = b.X,
                Y2 = b.Y,
                Stroke = highlighted ? Brushes.Red : Brushes.Gray,
                StrokeThickness = highlighted ? 5 : 2
            });
        }


        foreach (var n in _graph.Nodes.Values)
        {
            var el = new Ellipse
            {
                Width = 32,
                Height = 32,
                Fill = GetBrushForNode(n.Id),
                Stroke = Brushes.White,
                StrokeThickness = 2,
                Tag = n.Id
            };

            Canvas.SetLeft(el, n.X - 16);
            Canvas.SetTop(el, n.Y - 16);
            el.MouseLeftButtonDown += Node_Click;
            GraphCanvas.Children.Add(el);

            GraphCanvas.Children.Add(new System.Windows.Controls.TextBlock
            {
                Text = n.Id.ToString(),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold
            });
        }

    }
    private System.Collections.Generic.HashSet<string> BuildEdgeSetFromPath(System.Collections.Generic.IReadOnlyList<int> path)
    {
        var set = new System.Collections.Generic.HashSet<string>();

        for (int i = 0; i < path.Count - 1; i++)
        {
            int a = path[i];
            int b = path[i + 1];
            set.Add(NormEdgeKey(a, b));
        }

        return set;
    }

    private bool IsEdgeHighlighted(int a, int b)
    {
        if (_highlightEdges == null) return false;
        return _highlightEdges.Contains(NormEdgeKey(a, b));
    }

    private string NormEdgeKey(int a, int b)
    {
        return (a < b) ? $"{a}-{b}" : $"{b}-{a}";
    }

    private Brush GetBrushForNode(int nodeId)
    {
        if (_nodeColors == null)
            return Brushes.DeepSkyBlue;

        if (!_nodeColors.TryGetValue(nodeId, out int colorIndex))
            return Brushes.DeepSkyBlue;

        Brush[] palette =
        {
        Brushes.DeepSkyBlue,
        Brushes.Orange,
        Brushes.LimeGreen,
        Brushes.MediumPurple,
        Brushes.Yellow,
        Brushes.Crimson,
        Brushes.Cyan,
        Brushes.Gold,
        Brushes.HotPink,
        Brushes.LightGreen
    };

        return palette[colorIndex % palette.Length];
    }


    private void Node_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        int id = (int)((Ellipse)sender).Tag;
        var n = _graph.Nodes[id];
        Log($"Node {id}: Active={n.Active}, Interaction={n.Interaction}, Degree={_graph.Degree(id)}");
    }

    // ================= LOG =================

    private void Log(string msg)
    {
        TxtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\n");
        TxtLog.ScrollToEnd();
    }
}


