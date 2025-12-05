namespace AdventOfCode2025.Day2;

static class Code
{
    public static void Solve()
    {
        string input = File.ReadAllText("../../../Day2/Input.txt");
        string exampleInput = File.ReadAllText("../../../Day2/ExampleInput.txt");

        PartOne(exampleInput, input);
        PartTwo(exampleInput, input);
    }

    private static void PartOne(string exampleInput, string input)
    {
        ulong exampleInputAnswer = 1227775554;
        ulong exampleOutput = SolvePartOne(exampleInput);

        if (exampleInputAnswer != exampleOutput)
        {
            Console.WriteLine($"Part one example answer did not come out! Expected {exampleInputAnswer}, but was {exampleOutput}");
            return;
        }
        else
        {
            ulong answer = SolvePartOne(input);
            Console.WriteLine($"Part one answer: {answer}");
        }
    }

    private static void PartTwo(string exampleInput, string input)
    {
        ulong exampleInputAnswer = 4174379265;
        ulong exampleOutput = SolvePartTwo(exampleInput);

        if (SolvePartTwo(exampleInput) != exampleInputAnswer)
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

    private static ulong SolvePartOne(string input)
    {
        List<IdRange> ranges = ParseInput(input);

        ulong total = 0;

        foreach (IdRange range in ranges)
        {
            total += range.GetTwiceReoccuringNumberTotal();
        }

        return total;
    }

    private static ulong SolvePartTwo(string input)
    {
        List<IdRange> ranges = ParseInput(input);

        ulong total = 0;

        foreach (IdRange range in ranges)
        {
            total += range.GetAnyReoccuringNumberTotal();
        }

        return total;
    }

    private static List<IdRange> ParseInput(string input)
    {
        string[] inputRanges = input.Split(",");
        List<IdRange> ranges = [];

        foreach (string inputRange in inputRanges)
        {
            ranges.Add(new IdRange { Start = ulong.Parse(inputRange.Split("-")[0]), End = ulong.Parse(inputRange.Split("-")[1]) });
        }

        return ranges;
    }

    public class IdRange()
    {
        public ulong Start { get; set; }
        public ulong End { get; set; }

        public ulong GetTwiceReoccuringNumberTotal()
        {
            ulong total = 0;

            for (ulong i = 0; i < End - Start + 1; i++)
            {
                string currNumber = (Start + i).ToString();

                if (currNumber.Length % 2 != 0)
                {
                    continue;
                }

                string partOne = currNumber[..(currNumber.Length / 2)];
                string partTwo = currNumber[(currNumber.Length / 2)..];

                if (partOne == partTwo)
                {
                    total += Start + i;
                }
            }

            return total;
        }

        public ulong GetAnyReoccuringNumberTotal()
        {
            ulong total = 0;

            for (ulong index = 0; index < End - Start + 1; index++)
            {
                string currNumber = (Start + index).ToString();
                
                if (IsNumberReoccurring(currNumber))
                {
                    total += Start + index;
                }
            }

            return total;
        }

        private static bool IsNumberReoccurring(string currNumber)
        {
            for (int index = 1; index < currNumber.Length / 2 + 1; index++)
            {
                string part = currNumber[..index];

                //It wouldn't fit 😩, so we skip
                if (currNumber.Length % index != 0)
                {
                    continue;
                }

                if (IsNumberPartReoccurring(currNumber, part))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsNumberPartReoccurring(string currNumber, string part)
        {
            for (int index = 0; index < currNumber.Length / part.Length; index++)
            {
                int fromIndex = index * part.Length;
                int toIndex = index * part.Length + part.Length;

                if (part != currNumber[fromIndex..toIndex])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
