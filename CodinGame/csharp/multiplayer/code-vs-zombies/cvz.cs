using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

/**
 * Save humans, destroy zombies!
 **/
class code_vs_zombies
{
    static Random rand = new Random();
    static void Main(string[] args)
    {
        string[] inputs;
        GameState bestSim = null;
        int processingTime = 900;
        int simId = 0;
        int minScore = -1;
        // game loop
        while (true)
        {
            var sw = Stopwatch.StartNew();

            var gs = new GameState();
            gs.Id = simId ++;
            gs.Ash = new Ash(new Point(Console.ReadLine()));

            int humanCount = int.Parse(Console.ReadLine());   
            gs.Humans = new Human[ humanCount ];         
            for (int i = 0; i < humanCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var h = new Human(
                    int.Parse(inputs[0]),
                    new Point(int.Parse(inputs[1]),int.Parse(inputs[2]))
                );
                gs.Humans[i] = h;
            }
            int zombieCount = int.Parse(Console.ReadLine());
            if( minScore < 0 ) minScore = zombieCount * 10;
            gs.Zombies = new Zombie[ zombieCount ];
            for (int i = 0; i < zombieCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var z = new Zombie {
                    Id = int.Parse(inputs[0]),
                    Pos = new Point(int.Parse(inputs[1]),int.Parse(inputs[2])),
                    NextPos = new Point(int.Parse(inputs[3]),int.Parse(inputs[4])),
                };
                gs.Zombies[i] = z;
            }
                        
            if( gs.Zombies.Length > 1)
            {
                
                var simulations = new SortedDictionary<int,GameState>();
                if( bestSim != null ) simulations.Add(bestSim.Points, bestSim);
                while( sw.ElapsedMilliseconds < processingTime)
                {
                    var sim = gs.Clone();                    
                    sim.Id = simId ++;
                    sim.Simulate(sw, processingTime);
                    if( sim.Points >= minScore && !simulations.ContainsKey(sim.Points))
                    {
                        simulations.Add(sim.Points, sim);
                    }
                }
                if(simulations.Any())
                {
                    bestSim = simulations.Last().Value;
                    Console.Error.WriteLine("Best Sim. Id: {0}. Points: {1}", bestSim.Id, bestSim.Points);
                    Console.WriteLine(bestSim.Moves.Dequeue());
                }
                else
                {
                    Console.WriteLine(gs.Zombies.Last().NextPos);
                }
            }
            else
            {
                
                Console.WriteLine(gs.Zombies[0].NextPos); // Your destination coordinates
            }
            processingTime = 95;
        }
    }
    class GameState
    {
        static Random rand = new Random();
        int[] Fibonacci = new int[] {
            0,1,1,2,3,5,8,13,21,34,55,89,144,233,377,610,987,1597,2584,4181,6765,10946
            ,17711,28657,46368,75025,121393,196418,317811,514229,832040,1346269,2178309
            ,3524578,5702887,9227465,14930352,24157817,39088169,63245986,102334155
        };
        public GameState()
        {
            
            Moves = new Queue<Point>();
        }
        public GameState Clone()
        {
            var gs =  new GameState
            {
                Id = Id,
                Points = Points,
                Ash = new Ash(Ash.Pos),
                Humans = new Human[Humans.Length],
            };

            Array.Copy(this.Humans, gs.Humans, this.Humans.Length);
            gs.Zombies = this.Zombies.Select(z => z.Clone()).ToArray();
            return gs;
        }
        public int Id {get; set; }
        public int Points {get;set;}
        public Ash Ash { get; set; }
        public Human[] Humans { get; set; }
        public Zombie[] Zombies { get; set; }
        public Queue<Point> Moves {get; private set;}
        
        
        public void SimulateTurn()
        {
            Moves.Enqueue(Ash.Pos);
            
            int killedZombies = 0;
            var aliveHumans = new List<Human>(Humans);
            var aliveZombies = new List<Zombie>(Zombies.Length);
            foreach(var z in Zombies)
            {
                if(z.NextPos.DistanceTo(Ash.Pos) > 2000)
                {
                    aliveZombies.Add(z);
                    for(int i = 0; i < Humans.Length; i++)
                    {
                        var h = Humans[i];
                        if( h.Pos == z.NextPos)
                        {
                            aliveHumans.Remove(h);
                        }
                    }
                }
                else
                {
                    killedZombies ++;
                }
            }

            if( aliveHumans.Count == 0 ) {
                Humans = new Human[0];
                Points = 0;
                return;
            }

            for(int z = 0; z < killedZombies; z++)
            {
                int zP = Humans.Length * Humans.Length * 10;
                if( killedZombies > 1 ) zP *= Fibonacci[z + 3];

                Points += zP;
            }

            Humans = aliveHumans.ToArray();
            Zombies = aliveZombies.ToArray();
            foreach(var z in Zombies)
            {
                z.Pos = z.NextPos;

                // find nearest human
                z.Target = Humans.OrderBy(h => z.Pos.DistanceTo(h.Pos)).First().Pos;
                if( z.Pos.DistanceTo(Ash.Pos) < z.Pos.DistanceTo(z.Target))
                {
                    z.Target = Ash.Pos;
                }
                z.NextPos = z.Pos.MoveToward(z.Target, 400);

            } 
        }
        public void Simulate(Stopwatch sw, int processingTime)
        {
                
            var targetZ = Zombies[rand.Next(0, Zombies.Length-1)];
            while( Humans.Any() && Zombies.Any() && sw.ElapsedMilliseconds < processingTime)
            {
                if( !Zombies.Any(z => z.Id == targetZ.Id))
                {
                    targetZ = Zombies[rand.Next(0, Zombies.Length-1)];
                }
                int distance = Ash.Pos.DistanceTo(targetZ.NextPos);
                distance = distance > 3000 ? 1000 : rand.Next(distance-2000, 1000);

                var angle = Util.ToDegrees( Util.AngleBetween(Ash.Pos, targetZ.NextPos));
                angle += rand.Next(-10,10);
                Ash.Pos = Ash.Pos.Move(Util.ToRadians(angle), distance);
                SimulateTurn();
            }
        }
    }

