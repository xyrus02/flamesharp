using System;
using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp.Data
{
	[PublicAPI]
	public class Buffer<T> : IBuffer<T> where T : struct
	{
		private bool _disposedValue;
		private T[] _buffer;

		~Buffer()
		{
			Dispose(false);
		}

		public Buffer([NotNull] T[] data)
		{
			_buffer = data ?? throw new ArgumentNullException(nameof(data));
			Length = data.Length;
		}
		public Buffer(long length)
		{
			if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

			_buffer = Alloc(length);
			Length = length;
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public T this[long offset]
		{
			get => _buffer[offset];
			set => _buffer[offset] = value;
		}
		public long Length { get; }

		public T[] AsArray()
		{
			return _buffer;
		}
		public void Clear()
		{
			Array.Clear(_buffer, 0, _buffer.Length);
		}

		protected virtual void CleanupOverride()
		{
		}

		private void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					CleanupOverride();
				}

				_buffer = null;
				_disposedValue = true;
			}
		}
		private static T[] Alloc(long length)
		{
			var ret = new T[length];
			return ret;
		}

		public T Get(int address) => this[address];
		public void Set(int address, T data) => this[address] = data;
		public void Load(IntPtr dataPtr, int dataLength)
		{
			throw new NotSupportedException();
		}
		public void Flush()
		{
		}
	}
}