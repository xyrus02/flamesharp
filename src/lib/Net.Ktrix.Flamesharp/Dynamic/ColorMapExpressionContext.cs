using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp.Dynamic
{
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	class ColorMapExpressionContext : MathExpressionContext, IColorMapExpressionContextSetup
	{
		private readonly Random mRandom = new Random(123456);
		private double mOffset;

		
		public double _ => mOffset;
		public double x => mOffset;

		[UsedImplicitly]
		public double noise => mRandom.NextDouble();

		[UsedImplicitly]
		public Rgb rgb(double gray) => rgb(gray, gray, gray);

		[UsedImplicitly]
		public Rgb rgb(double r, double g, double b)
		{
			return new Rgb(
				Math.Max(0, Math.Min(r, 1)),
				Math.Max(0, Math.Min(g, 1)),
				Math.Max(0, Math.Min(b, 1)));
		}

		void IColorMapExpressionContextSetup.SetOffset(double o)
		{
			mOffset = o;
		}
	}
}