namespace AdventOfCode2025.Day12;

static class Code
{
    public static void Solve()
    {
        string[] input = File.ReadAllLines("../../../Day12/Input.txt");
        string[] exampleInput = File.ReadAllLines("../../../Day12/ExampleInput.txt");

        PartOne(exampleInput, input);
        PartTwo(exampleInput, input);
    }

    private static void PartOne(string[] exampleInput, string[] input)
    {
        int exampleInputAnswer = 2;
        int exampleOutput = SolvePartOne(exampleInput);

        if (exampleInputAnswer != exampleOutput)
        {
            Console.WriteLine($"Part one example answer did not come out! Expected {exampleInputAnswer}, but was {exampleOutput}");
            return;
        }
        else
        {
            int answer = SolvePartOne(input);
            Console.WriteLine($"Part one answer: {answer}");
        }
    }

    private static void PartTwo(string[] exampleInput, string[] input)
    {
        int exampleInputAnswer = 1;
        int exampleOutput = SolvePartTwo(exampleInput);

        if (exampleOutput != exampleInputAnswer)
        {
            Console.WriteLine($"Part two example answer did not come out! Expected {exampleInputAnswer}, but was {exampleOutput}");
            return;
        }
        else
        {
            int answer = SolvePartTwo(input);
            Console.WriteLine($"Part two answer: {answer}");
        }
    }

    private static int SolvePartOne(string[] input)
    {
        List<Present> presents = Present.ParsePresents(input[..30]);
        List<Grid> grids = Grid.ParseGrids(input[30..]);

        foreach (Grid grid in grids)
        {
            grid.SetCanFitPresents(presents);
        }

        return grids.Count(g => g.CanFitPresents);
    }

    private static int SolvePartTwo(string[] input)
    {


        return 0;
    }

    public class Grid
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<int> PresentCounts { get; set; } = [];
        public bool CanFitPresents { get; set; }

        public Grid(string gridLine)
        {
            string[] parts = gridLine.Split(" ");

            string[] dimensions = parts[0].Split("x");

            Width = int.Parse(dimensions[0]);
            Height = int.Parse(dimensions[1][..^1]);

            PresentCounts.AddRange(parts[1..].Select(int.Parse));
        }

        public static List<Grid> ParseGrids(string[] gridLines)
        {
            List<Grid> grids = [];

            foreach (string gridLine in gridLines)
            {
                grids.Add(new Grid(gridLine));
            }

            return grids;
        }

        public void SetCanFitPresents(List<Present> presents)
        {
            if (GetTotalPresentSize(presents) > Width * Height)
            {
                return;
            }


        }

        public int GetTotalPresentSize(List<Present> presents)
        {
            int totalSize = 0;

            for (int index = 0; index < presents.Count; index++)
            {
                totalSize += PresentCounts[index] * presents[index].Size;
            }

            return totalSize;
        }
    }

    public class Present
    {
        public List<List<bool>> PresentSpaces { get; set; } = [];
        public int Size => PresentSpaces.SelectMany(ps => ps).Count(ps => ps);

        public Present(List<string> presentLines)
        {
            for (int index = 0; index < presentLines.Count; index++)
            {
                string presentLine = presentLines[index];
                
                PresentSpaces.Add([]);

                foreach (char presentSpace in presentLine)
                {
                    PresentSpaces[index].Add(presentSpace == '#');
                }
            }
        }

        public static List<Present> ParsePresents(string[] allPresentLines)
        {
            List<Present> presents = [];
            List<string> currPresentLines = [];

            foreach (string presentLine in allPresentLines)
            {
                if (presentLine != string.Empty)
                {
                    currPresentLines.Add(presentLine);
                    continue;
                }

                presents.Add(new Present(currPresentLines[1..]));

                currPresentLines = [];
            }

            return presents;
        }
    }
}
