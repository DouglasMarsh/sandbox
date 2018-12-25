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
class mars_lander_2
{
    static void Main(string[] args)
    {
        int landingX0 = -1;
        int landingX1 = -1;
        int landingY = -1;
        int targetX = -1;

        var surface = new List<Point>();

        string[] inputs;
        int surfaceN = int.Parse(Console.ReadLine()); // the number of points used to draw the surface of Mars.
        for (int i = 0; i < surfaceN; i++)
        {
            inputs = Console.ReadLine().Split(' ');

            int landX = int.Parse(inputs[0]); // X coordinate of a surface point. (0 to 6999)
            int landY = int.Parse(inputs[1]); // Y coordinate of a surface point. By linking all the points together in a sequential fashion, you form the surface of Mars.

            var p = new Point(landX, landY);
            if (surface.Any())
            {
                var prev = surface.Last();
                if (prev.Y == landY && landX - prev.X >= 1000)
                {
                    landingX0 = prev.X;
                    landingX1 = p.X;
                    landingY = p.Y;

                    targetX = landingX0 + (landingX1 - landingX0) / 2;
                }
            }
            surface.Add(p);
        }

        Console.Error.WriteLine("LZ: {0} {1} @ Y:{2}", landingX0, landingX1, landingY);

        string state = "ON-APPROACH";

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int X = int.Parse(inputs[0]);
            int Y = int.Parse(inputs[1]);
            int hSpeed = int.Parse(inputs[2]); // the horizontal speed (in m/s), can be negative.
            int vSpeed = int.Parse(inputs[3]); // the vertical speed (in m/s), can be negative.
            int fuel = int.Parse(inputs[4]); // the quantity of remaining fuel in liters.
            int rotate = int.Parse(inputs[5]); // the rotation angle in degrees (-90 to 90).
            int power = int.Parse(inputs[6]); // the thrust power (0 to 4).

            int angle = 0;
            int thrust = 3;
            string output = "0 4\n0 3";
            Console.Error.WriteLine("{0} {1} {2}/{3} {4} {5}",
                        X, Y, hSpeed, vSpeed, rotate, power);

            int distToTarget = X - targetX;
            bool overLanding = X > landingX0 + 250 && X < landingX1 - 250;
            switch (state)
            {
                case "ON-APPROACH":
                    Console.Error.WriteLine("On Approach");
                    angle = distToTarget < 0 ? -23 : 23;
                    thrust = 4;

                    // calc breaking distance
                    int absHSpeed = Math.Abs(hSpeed);
                    int breakingDist = absHSpeed * 3;
                    while (absHSpeed > 0)
                    {
                        breakingDist += (--absHSpeed);
                    }
                    if (Math.Abs(distToTarget) <= breakingDist) state = "REORIENTING";

                    if( Y < landingY )
                    {
                        angle = 0;
                        thrust = 4;
                    }
                    output = string.Format("{0} {1}", angle, thrust);
                    break;
                case "REORIENTING":
                    Console.Error.WriteLine("Reorienting to Vertical");

                    angle = hSpeed < 0 ? -23 : 23;
                    thrust = 4;

                    if (Math.Abs(hSpeed) <= 5)
                    {
                        angle = 0;
                        state = overLanding ? "DECENT" : "ON-APPROACH";
                    }

                    output = string.Format("{0} {1}", angle, thrust);
                    break;
                case "DECENT":
                    Console.Error.WriteLine("Decending");
                    if (Y - landingY < 300) state = "FINAL";

                    angle = 0;
                    thrust = vSpeed < -30 ? 4 :
                             vSpeed < -10 ? 3 : 2;

                    output = string.Format("{0} {1}", angle, thrust);
                    break;
                case "FINAL":
                    Console.Error.WriteLine("On Final Decent");
                    break;
            }

            Console.WriteLine(output);

        }
    }

    struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public Point(Point p1) : this(p1.X, p1.Y) { }

        public static Point Zero { get { return new Point(0, 0); } }
        public static bool operator ==(Point p1, Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }
        public static bool operator !=(Point p1, Point p2)
        {
            return !p1.Equals(p2);
        }
        public bool Equals(Point p)
        {
            return p == this;
        }
        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                return (Point)obj == this;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        public override string ToString()
        {
            return X + " " + Y;
        }
    }
}