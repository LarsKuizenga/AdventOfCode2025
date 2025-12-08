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
        ulong exampleInputAnswer = 1;
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

        for (int index = 0; index < connectionsToFind; index++)
        {
	        Connection? shortestConnection = null;

			foreach (JunctionBox leftJunctionBox in junctionBoxes)
	        {
		        foreach (JunctionBox rightJunctionBox in junctionBoxes)
		        {
			        if (connections.Any(c => c.LeftJunctionBox == leftJunctionBox && c.RightJunctionBox == rightJunctionBox))
			        {
                        continue;
			        }

			        double distance = 0; //Calculate distance between junction boxes

			        if (shortestConnection is null)
			        {
				        shortestConnection = new Connection
				        {
							LeftJunctionBox = leftJunctionBox,
							RightJunctionBox = rightJunctionBox,
							Length = distance
				        };

                        continue;
			        }

			        if (distance < shortestConnection.Length)
			        {
				        shortestConnection = new Connection
				        {
					        LeftJunctionBox = leftJunctionBox,
					        RightJunctionBox = rightJunctionBox,
					        Length = distance
				        };
			        }

		        }
	        }
            
	        connections.Add(shortestConnection);

			//Look for circuit on both sides of the connection
			Circuit? circuit = circuits.FirstOrDefault(c => c.JunctionBoxes.Any(jb => jb == shortestConnection.LeftJunctionBox || jb == shortestConnection.RightJunctionBox));

	        if (circuit is not null)
	        {
		        circuit.JunctionBoxes.Add(shortestConnection.LeftJunctionBox);
		        circuit.JunctionBoxes.Add(shortestConnection.RightJunctionBox);
	        }
	        else
	        {
		        Circuit newCircuit = new Circuit();
		        newCircuit.JunctionBoxes.Add(shortestConnection.LeftJunctionBox);
		        newCircuit.JunctionBoxes.Add(shortestConnection.RightJunctionBox);

		        circuits.Add(newCircuit);
	        }
        }

        return 0;
    }

    private static ulong SolvePartTwo(string[] input)
    {
        

	    return 0;
    }

    public class JunctionBox
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int ZPos { get; set; }

        public JunctionBox(string line)
        {
            string[] parts = line.Split(',');

			XPos = int.Parse(parts[0]);
            YPos = int.Parse(parts[1]);;
            ZPos = int.Parse(parts[2]);;
		}
	}

    public class Connection
    {
        public JunctionBox LeftJunctionBox { get; set; }
        public JunctionBox RightJunctionBox { get; set; }
        public double Length { get; set; }
	}

    public class Circuit
    {
	    public List<JunctionBox> JunctionBoxes { get; set; } = [];
    }
}
