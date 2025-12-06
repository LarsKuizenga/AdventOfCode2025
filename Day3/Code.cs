namespace AdventOfCode2025.Day3;

static class Code
{
    public static void Solve()
    {
        string[] input = File.ReadAllLines("../../../Day3/Input.txt");
        string[] exampleInput = File.ReadAllLines("../../../Day3/ExampleInput.txt");

        PartOne(exampleInput, input);
        PartTwo(exampleInput, input);
    }

    private static void PartOne(string[] exampleInput, string[] input)
    {
        int exampleInputAnswer = 357;
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
        ulong exampleInputAnswer = 3121910778619;
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

    private static int SolvePartOne(string[] input)
    {
        List<Batterypack> batterypacks = [];

        foreach (string line in input)
        {
            batterypacks.Add(new Batterypack(line));
        }

        int totalJoltage = 0;

        foreach (Batterypack batterypack in batterypacks)
        {
            totalJoltage += batterypack.GetSafeJoltage();
        }

        return totalJoltage;
    }

    private static ulong SolvePartTwo(string[] input)
    {
        List<Batterypack> batterypacks = [];

        foreach (string line in input)
        {
            batterypacks.Add(new Batterypack(line));
        }

        ulong totalJoltage = 0;

        foreach (Batterypack batterypack in batterypacks)
        {
            totalJoltage += batterypack.GetUnsafeJoltage();
        }

        return totalJoltage;
    }

    public class Batterypack
    {
        public List<int> Batteries = [];

        public Batterypack(string packInput)
        {
            foreach (char character in packInput)
            {
                Batteries.Add(int.Parse(character.ToString()));
            }
        }

        public int GetMaxNumberIndex()
        {
            int max = Batteries.Max();

            return Batteries.FindIndex(i => i == max);
        }

        public int GetMaxNumberIndex(int lowerIndex, int upperIndex)
        {
            int max = Batteries[lowerIndex..upperIndex].Max();

            return Batteries[lowerIndex..upperIndex].FindIndex(i => i == max) + lowerIndex;
        }

        public int GetSafeJoltage()
        {
            int maxNumberIndex = GetMaxNumberIndex();
            string leftNumber;
            string rightNumber;

            if (maxNumberIndex == Batteries.Count - 1)
            {
                rightNumber = Batteries[maxNumberIndex].ToString();

                int lowerIndex = 0;
                int upperIndex = maxNumberIndex;

                int leftNumberIndex = GetMaxNumberIndex(lowerIndex, upperIndex);

                leftNumber = Batteries[leftNumberIndex].ToString();
            }
            else
            {
                leftNumber = Batteries[maxNumberIndex].ToString();

                int lowerIndex = maxNumberIndex + 1;
                int upperIndex = Batteries.Count;

                int rightNumberIndex = GetMaxNumberIndex(lowerIndex, upperIndex);

                rightNumber = Batteries[rightNumberIndex].ToString();
            }

            return int.Parse(leftNumber + rightNumber);
        }

        public ulong GetUnsafeJoltage()
        {
            int maxNumberIndex = GetMaxNumberIndex(0, Batteries.Count - 11);

            if (maxNumberIndex == Batteries.Count - 12)
            {
                return ulong.Parse(string.Join("", Batteries[maxNumberIndex..Batteries.Count]));
            }

            List<int> numbers = [];
            numbers.Add(Batteries[maxNumberIndex]);

            for (int index = 10; index >= 0; index--)
            {
                maxNumberIndex = GetMaxNumberIndex(maxNumberIndex + 1, Batteries.Count - index);
                numbers.Add(Batteries[maxNumberIndex]);

                if (maxNumberIndex == Batteries.Count - index - 1)
                {
                    numbers.AddRange(Batteries[(maxNumberIndex + 1)..Batteries.Count]);
                    break;
                }
            }

            return ulong.Parse(string.Join("", numbers));
        }
    }
}
