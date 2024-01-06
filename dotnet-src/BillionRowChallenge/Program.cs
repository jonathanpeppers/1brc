﻿
using var stream = File.OpenRead("./measurements.txt");
using var reader = new StreamReader(stream);

var measurements = new Dictionary<string, Measurement>(StringComparer.Ordinal);

// Define variables outside of the loop
string line;
int index;
string name;
float value;

while (!reader.EndOfStream)
{
    line = reader.ReadLine()!;
    index = line.IndexOf(';');
    name = line[..index];
    value = FastParse(line.AsSpan()[(index + 1)..]);
    if (measurements.TryGetValue(name, out var m))
    {
        m.Add(value);
    }
    else
    {
        measurements.Add(name, new Measurement(value));
    }
}

Console.Write('{');
bool first = true;
foreach (var measurement in measurements.OrderBy(x => x.Key))
{
    if (first)
    {
        first = false;
    }
    else
    {
        Console.Write(", ");
    }
    Console.Write($"{measurement.Key}={measurement.Value.Min:0.#}/{measurement.Value.Mean:0.#}/{measurement.Value.Max:0.#}");
}
Console.WriteLine('}');

static float FastParse(ReadOnlySpan<char> text)
{
    float value = 0;
    int position = 0;
    bool negative = false;
    char c;
    if (text[0] == '-')
    {
        negative = true;
        position++;
    }
    while (position < text.Length)
    {
        c = text[position];
        if (c == '.')
        {
            break;
        }
        value *= 10;
        value += c - '0';
        position++;
    }
    return negative ? -value : value;
}

class Measurement
{
    public Measurement(float value)
    {
        Count++;
        Min = Max = Total = value;
    }

    public void Add (float value)
    {
        Count++;
        Total += value;
        Min = Math.Min(Min, value);
        Max = Math.Max(Max, value);
    }

    public float Min { get; set; }

    public int Count { get; set; }
    public float Total { get; set; }
    public float Max { get; set; }

    public float Mean => Total / Count;
}
