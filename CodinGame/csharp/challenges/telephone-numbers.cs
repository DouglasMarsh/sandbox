using System;
using System.Collections.Generic;


class telephone_numbers
{
    static void Main(string[] args)
    {
        var roots = new Dictionary<int, Node>();
        int N = int.Parse(Console.ReadLine());
        int count = 0;
        for (int i = 0; i < N; i++)
        {
            string telephone = Console.ReadLine();

            Node node = null;
            int root = telephone[0] - 48;
            if (!roots.TryGetValue(root, out node))
            {
                node = new Node(root);
                roots.Add(root, node);
                count++;
            }

            for(int t = 1; t < telephone.Length; t++)
            {
                int n = telephone[t] - 48;
                Node nx = null;
                if(!node.Branches.TryGetValue(n, out nx))
                {
                    nx = new Node(n);
                    node.Branches.Add(n, nx);
                    count++;
                }
                node = nx;
            }
        }

       
        
        Console.WriteLine(count);
    }
    class Node
    {
        public int Value { get; set; }
        public Dictionary<int, Node> Branches { get; private set; }

        public Node(int v)
        {
            Value = v;
            Branches = new Dictionary<int,Node>();
        }
    }
}

