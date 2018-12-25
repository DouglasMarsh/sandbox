using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class network_cabling
{
    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        int x0 = int.MaxValue;
        int x1 = int.MinValue;

        int countAbove = 0;
        long totalAbove = 0;
        int countBelow = 0;
        long totalBelow = 0;
        var bInRow = new SortedDictionary<int, int>();
        int maxInRow = 0;
        int medianValue = 0;
        long answer = 0;
        if (N > 1)
        {
            for (int i = 0; i < N; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');
                int X = int.Parse(inputs[0]);
                int Y = int.Parse(inputs[1]);

                if (!bInRow.ContainsKey(Y)) bInRow.Add(Y, 0);
                bInRow[Y] += 1;
                if (bInRow[Y] > maxInRow)
                {
                    maxInRow = bInRow[Y];
                    medianValue = Y;
                }

                x0 = Math.Min(X, x0);
                x1 = Math.Max(X, x1);

                if (Y > 0)
                {
                    totalAbove += Math.Abs(Y);
                    countAbove++;
                }
                else
                {
                    totalBelow += Math.Abs(Y);
                    countBelow++;
                }
            }
            if (maxInRow == 1)
            {

                totalAbove = totalBelow = 0;
                countAbove = countBelow = 0;


                Console.Error.WriteLine("Recalculate Median");
                int medianIdx = ((bInRow.Count + 1) / 2) - 1;
                medianValue = bInRow.Keys.ToArray()[medianIdx];
                Console.Error.Write("Y Values:: ");
                foreach (var row in bInRow)
                {
                    Console.Error.Write(row.Key + " ");

                    // we are just creating 1 total
                    totalAbove += Math.Abs(medianValue - row.Key);
                }
                Console.Error.WriteLine("; Median:" + medianValue);

            }
            else
            {
                Console.Error.WriteLine("Trunk Length: {0}, Above: {1}/{2}, Below: {3}/{4}",
                    (x1 - x0), totalAbove, countAbove, totalBelow, countBelow);

                totalAbove -= (medianValue * countAbove);
                totalBelow += (medianValue * countBelow);

                Console.Error.WriteLine("Adjusted:: Trunk Length: {0}, Above: {1}/{2}, Below: {3}/{4}, Y: {5}",
                    (x1 - x0), totalAbove, countAbove, totalBelow, countBelow, medianValue);
            }
            Console.Error.WriteLine("Adjusted:: Trunk Length: {0}, Above: {1}, Below: {2}, mV: {3}",
                    (x1 - x0), totalAbove, totalBelow,  medianValue);
            answer = (x1 - x0) + totalAbove + Math.Abs(totalBelow);
        }

        Console.WriteLine(answer);
    }

}