    struct Point
    {
        public int X {get;set;}
        public int Y {get;set;}

        public Point(int x, int y)
        {
            this.X = Math.Min(16000,Math.Max(0,x));
            this.Y = Math.Min(9000,Math.Max(0,y));
        }
        public Point(Point p1): this( p1.X, p1.Y) {}
        public Point(string p)
        {
            var inputs = p.Split(' ');
            X = int.Parse(inputs[0]);
            Y = int.Parse(inputs[1]);
        }
        public static bool operator == (Point p1, Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }
        public static bool operator != (Point p1, Point p2)
        {
            return !(p1 == p2);
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

        public int DistanceTo(Point p)
        {
            return DistanceTo(p.X, p.Y);
        }
        public int DistanceTo(int pX, int pY)
        {
            if( X == pX && Y == pY ) return 0;
            int x = (pX - X) * (pX - X);
            int y = (pY - Y) * (pY - Y);
            return (int)Math.Sqrt( x + y);
        }
        public bool IsInCircle(int cx, int cy, int radius)
        {
            int dx = (int)Math.Abs(X - cx);
            int dy = (int)Math.Abs(Y-cy);

            return (dx*dx + dy*dy <= radius * radius);
        }
        public bool IsInCircle(Point p, int radius)
        {
            return IsInCircle(p.X, p.Y, radius);
        }
        public int TurnsToPoint(Point start, int speed)
        {
            return DistanceTo(start) / speed;
        }
        public Point MoveToward(Point p, int distance)
        {
            return MoveToward(p.X, p.Y, distance);            
        }
        public Point MoveToward(int pX, int pY, int distance)
        {

            double distTo = DistanceTo(pX, pY);
            if( distTo < distance) return new Point(pX, pY);

            double D = (double)distance / distTo;
            var x = X + (D*(pX - X));
            var y = Y + (D*(pY - Y));
            
            return new Point((int)x,(int)y);
        }
        
        public Point Move(double angle, int distance)
        {
            return new Point(
                X + (int)( distance * Math.Cos( angle ) ),
                Y + (int)( distance * Math.Sin( angle ) )
            );
        }
        
    }
    class Human
    {
        public Human(int id, Point p)
        {
            Id = id;
            Pos = p;
        }
        public int Id { get; }
        public Point Pos { get; }

        public bool Equals(Human h)
        {
            return Id == h.Id;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return Equals((Human)obj);
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id;
        }
    }
    class Zombie
    {
        public int Id { get; set; }
        public Point Pos { get; set; }
        public Point NextPos { get; set; }
        public Point Target {get; set;}
        
        public Zombie Clone()
        {
            var z = new Zombie
            {
                Id = Id,
                Pos = new Point(Pos),
                NextPos = new Point(NextPos),
                Target = new Point(Target),
            };
            return z;
        }
    }
    class Ash
    {
        public Ash(Point p)
        {
            Pos = p;
        }
        public int Id => -1;
        public Point Pos { get; set;}
        
    }
    class Util
    {
        public static double AngleBetween(Point p1, Point p2)
        {
            return Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);            
        }
        public static double ToDegrees(double angle)
        {
            angle = angle * (180/Math.PI);
            return (angle < 0) ? (360d + angle) : angle;
        }
        public static double ToRadians(double angle)
        {
            return angle * Math.PI/180;
        }
    }
    
}