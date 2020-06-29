using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Net.Ktrix.Flamesharp.Variations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using XyrusWorx;
using XyrusWorx.Collections;
using XyrusWorx.Runtime;

namespace Net.Ktrix.Flamesharp.ObjectModel
{
	[PublicAPI]
	public class VariationModel
	{
		private static readonly NamingStrategy mExtensionDataNamingStrategy = new CamelCaseNamingStrategy();
		private readonly object _instanceLock = new object();

		[JsonExtensionData]
		private readonly IDictionary<string, JToken> _extensionData;
		private readonly IDictionary<string, object> _valueCache;

		private Variation _instance;
		private double _weight;
		private string _class;

		public VariationModel()
		{
			_extensionData = new Dictionary<string, JToken>();

			_class = "linear";
			_weight = 1.0;
			_instance = new LinearVariation();
			_valueCache = new Dictionary<string, object>();
		}

		[NotNull]
		public Variation Instance => _instance;

		[NotNull]
		public string Class
		{
			get { return _class; }
			set
			{
				if (value.NormalizeNull() == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (string.Equals(value, _class, StringComparison.InvariantCultureIgnoreCase))
				{
					return;
				}

				_class = value;

				lock (_instanceLock)
				{
					_instance = null;
				}
			}
		}

		public double Weight
		{
			get { return _weight; }
			set
			{
				if (Equals(value, _weight))
				{
					return;
				}

				_weight = value;

				lock (_instanceLock)
				{
					if (_instance != null)
					{
						_instance.Weight = value;
					}
				}
			}
		}

		[CanBeNull]
		public T Value<T>(StringKey key)
		{
			return (T)(Value(key, typeof(T)) ?? default(T));
		}

		[CanBeNull]
		public object Value(StringKey key, [NotNull] Type targetType)
		{
			if (key.IsEmpty)
			{
				return null;
			}

			if (targetType == null)
			{
				throw new ArgumentNullException(nameof(targetType));
			}

			if (_valueCache.ContainsKey(key))
			{
				return _valueCache[key];
			}

			lock (_instanceLock)
			{
				return GetExtensionData(key, targetType);
			}
		}

		private object GetExtensionData(StringKey key, Type targetType)
		{
			var contractName = key.RawData[0].ToString().ToLower() + (key.RawData.Length > 1 ? key.RawData.Substring(1) : string.Empty);
			var token = _extensionData.GetValueByKeyOrDefault(contractName);

			object value = null;

			token?.Value<string>().TryDeserialize(targetType, out value, CultureInfo.InvariantCulture);

			_valueCache.AddOrUpdate(key, value);
			return value;
		}
		private void SetExtensionData(object instance, Type type, HashSet<StringKey> visited)
		{
			var properties =
				from property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				where property.CanWrite && property.GetIndexParameters().Length == 0
				select new
				{
					property.Name,
					Type = property.PropertyType,
					Setter = new Action<object>(o => property.SetValue(instance, o))
				};

			var fields = 
				from field in type.GetFields(BindingFlags.Instance | BindingFlags.Public)
				where !field.IsInitOnly
				select new
				{
					field.Name,
					Type = field.FieldType,
					Setter = new Action<object>(o => field.SetValue(instance, o))
				};

			foreach (var property in properties.Concat(fields))
			{
				if (visited.Contains(property.Name))
				{
					continue;
				}

				var value = GetExtensionData(property.Name, property.Type);
				if (value != null)
				{
					property.Setter(value);
				}
				
				visited.Add(property.Name);
			}

			if (type.BaseType != null && type.BaseType != typeof(object))
			{
				SetExtensionData(instance, type.BaseType, visited);
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			var registry = ServiceLocator.Default.Resolve<VariationRegistry>();

			_valueCache.Clear();
			_instance = registry.GetByName(Class, Weight);

			SetExtensionData(_instance, _instance.GetType(), new HashSet<StringKey>());
		}
	}
}