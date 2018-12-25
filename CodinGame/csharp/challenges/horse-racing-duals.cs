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
class Horse_Racing_Duals
{
    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        var horses = new SortedSet<int>();

        for (int i = 0; i < N; i++)
        {
            int h = int.Parse(Console.ReadLine());
            if (!horses.Add(h))
            {
                Console.WriteLine("0");
                return;
            }
        }

        if (horses.Count == 1)
        {
            Console.WriteLine("0");
            return;
        }

        int diff = int.MaxValue;
        int prev = -1;
        int current = -1;
        foreach (var h in horses)
        {
            if (current > -1) prev = current;
            current = h;

            if (prev > -1)
            {
                int newDiff = (current - prev);
                diff = newDiff < diff ? newDiff : diff;
            }
        }

        Console.WriteLine(diff);
    }
}