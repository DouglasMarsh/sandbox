using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Don't let the machines win. You are humanity's last hope...
 **/
class this_is_no_spoon_1
{
    static void Main(string[] args)
    {

        int width = int.Parse(Console.ReadLine()); // the number of cells on the X axis
        int height = int.Parse(Console.ReadLine()); // the number of cells on the Y axis

        var grid = new bool[width, height];
        for (int h = 0; h < height; h++)
        {
            string line = Console.ReadLine(); // width characters, each either 0 or .
            for (int w = 0; w < width; w++)
            {
                grid[w, h] = line[w] == '0';
                if (grid[w, h]) Console.Error.WriteLine("Node at {0} {1}", w, h);
            }
        }

        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                string output = WriteNode(width, w, height, h, grid);
                if (output != null) Console.WriteLine(output);

            }
        }
    }
    static string WriteNode(int maxW, int w, int maxH, int h, bool[,] grid)
    {
        Console.Error.WriteLine("MW:{0}, W:{1}, MH:{2}, H:{3}", maxW, w, maxH, h);

        string output = null;
        if (grid[w, h])
        {
            output = string.Format("{0} {1} {2} {3}",
                w, h, RightNode(maxW, w, h, grid), DownNode(w, maxH, h, grid));
        }

        return output;
    }
    static string RightNode(int maxW, int w, int h, bool[,] grid)
    {
        while (++w < maxW)
        {
            if (grid[w, h])
            {
                return string.Format("{0} {1}", w, h);
            }
        }
        return "-1 -1";
    }
    static string DownNode(int w, int maxH, int h, bool[,] grid)
    {
        while (++h < maxH)
        {
            if (grid[w, h])
            {
                return string.Format("{0} {1}", w, h);
            }
        }
        return "-1 -1";
    }
}