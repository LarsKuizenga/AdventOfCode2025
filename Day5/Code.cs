namespace AdventOfCode2025.Day5;

static class Code
{
    public static void Solve()
    {
        string[] input = File.ReadAllLines("../../../Day5/Input.txt");
        string[] exampleInput = File.ReadAllLines("../../../Day5/ExampleInput.txt");

        PartOne(exampleInput, input);
        PartTwo(exampleInput, input);
    }

    private static void PartOne(string[] exampleInput, string[] input)
    {
        int exampleInputAnswer = 3;
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
        long exampleInputAnswer = 14;
        long exampleOutput = SolvePartTwo(exampleInput);

        if (exampleOutput != exampleInputAnswer)
        {
            Console.WriteLine($"Part two example answer did not come out! Expected {exampleInputAnswer}, but was {exampleOutput}");
            return;
        }
        else
        {
            long answer = SolvePartTwo(input);
            Console.WriteLine($"Part two answer: {answer}");
        }
    }

    private static int SolvePartOne(string[] input)
    {
        List<FreshRange> freshRanges = [];
        List<ulong> idList = [];
        int freshIdCount = 0;
        int idStartIndex = 0;

        for (int index = 0; index < input.Length; index++)
        {
            if (input[index] == string.Empty)
            {
                idStartIndex = index + 1;
                break;
            }

            freshRanges.Add(new FreshRange(input[index]));
        }

        for (int index = idStartIndex; index < input.Length; index++)
        {
            idList.Add(ulong.Parse(input[index]));
        }

        foreach (long id in idList)
        {
            if (freshRanges.Any(fr => fr.IsNumberInRange(id)))
            {
                freshIdCount++;
            }
        }

        return freshIdCount;
    }

    private static long SolvePartTwo(string[] input)
    {
        List<FreshRange> freshRanges = [];
        long possibleIdCount = 0;

        for (int index = 0; index < input.Length; index++)
        {
            if (input[index] == string.Empty)
            {
                break;
            }

            freshRanges.Add(new FreshRange(input[index]));
        }

        freshRanges = freshRanges.DistinctBy(fr => new { fr.Start, fr.End })
            .OrderBy(fr => fr.Start)
            .ThenBy(fr => fr.End)
            .ToList();

        for (int index = 0; index < freshRanges.Count; index++)
        {
            FreshRange freshRange = freshRanges[index];

            long oldPossibleIdCount = possibleIdCount;

            possibleIdCount += freshRange.End - freshRange.Start + 1;

            if (index == 0)
            {
                continue;
            }

            FreshRange prevFreshRange = freshRanges[index - 1];

            if (prevFreshRange.Start <= freshRange.Start && prevFreshRange.End >= freshRange.End)
            {
                possibleIdCount -= freshRange.End - freshRange.Start + 1;
                continue;
            }

            if (prevFreshRange.End >= freshRange.Start && prevFreshRange.End <= freshRange.End)
            {
                possibleIdCount -= prevFreshRange.End - freshRange.Start + 1;
            }
        }

        return possibleIdCount;
    }

    public class FreshRange
    {
        public long Start { get; set; }
        public long End { get; set; }

        public FreshRange(string inputLine)
        {
            string[] splitLine = inputLine.Split("-");

            Start = long.Parse(splitLine[0]);
            End = long.Parse(splitLine[1]);
        }

        public bool IsNumberInRange(long number)
        {
            return number >= Start && number <= End;
        }
    }
}
