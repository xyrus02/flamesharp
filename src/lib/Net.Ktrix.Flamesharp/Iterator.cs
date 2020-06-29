using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Net.Ktrix.Flamesharp.ObjectModel;
using XyrusWorx;
using XyrusWorx.Diagnostics;
using XyrusWorx.Threading;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public class Iterator
	{
		private const long mBatchSize = 1_000_000L;

		private readonly IteratorContext _context;
		private Action<IIteratorContextConfiguration> _configuration;

		public Iterator([NotNull] BackBuffer backBuffer)
		{
			_context = new IteratorContext(backBuffer);
		}

		public bool AllowMultithreading { get; set; } = true;

		[NotNull]
		public Iterator Configure([NotNull] Action<IIteratorContextConfiguration> configuration)
		{
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			return this;
		}

		public double GetCurrentDensity()
		{
			return _context.CurrentDensity;
		}

		public void Iterate([NotNull] AttractorModel attractor, CancellationToken cancellationToken = default) => Iterate(attractor, 1, cancellationToken);
		public void Iterate([NotNull] AttractorModel attractor, double density, CancellationToken cancellationToken = default)
		{
			if (attractor == null)
			{
				throw new ArgumentNullException(nameof(attractor));
			}

			if (density <= 0 || cancellationToken.IsCancellationRequested)
			{
				return;
			}

			var batchSize = mBatchSize;
			var totalSize = (long)Math.Ceiling(_context.BackBuffer.BufferSize * density);

			_configuration?.Invoke(_context);

			var batchCount = (int)Math.Ceiling((double)totalSize / batchSize);
			var threads = Enumerable.Range(0, batchCount).Select(x => new IterationThread(_context)).ToArray();

			using (new Scope(() => _context.Begin(attractor), () => _context.End(threads.Length, density)).Enter())
			{
				var parallelOptions = new ParallelOptions();

				if (!AllowMultithreading)
				{
					parallelOptions.MaxDegreeOfParallelism = 1;
					batchSize = (long)Math.Ceiling(_context.BackBuffer.BufferSize * density);
				}

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				Parallel.ForEach(threads, parallelOptions, thread => ProcessBatch(thread, batchSize, cancellationToken));
			}

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			using (_context.PerformanceCounter.EnterMeasureScope("Bitmap processing"))
			{
				_context.BackBuffer.ProcessImage(this, _context, cancellationToken);
			}
		}

		private void ProcessBatch([NotNull] IterationThread thread, long batchSize, CancellationToken cancellationToken)
		{
			if (thread == null)
			{
				throw new ArgumentNullException(nameof(thread));
			}

			thread.DispatchMode = OperationDispatchMode.Synchronous;
			thread.BatchSize = batchSize;

			try
			{
				thread.Run(cancellationToken);
			}
			catch (Exception exception)
			{
				thread.Context.Log?.WriteError($"Batch aborted because of an exception in the accumulation loop: {exception.GetBaseException()}");
			}
		}
	}
}