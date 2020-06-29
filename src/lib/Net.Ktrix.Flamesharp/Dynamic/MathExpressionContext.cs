using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp.Dynamic
{
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	abstract class MathExpressionContext
	{
		public double pi => Math.PI;
		public double e => Math.E;

		public double acos(double d) => Math.Acos(d);
		public double asin(double d) => Math.Asin(d);
		public double atan(double d) => Math.Atan(d);
		public double atan2(double y, double x) => Math.Atan2(y, x);

		public double cos(double d) => Math.Cos(d);
		public double sin(double d) => Math.Sin(d);
		public double tan(double d) => Math.Tan(d);

		public double cosh(double d) => Math.Cosh(d);
		public double sinh(double d) => Math.Sinh(d);
		public double tanh(double d) => Math.Tanh(d);

		public double abs(double d) => Math.Abs(d);
		public double round(double d) => Math.Round(d);
		public double round(double d, uint n) => Math.Round(d, (int)n);
		public double ceil(double d) => Math.Ceiling(d);
		public double floor(double d) => Math.Floor(d);
		public double frac(double d) => Math.Sign(d) * Math.Abs(d - Math.Floor(d));
		public double sgn(double d) => Math.Sign(d);

		public double sqrt(double d) => Math.Sqrt(d);
		public double pow(double d, double n) => Math.Pow(d, n);

		public double log(double d) => Math.Log(d);
		public double log10(double d) => Math.Log10(d);
		public double exp(double d) => Math.Exp(d);

		public double max(double a, double b) => Math.Max(a, b);
		public double min(double a, double b) => Math.Min(a, b);
	}
}