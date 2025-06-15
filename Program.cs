using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KruskalTesting
{
    public class Edge : IComparable<Edge>
    {
        public int Source, Destination, Weight;
        public Edge(int source, int dest, int weight)
        {
            Source = source;
            Destination = dest;
            Weight = weight;
        }

        public int CompareTo(Edge other)
        {
            return this.Weight.CompareTo(other.Weight);
        }
    }

    public class UnionFind
    {
        private int[] parent, rank;

        public UnionFind(int size)
        {
            parent = new int[size];
            rank = new int[size];
            for (int i = 0; i < size; i++)
                parent[i] = i;
        }

        public int Find(int i)
        {
            if (parent[i] != i)
                parent[i] = Find(parent[i]);
            return parent[i];
        }

        public void Union(int x, int y)
        {
            int xRoot = Find(x), yRoot = Find(y);
            if (xRoot == yRoot) return;

            if (rank[xRoot] < rank[yRoot])
                parent[xRoot] = yRoot;
            else if (rank[xRoot] > rank[yRoot])
                parent[yRoot] = xRoot;
            else
            {
                parent[yRoot] = xRoot;
                rank[xRoot]++;
            }
        }
    }

    public class Kruskal
    {
        public static (List<Edge> mst, int totalWeight) FindMST(List<Edge> edges, int vertexCount)
        {
            List<Edge> result = new List<Edge>();
            edges.Sort();
            UnionFind uf = new UnionFind(vertexCount);
            int totalWeight = 0;

            foreach (var edge in edges)
            {
                if (uf.Find(edge.Source) != uf.Find(edge.Destination))
                {
                    result.Add(edge);
                    totalWeight += edge.Weight;
                    uf.Union(edge.Source, edge.Destination);
                }
            }

            return (result, totalWeight);
        }
    }

    class Program
    {
        static Random rnd = new Random();

        static List<Edge> GenerateGraph(string type, int vertices, int edges)
        {
            List<Edge> edgeList = new List<Edge>();
            HashSet<(int, int)> existing = new HashSet<(int, int)>();

            if (type == "tree")
            {
                // Генерация дерева: соединяем i с случайным j < i
                for (int i = 1; i < vertices; i++)
                {
                    int j = rnd.Next(0, i);
                    int w = rnd.Next(1, 20);
                    edgeList.Add(new Edge(i, j, w));
                    existing.Add((Math.Min(i, j), Math.Max(i, j)));
                }
                return edgeList;
            }

            int maxPossibleEdges = vertices * (vertices - 1) / 2;
            int targetEdges = Math.Min(edges, maxPossibleEdges);

            while (edgeList.Count < targetEdges)
            {
                int u = rnd.Next(vertices);
                int v = rnd.Next(vertices);
                if (u == v) continue;
                var key = (Math.Min(u, v), Math.Max(u, v));
                if (existing.Contains(key)) continue;
                existing.Add(key);
                int w = rnd.Next(1, 20);
                edgeList.Add(new Edge(u, v, w));
            }

            return edgeList;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Тип графа\tВершины\tРёбра\tВес MST\tВремя (мс)\tПримечания");

            TestGraph("Полный граф", 10, 45, "Граф полностью связан");
            TestGraph("Разреженный граф", 10, 15, "Граф с небольшим количеством рёбер");
            TestGraph("Полный граф", 20, 190, "Полный граф с увеличенным числом вершин");
            TestGraph("Разреженный граф", 20, 30, "Граф с увеличенным числом вершин и рёбер");
            TestGraph("Дерево", 15, 14, "Граф уже является остовным деревом", "tree");
            TestGraph("Циклический граф", 10, 20, "Граф с циклами");
            TestGraph("Полный граф", 50, 1225, "Тест на большом графе");
            TestGraph("Разреженный граф", 50, 75, "Граф с небольшим количеством рёбер и вершин");
        }

        static void TestGraph(string graphType, int vertices, int edges, string comment, string mode = "normal")
        {
            var graph = GenerateGraph(mode, vertices, edges);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var (mst, totalWeight) = Kruskal.FindMST(graph, vertices);
            sw.Stop();

            Console.WriteLine($"{graphType}\t{vertices}\t{edges}\t{totalWeight}\t{sw.ElapsedMilliseconds}\t{comment}");
        }
    }
}
