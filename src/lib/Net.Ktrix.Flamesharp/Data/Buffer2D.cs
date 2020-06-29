using System;
using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp.Data
{
	[PublicAPI]
	public class Buffer2D<T> : IBuffer<T> where T : struct
	{
		private readonly int _width;
		private readonly int _height;
		private bool _disposedValue;
		private T[,] _buffer;

		~Buffer2D()
		{
			Dispose(false);
		}
		public Buffer2D(int width, int height)
		{
			if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
			if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

			_buffer = Alloc(width, height);
			_width = width;
			_height = height;
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public T this[int x, int y]
		{
			get => _buffer[x,y];
			set => _buffer[x,y] = value;
		}
		public T this[long address]
		{
			get => this[(int)(address%_width), (int)(address/_width)];
			set => this[(int)(address%_width), (int)(address/_width)] = value;
		}

		public int Width => _width;
		public int Height => _height;
		public long Length => _width*_height;

		public virtual void Clear()
		{
			Array.Clear(_buffer, 0, _width * _height);
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
		private static T[,] Alloc(int pWidth, int pHeight)
		{
			return new T[pWidth,pHeight];
		}

		public T Get(int x, int y) => this[x, y];
		public void Set(int x, int y, T data) => this[x, y] = data;
		public void Load(IntPtr dataPtr, int dataLength)
		{
			throw new NotSupportedException();
		}
		public void Flush()
		{
		}

		public T[] AsArray()
		{
			var temp = new T[_width * _height];
			for (var j = 0; j < _height; j++)
			{
				for (var i = 0; i < _width; i++)
				{
					temp[j*_width + i] = _buffer[i, j];
				}
			}
			return temp;
		}
	}
}