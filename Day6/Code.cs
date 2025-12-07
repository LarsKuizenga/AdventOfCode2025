namespace AdventOfCode2025.Day6;

static class Code
{
    public static void Solve()
    {
        string[] input = File.ReadAllLines("../../../Day6/Input.txt");
        string[] exampleInput = File.ReadAllLines("../../../Day6/ExampleInput.txt");

        PartOne(exampleInput, input);
        PartTwo(exampleInput, input);
    }

    private static void PartOne(string[] exampleInput, string[] input)
    {
        ulong exampleInputAnswer = 4277556;
        ulong exampleOutput = SolvePartOne(exampleInput, 3);

        if (exampleInputAnswer != exampleOutput)
        {
            Console.WriteLine($"Part one example answer did not come out! Expected {exampleInputAnswer}, but was {exampleOutput}");
            return;
        }
        else
        {
            ulong answer = SolvePartOne(input, 4);
            Console.WriteLine($"Part one answer: {answer}");
        }
    }

    private static void PartTwo(string[] exampleInput, string[] input)
    {
        ulong exampleInputAnswer = 3263827;
        ulong exampleOutput = SolvePartTwo(exampleInput, 3);

        if (exampleOutput != exampleInputAnswer)
        {
            Console.WriteLine($"Part two example answer did not come out! Expected {exampleInputAnswer}, but was {exampleOutput}");
            return;
        }
        else
        {
            ulong answer = SolvePartTwo(input, 4);
            Console.WriteLine($"Part two answer: {answer}");
        }
    }

    private static ulong SolvePartOne(string[] input, int NumberCount)
    {
        List<Problem> problems = Problem.ParseInput(input, NumberCount);
        ulong total = 0;

        foreach (Problem problem in problems)
        {
            total += problem.Solve();
        }

        return total;
    }

    private static ulong SolvePartTwo(string[] input, int NumberCount)
    {
        List<Problem> problems = Problem.ParseInputForCepholopods(input, NumberCount);
        ulong total = 0;

        foreach (Problem problem in problems)
        {
            total += problem.Solve();
        }

        return total;
    }

    public class Problem
    {
        public List<ulong> Numbers { get; set; } = [];
        public Operation Operation { get; set; }

        public static List<Problem> ParseInput(string[] input, int numberCount)
        {
            List<Problem> problems = [];
            List<string> splitLine;

            for (int lineIndex = 0; lineIndex < numberCount; lineIndex++)
            {
                splitLine = input[lineIndex].Split(" ").ToList();
                splitLine.RemoveAll(n => n == string.Empty);

                List<ulong> numbers = splitLine.Select(ulong.Parse).ToList();

                if (lineIndex == 0)
                {
                    for (int numberIndex = 0; numberIndex < numbers.Count; numberIndex++)
                    {
                        problems.Add(new Problem());
                    }
                }

                for (int problemIndex = 0; problemIndex < problems.Count; problemIndex++)
                {
                    Problem problem = problems[problemIndex];
                    problem.Numbers.Add(numbers[problemIndex]);
                }
            }

            splitLine = input[numberCount].Split(" ").ToList();
            splitLine.RemoveAll(n => n == string.Empty);

            List<Operation> operations = splitLine.Select(ParseToOperation).ToList();

            for (int problemIndex = 0; problemIndex < problems.Count; problemIndex++)
            {
                problems[problemIndex].Operation = operations[problemIndex];
            }

            return problems;
        }

        public static List<Problem> ParseInputForCepholopods(string[] input, int numberCount)
        {
            List<Problem> problems = [];

            int lineLength = input[0].Length;

            Problem problem = new Problem();

            for (int columnIndex = 0; columnIndex < lineLength; columnIndex++)
            {                
                string numberString = string.Empty;
                int digitCharEmptyCount = 0;

                for (int rowIndex = 0; rowIndex < numberCount; rowIndex++)
                {
                    char digitChar = input[rowIndex].ElementAtOrDefault(columnIndex);

                    if (digitChar == default || digitChar == ' ')
                    {
                        digitCharEmptyCount++;
                        continue;
                    }

                    numberString += digitChar;
                }

                if (digitCharEmptyCount == numberCount)
                {
                    problems.Add(problem);
                    problem = new Problem();
                }
                else
                {
                    problem.Numbers.Add(ulong.Parse(numberString));
                }

                if (columnIndex == lineLength -1)
                {
                    problems.Add(problem);
                }
            }

            List<string> splitLine = input[numberCount].Split(" ").ToList();
            splitLine.RemoveAll(n => n == string.Empty);

            List<Operation> operations = splitLine.Select(ParseToOperation).ToList();

            for (int problemIndex = 0; problemIndex < problems.Count; problemIndex++)
            {
                problems[problemIndex].Operation = operations[problemIndex];
            }

            return problems;
        }

        public ulong Solve()
        {
            ulong answer = 0;

            for (int index = 0; index < Numbers.Count; index++)
            {
                ulong number = Numbers[index];
                if (Operation == Operation.Addition)
                {
                    answer += number;
                }
                else
                {
                    if (index == 0)
                    {
                        answer = number;
                        continue;
                    }

                    answer *= number;
                }
            }

            return answer;
        }
    }

    public enum Operation
    {
        Addition,
        Multiplication
    }

    public static Operation ParseToOperation(string input)
    {
        if (input == "*")
        {
            return Operation.Multiplication;
        }
        else
        {
            return Operation.Addition;
        }
    }
}
