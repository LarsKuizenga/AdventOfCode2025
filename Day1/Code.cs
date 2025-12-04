namespace AdventOfCode2025.Day1;

static class Code
{
    public static void SolvePartOne()
    {
        string[] lines = File.ReadAllLines("../../../Day1/Input.txt");

        int curr_position = 50;
        int zero_occured_count = 0;

        foreach (string line in lines)
        {
            if (line[0] == 'L')
            {
                curr_position -= int.Parse(line[1..]);
            }
            else
            {
                curr_position += int.Parse(line[1..]);
            }

            if (curr_position % 100 == 0)
            {
                zero_occured_count++;
            }
        }

        Console.WriteLine(zero_occured_count);
    }

    public static void SolvePartTwo()
    {
        string[] lines = File.ReadAllLines("../../../Day1/Input.txt");

        int curr_position = 100050;
        int curr_hundreds = 1000;
        int zero_occured_count = 0;
        bool prev_was_zero = false;

        foreach (string line in lines)
        {
            if (line[0] == 'L')
            {
                curr_position -= int.Parse(line[1..]);
            }
            else
            {
                curr_position += int.Parse(line[1..]);
            }

            int new_hundreds = curr_position / 100;

            if (new_hundreds != curr_hundreds)
            {
                int hundreds_change = Math.Abs(new_hundreds - curr_hundreds);

                if ((curr_position % 100 == 0 && line[0] == 'R') || (prev_was_zero && line[0] == 'L'))
                {
                    hundreds_change -= 1;
                }

                zero_occured_count += hundreds_change;
            }

            if (curr_position % 100 == 0)
            {
                zero_occured_count++;
                prev_was_zero = true;
            }
            else
            {
                prev_was_zero = false;
            }

            curr_hundreds = new_hundreds;

            //Console.WriteLine($"{line}\t{curr_position}\t{zero_occured_count}");
        }

        Console.WriteLine(zero_occured_count);
    }
}
