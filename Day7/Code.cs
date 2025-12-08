using System.Diagnostics;

namespace AdventOfCode2025.Day7;

static class Code
{
    public static void Solve()
    {
        string[] input = File.ReadAllLines("../../../Day7/Input.txt");
        string[] exampleInput = File.ReadAllLines("../../../Day7/ExampleInput.txt");

        PartOne(exampleInput, input);
        PartTwo(exampleInput, input);
    }

    private static void PartOne(string[] exampleInput, string[] input)
    {
        int exampleInputAnswer = 21;
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
        ulong exampleInputAnswer = 40;
        ulong exampleOutput = SolvePartTwo(exampleInput);

        if (exampleOutput != exampleInputAnswer)
        {
            Console.WriteLine($"Part two example answer did not come out! Expected {exampleInputAnswer}, but was {exampleOutput}");
            return;
        }
        else
        {
            ulong answer = SolvePartTwo(input);
            Console.WriteLine($"Part two answer: {answer}");
        }
    }

    private static int SolvePartOne(string[] input)
    {
        Grid grid = new Grid(input);
        int splits = 0;

        for (int index = 0; index < input.Length; index++)
        {
            splits += grid.Progress();
        }

        return splits;
    }

    private static ulong SolvePartTwo(string[] input)
    {
        Grid grid = new Grid(input);

        ulong dimensionCount = grid.QuantumPathing();

        return dimensionCount;
    }

    public class Grid
    {
        List<List<Tile>> Tiles { get; set; } = [];
        
        private int lineProgressIndex = 1;
        private Splitter RootSplitter = null!;

        public Grid(string[] input)
        {
            int lineLength = input[0].Length;

            for (int rowIndex = 0; rowIndex < input.Length; rowIndex++)
            {
                Tiles.Add([]);

                for (int columnIndex = 0; columnIndex < lineLength; columnIndex++)
                {
                    char character = input[rowIndex][columnIndex];
                    State state = character switch
                    {
                        '^' => State.Splitter,
                        'S' => State.Start,
                        _ => State.Empty,
                    };

                    Tile tile = new Tile()
                    {
                        XPos = columnIndex,
                        YPos = rowIndex,
                        State = state,
                        Splitter = state == State.Splitter ? new Splitter() : null 
                    };

                    if (tile.Splitter is not null)
                    {
                        tile.Splitter.ParentTile = tile;
                    }

                    Tiles[rowIndex].Add(tile);
                }
            }
        }

        public int Progress()
        {
            if (lineProgressIndex == Tiles.Count - 1) return 0;

            int splitCount = 0;

            List<Tile> previousLine = Tiles[lineProgressIndex - 1];
            List<Tile> currentLine = Tiles[lineProgressIndex];

            for (int columnIndex = 0; columnIndex < previousLine.Count; columnIndex++)
            {
                Tile previousLineTile = previousLine[columnIndex];
                Tile currentLineTile = currentLine[columnIndex];

                if (currentLineTile.State == State.Empty && (previousLineTile.State == State.Laser || previousLineTile.State == State.Start))
                {
                    currentLineTile.State = State.Laser;
                }

                if (previousLineTile.State == State.Laser && currentLineTile.State == State.Splitter)
                {
                    currentLine[columnIndex - 1].State = State.Laser;
                    currentLine[columnIndex + 1].State = State.Laser;

                    splitCount++;
                }
            }
                        
            lineProgressIndex++;

            return splitCount;
        }

        public void SetRootSplitter()
        {
            for (int columnIndex = 0; columnIndex < Tiles[0].Count; columnIndex++)
            {
                Splitter? splitter = Tiles[2][columnIndex].Splitter;

                if (splitter is not null)
                {
                    RootSplitter = splitter;
                }
            }
        }

        public void PlotSplitterRelations(Splitter splitterToPlot)
        {
            for (int rowIndex = splitterToPlot.ParentTile.YPos + 2; rowIndex < Tiles.Count; rowIndex += 2)
            {
                if (splitterToPlot.LeftSplitter is not null && splitterToPlot.RightSplitter is not null) break;

                if (splitterToPlot.LeftSplitter is null)
                {
                    Splitter? splitterLeft = Tiles[rowIndex][splitterToPlot.ParentTile.XPos - 1].Splitter;

                    if (splitterLeft is not null)
                    {
                        splitterToPlot.LeftSplitter = splitterLeft;
                        PlotSplitterRelations(splitterLeft);
                    }
                }
                
                if (splitterToPlot.RightSplitter is null)
                {
                    Splitter? splitterRight = Tiles[rowIndex][splitterToPlot.ParentTile.XPos + 1].Splitter;

                    if (splitterRight is not null)
                    {
                        splitterToPlot.RightSplitter = splitterRight;
                        PlotSplitterRelations(splitterRight);
                    }
                }
            }
        }

        public static ulong QuantamPathSearch(Splitter splitter)
        {
            ulong total = 0;

            if (splitter.Visited) return splitter.Value;

			splitter.Visited = true;

            if (splitter.LeftSplitter is null && splitter.RightSplitter is null)
            {
	            splitter.Value = 2;
                return 2;
            }

            if (splitter.LeftSplitter is null || splitter.RightSplitter is null)
            {
                total += 1;
            }

            if (splitter.LeftSplitter is not null)
            {
	            if (splitter.LeftSplitter.Visited)
	            {
                    total += splitter.LeftSplitter.Value;
				}
	            else
	            {
		            total += QuantamPathSearch(splitter.LeftSplitter);
	            }
            }

            if (splitter.RightSplitter is not null)
            {
	            if (splitter.RightSplitter.Visited)
	            {
		            total += splitter.RightSplitter.Value;
	            }
	            else
	            {
		            total += QuantamPathSearch(splitter.RightSplitter);
	            }
			}

            splitter.Value = total;

            return total;
        }

        public ulong QuantumPathing()
        {
            SetRootSplitter();
            PlotSplitterRelations(RootSplitter);

            return QuantamPathSearch(RootSplitter);
        }
    }

    public class Tile
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        public State State { get; set;}
        public Splitter? Splitter { get; set; }
    }

    public class Splitter
    {
        public Splitter? LeftSplitter { get; set; }
        public Splitter? RightSplitter { get; set; }
        public Tile ParentTile { get; set; } = null!;

        public bool Visited { get; set; }
        public ulong Value { get; set; }
    }

    public enum State
    {
        Empty,
        Start,
        Laser,
        Splitter
    }
}
