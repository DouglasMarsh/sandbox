using System;
using System.Collections.Generic;
using System.Linq;

class dwarfs
{
    static void Main(string[] args)
    {
        var graph = new DirectedGraph();
        int n = int.Parse(Console.ReadLine()); // the number of relationships of influence
        for (int i = 0; i < n; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            int x = int.Parse(inputs[0]); // a relationship of influence between two people (x influences y)
            int y = int.Parse(inputs[1]);

            Console.Error.WriteLine("{0} influence {1}", x, y);
            graph.AddEdge(x, y);
        }

        var result = new List<int>();
        var search = new List<int>();
        foreach (var v in graph.Keys)
        {
            search = graph.LongestPath(v);
            if (search.Count > result.Count) result = search;


            search = new List<int>();
        }


        // The number of people involved in the longest succession of influences
        Console.WriteLine(result.Count);
    }
    class DirectedGraph : Dictionary<int, HashSet<int>>
    {

        public void AddEdge(int n1, int n2)
        {
            if (!ContainsKey(n1)) Add(n1, new HashSet<int>());
            this[n1].Add(n2);

        }

        public List<int> LongestPath(int root)
        {
            var result = new List<int>();
            var search = new List<int>();

            if (this.ContainsKey(root))
            {
                foreach (var dependant in this[root])
                {
                    search.AddRange(LongestPath(dependant));

                    if (search.Count > result.Count)
                    {
                        result = search;
                    }
                    search = new List<int>();
                }
            }
            result.Insert(0, root);
            return result;
        }
    }
}
