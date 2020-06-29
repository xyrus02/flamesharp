using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using XyrusWorx;
using XyrusWorx.Collections;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public sealed class VariationRegistry
	{
		private readonly Dictionary<StringKey, Func<double, Variation>> _variations;
		private readonly Dictionary<StringKey, StringKey> _names;

		public VariationRegistry()
		{
			_variations = new Dictionary<StringKey, Func<double, Variation>>();
			_names = new Dictionary<StringKey, StringKey>();
		}

		public void Register([NotNull] Type variationType, StringKey variationName = default)
		{
			if (variationType == null)
			{
				throw new ArgumentNullException(nameof(variationType));
			}

			if (variationName.IsEmpty)
			{
				var attribute = variationType.GetCustomAttribute<VariationAttribute>();
				if (attribute == null)
				{
					throw new TypeLoadException($"The type \"{variationType.FullName}\" is not decorated with a \"{nameof(VariationAttribute)}\" and no specific variation name has been provided.");
				}

				variationName = attribute.Name;
			}

			_variations.AddOrUpdate(variationName.Normalize(), d => Create(variationType, d));
			_names.AddOrUpdate(variationName.Normalize(), variationName);
		}
		public void Register([NotNull] Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			var types =
				from type in assembly.GetLoadableTypes()

				where typeof(Variation).IsAssignableFrom(type)

				let compatibleConstructors =
					from constructor in type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
					where constructor.GetParameters().Length == 0
					select constructor

				let attribute = type.GetCustomAttribute<VariationAttribute>()

				where 
					!type.IsAbstract && 
					!type.IsInterface && 
					compatibleConstructors.Any() &&
					attribute != null

				select type;

			foreach (var type in types)
			{
				Register(type);
			}
		}

		[NotNull]
		public IEnumerable<StringKey> Names => _names.Values;

		[NotNull]
		public Variation GetByName(StringKey name, double weight = 1.0)
		{
			if (!_variations.ContainsKey(name.Normalize()))
			{
				throw new KeyNotFoundException($"The variation \"{name}\" is not registered.");
			}

			return _variations[name.Normalize()](weight);
		}

		[NotNull]
		public IEnumerable<StringKey> GetNames()
		{
			return _names.Values;
		}

		private Variation Create([NotNull] Type variationType, double weight)
		{
			if (variationType == null)
			{
				throw new ArgumentNullException(nameof(variationType));
			}

			var variation = (Variation)Activator.CreateInstance(variationType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);

			variation.Weight = weight;

			return variation;
		}
	}
}