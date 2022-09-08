using Makina.Calculation;

using System.Diagnostics;

namespace ConsoleApp1;

internal class Program
{
	private static void Main(string[] args)
	{
		var m = new Matrix(2, 4, new float[] { 3, 2, 1, 5, 9, 1, 3, 0 });
		var m2 = new Matrix(4, 3, new float[] { 2, 9, 0, 1, 3, 5, 2, 4, 7, 8, 1, 5 });
		Console.WriteLine(m);
		Console.WriteLine(m2);

		var sw = Stopwatch.StartNew();
		var m3 = m2.Transpose();
		sw.Stop();
		Console.WriteLine($"For: {sw.ElapsedMilliseconds}ms");
		sw.Stop();
		Console.WriteLine(m3);
	}
}