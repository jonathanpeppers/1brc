
// https://github.com/CarlVerret/csFastFloat
using csFastFloat;

var measurements = new Dictionary<string, Measurement>(StringComparer.Ordinal);

// Define variables outside of the loop
int index;
string name;
string? line;
float value;

using var stream = File.OpenRead("./measurements.txt");
using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, false, 1024 * 1024);

while ((line = reader.ReadLine()) != null)
{
    index = line.IndexOf(';');
    name = line[..index];
    value = FastFloatParser.ParseFloat(line[(index + 1)..]);
    if (measurements.TryGetValue(name, out var m))
    {
        m.Add(value);
    }
    else
    {
        measurements.Add(name, new Measurement(value));
    }
}

// Print output, this part is not really performance critical
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
        if (value < Min)
        {
            Min = value;
        }
        if (value > Max)
        {
            Max = value;
        }
    }

    public float Min;

    public int Count;
    public float Total;
    public float Max;

    public float Mean => Total / Count;
}
