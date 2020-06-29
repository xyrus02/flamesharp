using System;
using JetBrains.Annotations;
using XyrusWorx;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	[MeansImplicitUse(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
	public sealed class VariationAttribute : Attribute
	{
		private readonly string _name;

		public VariationAttribute([NotNull] string name)
		{
			if (name.NormalizeNull() == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			_name = name;
		}

		[NotNull]
		public string Name => _name;
	}
}
