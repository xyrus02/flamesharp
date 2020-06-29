using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	[DebuggerDisplay("{X}, {Y}, {C}")]
	public struct Vertex : IEquatable<Vertex>
	{
		public Vertex(double x, double y, double c = 0)
		{
			X = x;
			Y = y;
			C = c;
		}

		public readonly double X;
		public readonly double Y;
		public readonly double C;

		public double LengthSquared => X * X + Y * Y;

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Vertex vertex && Equals(vertex);
		}
		public bool Equals(Vertex other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y) && C.Equals(other.C);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = X.GetHashCode();
				hashCode = (hashCode * 397) ^ Y.GetHashCode();
				hashCode = (hashCode * 397) ^ C.GetHashCode();
				return hashCode;
			}
		}

		internal Vertex Copy(double? x = null, double? y = null, double? c = null) => new Vertex(x ?? X, y ?? Y, c ?? C);
		internal Vertex ResetNaNs() => new Vertex(double.IsNaN(X) ? 0 : X, double.IsNaN(Y) ? 0 : Y, double.IsNaN(C) ? 0 : C);

		public static Vertex Add(Vertex left, Vertex right) => new Vertex(left.X + right.X, left.Y + right.Y, left.C);
		public static Vertex Add(Vertex left, double right) => new Vertex(left.X + right, left.Y + right, left.C);
		public static Vertex Add(double left, Vertex right) => new Vertex(left + right.X, left + right.Y, right.C);

		public static Vertex Subtract(Vertex left, Vertex right) => new Vertex(left.X - right.X, left.Y - right.Y, left.C);
		public static Vertex Subtract(Vertex left, double right) => new Vertex(left.X - right, left.Y - right, left.C);
		public static Vertex Subtract(double left, Vertex right) => new Vertex(left - right.X, left - right.Y, right.C);

		public static Vertex Multiply(Vertex left, Vertex right) => new Vertex(left.X * right.X, left.Y * right.Y, left.C);
		public static Vertex Multiply(Vertex left, double right) => new Vertex(left.X * right, left.Y * right, left.C);
		public static Vertex Multiply(double left, Vertex right) => new Vertex(left * right.X, left * right.Y, right.C);

		public static Vertex Divide(Vertex left, Vertex right) => new Vertex(left.X / right.X, left.Y / right.Y, left.C);
		public static Vertex Divide(Vertex left, double right) => new Vertex(left.X / right, left.Y / right, left.C);
		public static Vertex Divide(double left, Vertex right) => new Vertex(left / right.X, left / right.Y, right.C);

		public static Vertex operator +(Vertex left, Vertex right) => Add(left, right);
		public static Vertex operator -(Vertex left, Vertex right) => Subtract(left, right);

		public static Vertex operator +(Vertex left, double right) => Add(left, right);
		public static Vertex operator -(Vertex left, double right) => Subtract(left, right);

		public static Vertex operator +(double left, Vertex right) => Add(left, right);
		public static Vertex operator -(double left, Vertex right) => Subtract(left, right);

		public static Vertex operator *(Vertex left, Vertex right) => Multiply(left, right);
		public static Vertex operator /(Vertex left, Vertex right) => Divide(left, right);

		public static Vertex operator *(Vertex left, double right) => Multiply(left, right);
		public static Vertex operator /(Vertex left, double right) => Divide(left, right);

		public static Vertex operator *(double left, Vertex right) => Multiply(left, right);
		public static Vertex operator /(double left, Vertex right) => Divide(left, right);

		public static bool operator ==(Vertex left, Vertex right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(Vertex left, Vertex right)
		{
			return !left.Equals(right);
		}
	}
}
