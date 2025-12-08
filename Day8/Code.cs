using System.Diagnostics;

namespace AdventOfCode2025.Day8;

static class Code
{
    public static void Solve()
    {
        string[] input = File.ReadAllLines("../../../Day8/Input.txt");
        string[] exampleInput = File.ReadAllLines("../../../Day8/ExampleInput.txt");

        PartOne(exampleInput, input);
        PartTwo(exampleInput, input);
    }

    private static void PartOne(string[] exampleInput, string[] input)
    {
        int exampleInputAnswer = 40;
        int exampleOutput = SolvePartOne(exampleInput, 10);

        if (exampleInputAnswer != exampleOutput)
        {
            Console.WriteLine($"Part one example answer did not come out! Expected {exampleInputAnswer}, but was {exampleOutput}");
        }
        else
        {
            int answer = SolvePartOne(input, 1000);
            Console.WriteLine($"Part one answer: {answer}");
        }
    }

    private static void PartTwo(string[] exampleInput, string[] input)
    {
        ulong exampleInputAnswer = 25272;
        ulong exampleOutput = SolvePartTwo(exampleInput);

        if (exampleOutput != exampleInputAnswer)
        {
            Console.WriteLine($"Part two example answer did not come out! Expected {exampleInputAnswer}, but was {exampleOutput}");
        }
        else
        {
            ulong answer = SolvePartTwo(input);
            Console.WriteLine($"Part two answer: {answer}");
        }
    }

    private static int SolvePartOne(string[] input, int connectionsToFind)
    {
        List<JunctionBox> junctionBoxes = [];
        List<Connection> connections = [];
        List<Circuit> circuits = [];

        junctionBoxes.AddRange(input.Select(line => new JunctionBox(line)));

        foreach (JunctionBox junctionBox in junctionBoxes)
        {
            circuits.Add(new Circuit { JunctionBoxes = [junctionBox] });
        }
                
        connections = Connection.SetConnections(junctionBoxes);

        for (int index = 0; index < connectionsToFind; index++)
        {
            Connection shortestConnection = connections[index];

            Circuit? circuitLeft = circuits.FirstOrDefault(c => c.JunctionBoxes.Any(jb => jb == shortestConnection.LeftJunctionBox));
            Circuit? circuitRight = circuits.FirstOrDefault(c => c.JunctionBoxes.Any(jb => jb == shortestConnection.RightJunctionBox));

            //Connection between junctionboxes of the same circuit
            if (circuitLeft is not null && circuitRight is not null && circuitLeft == circuitRight) continue;

            if (circuitLeft is not null && circuitRight is not null)
            {
                circuitLeft.JunctionBoxes.AddRange(circuitRight.JunctionBoxes);
                circuitRight.JunctionBoxes.RemoveAll(jb => true);
                circuits.Remove(circuitRight);
            }
            else if (circuitLeft is not null && circuitRight is null)
            {
                circuitLeft.JunctionBoxes.Add(shortestConnection.RightJunctionBox);
            }
            else if (circuitLeft is null && circuitRight is not null)
            {
                circuitRight.JunctionBoxes.Add(shortestConnection.LeftJunctionBox);
            }
        }

        circuits = circuits.OrderByDescending(c => c.JunctionBoxes.Count).ToList();

        return circuits[0].JunctionBoxes.Count * circuits[1].JunctionBoxes.Count * circuits[2].JunctionBoxes.Count;
    }

