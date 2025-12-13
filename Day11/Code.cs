namespace AdventOfCode2025.Day11;

static class Code
{
    public static void Solve()
    {
        string[] input = File.ReadAllLines("../../../Day11/Input.txt");
        string[] exampleInputPart1 = File.ReadAllLines("../../../Day11/ExampleInputPart1.txt");
        string[] exampleInputPart2 = File.ReadAllLines("../../../Day11/ExampleInputPart2.txt");

        PartOne(exampleInputPart1, input);
        PartTwo(exampleInputPart2, input);
    }

    private static void PartOne(string[] exampleInput, string[] input)
    {
        int exampleInputAnswer = 5;
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
        int exampleInputAnswer = 2;
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
        List<Device> devices = input.Select(line => new Device(line)).ToList();

        return Device.FindAllPaths(devices, "you", "out");
    }

    private static int SolvePartTwo(string[] input)
    {
        List<Device> devices = input.Select(line => new Device(line)).ToList();

        //int pathsDacToSvr = Device.FindAllPaths(devices, "dac", "svr", false);
        int pathsFftToSvr = Device.FindAllPaths(devices, "fft", "svr", false);

        int pathsDacToFft = Device.FindAllPaths(devices, "dac", "fft");
        int pathsFftToDac = Device.FindAllPaths(devices, "dac", "fft", false);

        int pathsDacToOut = Device.FindAllPaths(devices, "dac", "out");
        //int pathsFftToOut = Device.FindAllPaths(devices, "fft", "out");

        return 2;
    }

    public class Device
    {
        public string Input { get; set; }
        public List<string> Output { get; set; } = [];

        public Device(string line)
        {
            string[] parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            Input = parts[0][..^1];

            for (int partIndex = 1; partIndex < parts.Length; partIndex++)
            {
                Output.Add(parts[partIndex]);
            }
        }

        public static int FindAllPaths(List<Device> devices, string startingNodeName, string endingNodename, bool outputToInput = true)
        {
            Device startingDevice = devices.First(d => d.Input == startingNodeName);

            return FindPath(startingDevice, devices, endingNodename, outputToInput);
        }

        public static int FindPath(Device device, List<Device> devices, string endingNodename, bool outputToInput)
        {
            int total = 0;

            if (outputToInput)
            {
                if (device.Output.Any(o => o == endingNodename))
                {
                    return 1;
                }

                if (device.Output.Any(o => o == "out"))
                {
                    return 0;
                }

                foreach (string output in device.Output)
                {
                    List<Device> inputDevices = devices.Where(d => d.Input == output).ToList();

                    foreach (Device inputDevice in inputDevices)
                    {
                        total += FindPath(inputDevice, devices, endingNodename, outputToInput);
                    }
                }
            }
            else
            {
                if (device.Input == endingNodename)
                {
                    return 1;
                }

                if (device.Output.First() == "out")
                {
                    return 0;
                }

                List<Device> outputDevices = devices.Where(d => d.Output.Any(o => o == device.Input)).ToList();

                foreach (Device outputDevice in outputDevices)
                {
                    total += FindPath(outputDevice, devices, endingNodename, outputToInput);
                }
            }

            return total;
        }
    }
}
