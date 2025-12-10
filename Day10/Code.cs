using System.Diagnostics;

namespace AdventOfCode2025.Day10;

static class Code
{
    public static void Solve()
    {
        string[] input = File.ReadAllLines("../../../Day10/Input.txt");
        string[] exampleInput = File.ReadAllLines("../../../Day10/ExampleInput.txt");

        PartOne(exampleInput, input);
        PartTwo(exampleInput, input);
    }

    private static void PartOne(string[] exampleInput, string[] input)
    {
        int exampleInputAnswer = 7;
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
        int exampleInputAnswer = 33;
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
        List<Machine> machines = [];

        foreach (string line in input)
        {
            machines.Add(new Machine(line));
        }

        int totalButtonPresses = 0;

        foreach (Machine machine in machines)
        {
            totalButtonPresses += machine.GetLeastButtonPressesNeededForLights();
        }

        return totalButtonPresses;
    }

    private static int SolvePartTwo(string[] input)
    {
        List<Machine> machines = [];

        foreach (string line in input)
        {
            machines.Add(new Machine(line));
        }

        int totalButtonPresses = 0;

        foreach (Machine machine in machines)
        {
            totalButtonPresses += machine.GetLeastButtonPressesNeededForJoltages();
        }

        return totalButtonPresses;
    }

    public class Machine
    {
        public List<Light> Lights { get; set; } = [];
        public List<Light> DesiredLightState { get; set; } = [];
        public List<Button> Buttons { get; set; } = [];
        public List<int> Joltages { get; set; } = [];
        public List<int> DesiredJoltages { get; set; } = [];

        public Machine(string line)
        {
            string[] parts = line.Split(" ");

            string lights = parts[0][1..^1];
            string[] buttons = parts[1..^1];
            string joltages = parts[^1][1..^1];

            for (int lightIndex = 0; lightIndex < lights.Length; lightIndex++)
            {
                Lights.Add(new Light());
                DesiredLightState.Add(new Light(lights[lightIndex]));
            }

            for (int buttonIndex = 0; buttonIndex < buttons.Length; buttonIndex++)
            {
                Buttons.Add(new Button(buttons[buttonIndex]));
            }

            for (int joltageIndex = 0; joltageIndex < joltages.Length; joltageIndex++)
            {
                Joltages.Add(0);
                DesiredJoltages = joltages[1..^1].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            }
        }

        public bool AreLightsInDesiredState()
        {
            for (int index = 0; index < Lights.Count; index++)
            {
                if (Lights[index].IsOn != DesiredLightState[index].IsOn)
                {
                    return false;
                }
            }

            return true;
        }

        public bool AreJoltagesInDesiredState()
        {
            for (int index = 0; index < Joltages.Count; index++)
            {
                if (Joltages[index] != DesiredJoltages[index])
                {
                    return false;
                }
            }

            return true;
        }

        public void ResetLights()
        {
            foreach (Light light in Lights)
            {
                light.IsOn = false;
            }
        }

        public void ResetJoltages()
        {
            for (int i = 0; i < Joltages.Count - 1; i++)
            {
                Joltages[i] = 0;
            }
        }

        public int GetLeastButtonPressesNeededForLights()
        {
            List<List<int>> buttonCombinations = AllPossibleButtonCombinations(Enumerable.Range(0, Buttons.Count).ToList()).OrderBy(b => b.Count).ToList();
            buttonCombinations.RemoveAt(0); //Remove first entry because it's empty

            foreach (List<int> buttonCombination in buttonCombinations)
            {
                ResetLights();

                if (IsCombinationValidWithLights(buttonCombination))
                {
                    return buttonCombination.Count;
                }
            }

            throw new Exception("None of the button combinations can reach the desired state");
        }

        public int GetLeastButtonPressesNeededForJoltages()
        {
            List<List<int>> buttonCombinations = AllPossibleButtonCombinations(Enumerable.Range(0, Buttons.Count).ToList()).OrderBy(b => b.Count).ToList();
            buttonCombinations.RemoveAt(0); //Remove first entry because it's empty

            foreach (List<int> buttonCombination in buttonCombinations)
            {
                ResetJoltages();

                if (IsCombinationValidWithJoltages(buttonCombination))
                {
                    return buttonCombination.Count;
                }
            }

            throw new Exception("None of the button combinations can reach the desired state");
        }

        //Genakte methode van internet >:)
        static List<List<int>> AllPossibleButtonCombinations(List<int> list)
        {
            int listCount = list.Count;
            List<List<int>> result = new List<List<int>>();

            for (int outerIndex = 0; outerIndex < (1 << listCount); outerIndex++)
            {
                List<int> subset = [];
                for (int innerIndex = 0; innerIndex < listCount; innerIndex++)
                {
                    if ((outerIndex & (1 << innerIndex)) != 0)
                    {
                        subset.Add(list[innerIndex]);
                    }
                }
                result.Add(subset);
            }

            return result;
        }

        public bool IsCombinationValidWithLights(List<int> combination)
        {
            foreach (int button in combination)
            {
                PressButtonForLights(button);
            }

            return AreLightsInDesiredState();
        }

        public bool IsCombinationValidWithJoltages(List<int> combination)
        {
            foreach (int button in combination)
            {
                PressButtonForJoltages(button);
            }

            return AreJoltagesInDesiredState();
        }

        public void PressButtonForLights(int buttonIndex)
        {
            Button button = Buttons[buttonIndex];

            foreach (int lightToggle in button.ToggleIndices)
            {
                Lights[lightToggle].Toggle();
            }
        }

        public void PressButtonForJoltages(int buttonIndex)
        {
            Button button = Buttons[buttonIndex];

            foreach (int toggleIndex in button.ToggleIndices)
            {
                Joltages[toggleIndex] += 1;
            }
        }
    }

    public class Light
    {
        public bool IsOn { get; set; }

        public Light()
        {

        }

        public Light(char input)
        {
            IsOn = input == '#';
        }

        public void Toggle()
        {
            IsOn = !IsOn;
        }
    }

    public class Button
    {
        public List<int> ToggleIndices { get; set; } = [];

        public Button(string input)
        {
            List<int> parts = input[1..^1].Split(",").Select(int.Parse).ToList();

            ToggleIndices.AddRange(parts);
        }
    }
}
