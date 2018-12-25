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
class ShadowsOfTheKnight1
{
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int W = int.Parse(inputs[0]); // width of the building.
        int H = int.Parse(inputs[1]); // height of the building.
        int N = int.Parse(Console.ReadLine()); // maximum number of turns before game over.
        inputs = Console.ReadLine().Split(' ');
        int X0 = int.Parse(inputs[0]);
        int Y0 = int.Parse(inputs[1]);

        int prevX0 = 0;
        int prevY0 = 0;
        string prevDir = "";

        double minW = 0;
        double maxW = W - 1;
        double minH = 0;
        double maxH = H - 1;


        Console.Error.WriteLine("Building W:{0} H:{1}; Jumps: {2}", W, H, N);

        // game loop
        while (true)
        {
            string bombDir = Console.ReadLine(); // the direction of the bombs from batman's current location (U, UR, R, DR, D, DL, L or UL)

            Console.Error.WriteLine("Prev Dir: {0}; Current Dir: {1}", prevDir, bombDir);
            Console.Error.WriteLine("Min W:H: {0}:{1}; Max W:H: {2}:{3}", minW, minH, maxW, maxH);

            double adjX = 0;
            double adjY = 0;
            foreach (var token in bombDir)
            {
                switch (token)
                {
                    case 'R':
                        minW = X0;
                        adjX = Math.Max(1, (maxW - X0) / 2);
                        break;
                    case 'L':
                        maxW = X0;
                        adjX = -Math.Max(1, ((X0 - minW) / 2));
                        break;
                    case 'U':
                        maxH = Y0;
                        adjY = -Math.Max(1, ((Y0 - minH) / 2));
                        break;
                    case 'D':
                        minH = Y0;
                        adjY = Math.Max(1, ((maxH - Y0) / 2));
                        break;


                }
            }

            Console.Error.WriteLine("AdjX: {0}, AdjY: {1}", adjX, adjY);
            X0 += (int)Math.Floor(adjX);
            Y0 += (int)Math.Floor(adjY);

            prevDir = bombDir;
            prevX0 = X0;
            prevY0 = Y0;

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");


            // the location of the next window Batman should jump to.
            Console.WriteLine("{0} {1}", Math.Min(W - 1, X0), Math.Min(H - 1, Y0));
        }
    }
}