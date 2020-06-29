using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp.Data
{
	[PublicAPI]
	public class NativeByteBuffer : IBuffer<byte>
	{
		[DllImport("Kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
		private static extern void ZeroMemory(IntPtr dest, IntPtr size);

		private bool _disposedValue;
		private IntPtr _buffer;

		~NativeByteBuffer()
		{
			Dispose(false);
		}

		public NativeByteBuffer(long length)
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

		public byte this[long offset]
		{
			get => Marshal.ReadByte(_buffer, (int)offset);
			set => Marshal.WriteByte(_buffer, (int)offset, value);
		}
		public long Length { get; }

		public void Clear()
		{
			ZeroMemory(_buffer, new IntPtr(Length));
		}

		public IntPtr Pointer => _buffer;

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

				_buffer = IntPtr.Zero;
				_disposedValue = true;
			}
		}
		private static IntPtr Alloc(long length)
		{
			var ret = Marshal.AllocHGlobal(new IntPtr(length));
			ZeroMemory(ret, new IntPtr(length));
			return ret;
		}
	}
}