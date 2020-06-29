using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public struct LogDensityStatistics
	{
		public double AverageBrightness;
		public double LowAccumulator;
		public double HighAccumulator;
	}
}