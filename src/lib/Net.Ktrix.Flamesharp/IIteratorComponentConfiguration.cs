using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public interface IIteratorComponentConfiguration<out T>
	{
		T Enabled();
		T Disabled();
	}
}