using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	[DebuggerDisplay("{R}, {G}, {B}")]
	public struct Rgb : IEquatable<Rgb>
	{
		public readonly double R;
		public readonly double G;
		public readonly double B;

		public Rgb(double r, double g, double b)
		{
			R = r;
			G = g;
			B = b;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Rgb rgb && Equals(rgb);
		}
		public bool Equals(Rgb other)
		{
			return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = R.GetHashCode();
				hashCode = (hashCode * 397) ^ G.GetHashCode();
				hashCode = (hashCode * 397) ^ B.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(Rgb left, Rgb right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(Rgb left, Rgb right)
		{
			return !left.Equals(right);
		}
	}
}