using System.Numerics;
using System.Security.AccessControl;
using System.Text;

namespace Makina.Calculation;

public readonly struct Matrix
{
	public readonly int rows;
	public readonly int columns;
	public readonly int length;
	public Matrix T => Transpose();
	private readonly float[] _data;

	public Matrix(int rows, int columns)
	{
		this.rows = rows;
		this.columns = columns;
		length = this.rows * this.columns;
		_data = new float[length];
	}

	public Matrix(int rows, int columns, float value) : this(rows, columns)
	{
		for (int i = 0; i < length; i++)
			_data[i] = value;
	}

	public Matrix(int rows, int columns, float[] value)
	{
		this.rows = rows;
		this.columns = columns;
		length = this.rows * this.columns;
#if DEBUG
		if (value.Length != length)
			throw new ArgumentException($"The length of the provided array does not match the expected size. Expected: {length}, Actual: {value.Length}", nameof(value));
#endif
		_data = value;
	}

	public float this[int row, int column]
	{
		get
		{
			return _data[row * columns + column];
		}
	}

	public Matrix Multiply(float value)
	{
		var result = new float[length];
		var c = Vector<float>.Count;
		int i;
		var l = length - (length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			v *= value;
			v.CopyTo(result, i);
		}
		if (l < length)
		{
			for (int j = i; j < length; j++)
				result[j] = _data[j] * value;
		}

		return new Matrix(rows, columns, result);
	}

	public Matrix MultiplyElements(Matrix right)
	{
#if DEBUG
		if (rows != right.rows || columns != right.columns)
			throw new InvalidOperationException("Matricies must be the same size.");
#endif
		var result = new float[length];
		var c = Vector<float>.Count;
		int i;
		var l = length - (length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			var v2 = new Vector<float>(right._data, i);
			v *= v2;
			v.CopyTo(result, i);
		}
		if (l < length)
		{
			for (int j = i; j < length; j++)
				result[j] = _data[j] * right._data[j];
		}

		return new Matrix(rows, columns, result);
	}

	public Matrix MultiplyCol(Matrix col)
	{
#if DEBUG
		if (rows != col.rows)
			throw new InvalidOperationException("Matricies must have the same number of rows.");
#endif
		var result = new float[length];
		var c = Vector<float>.Count;
		var l = columns - (columns % c);
		for (int i = 0; i < rows; i++)
		{
			int j;
			var v2 = new Vector<float>(col._data[i]);
			for (j = 0; j < l; j += c)
			{
				var v = new Vector<float>(_data, j);
				v *= v2;
				v.CopyTo(result, i * columns + j);
			}
			if (l < columns)
			{
				for (int k = j; k < columns; k++)
					result[i * columns + k] = _data[i * columns + k] * col._data[i];
			}
		}

		return new Matrix(rows, columns, result);
	}

	public Matrix Multiply(Matrix right)
	{
#if DEBUG
		if (columns != right.rows)
			throw new InvalidOperationException("Matricies can't be multiplied");
#endif
		var result = new float[rows * right.columns];
		var n = rows;
		var p = right.columns;
		var m = columns;
		float b1, b2, b3, b4, b5, b6, b7, b8;
		for (int j = 0; j < p; j++)
		{
			int k;
			for (k = 0; k < (m - 7); k += 8)
			{
				b1 = right._data[k * p + j];
				b2 = right._data[k * p + j + 1];
				b3 = right._data[k * p + j + 2];
				b4 = right._data[k * p + j + 3];
				b5 = right._data[k * p + j + 4];
				b6 = right._data[k * p + j + 5];
				b7 = right._data[k * p + j + 6];
				b8 = right._data[k * p + j + 7];
				for (int i = 0; i < n; i++)
				{
					result[i * p + j] += _data[i * m + k] * b1;
					result[i * p + j] += _data[i * m + (k + 1)] * b2;
					result[i * p + j] += _data[i * m + (k + 2)] * b3;
					result[i * p + j] += _data[i * m + (k + 3)] * b4;
					result[i * p + j] += _data[i * m + (k + 4)] * b5;
					result[i * p + j] += _data[i * m + (k + 5)] * b6;
					result[i * p + j] += _data[i * m + (k + 6)] * b7;
					result[i * p + j] += _data[i * m + (k + 7)] * b8;
				}
			}
			if (m % 8 > 0)
			{
				do
				{
					b1 = right._data[k * p + j];
					for (int i = 0; i < n; i++)
					{
						result[i * p + j] += _data[i * m + k] * b1;
					}
				} while (++k < m);
			}
		}

		return new Matrix(rows, right.columns, result);
	}

	public Matrix Dot(Matrix right)
	{
#if DEBUG
		if (rows != right.rows)
			throw new InvalidOperationException("Matricies must have the same number of rows.");
#endif
		var result = new float[right.columns];
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				result[i] += _data[i * columns + j] * right._data[j * right.columns + i];
			}
		}
		return new Matrix(1, right.columns, result);
	}

	public Matrix Add(Matrix right)
	{
#if DEBUG
		if (columns != right.columns || rows != right.rows)
			throw new InvalidOperationException("Matricies can't be added");
#endif
		var result = new float[length];
		var c = Vector<float>.Count;
		int i;
		var l = length - (length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			var v2 = new Vector<float>(right._data, i);
			v += v2;
			v.CopyTo(result, i);
		}
		if (l < length)
		{
			for (int j = i; j < length; j++)
				result[j] = _data[j] + right._data[j];
		}
		return new Matrix(rows, columns, result);
	}

	public Matrix Add(float right)
	{
		var result = new float[length];
		var c = Vector<float>.Count;
		int i;
		var l = length - (length % c);
		var v2 = new Vector<float>(right);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			v += v2;
			v.CopyTo(result, i);
		}
		if (l < length)
		{
			for (int j = i; j < length; j++)
				result[j] = _data[j] + right;
		}
		return new Matrix(rows, columns, result);
	}

	public Matrix AddCol(Matrix col)
	{
#if DEBUG
		if (rows != col.rows)
			throw new InvalidOperationException("Matricies must have the same number of rows.");
#endif
		var result = new float[length];
		var c = Vector<float>.Count;
		var l = columns - (columns % c);
		for (int i = 0; i < rows; i++)
		{
			int j;
			var v2 = new Vector<float>(col._data[i]);
			for (j = 0; j < l; j += c)
			{
				var v = new Vector<float>(_data, j);
				v += v2;
				v.CopyTo(result, i * columns + j);
			}
			if (l < columns)
			{
				for (int k = j; k < columns; k++)
					result[i * columns + k] = _data[i * columns + k] + col._data[i];
			}
		}

		return new Matrix(rows, columns, result);
	}

	public Matrix Subtract(Matrix right)
	{
#if DEBUG
		if (rows != right.rows || columns != right.columns)
			throw new InvalidOperationException("Matricies must be the same size.");
#endif
		var result = new float[length];
		var c = Vector<float>.Count;
		int i;
		var l = length - (length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			var v2 = new Vector<float>(right._data, i);
			v -= v2;
			v.CopyTo(result, i);
		}
		if (l < length)
		{
			for (int j = i; j < length; j++)
				result[j] = _data[j] - right._data[j];
		}
		return new Matrix(rows, columns, result);
	}

	public Matrix Subtract(float right)
	{
		var result = new float[length];
		var c = Vector<float>.Count;
		int i;
		var l = length - (length % c);
		var v2 = new Vector<float>(right);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			v -= v2;
			v.CopyTo(result, i);
		}
		if (l < length)
		{
			for (int j = i; j < length; j++)
				result[j] = _data[j] - right;
		}
		return new Matrix(rows, columns, result);
	}

	public Matrix SubtractCol(Matrix col)
	{
#if DEBUG
		if (rows != col.rows)
			throw new InvalidOperationException("Matricies must have the same number of rows.");
#endif
		var result = new float[length];
		var c = Vector<float>.Count;
		var l = columns - (columns % c);
		for (int i = 0; i < rows; i++)
		{
			int j;
			var v2 = new Vector<float>(col._data[i]);
			for (j = 0; j < l; j += c)
			{
				var v = new Vector<float>(_data, j);
				v -= v2;
				v.CopyTo(result, i * columns + j);
			}
			if (l < columns)
			{
				for (int k = j; k < columns; k++)
					result[i * columns + k] = _data[i * columns + k] - col._data[i];
			}
		}

		return new Matrix(rows, columns, result);
	}

	public Matrix Divide(Matrix right)
	{
#if DEBUG
		if (rows != right.rows || columns != right.columns)
			throw new InvalidOperationException("Matricies must be the same size.");
#endif
		var result = new float[length];
		var c = Vector<float>.Count;
		int i;
		var l = length - (length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			var v2 = new Vector<float>(right._data, i);
			v /= v2;
			v.CopyTo(result, i);
		}
		if (l < length)
		{
			for (int j = i; j < length; j++)
				result[j] = _data[j] / right._data[j];
		}
		return new Matrix(rows, columns, result);
	}

	public Matrix Divide(float right)
	{
		var result = new float[length];
		var c = Vector<float>.Count;
		int i;
		var l = length - (length % c);
		var v2 = new Vector<float>(right);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			v /= v2;
			v.CopyTo(result, i);
		}
		if (l < length)
		{
			for (int j = i; j < length; j++)
				result[j] = _data[j] / right;
		}
		return new Matrix(rows, columns, result);
	}

	public Matrix Square()
	{
		var result = new float[length];
		var c = Vector<float>.Count;
		int i;
		var l = length - (length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			v *= v;
			v.CopyTo(result, i);
		}
		if (l < length)
		{
			for (int j = i; j < length; j++)
			{
				var d = _data[j];
				result[j] = d * d;
			}
		}
		return new Matrix(rows, columns, result);
	}

	public Matrix Activate(ActivationFunction activation)
	{
		var result = new float[length];
		var c = Vector<float>.Count;
		int i;
		var l = length - (length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			v = activation.Activate(v);
			v.CopyTo(result, i);
		}
		if (l < length)
		{
			for (int j = i; j < length; j++)
				result[j] = activation.Activate(_data[j]);
		}
		return new Matrix(rows, columns, result);
	}

	public Matrix DeActivate(ActivationFunction activation)
	{
		var result = new float[length];
		var c = Vector<float>.Count;
		int i;
		var l = length - (length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			v = activation.DeActivate(v);
			v.CopyTo(result, i);
		}
		if (l < length)
		{
			for (int j = i; j < length; j++)
				result[j] = activation.DeActivate(_data[j]);
		}
		return new Matrix(rows, columns, result);
	}

	public Matrix SumColumns()
	{
		var result = new float[rows];
		var c = Vector<float>.Count;
		var l = columns - (columns % c);
		for (int i = 0; i < rows; i++)
		{
			int j;
			for (j = 0; j < l; j += c)
			{
				var v = new Vector<float>(_data, j);
				result[i] = Vector.Sum(v);
			}
			if (l < columns)
			{
				for (int k = j; k < columns; k++)
					result[i] += _data[i * columns + k];
			}
		}

		return new Matrix(rows, 1, result);
	}

	public bool Equals(Matrix right)
	{
#if DEBUG
		if (rows != right.rows || columns != right.columns)
			throw new InvalidOperationException("Matricies must be the same size.");
#endif
		var c = Vector<float>.Count;
		int i;
		var l = length - (length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			var v2 = new Vector<float>(right._data, i);
			if (v != v2)
				return false;
		}
		if (l < length)
		{
			for (int j = i; j < length; j++)
			{
				if (_data[j] != right._data[j])
					return false;
			}
		}
		return true;
	}

	public Matrix Transpose()
	{
		var r = rows;
		var c = columns;
		var result = new float[length];
		for (int i = 0; i < r; i++)
		{
			for (int j = 0; j < c; j++)
			{
				result[j * r + i] = _data[i * c + j];
			}
		}
		return new Matrix(columns, rows, result);
	}

	public static Matrix Identity(int size)
	{
		var data = new float[size * size];
		var d = 0;
		for (int i = 0; i < data.Length; i += size)
		{
			data[i + d++] = 1;
		}
		return new Matrix(size, size, data);
	}

	public static Matrix Ones(int rows, int columns)
	{
		return new Matrix(rows, columns, 1);
	}

	public static Matrix Zeros(int rows, int columns)
	{
		return new Matrix(rows, columns);
	}

	public static Matrix Random(int rows, int columns, int seed = 0)
	{
		var random = seed == 0 ? new Random() : new Random(seed);
		var data = new float[rows * columns];
		for (int i = 0; i < data.Length; i++)
		{
			data[i] = random.NextSingle();
		}		
		return new Matrix(rows, columns, data);
	}

	public float Sum()
	{
		return Sum(_data);
	}

	private static float Sum(in float[] arr)
	{
		var sum = 0f;
		var c = Vector<float>.Count;
		int i;
		var l = arr.Length - (arr.Length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(arr, i);
			sum += Vector.Sum(v);
		}
		if (l < arr.Length)
		{
			for (int j = i; j < arr.Length; j++)
				sum = arr[j];
		}
		return sum;
	}

	public static Matrix operator +(Matrix a, Matrix b) => a.Add(b);

	public static Matrix operator +(Matrix a, float b) => a.Add(b);

	public static Matrix operator +(float a, Matrix b) => b.Add(a);

	public static Matrix operator -(Matrix a, Matrix b) => a.Subtract(b);

	public static Matrix operator -(Matrix a, float b) => a.Subtract(b);

	public static Matrix operator -(float a, Matrix b) => -b.Add(a);

	public static Matrix operator -(Matrix a) => a.Multiply(-1);

	public static Matrix operator *(Matrix a, Matrix b) => a.Multiply(b);

	public static Matrix operator *(Matrix a, float b) => a.Multiply(b);

	public static Matrix operator *(float a, Matrix b) => b.Multiply(a);

	public static Matrix operator /(Matrix a, float b) => a.Divide(b);

	public static Matrix operator /(Matrix a, Matrix b) => a.Dot(b);

	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.AppendLine("{");
		for (int i = 0; i < rows; i++)
		{
			sb.Append("\t[ ");
			for (int j = 0; j < columns; j++)
			{
				var index = i * columns + j;
				sb.Append($"{_data[index],6:0.00}");
				if (j + 1 != columns)
					sb.Append(',');
				sb.Append(' ');
			}
			sb.AppendLine("]");
		}
		sb.AppendLine("}");

		return sb.ToString();
	}
}