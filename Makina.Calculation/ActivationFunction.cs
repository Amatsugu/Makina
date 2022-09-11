using System.Numerics;

namespace Makina.Calculation;

public abstract class ActivationFunction
{
	public static Sigmoid Sigmoid => new();
	public static ReLu ReLu => new();
	public static TanH TanH => new();

	public abstract float Activate(float value);

	public abstract Vector<float> Activate(Vector<float> value);

	public abstract float DeActivate(float value);

	public abstract Vector<float> DeActivate(Vector<float> value);
}

public class Sigmoid : ActivationFunction
{
	public override float Activate(float value)
	{
		return (1 / (1 - MathF.Exp(-value)));
	}

	public override Vector<float> Activate(Vector<float> value)
	{
		return (Vector<float>.One / (Vector<float>.One + (-value).Exp()));
	}

	public override float DeActivate(float value)
	{
		value = Activate(value);
		return value * (1 - value);
	}

	public override Vector<float> DeActivate(Vector<float> value)
	{
		value = Activate(value);
		return value * (Vector<float>.One - value);
	}
}

public sealed class ReLu : ActivationFunction
{
	public override float Activate(float value)
	{
		return MathF.Max(0, value);
	}

	public override Vector<float> Activate(Vector<float> value)
	{
		return Vector.Max(Vector<float>.Zero, value);
	}

	public override float DeActivate(float value)
	{
		return (value == 0 ? .5f : (value > 0 ? 1 : 0));
	}

	public override Vector<float> DeActivate(Vector<float> value)
	{
		var zeros = Vector.Equals(value, Vector<float>.Zero);
		var point5 = new Vector<float>(.5f);
		var gt = Vector.GreaterThan(value, Vector<float>.Zero);
		value = Vector.ConditionalSelect(gt, Vector<float>.One, Vector<float>.Zero);
		return Vector.ConditionalSelect(zeros, point5, value);
	}
}

public sealed class TanH : Sigmoid
{
	public override float Activate(float value)
	{
		return (2 * base.Activate(2 * value)) - 1;
	}

	public override Vector<float> Activate(Vector<float> value)
	{
		var s = 2 * base.Activate(2 * value);
		return s - Vector<float>.One;
	}

	public override float DeActivate(float value)
	{
		var a = Activate(value);
		return 1 - a * a;
	}

	public override Vector<float> DeActivate(Vector<float> value)
	{
		var a = Activate(value);
		return Vector<float>.One - a * a;
	}
}