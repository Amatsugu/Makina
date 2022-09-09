using Makina.Calculation;

using System.Diagnostics;

namespace ConsoleApp1;

internal class Program
{
	private static void Main(string[] args)
	{
		var m = new Matrix(2, 2, new float[] { 1, 2, 3, 4 });
		var m2 = new Matrix(2, 2, new float[] { 5, 6, 7, 8 });
		Console.WriteLine(m);
		Console.WriteLine(m2);

		var sw = Stopwatch.StartNew();
		var dot = m.Dot(m2);
		var dot2 = m.Multiply(m2);
		sw.Stop();
		Console.WriteLine($"For: {sw.ElapsedMilliseconds}ms");
		sw.Stop();
		Console.WriteLine(dot);
		Console.WriteLine(dot2);
	}
}