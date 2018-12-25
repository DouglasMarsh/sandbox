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
class bender_1
{
    static Point _position;
    static bool _reverse = false;
    static bool _breaker = false;
    static char[,] _map = null;
    static HashSet<State>[,] _history = null;
    static string _direction = "SOUTH";
    static List<Point> _teleporters = null;
    static List<string> _output = new List<string>();
    static void Main(string[] args)
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int L = int.Parse(inputs[0]);
        int C = int.Parse(inputs[1]);

        _map = new char[C,L];
        _history = new HashSet<State>[C, L];
        Point start = new Point();
        Point end = new Point();
        for (int l = 0; l < L; l++)
        {
            string row = Console.ReadLine();
            for( int c = 0; c < C; c++)
            {
                _map[c,l] = row[c];
                _history[c, l] = new HashSet<State>();
                if (_map[c, l] == '@') start = new Point(l, c);
                else if (_map[c, l] == '$') end = new Point(l, c);
                else if (_map[c, l] == 'T')
                {
                    _teleporters = _teleporters ?? new List<Point>();
                    _teleporters.Add(new Point(l, c));
                }
            }
        }
        Console.Error.WriteLine("Start: " + start);
        Console.Error.WriteLine("End: " + end);

        _position = start;
        while ( _direction != "LOOP" && _map[_position.C, _position.L] != '$' )
        {
            Move();
            _output.Add(_direction);
            // if state has been duplicated for this position, LOOP
            var s = new State(_direction, _reverse, _breaker);
            if ( !_history[_position.C, _position.L].Add(s) )
            {
                Console.Error.WriteLine("Loop detected: {0}. State: {1}", _position, s);
                Console.Error.Write(string.Join("\n",_output));
                _direction = "LOOP";
                break;
            }
        }

        if (_direction == "LOOP")
            Console.WriteLine("LOOP");
        else
            Console.Write(string.Join("\n",_output));
    }
    static void Move()
    {
        var p = _position;
        switch(_map[p.C, p.L])
        {
            case 'N':
                _direction = "NORTH";
                break;
            case 'S':
                _direction = "SOUTH";
                break;
            case 'E':
                _direction = "EAST";
                break;
            case 'W':
                _direction = "WEST";
                break;
            case 'I':
                _reverse = !_reverse;
                break;
            case 'B':
                _breaker = !_breaker;
                break;
            case 'T':
                var t = _teleporters[0];
                if (t.L == p.L && t.C == p.C)
                {
                    p = _teleporters[1];
                }
                else
                {
                    p = _teleporters[0];
                }
                break;
            case 'X':
                if (_breaker)
                {
                    _map[p.C, p.L] = ' ';
                    ClearHistory();
                }
                else
                {
                    HandleBlocked();
                }
                break;
            case '#':
                HandleBlocked();
                break;
        }

        if (_direction == "SOUTH") p += Point.South;
        else if (_direction == "EAST") p += Point.East;
        else if (_direction == "NORTH") p += Point.North;
        else if (_direction == "WEST") p += Point.West;

        // handle new position
        var c = _map[p.C, p.L];
        switch (c)
        {
            case 'I':
                _reverse = !_reverse;

                Console.Error.WriteLine("{0}", _reverse ? "Reversed" : "Back to Normal");
                break;
            case 'B':
                _breaker = !_breaker;
                Console.Error.WriteLine("{0}", _breaker ? "BEER!!! Bender SMASH!!!" : "Sober");
                break;
            case 'T':
                var t = _teleporters[0];
                if (t.L == p.L && t.C == p.C)
                {
                    p = _teleporters[1];
                }
                else
                {
                    p = _teleporters[0];
                }

                Console.Error.WriteLine("Fast Travel to {0}", p);
                break;
            case 'X':
                if (_breaker)
                {
                    Console.Error.WriteLine("BEER!!! Wall Smashed at {0}", p);
                    _map[p.C, p.L] = ' ';
                    ClearHistory();
                }
                else
                {
                    Console.Error.WriteLine("Blocked by X at {0}", p);
                    p = HandleBlocked();
                }
                break;
            case '#':
                Console.Error.WriteLine("Blocked by # at {0}", p);
                p = HandleBlocked();
                break;
        }


        _position = p;
    }
    static void HandleBlocked()
    {
        var directions = new string[] { "SOUTH", "EAST", "NORTH", "WEST" };
        if( _reverse ) directions = new string[] {  "WEST", "NORTH", "EAST","SOUTH" };

        Point p = _position;
        var c = _map[p.C, p.L];
        
        foreach (var d in directions)
        {
            
            if (d == "SOUTH") p += Point.South;
            else if (d == "EAST") p += Point.East;
            else if (d == "NORTH") p += Point.North;
            else if (d == "WEST") p += Point.West;

            switch (_map[p.C, p.L])
            {
                case 'X':                
                case '#':
                    continue;
                
                default:
                    _direction = d;
                    break;
            }
        }

    }
    static void ClearHistory()
    {
        Console.Error.WriteLine("Resetting History");
        foreach( var h in _history)
        {
            h.Clear();
        }
    }
    struct Point
    {
        public int L { get; set; }
        public int C { get; set; }

        public Point(int l, int c)
        {
            L = l;
            C = c;
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.L + p2.L, p1.C + p2.C);
        }
        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.L - p2.L, p1.C - p2.C);
        }

        public static readonly Point South = new Point(1,0);
        public static readonly Point North = new Point(-1, 0);
        public static readonly Point East = new Point(0, 1);
        public static readonly Point West = new Point(0, -1);

        public override string ToString()
        {
            return C + "x" + L;
        }
    }

    private struct State
    {
        public string Direction { get; set; }
        public bool Reverse { get; set; }
        public bool Breaker { get; set; }
        public State(string d, bool r, bool b)
        {
            Direction = d;
            Reverse = r;
            Breaker = b;
        }

        public static bool operator ==(State s1, State s2)
        {
            return s1.Direction == s2.Direction
                && s1.Reverse == s2.Reverse
                && s1.Breaker == s2.Breaker;
        }
        public static bool operator !=(State s1, State s2)
        {
            return !(s1 == s2);
        }
        public bool Equals(State s)
        {
            return this == s;
        }
        public override bool Equals(object obj)
        {
            if (obj is State) return Equals((State)obj);
            return false;
        }
        public override int GetHashCode()
        {
            return Direction.GetHashCode()
                ^ Reverse.GetHashCode()
                ^ Breaker.GetHashCode();
        }

        public override string ToString()
        {
            return "D: " + Direction +
                " R: " + Reverse +
                " B: " + Breaker;
        }
    }
}