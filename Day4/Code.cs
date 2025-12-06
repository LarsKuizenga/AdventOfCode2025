using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode2025.Day4;

static class Code
{
    public static void Solve()
    {
        string[] input = File.ReadAllLines("../../../Day4/Input.txt");
        string[] exampleInput = File.ReadAllLines("../../../Day4/ExampleInput.txt");

        PartOne(exampleInput, input);
        PartTwo(exampleInput, input);
    }

    private static void PartOne(string[] exampleInput, string[] input)
    {
        int exampleInputAnswer = 13;
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
        int exampleInputAnswer = 43;
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
        Grid grid = new Grid(input);

        grid.SetIsPaperRoleMoveAble();

        int moveAblePaperRounds = grid.GetMoveAblePaperRoleCount();

        return moveAblePaperRounds;
    }

    private static int SolvePartTwo(string[] input)
    {
        Grid grid = new Grid(input);

        int moveAbleCount = 0;

        do
        {
            grid.SetIsPaperRoleMoveAble();

            moveAbleCount = grid.GetMoveAblePaperRoleCount();

            grid.RemoveMoveAbleRoles();

        } while (moveAbleCount != 0);

        return grid.RemovedRollsCount;
    }

    public class Grid
    {
        public List<List<Tile>> Tiles = [];
        public int RemovedRollsCount = 0;

        public Grid(string[] input)
        {
            for (int rowIndex = 0; rowIndex < input.Length; rowIndex++)
            {
                Tiles.Add([]);

                for (int columnIndex = 0; columnIndex < input[0].Length; columnIndex++)
                {
                    bool hasRole = input[rowIndex][columnIndex] == '@';

                    Tiles[rowIndex].Add(new Tile { XPos = columnIndex, YPos = rowIndex, PaperRole = hasRole ? new PaperRole() : null });
                }
            }
        }

        public void SetIsPaperRoleMoveAble()
        {
            foreach (Tile tile in Tiles.SelectMany(t => t))
            {
                tile.SetIsPaperRoleMoveAble(this, 4);
            }
        }

        public int GetMoveAblePaperRoleCount()
        {
            return Tiles.SelectMany(t => t).Where(t => t.PaperRole is not null && t.PaperRole.IsMoveAble).Count();
        }

        public void RemoveMoveAbleRoles()
        {
            foreach (var tile in Tiles.SelectMany(t => t).Where(t => t.PaperRole is not null))
            {
                if (tile.PaperRole!.IsMoveAble)
                {
                    tile.PaperRole = null;
                    RemovedRollsCount++;
                }
            }
        }
    }

    public class Tile
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        public PaperRole? PaperRole { get; set; }

        public void SetIsPaperRoleMoveAble(Grid grid, int moveAbleCount)
        {
            if (PaperRole is not null && GetPaperRoleAroundCount(grid) < moveAbleCount)
            {
                PaperRole.IsMoveAble = true;
            }
        }

        public int GetPaperRoleAroundCount(Grid grid)
        {
            int paperRoleAroundCount = 0;

            List<(int, int)> indicesToCheck = [
                (XPos - 1, YPos - 1), (XPos, YPos - 1), (XPos + 1, YPos - 1),
                (XPos - 1, YPos),                       (XPos + 1, YPos),
                (XPos - 1, YPos + 1), (XPos, YPos + 1), (XPos + 1, YPos + 1),
            ];

            indicesToCheck.ForEach((i) => { if (DoesTileHavePaperRole(grid, i.Item1, i.Item2)) paperRoleAroundCount++; });

            return paperRoleAroundCount;
        }

        public static bool DoesTileHavePaperRole(Grid grid, int x, int y)
        {
            return grid.Tiles.ElementAtOrDefault(y)?.ElementAtOrDefault(x)?.PaperRole is not null;
        }
    }

    public class PaperRole
    {
        public bool IsMoveAble { get; set; } = false;
    }
}