    private static ulong SolvePartTwo(string[] input)
    {
        List<JunctionBox> junctionBoxes = [];
        List<Connection> connections = [];
        List<Circuit> circuits = [];

        junctionBoxes.AddRange(input.Select(line => new JunctionBox(line)));

        foreach (JunctionBox junctionBox in junctionBoxes)
        {
            circuits.Add(new Circuit { JunctionBoxes = [junctionBox] });
        }

        connections = Connection.SetConnections(junctionBoxes);

        Connection? shortestConnection = null;

        int index = 0;
        while (circuits.Count != 1)
        {
            shortestConnection = connections[index];
            index++;

            Circuit? circuitLeft = circuits.FirstOrDefault(c => c.JunctionBoxes.Any(jb => jb == shortestConnection.LeftJunctionBox));
            Circuit? circuitRight = circuits.FirstOrDefault(c => c.JunctionBoxes.Any(jb => jb == shortestConnection.RightJunctionBox));

            //Connection between junctionboxes of the same circuit
            if (circuitLeft is not null && circuitRight is not null && circuitLeft == circuitRight) continue;

            if (circuitLeft is not null && circuitRight is not null)
            {
                circuitLeft.JunctionBoxes.AddRange(circuitRight.JunctionBoxes);
                circuitRight.JunctionBoxes.RemoveAll(jb => true);
                circuits.Remove(circuitRight);
            }
            else if (circuitLeft is not null && circuitRight is null)
            {
                circuitLeft.JunctionBoxes.Add(shortestConnection.RightJunctionBox);
            }
            else if (circuitLeft is null && circuitRight is not null)
            {
                circuitRight.JunctionBoxes.Add(shortestConnection.LeftJunctionBox);
            }
        }

        if (shortestConnection is null) throw new UnreachableException();

        return ((ulong)shortestConnection.LeftJunctionBox.XPos) * ((ulong)shortestConnection.RightJunctionBox.XPos);
    }

    [DebuggerDisplay("{XPos} {YPos} {ZPos}")]
    public class JunctionBox
    {
        public double XPos { get; set; }
        public double YPos { get; set; }
        public double ZPos { get; set; }

        public JunctionBox(string line)
        {
            string[] parts = line.Split(',');

            XPos = double.Parse(parts[0]);
            YPos = double.Parse(parts[1]); ;
            ZPos = double.Parse(parts[2]); ;
        }
    }

    [DebuggerDisplay("L: {LeftJunctionBox} | R: {RightJunctionBox} | {Length}")]
    public class Connection
    {
        public JunctionBox LeftJunctionBox { get; set; } = null!;
        public JunctionBox RightJunctionBox { get; set; } = null!;
        public double Length { get; set; }

        public static List<Connection> SetConnections(List<JunctionBox> junctionBoxes)
        {
            List<Connection> connections = [];

            for (int leftJuctionBoxIndex = 0; leftJuctionBoxIndex < junctionBoxes.Count; leftJuctionBoxIndex++)
            {
                JunctionBox leftJunctionBox = junctionBoxes[leftJuctionBoxIndex];

                for (int rightJunctionBoxIndex = leftJuctionBoxIndex + 1; rightJunctionBoxIndex < junctionBoxes.Count; rightJunctionBoxIndex++)
                {
                    if (rightJunctionBoxIndex == leftJuctionBoxIndex) continue;

                    JunctionBox rightJunctionBox = junctionBoxes[rightJunctionBoxIndex];

                    double distance = GetDistance(leftJunctionBox, rightJunctionBox);

                    connections.Add(new Connection
                    {
                        LeftJunctionBox = leftJunctionBox,
                        RightJunctionBox = rightJunctionBox,
                        Length = distance,
                    });
                }
            }

            return connections.OrderBy(c => c.Length).ToList();
        }

        public static double GetDistance(JunctionBox leftJunctionBox, JunctionBox rightJunctionBox)
        {
            double x = Math.Pow(Math.Abs(leftJunctionBox.XPos - rightJunctionBox.XPos), 2);
            double y = Math.Pow(Math.Abs(leftJunctionBox.YPos - rightJunctionBox.YPos), 2);
            double z = Math.Pow(Math.Abs(leftJunctionBox.ZPos - rightJunctionBox.ZPos), 2);

            return Math.Sqrt(x + y + z);
        }
    }

    [DebuggerDisplay("Count = {JunctionBoxes.Count}")]
    public class Circuit
    {
        public List<JunctionBox> JunctionBoxes { get; set; } = [];
    }
}
