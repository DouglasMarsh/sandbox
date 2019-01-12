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
class Anakin2
{
    static void Main(string[] args)
    {
        string[] inputs;
        bool initializing = true;
        Point prevCheckPoint = new Point(0,0);
        Point myPrevLoc = new Point(0,0);
        Point opPrevLoc = new Point(0,0);
        int prevDist = 0;
        int prevAngle = 0;
        int velocity = 0;
        int opVelocity = 0;
        
        State state = State.Approach;
        bool boostAvailable = true;
        Track track = new Track();
        string msg = "";

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            Point myLoc = new Point(int.Parse(inputs[0]), int.Parse(inputs[1]));
            Point checkpoint = new Point(int.Parse(inputs[2]), int.Parse(inputs[3]));

            int dist = int.Parse(inputs[4]); // distance to the next checkpoint
            int angle = int.Parse(inputs[5]); // angle between your pod orientation and the direction of the next checkpoint
            inputs = Console.ReadLine().Split(' ');
            Point opLoc = new Point(int.Parse(inputs[0]), int.Parse(inputs[1]));
            
            if( initializing ) {
                initializing = false;
                myPrevLoc = new Point( myLoc );
                prevCheckPoint = new Point( checkpoint );
                prevDist = dist;
                prevAngle = angle;
                opPrevLoc = opLoc;

                track.Add( myLoc);
                track.Add( checkpoint );
            }           
            velocity = Util.Distance(myPrevLoc, myLoc);
            opVelocity = Util.Distance(opPrevLoc, opLoc);
            
            Console.Error.WriteLine("State: {2}; Dist: {0}; Angle: {1}", dist, angle, state);

            Console.Error.WriteLine(
                "My:: Loc {0}; Velocity: {1}; Op:: Loc {2}; Velocity: {3};",
                myLoc, velocity,opLoc, opVelocity);
                
            if( checkpoint != prevCheckPoint){
                state = State.Reorient;
                track.Add(checkpoint);
                Console.Error.WriteLine("Reorienting to CheckPoint: {0}",checkpoint);
            }

            double maxThrust = 100;
            if( state == State.Reorient){
                if( Math.Abs(angle) < 25 ){
                    state = State.Approach;
                }
                else
                {
                    maxThrust = 95;
                    if( dist < 3000){
                        maxThrust = 90;
                    }
                }
            }
            
            double adjustThrust = 0;
            if( state == State.Approach){
                // we missed the checkpoint
                if( dist - prevDist > 0 && Math.Abs(angle) > 5) {
                    adjustThrust = maxThrust *.9;
                    Console.Error.WriteLine("Missed Checkpoint");
                }
                else if( dist < 2500){
                    if( track.IsComplete)
                    {
                        Point nextCP = track.Next(checkpoint);
                        double nextAngle = Math.Abs(Util.Angle(myLoc, nextCP));
                        int nextDist = Util.Distance(checkpoint, nextCP);

                        Console.Error.WriteLine("Next CP:: {0}; Dist: {1}; Angle: {2}", nextCP, nextDist, nextAngle);
                        if( nextAngle > 90 )
                        {
                            adjustThrust = maxThrust * .25;
                            
                        }
                        else if( nextAngle > 45 )
                        {
                            adjustThrust = maxThrust * .10;
                        }
                        if( nextDist < 3000 && velocity > 500) adjustThrust *= 2.5;
                    }
                    else
                    {
                        adjustThrust = maxThrust * .10;
                    }

                    if( Math.Abs(angle) > Math.Abs(prevAngle))
                    {
                        maxThrust *= .75;
                    }
                }
            }
            else if( state == State.Reorient){
                int absAngle = Math.Abs(angle);
                if( absAngle > 90 ){
                    if( dist < 3000 && velocity > 400) {
                        adjustThrust = maxThrust * .75;
                    }
                    else {
                        adjustThrust = maxThrust * .5;
                    }
                }
                else {                    
                    if( dist < 3000 && velocity > 400) {
                        adjustThrust = absAngle;
                    }
                    else {
                        adjustThrust = absAngle*.5;
                    }
                }
            }

            Console.Error.WriteLine("MaxThrust: {0}; AdjustThrust: {1}", maxThrust, adjustThrust);
            string thrust = ((int)Math.Ceiling(Math.Max( maxThrust * .1,(maxThrust - adjustThrust)))).ToString();                        
            if(boostAvailable && thrust == "100" && dist > 3000){
                thrust = "BOOST";
                boostAvailable = false;
            }

            Console.WriteLine(checkpoint + " " + thrust + msg);

            myPrevLoc = new Point( myLoc );
            prevCheckPoint = new Point( checkpoint );
            prevDist = dist;
            prevAngle = angle;
            opPrevLoc = opLoc;
        }
    }

    struct Point
    {
        public int X {get;set;}
        public int Y {get;set;}

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public Point(Point p1): this( p1.X, p1.Y) {}
        public static bool operator == (Point p1, Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }
        public static bool operator != (Point p1, Point p2)
        {
            return !p1.Equals(p2);
        }
        public bool Equals(Point p)
        {
            return p == this;
        }
        public override bool Equals(object obj)
        {
            if(obj is Point)
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
    class Util
    {
        public static int Distance(Point p1, Point p2)
        {
            if( p1 == p2 ) return 0;

            return (int)Math.Sqrt(Math.Pow( p2.X - p1.X, 2) + Math.Pow( p2.Y - p1.Y, 2));
        }
        public static int Angle(Point p1, Point p2)
        {
            double deltaY = (p1.Y - p2.Y);
            double deltaX = (p2.X - p1.X);
            double radians = Math.Atan2(deltaY, deltaX); 
            double degrees = (180 / Math.PI) * radians;
    
            degrees = degrees > 180 ? degrees - 180 : degrees;
            return (int)degrees;
        }
        public static double ToRadians( double degrees )
        {
            return degrees * Math.PI/180;
        }
        public static double ToDegrees( double radians )
        {
            return radians * 180/Math.PI;
        }
    }
    class Track: List<Point>
    {
        public bool IsComplete {get; private set;}
        public Point Next(Point checkpoint)
        {            
            if( !IsComplete) return checkpoint;
            int idx = this.IndexOf( checkpoint );
            if( idx+1 == this.Count) return this[0];

            return this[ idx + 1];            
        }

        public new void Add(Point p)
        {
            if( this.Contains(p))
            {
                this.IsComplete = true;
                return;
            }
            base.Add(p);
        }
    }
    enum State
    {
        None = 0,
        Approach,
        Reorient
    }
    
}