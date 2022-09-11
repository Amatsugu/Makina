using Makina.Calculation;

namespace Makina.Neural;
public class NeuralNetwork
{
	private Matrix _w1;
	private Matrix _b1;
	private Matrix _w2;
	private Matrix _b2;
	private readonly Matrix _x;
	private readonly Matrix _y;

	public NeuralNetwork(Matrix x, Matrix y, int hiddenLayerSize)
	{
		var nX = x.rows;
		var nY = y.rows;

		InitializeParameters(nX, nY, hiddenLayerSize);
		_x = x;
		_y = y;
	}

	public void InitializeParameters(int nX, int nY, int layers)
	{
		_w1 = Matrix.Random(layers, nX);
		_b1 = Matrix.Zeros(layers, 1);
		_w2 = Matrix.Random(nY, layers);
		_b2 = Matrix.Zeros(nY, 1);
	}

	public (Matrix Z1, Matrix A1, Matrix Z2, Matrix A2) ForwardPropagation(ActivationFunction activation, Matrix x)
	{
		var z1 = (_w1 * x).AddCol(_b1);
		var a1 = z1.Activate(activation);
		var z2 = (_w2 * a1).AddCol(_b2);
		var a2 = z2.Activate(activation);

		return (z1, a1, z2, a2);
	}

	public (Matrix dW1, Matrix db1, Matrix dW2, Matrix db2) BackPropagation((Matrix Z1, Matrix A1, Matrix Z2, Matrix A2) forward)
	{
		var (z1, a1, z2, a2) = forward;
		var m = _y.columns;
		var dZ2 = a2 - _y;
		var dW2 = (dZ2 * a1.T) / m;
		var db2 = dZ2.SumColumns();
		var a1Sq = a1.Square();
		var dz1 = (_w2.T * dZ2).MultiplyElements(1 - a1Sq);
		var dW1 = (dz1 * _x.T) / m;
		var db1 = dz1.SumColumns() / m;

		return (dW1, db1, dW2, db2);
	}

	public void UpdateParameters((Matrix dW1, Matrix db1, Matrix dW2, Matrix db2) back, float learningRate)
	{
		var (dW1, db1, dW2, db2) = back;
		_w1 -= learningRate * dW1;
		_b1 -= learningRate * db1;
		_w2 -= learningRate * dW2;
		_b2 -= learningRate * db2;
	}

	public void Train(int iterations = 10000, float learningRate = 1.2f)
	{
		for (int i = 0; i < iterations; i++)
		{
			var fwd = ForwardPropagation(ActivationFunction.ReLu, _x);
			var back = BackPropagation(fwd);
			UpdateParameters(back, learningRate);
		}
	}

	public Matrix Predict(Matrix x)
	{
		return ForwardPropagation(ActivationFunction.ReLu, x).A2;
	}
}
