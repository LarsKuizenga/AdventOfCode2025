using System.Diagnostics;

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
        public bool IsOutside { get; set; }
        public bool IsRight { get; set; }

        public Tile()
        {

        }

        public Tile(string line)
        {
            List<string> parts = [.. line.Split(',')];

			XPos = int.Parse(parts[0]);
            YPos = int.Parse(parts[1]);
		}

        public bool HasSamePosition(Tile otherTile)
        {
            return XPos == otherTile.XPos && YPos == otherTile.YPos;
        }
	}

    [DebuggerDisplay("<^{TopLeft} --- >^{TopRight} --- >v{BottomRight} --- <v{BottomLeft} | Size: {Size}")]
    public class Rectangle
    {
        public Tile TopLeft { get; set; } = null!;
        public Tile TopRight { get; set; } = null!;
        public Tile BottomLeft { get; set; } = null!;
        public Tile BottomRight { get; set; } = null!;
        public bool IsOutside { get; set; }
        public ulong Size { get; set; }

        public Rectangle(long lowestX, long highestX, long lowestY, long highestY)
        {
            TopLeft = new Tile { XPos = lowestX, YPos = lowestY, IsRight = false };
            TopRight = new Tile { XPos = highestX, YPos = lowestY, IsRight = true };
            BottomLeft = new Tile { XPos = lowestX, YPos = highestY, IsRight = false };
            BottomRight = new Tile { XPos = highestX, YPos = highestY, IsRight = true };
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

        public List<Tile> GetRectangleCorners()
        {
            return [TopLeft, TopRight, BottomRight, BottomLeft];
        }

        public bool SetIsOutside(List<Line> lines)
        {
            List<Tile> corners = GetRectangleCorners();

            foreach (Tile corner in corners)
            {
                bool isOnAnyLine = 
                    (corner.IsRight 
                    && lines.Any(l => l.IsVertical && (corner.HasSamePosition(l.Tile1) || corner.HasSamePosition(l.Tile2))))
                    || lines.Any(l => l.IsHorizontal && l.IsTileOnLine(corner));
                
                int rightLineCount = isOnAnyLine ? 1 : 0;

                foreach (Line line in lines)
                {
                    if (line.IsAbove(corner.YPos) ||
                        line.IsLeftOf(corner.XPos) ||
                        line.IsBelow(corner.YPos) ||
                        (line.IsHorizontal && line.Tile1.YPos == corner.YPos && !line.IsTileOnLine(corner)))
                    {
                        continue;
                    }

                    if (line.IsVertical && line.IsRightOf(corner.XPos))
                    {
                        rightLineCount++;
                    }
                }

                if (rightLineCount % 2 == 0)
                {
                    corner.IsOutside = true;
                }
            }

            if (corners.Any(c => c.IsOutside))
            {
                IsOutside = true;
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
                    lines.Add(new Line { Tile1 = lastTile, Tile2 = redTiles[0] });
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
                foreach (Rectangle rectangle in rectangles)
                {
                     rectangle.SetIsOutside(lines);
                }

                rectangles.RemoveAll(r => r.IsOutside);
            }

            //227697704
            //4064712769

            return rectangles;
		}
	}

    [DebuggerDisplay("Tile1: {Tile1} {IsVertical ? | : -} Tile2: {Tile2} --- IsOutside = {IsOutside}")]
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

        public bool IsAbove(long yPos)
        {
            return (IsHorizontal && Tile1.YPos < yPos) || (IsVertical && Math.Max(Tile1.YPos, Tile2.YPos) < yPos);
        }

        public bool IsBelow(long yPos)
        {
            return (IsHorizontal && Tile1.YPos > yPos) || (IsVertical && Math.Min(Tile1.YPos, Tile2.YPos) > yPos);
        }

        public bool IsLeftOf(long xPos)
        {
            return (IsHorizontal && Math.Max(Tile1.XPos, Tile2.XPos) < xPos) || (IsVertical && Tile1.XPos < xPos);
        }

        public bool IsRightOf(long xPos)
        {
            return (IsHorizontal && Math.Min(Tile1.XPos, Tile2.XPos) > xPos) || (IsVertical && Tile1.XPos > xPos);
        }

        public bool IsTileOnLine(Tile tile)
        {
            return (IsHorizontal && tile.YPos == Tile1.YPos && tile.XPos >= Math.Min(Tile1.XPos, Tile2.XPos) && tile.XPos <= Math.Max(Tile1.XPos, Tile2.XPos))
                || (IsVertical   && tile.XPos == Tile1.XPos && tile.YPos >= Math.Min(Tile1.YPos, Tile2.YPos) && tile.YPos <= Math.Max(Tile1.YPos, Tile2.YPos));
        }
    }
}
