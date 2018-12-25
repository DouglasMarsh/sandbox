using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class stock_exchange_losses
{
    static void Main(string[] args)
    {
        var analysis = new Dictionary<int, int>();
        int n = int.Parse(Console.ReadLine());
        string[] inputs = Console.ReadLine().Split(' ');

        int maxLoss = 0;
        for (int i = 0; i < n; i++)
        {
            int v = int.Parse(inputs[i]);
            var keys = analysis.Keys.ToArray();
            foreach (var k in keys)
            {
                analysis[k] = v < k ? v - k : analysis[k];
                maxLoss = analysis[k] < maxLoss ? analysis[k] : maxLoss;
            }
            if (v > Math.Abs(maxLoss)) analysis[v] = 0;
        }


        Console.WriteLine(maxLoss);
    }
}