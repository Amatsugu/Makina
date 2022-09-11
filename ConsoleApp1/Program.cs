using Makina.Calculation;
using Makina.Neural;

using System.Diagnostics;

namespace ConsoleApp1;

internal class Program
{
	private static void Main(string[] args)
	{
		var n = 50000;
		var data = Enumerable.Range(1, n);
		var result = data.Select(d => d /2f == ( d/2 ) ? 1 : 0);
		var x = new Matrix(1, n, data.Select(n => n * 1f).ToArray());
		var y = new Matrix(1, n, result.Select(n => n * 1f).ToArray());

		var r = Enumerable.Range(1, 10);
		var p = new Matrix(1, 10, r.Select(n => n * 1f).ToArray());
		var net = new NeuralNetwork(x, y, 3);

		net.Train(100);
		Console.WriteLine(net.Predict(p));
	}



}