using System.Numerics;
using System.Text;

namespace Makina.Calculation;

public readonly struct Matrix
{
	public int Rows { get; init; }
	public int Columns { get; init; }
	public int Length { get; init; }
	private readonly float[] _data;

	public Matrix(int rows, int columns)
	{
		Rows = rows;
		Columns = columns;
		Length = Rows * Columns;
		_data = new float[Length];
	}

	public Matrix(int rows, int columns, float value) : this(rows, columns)
	{
		for (int i = 0; i < Length; i++)
			_data[i] = value;
	}

	public Matrix(int rows, int columns, float[] value)
	{
		Rows = rows;
		Columns = columns;
		Length = Rows * Columns;
#if DEBUG
		if (value.Length != Length)
			throw new ArgumentException($"The length of the provided array does not match the expected size. Expected: {Length}, Actual: {value.Length}", nameof(value));
#endif
		_data = value;
	}

	public float this[int row, int column]
	{
		get
		{
			return _data[row * Columns + Columns];
		}
	}

	public Matrix Multiply(float value)
	{
		var result = new float[Length];
		var c = Vector<float>.Count;
		int i;
		var l = Length - (Length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			v *= value;
			v.CopyTo(result, i);
		}
		if (l < Length)
		{
			for (int j = i; j < Length; j++)
				result[j] = _data[j] * value;
		}

		return new Matrix(Rows, Columns, result);
	}

	public Matrix Multiply(Matrix right)
	{
#if DEBUG
		if (Columns != right.Rows)
			throw new InvalidOperationException("Matricies can't be multiplied");
#endif
		var result = new float[Rows * right.Columns];
		var n = Rows;
		var p = right.Columns;
		var m = Columns;
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

		return new Matrix(Rows, right.Columns, result);
	}

	public Matrix Dot(Matrix right)
	{
		var result = new float[right.Columns];
		for (int i = 0; i < Rows; i++)
		{
			for (int j = 0; j < Columns; j++)
			{
				result[i] += _data[i * Columns + j] * right._data[j * right.Columns + i];
			}
		}
		return new Matrix(1, right.Columns, result);
	}

	public Matrix Add(Matrix right)
	{
#if DEBUG
		if (Columns != right.Columns || Rows != right.Rows)
			throw new InvalidOperationException("Matricies can't be added");
#endif
		var result = new float[Length];
		var c = Vector<float>.Count;
		int i;
		var l = Length - (Length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			var v2 = new Vector<float>(right._data, i);
			v += v2;
			v.CopyTo(result, i);
		}
		if (l < Length)
		{
			for (int j = i; j < Length; j++)
				result[j] = _data[j] + right._data[j];
		}
		return new Matrix(Rows, Columns, result);
	}

	public Matrix Add(float right)
	{
		var result = new float[Length];
		var c = Vector<float>.Count;
		int i;
		var l = Length - (Length % c);
		var v2 = new Vector<float>(right);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			v += v2;
			v.CopyTo(result, i);
		}
		if (l < Length)
		{
			for (int j = i; j < Length; j++)
				result[j] = _data[j] + right;
		}
		return new Matrix(Rows, Columns, result);
	}

	public Matrix AddCol(Matrix col)
	{
		var result = new float[Length];
		var c = Vector<float>.Count;
		var l = Columns - (Columns % c);
		for (int i = 0; i < Rows; i++)
		{
			int j;
			var v2 = new Vector<float>(col._data[i]);
			for (j = 0; j < l; j += c)
			{
				var v = new Vector<float>(_data, j);
				v += v2;
				v.CopyTo(result, j);
			}
			if (l < Columns)
			{
				for (int k = j; k < Columns; k++)
					result[i * Columns + j] = _data[i * Columns + j] + col._data[i];
			}
		}

		return new Matrix(Rows, Columns, result);
	}

	public Matrix Subtract(Matrix right)
	{
		var result = new float[Length];
		var c = Vector<float>.Count;
		int i;
		var l = Length - (Length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			var v2 = new Vector<float>(right._data, i);
			v -= v2;
			v.CopyTo(result, i);
		}
		if (l < Length)
		{
			for (int j = i; j < Length; j++)
				result[j] = _data[j] - right._data[j];
		}
		return new Matrix(Rows, Columns, result);
	}

	public Matrix Subtract(float right)
	{
		var result = new float[Length];
		var c = Vector<float>.Count;
		int i;
		var l = Length - (Length % c);
		var v2 = new Vector<float>(right);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			v -= v2;
			v.CopyTo(result, i);
		}
		if (l < Length)
		{
			for (int j = i; j < Length; j++)
				result[j] = _data[j] - right;
		}
		return new Matrix(Rows, Columns, result);
	}

	public Matrix Divide(Matrix right)
	{
		var result = new float[Length];
		var c = Vector<float>.Count;
		int i;
		var l = Length - (Length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			var v2 = new Vector<float>(right._data, i);
			v /= v2;
			v.CopyTo(result, i);
		}
		if (l < Length)
		{
			for (int j = i; j < Length; j++)
				result[j] = _data[j] / right._data[j];
		}
		return new Matrix(Rows, Columns, result);
	}

	public Matrix Divide(float right)
	{
		var result = new float[Length];
		var c = Vector<float>.Count;
		int i;
		var l = Length - (Length % c);
		var v2 = new Vector<float>(right);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			v /= v2;
			v.CopyTo(result, i);
		}
		if (l < Length)
		{
			for (int j = i; j < Length; j++)
				result[j] = _data[j] / right;
		}
		return new Matrix(Rows, Columns, result);
	}

	public bool Equals(Matrix right)
	{
		var c = Vector<float>.Count;
		int i;
		var l = Length - (Length % c);
		for (i = 0; i < l; i += c)
		{
			var v = new Vector<float>(_data, i);
			var v2 = new Vector<float>(right._data, i);
			if (v != v2)
				return false;
		}
		if (l < Length)
		{
			for (int j = i; j < Length; j++)
			{
				if (_data[j] != right._data[j])
					return false;
			}
		}
		return true;
	}

	public Matrix Transpose()
	{
		var result = new float[Length];
		for (int i = 0; i < Rows; i++)
		{
			for (int j = 0; j < Columns; j++)
			{
				result[j * Rows + i] = _data[i * Columns + j];
			}
		}
		return new Matrix(Columns, Rows, result);
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

	public static Matrix Zeo(int rows, int columns)
	{
		return new Matrix(rows, columns);
	}

	private static float Sum(in float[] arr)
	{
		var sum = 0f;
		for (int i = 0; i < arr.Length; i++)
		{
			sum += arr[i];
		}
		return sum;
	}

	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.AppendLine("{");
		for (int i = 0; i < Rows; i++)
		{
			sb.Append("\t[ ");
			for (int j = 0; j < Columns; j++)
			{
				var index = i * Columns + j;
				sb.Append($"{_data[index],6:0.00}");
				if (j + 1 != Columns)
					sb.Append(',');
				sb.Append(' ');
			}
			sb.AppendLine("]");
		}
		sb.AppendLine("}");

		return sb.ToString();
	}
}