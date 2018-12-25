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
    static State _state = new State("SOUTH", false, false);
    static char[,] _map = null;
    static HashSet<State>[,] _history = null;
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
        while ( _state.Direction != "LOOP" && _map[_position.C, _position.L] != '$' )
        {            
            Move();
            if(!_history[_position.C, _position.L].Add(_state))
            {
                _state.Direction = "LOOP";
                break;
            }

            _output.Add(_state.Direction);
        }

        if (_state.Direction == "LOOP")
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
                _state.Direction = "NORTH";
                break;
            case 'S':
                _state.Direction = "SOUTH";
                break;
            case 'E':
                _state.Direction = "EAST";
                break;
            case 'W':
                _state.Direction = "WEST";
                break;
            case 'I':
                _state.Reverse = !_state.Reverse;
                break;
            case 'B':
                _state.Breaker = !_state.Breaker;
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
        }

        if (_state.Direction == "SOUTH") p += Point.South;
        else if (_state.Direction == "EAST") p += Point.East;
        else if (_state.Direction == "NORTH") p += Point.North;
        else if (_state.Direction == "WEST") p += Point.West;

        // handle new position
        var c = _map[p.C, p.L];
        switch (c)
        {
            case 'X':
                if (_state.Breaker)
                {
                    _map[p.C, p.L] = ' ';
                    ClearHistory();
                }
                else
                {
                    p = HandleBlocked();
                }
                break;
            case '#':
                p = HandleBlocked();
                break;
        }


        _position = p;
    }
    static Point HandleBlocked()
    {
        var directions = new string[] { "SOUTH", "EAST", "NORTH", "WEST" };
        if( _state.Reverse ) directions = new string[] {  "WEST", "NORTH", "EAST","SOUTH" };

        
        foreach (var d in directions)
        {
            Point p = _position;
        
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
                    _state.Direction = d;
                    return p;
                    break;
            }
        }
        
        throw new ApplicationException("Shouldn't Reach Here");

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