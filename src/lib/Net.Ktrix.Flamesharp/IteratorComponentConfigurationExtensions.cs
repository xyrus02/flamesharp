using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public static class IteratorComponentConfigurationExtensions
	{
		public static T SetEnabled<T>(this IIteratorComponentConfiguration<T> instance, bool isEnabled)
		{
			if (isEnabled)
			{
				return instance.Enabled();
			}

			return instance.Disabled();
		}
	}
}