using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
        ulong exampleInputAnswer = 24;
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
        List<Tile> redTiles = [];

		redTiles.AddRange(input.Select(line => new Tile(line)));

		List<Rectangle> rectangles = Rectangle.GetAllRectangles(redTiles, false);

        return rectangles.MaxBy(r => r.Size)!.Size;
    }

    private static ulong SolvePartTwo(string[] input)
    {
        List<Tile> redTiles = [];

        redTiles.AddRange(input.Select(line => new Tile(line)));

        List<Rectangle> rectangles = Rectangle.GetAllRectangles(redTiles, true);

        return rectangles.MaxBy(r => r.Size)!.Size;
    }

    [DebuggerDisplay("[X: {XPos} | Y: {YPos}]")]
    public class Tile
    {
		public long XPos { get; set; }
        public long YPos { get; set; }

        public Tile()
        {

        }

        public Tile(string line)
        {
            List<string> parts = [.. line.Split(',')];

			XPos = int.Parse(parts[0]);
            YPos = int.Parse(parts[1]);
		}
	}

    [DebuggerDisplay("<^{TopLeft} --- >^{TopRight} --- >v{BottomRight} --- <v{BottomLeft} | Size: {Size}")]
    public class Rectangle
    {
        public Tile TopLeft { get; set; } = null!;
        public Tile TopRight { get; set; } = null!;
        public Tile BottomLeft { get; set; } = null!;
        public Tile BottomRight { get; set; } = null!;
        public ulong Size { get; set; }

        public Rectangle(long lowestX, long highestX, long lowestY, long highestY)
        {
            TopLeft = new Tile { XPos = lowestX, YPos = lowestY };
            TopRight = new Tile { XPos = highestX, YPos = lowestY };
            BottomLeft = new Tile { XPos = lowestX, YPos = highestY };
            BottomRight = new Tile { XPos = highestX, YPos = highestY };
            Size = GetRectangleSize();
        }

        public ulong GetRectangleSize()
        {
            long width = Math.Abs(TopLeft.XPos - TopRight.XPos) + 1;
            long height = Math.Abs(TopLeft.YPos - BottomLeft.YPos) + 1;

            return (ulong)(width * height);
		}

        public bool IsTileInRectangle(Tile tile)
        {
            return tile.XPos > TopLeft.XPos 
                && tile.XPos < TopRight.XPos
                && tile.YPos > TopLeft.YPos
                && tile.YPos < BottomLeft.YPos;
        }

        public bool DoesLineCrossBounds(Line inputLine)
        {
            Line topLine = new Line { Tile1 = TopLeft, Tile2 = TopRight };
            Line rightLine = new Line { Tile1 = TopRight, Tile2 = BottomRight };
            Line bottomLine = new Line { Tile1 = BottomRight, Tile2 = BottomLeft };
            Line leftLine = new Line { Tile1 = BottomLeft, Tile2 = TopLeft };

            if (inputLine.DoLinesCross(topLine) 
                || inputLine.DoLinesCross(rightLine) 
                || inputLine.DoLinesCross(bottomLine) 
                || inputLine.DoLinesCross(leftLine))
            {
                return true;
            }

            return false;
        }

        public static List<Rectangle> GetAllRectangles(List<Tile> redTiles, bool removeOutOfBounds)
        {
            List<Rectangle> rectangles = [];
            List<Line> lines = [];
            Tile? lastTile = null;

            for (int index = 0; index < redTiles.Count; index++)
            {
                lastTile = redTiles[index];

                Tile? nextTile = redTiles.ElementAtOrDefault(index + 1);

                if (nextTile == null)
                {
                    break;
                }

                lines.Add(new Line { Tile1 = lastTile, Tile2 = nextTile });
            }

            for (int outerTileIndex = 0; outerTileIndex < redTiles.Count; outerTileIndex++)
            {
                for (int innerTileIndex = outerTileIndex + 1; innerTileIndex < redTiles.Count; innerTileIndex++)
                {
                    Tile innerTile = redTiles[innerTileIndex];
                    Tile outerTile = redTiles[outerTileIndex];

                    long lowestX = Math.Min(innerTile.XPos, outerTile.XPos);
                    long lowestY = Math.Min(innerTile.YPos, outerTile.YPos);
                    long highestX = Math.Max(innerTile.XPos, outerTile.XPos);
                    long highestY = Math.Max(innerTile.YPos, outerTile.YPos);

                    Rectangle rectangle = new Rectangle(lowestX, highestX, lowestY, highestY);

                    rectangles.Add(rectangle);
                }
            }

            if (removeOutOfBounds)
            {
                rectangles.RemoveAll(r => redTiles.Any(t => r.IsTileInRectangle(t)) || lines.Any(l => r.DoesLineCrossBounds(l)));
            }

            return rectangles;
		}
	}

    public class Line
    {
        public Tile Tile1 { get; set; } = null!;
        public Tile Tile2 { get; set; } = null!;

        public bool IsStraight => Tile1.XPos == Tile2.XPos || Tile1.YPos == Tile2.YPos;
        public bool IsVertical => IsStraight && Tile1.XPos == Tile2.XPos;
        public bool IsHorizontal => IsStraight && Tile1.YPos == Tile2.YPos;

        public bool DoLinesCross(Line otherLine)
        {
            //Parallel lines
            if ((IsHorizontal && otherLine.IsHorizontal) || (IsVertical && otherLine.IsVertical))
            {
                return false;
            }

            //Crossing lines
            if ((IsHorizontal 
                    && otherLine.IsVertical 
                    && Math.Min(Tile1.XPos, Tile2.XPos) < otherLine.Tile1.XPos 
                    && Math.Max(Tile1.XPos, Tile2.XPos) > otherLine.Tile1.XPos
                    && Math.Min(otherLine.Tile1.YPos, otherLine.Tile2.YPos) < Tile1.YPos
                    && Math.Max(otherLine.Tile1.YPos, otherLine.Tile2.YPos) > Tile1.YPos)
                || (IsVertical 
                    && otherLine.IsHorizontal 
                    && Math.Min(Tile1.YPos, Tile2.YPos) < otherLine.Tile1.YPos 
                    && Math.Max(Tile1.YPos, Tile2.YPos) > otherLine.Tile1.YPos
                    && Math.Min(otherLine.Tile1.XPos, Tile2.XPos) < Tile1.XPos
                    && Math.Max(otherLine.Tile1.XPos, Tile2.XPos) > Tile1.XPos))
            {
                return true;
            }

            return false;
        }

        public bool IsAbove(int index)
        {
            return (IsHorizontal && Tile1.YPos < index) || (IsVertical && Math.Max(Tile1.YPos, Tile2.YPos) < index);
        }

        public bool IsBelow(int index)
        {
            return (IsHorizontal && Tile1.YPos > index) || (IsVertical && Math.Min(Tile1.YPos, Tile2.YPos) > index);
        }

        public bool IsLeftOf(int index)
        {
            return (IsHorizontal && Math.Max(Tile1.XPos, Tile2.XPos) < index) || (IsVertical && Tile1.XPos < index);
        }

        public bool IsLeftRight(int index)
        {
            return (IsHorizontal && Math.Min(Tile1.XPos, Tile2.XPos) > index) || (IsVertical && Tile1.XPos > index);
        }
    }
}
