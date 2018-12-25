using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class conway_sequence
{
    static void Main(string[] args)
    {
        int R = int.Parse(Console.ReadLine());
        int L = int.Parse(Console.ReadLine());

        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");

        var sequence = new Queue<int>();
        sequence.Enqueue(R);
        for (int i = 1; i < L; i++)
        {
            var prev = new Stack<int>(sequence.Reverse());
            sequence = new Queue<int>();

            Console.Error.WriteLine("Prev: " + string.Join(" ", prev));

            int v = prev.Pop();

            int count = 1;
            while (prev.Any())
            {
                if (v != prev.Peek())
                {
                    Console.Error.WriteLine("Encode: {0} {1}", count, v);
                    sequence.Enqueue(count);
                    sequence.Enqueue(v);
                    count = 1;
                }
                else
                {
                    count++;
                }
                v = prev.Pop();

            }
            Console.Error.WriteLine("Encode: {0} {1}", count, v);
            sequence.Enqueue(count);
            sequence.Enqueue(v);
            Console.Error.WriteLine("Cur: " + string.Join(" ", sequence));
        }

        Console.WriteLine(string.Join(" ", sequence));
    }
}
