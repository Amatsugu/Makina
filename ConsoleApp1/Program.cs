using Makina.Calculation;
using Makina.Neural;

using System.Diagnostics;

namespace ConsoleApp1;

internal class Program
{
	private static void Main(string[] args)
	{
		var n = 10;
		var data = Enumerable.Range(1, n);
		var result = data.Select(d => d /2f == ( d/2 ) ? 1 : 0);
		var x = new Matrix(1, n, data.Select(d => d / (float)n).ToArray());
		var y = new Matrix(1, n, result.Select(n => n * 1f).ToArray());

		var r = Enumerable.Range(1, 10).Select(d => d / (float)n).ToArray();
		var p = new Matrix(1, 10, r);
		var net = new NeuralNetwork(x, y, 3);

		Console.WriteLine(p.Activate(ActivationFunction.TanH));
		net.Train(1000, learningRate: .1f);
		Console.WriteLine(p);
		Console.WriteLine(net.Predict(p));
	}



}