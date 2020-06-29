using System;
using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp.Data
{
	[PublicAPI]
	public interface IBuffer<T> : IBuffer where T: struct
	{
		T this[long offset] { get; set; }
	}

	[PublicAPI]
	public interface IBuffer : IDisposable
	{
		long Length { get; }
	}
}