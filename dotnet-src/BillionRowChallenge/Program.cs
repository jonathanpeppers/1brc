
// https://github.com/CarlVerret/csFastFloat
using System.Text;
using csFastFloat;

const int BufferSize = 64 * 1024;
var buffer = new byte[BufferSize];
var chars = new char[1024];
var measurements = new Dictionary<string, Measurement>(StringComparer.Ordinal);
var encoding = Encoding.UTF8;

// Define variables outside of the loop
int index;
string name;
Span<byte> chunk;
ReadOnlySpan<char> line = chars.AsSpan();
int lineLength;
float value;
int bytes;
int offset = 0;

using var stream = File.OpenRead("./measurements.txt");

do
{
READ:
    bytes = stream.Read(buffer, offset, BufferSize - offset);
    if (bytes > 0)
    {
        chunk = buffer.AsSpan();

        do
        {
            if (offset == BufferSize)
            {
                offset = 0;
                goto READ;
            }

            // Find the next newline
            index = chunk.IndexOf((byte)'\n');
            if (index == -1)
            {
                // No \n found, so copy the last partial line to the beginning of the buffer
                chunk[offset..].CopyTo(buffer);
                offset += bytes;
                goto READ;
            }
            lineLength = encoding.GetChars(chunk[..index], chars.AsSpan());

            // Advance chunk
            chunk = chunk[(index + 1)..];

            // Now find ; within the line
            index = line.IndexOf(';');
            if (index == -1)
            {
                // No ; found, so copy the last partial line to the beginning of the buffer
                chunk[offset..].CopyTo(buffer);
                offset += bytes;
                goto READ;
            }
            name = line[..index].ToString();
            value = FastFloatParser.ParseFloat(line[(index + 1)..lineLength]);
            if (measurements.TryGetValue(name, out var m))
            {
                m.Add(value);
            }
            else
            {
                measurements.Add(name, new Measurement(value));
            }
        } while (true);

        // if we get here, reset offset
        offset = 0;
    }
} while (bytes > 0);

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
