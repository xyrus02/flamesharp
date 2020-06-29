using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp
{
	public abstract class Variation
	{
		public double Weight { get; set; } = 1.0;

		public abstract void Calculate([NotNull] CalculationState calculationState);
	}
}