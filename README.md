classDiagram
    class Graph {
        +Dictionary<int, Node> Nodes
        +List<Edge> Edges
        +AddNode()
        +AddEdge()
        +Degree()
        +HasNode()
    }

    class Node {
        +int Id
        +double Aktiflik
        +int Etkilesim
        +int BaglantiSayisi
    }

    class Edge {
        +int From
        +int To
        +double Weight
    }

    class Bfs {
        +Run(Graph, int)
    }

    class Dfs {
        +Run(Graph, int)
    }

    class Dijkstra {
        +Run(Graph, int, int)
    }

    class AStar {
        +Run(Graph, int, int)
    }

    class WelshPowellColoring {
        +ColorAllCommunities(Graph)
    }

    Graph --> Node
    Graph --> Edge
    Graph --> Bfs
    Graph --> Dfs
    Graph --> Dijkstra
    Graph --> AStar
    Graph --> WelshPowellColoring

    flowchart TD
    A[Baþla] --> B[Baþlangýç düðümü seç]
    B --> C[Mesafeleri sonsuz yap]
    C --> D[En küçük mesafeli düðümü seç]
    D --> E{Hedef mi?}
    E -- Hayýr --> F[Komþularý güncelle]
    F --> D
    E -- Evet --> G[En kýsa yolu yazdýr]
    G --> H[Bitir]

    | Algoritma    | Düðüm Sayýsý | Süre (ms) |
| ------------ | ------------ | --------- |
| BFS          | 20           | 2         |
| DFS          | 20           | 1         |
| Dijkstra     | 50           | 8         |
| A*           | 50           | 5         |
| Welsh–Powell | 50           | 3         |

Bu projede, sosyal aðlar graf veri yapýsý kullanýlarak modellenmiþ ve çeþitli graf algoritmalarý ile analiz edilmiþtir.
Geliþtirilen uygulama sayesinde kullanýcýlar arasýndaki iliþkiler görselleþtirilmiþ,
en kýsa yollar, topluluklar ve etkili kullanýcýlar baþarýyla tespit edilmiþtir.
Uygulama, nesne yönelimli programlama prensiplerine uygun olarak geliþtirilmiþ
ve kullanýcý dostu bir arayüz ile desteklenmiþtir.