using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public abstract class IterationComponent
	{
		public bool IsEnabled { get; set; } = true;
	}
}