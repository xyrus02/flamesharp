using System;
using System.Threading;
using JetBrains.Annotations;
using XyrusWorx;
using XyrusWorx.Threading;

namespace Net.Ktrix.Flamesharp
{
	class IterationThread : Operation
	{
		private readonly IteratorContext _context;
		private CalculationState _state;

		private long _batchSize = 1024;
		private const int mFuse = 30;

		internal IterationThread([NotNull] IteratorContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public long BatchSize
		{
			get { return _batchSize; }
			set
			{
				if (_batchSize <= 0)
				{
					throw new ArgumentOutOfRangeException();
				}

				_batchSize = value;
			}
		}

		[NotNull]
		public IteratorContext Context => _context;

		protected override IResult Initialize()
		{
			using (_context.PerformanceCounter.EnterMeasureScope("Thread overhead"))
			{
				_state = _context.CreateThreadState();
			}

			return Result.Success;
		}
		protected override IResult Execute(CancellationToken cancellationToken)
		{
			for (var i = 0; i < BatchSize; i++)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}

				using (_context.PerformanceCounter.EnterMeasureScope("Affine transformation"))
				{
					_context.BeginIteration(_state);
				}

				using (_context.PerformanceCounter.EnterMeasureScope("Variations"))
				{
					foreach (var variation in _state.Transform.Variations)
					{
						variation.Instance.Calculate(_state);
					}
				}

				if (i >= mFuse)
				{
					using (_context.PerformanceCounter.EnterMeasureScope("Plotting"))
					{
						_context.Plot(_state);
					}
				}

				using (_context.PerformanceCounter.EnterMeasureScope("Swapping"))
				{
					_context.EndIteration(_state);
				}
			}

			return Result.Success;
		}

		protected override void Cleanup(bool wasCancelled)
		{
			_state = null;
		}
	}
}