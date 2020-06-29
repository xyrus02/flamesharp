using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Net.Ktrix.Flamesharp.Data;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public class BackBuffer : IDisposable
	{
		[DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
		private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

		private NativeByteBuffer _buffer;
		private LogDensityStatistics _statistics;

		public BackBuffer(int width, int height)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width));
			}

			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height));
			}

			_buffer = new NativeByteBuffer(width * height * 4);
			Width = width;
			Height = height;
		}

		public int Width { get; }
		public int Height { get; }

		public double PixelsPerUnit => 25;

		[NotNull]
		public Bitmap CreateBitmap() => CreateBitmap(out var dummy);

		[NotNull]
		public Bitmap CreateBitmap(out LogDensityStatistics statistics)
		{
			var bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

			statistics = WriteTo(bitmap);

			return bitmap;
		}

		public LogDensityStatistics WriteTo([NotNull] Bitmap bitmap)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException(nameof(bitmap));
			}

			if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
			{
				throw new NotSupportedException("Unsupported pixel format");
			}

			BitmapData bits = null;

			try
			{
				bits = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
				CopyMemory(bits.Scan0, _buffer.Pointer, (uint)_buffer.Length);
			}
			finally
			{
				if (bits != null)
				{
					bitmap.UnlockBits(bits);
				}
			}

			return _statistics;
		}
		public void Dispose()
		{
			_buffer?.Dispose();
			_buffer = null;
		}

		internal long BufferSize => Width * Height;

		internal void ProcessImage([NotNull] Iterator iterator, [NotNull] IteratorContext context, CancellationToken cancellationToken = default)
		{
			if (iterator == null)
			{
				throw new ArgumentNullException(nameof(iterator));
			}

			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			var stride = Width * 4;

			var parallelOptions = new ParallelOptions();

			if (!iterator.AllowMultithreading)
			{
				parallelOptions.MaxDegreeOfParallelism = 1;
			}

			var averageBrightness = -1d;
			var lowAccumulator = double.MaxValue;
			var highAccumulator = double.MinValue;

			Parallel.For(0, Width * Height, parallelOptions, (addr, state) =>
			{
				if (cancellationToken.IsCancellationRequested)
				{
					state.Break();
					return;
				}

				
				var hip = context.Histogram[addr];
				var log = hip.Count;

				lowAccumulator = Math.Min(lowAccumulator, hip.Count);
				highAccumulator = Math.Max(highAccumulator, hip.Count);

				var r = (uint)Math.Max(0, Math.Min(255 * hip.Red / log, 255));
				var g = (uint)Math.Max(0, Math.Min(255 * hip.Green / log, 255));
				var b = (uint)Math.Max(0, Math.Min(255 * hip.Blue / log, 255));

				if (averageBrightness < 0)
				{
					averageBrightness = (r + g + b) / 765.0;
				}
				else
				{
					averageBrightness = (averageBrightness + (r + g + b) / 765.0) * 0.5;
				}

				var x = addr % Width;
				var y = addr / Height;

				unchecked
				{
					var color = (r << 16) + (g << 8) + b + 0xff000000;
					var address = _buffer.Pointer + x * 4 + y * stride;

					Marshal.WriteInt32(address, (int)color);
				}
			});

			_statistics = new LogDensityStatistics
			{
				AverageBrightness = averageBrightness,
				LowAccumulator = lowAccumulator,
				HighAccumulator = highAccumulator
			};
		}
	}
}