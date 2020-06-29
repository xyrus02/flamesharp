using System;
using JetBrains.Annotations;
using XyrusWorx.Diagnostics;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public interface IIteratorContextConfiguration
	{
		[NotNull]
		IIteratorContextConfiguration Log(ILogWriter log);

		[NotNull]
		IIteratorContextConfiguration OnBeginning(Action action);

		[NotNull]
		IIteratorContextConfiguration OnFinalize(Action action);

		[NotNull]
		IIteratorContextConfiguration PerformanceCounter(Action<IIterationPerformanceCounterConfiguration> configuration);

		[NotNull]
		IIteratorContextConfiguration Statistics(Action<IIterationStatisticsConfiguration> configuration);
	}
}