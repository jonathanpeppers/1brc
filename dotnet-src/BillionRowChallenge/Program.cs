
using var stream = File.OpenRead("./measurements.txt");
using var reader = new StreamReader(stream);

var measurements = new Dictionary<string, Measurement>(StringComparer.Ordinal);

while (!reader.EndOfStream)
{
    var line = reader.ReadLine();
    if (line == null)
        continue;
    int index = line.IndexOf(';');
    string name = line.Substring(0, index);
    float value = float.Parse(line.Substring(index + 1));
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
