using System;
using System.Linq;


class last_crusade_1
{

    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int W = int.Parse(inputs[0]); // number of columns.
        int H = int.Parse(inputs[1]); // number of rows.

        var rooms = new int[W, H];
        for (int h = 0; h < H; h++)
        {
            string LINE = Console.ReadLine(); // represents a line in the grid and contains W integers. Each integer represents one room of a given type.
            Console.Error.WriteLine("Rooms at {0}. {1}", h, LINE);
            string[] types = LINE.Split(' ');
            for (int w = 0; w < W; w++)
            {
                rooms[w, h] = int.Parse(types[w]);
            }
        }
        int EX = int.Parse(Console.ReadLine()); // the coordinate along the X axis of the exit (not useful for this first mission, but must be read).

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int XI = int.Parse(inputs[0]);
            int YI = int.Parse(inputs[1]);
            string POS = inputs[2];

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            int roomType = rooms[XI, YI];

            Console.Error.WriteLine("Type {0} at ({1},{2}) from {3}", roomType, XI, YI, POS);

            if (new[] { 1, 3, 7, 8, 9, 12, 13 }.Contains(roomType))
            {
                YI++;
            }
            else if (roomType == 2 || roomType == 6)
            {
                XI += POS == "LEFT" ? 1 : -1;
            }
            else if (roomType == 4)
            {
                if (POS == "TOP") XI--;
                else YI++;
            }
            else if (roomType == 5)
            {
                if (POS == "TOP") XI++;
                else YI++;
            }
            else if (roomType == 10) XI--;
            else if (roomType == 11) XI++;

            // One line containing the X Y coordinates of the room in which you believe Indy will be on the next turn.
            Console.WriteLine("{0} {1}", XI, YI);
        }
    }
}