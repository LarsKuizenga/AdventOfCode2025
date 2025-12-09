namespace AdventOfCode2025.Day9;

static class Code
{
    public static void Solve()
    {
        string[] input = File.ReadAllLines("../../../Day9/Input.txt");
        string[] exampleInput = File.ReadAllLines("../../../Day9/ExampleInput.txt");

        PartOne(exampleInput, input);
        PartTwo(exampleInput, input);
    }

    private static void PartOne(string[] exampleInput, string[] input)
    {
        ulong exampleInputAnswer = 50;
        ulong exampleOutput = SolvePartOne(exampleInput);

        if (exampleInputAnswer != exampleOutput)
        {
            Console.WriteLine($"Part one example answer did not come out! Expected {exampleInputAnswer}, but was {exampleOutput}");
        }
        else
        {
	        ulong answer = SolvePartOne(input);
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

    private static ulong SolvePartOne(string[] input)
    {
        List<RedTile> redTiles = [];

		redTiles.AddRange(input.Select(line => new RedTile(line)));

		List<Rectangle> rectangles = Rectangle.GetAllRectangles(redTiles);

        return rectangles.MaxBy(r => r.GetRectangleSize())!.GetRectangleSize();
    }

    private static ulong SolvePartTwo(string[] input)
    {
        

	    return 0;
    }

    public class RedTile
    {
		public long XPos { get; set; }
        public long YPos { get; set; }

        public RedTile(string line)
        {
            List<string> parts = line.Split(',').ToList();

			XPos = long.Parse(parts[0]);
            YPos = long.Parse(parts[1]);
		}
	}

    public class Rectangle
    {
        public RedTile FirstCorner { get; set; }
        public RedTile SecondCorner { get; set; }

        public ulong GetRectangleSize()
        {
	        long width = Math.Abs(FirstCorner.XPos - SecondCorner.XPos) + 1;
	        long height = Math.Abs(FirstCorner.YPos - SecondCorner.YPos) + 1;

            return (ulong)(width * height);
		}

        public static List<Rectangle> GetAllRectangles(List<RedTile> redTiles)
        {
            List<Rectangle> rectangles = [];
            
            for (int outerTileIndex = 0; outerTileIndex < redTiles.Count; outerTileIndex++)
            {
                for (int innerTileIndex = outerTileIndex + 1; innerTileIndex < redTiles.Count; innerTileIndex++)
                {
                    rectangles.Add(new Rectangle
                    {
                        FirstCorner = redTiles[outerTileIndex],
                        SecondCorner = redTiles[innerTileIndex]
                    });
                }
            }

            return rectangles;
		}
	}
}
