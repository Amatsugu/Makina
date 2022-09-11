using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Makina.Calculation;
public static class Extensions
{
	private const int n = 10;

	public static Vector<float> Exp(this Vector<float> value)
	{
		var sum = Vector<float>.Zero;
		sum = Vector<float>.One + value * sum / new Vector<float>(9);
		sum = Vector<float>.One + value * sum / new Vector<float>(8);
		sum = Vector<float>.One + value * sum / new Vector<float>(7);
		sum = Vector<float>.One + value * sum / new Vector<float>(6);
		sum = Vector<float>.One + value * sum / new Vector<float>(5);
		sum = Vector<float>.One + value * sum / new Vector<float>(4);
		sum = Vector<float>.One + value * sum / new Vector<float>(3);
		sum = Vector<float>.One + value * sum / new Vector<float>(2);
		sum = Vector<float>.One + value * sum / new Vector<float>(1);
		return sum;
	}

	public static Vector<float> ExpLoop(this Vector<float> value)
	{
		var sum = Vector<float>.Zero;
		for (int i = n - 1; i > 0; i--)
			sum = Vector<float>.One + value * sum / new Vector<float>(i);
		return sum;
	}
}
