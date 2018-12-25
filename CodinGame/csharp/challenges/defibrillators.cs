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
class Defibrillators
{
    static void Main(string[] args)
    {
        double LON = ToRadians(double.Parse(Console.ReadLine().Replace(',', '.'))); 
        double LAT = ToRadians(double.Parse(Console.ReadLine().Replace(',', '.')));
        int N = int.Parse(Console.ReadLine());

        var locations = new SortedList<double, Location>();
        for (int i = 0; i < N; i++)
        {
            var l = new Location(Console.ReadLine());
            var d = Distance(LAT, LON, l.Latitude, l.Longitude);
            Console.Error.WriteLine("{0}:: {1}", l.Name, d);

            locations.Add(d, l);
        }

        Console.WriteLine(locations.First().Value.Name);

    }
    static double ToRadians(double degree)
    {
        return degree * Math.PI / 180;
    }
    static double Distance(
        double latA, double longA, double latB, double longB)
    {
        double x = (longB - longA) * Math.Cos((latA + latB) * .5);
        double y = (latB - latA);

        return Math.Sqrt((x*x)+(y*y)) * 6371;
    }
    class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public Location(string loc)
        {
            Console.Error.WriteLine(loc);

            var tokens = loc.Split(';');
            Id = int.Parse(tokens[0]);
            Name = tokens[1];
            Address = tokens[2];
            PhoneNumber = tokens[3];
            Longitude = ToRadians(double.Parse(tokens[4].Replace(',', '.')));
            Latitude = ToRadians(double.Parse(tokens[5].Replace(',', '.')));
        }
    }
}
