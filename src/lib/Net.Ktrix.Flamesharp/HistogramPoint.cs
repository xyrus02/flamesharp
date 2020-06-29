using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public struct HistogramPoint
	{
		public double Red;
		public double Green;
		public double Blue;
		public double Count;
	}
}