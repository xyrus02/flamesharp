using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Net.Ktrix.Flamesharp.Data;
using Net.Ktrix.Flamesharp.ObjectModel;
using XyrusWorx.Diagnostics;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public class IteratorContext : IIteratorContextConfiguration
	{
		private readonly object _randomLock = new object();
		private readonly object _histogramLock = new object();

		private readonly BackBuffer _backBuffer;
		private readonly Stopwatch _stopwatch;

		private Random _random;
		private TransformSelector _selector;
		private TransformModel[] _transforms;
		private Rgb[] _colors;
		private AttractorModel _attractor;

		private Action _onBeginning;
		private Action _onFinalize;

		public IteratorContext([NotNull] BackBuffer backBuffer)
		{
			_backBuffer = backBuffer ?? throw new ArgumentNullException(nameof(backBuffer));

			_stopwatch = new Stopwatch();

			PerformanceCounter = new IterationPerformanceCounter();
			Statistics = new IterationStatistics();
			Histogram = new Buffer<HistogramPoint>(backBuffer.BufferSize);
		}

		public double CurrentDensity { get; private set; }

		[CanBeNull]
		public ILogWriter Log { get; private set; }

		[NotNull]
		public Buffer<HistogramPoint> Histogram { get; }

		[NotNull]
		public AttractorModel Attractor => _attractor;

		[NotNull]
		public BackBuffer BackBuffer => _backBuffer;

		[NotNull]
		public IterationPerformanceCounter PerformanceCounter { get; }

		[NotNull]
		public IterationStatistics Statistics { get; }

		[Pure] // yup, I know, it's not pure by definition!
		public double Random()
		{
			lock (_randomLock)
			{
				return _random.NextDouble();
			}
		}

		[Pure] // yup, I know, it's not pure by definition!
		public int RandomInteger()
		{
			lock (_randomLock)
			{
				return _random.Next();
			}
		}

		internal void Begin([NotNull] AttractorModel attractor)
		{
			if (attractor == null)
			{
				throw new ArgumentNullException(nameof(attractor));
			}

			Statistics.Reset(attractor);
			PerformanceCounter.Reset();
			_stopwatch.Reset();

			lock (_randomLock)
			{
				_random = new Random((int)DateTime.Now.Ticks);
			}

			if (!Equals(attractor, _attractor))
			{
				_attractor = attractor;
				_transforms = attractor.Transforms.ToArray();
				_colors = attractor.ColorMap.Render(1024);
				_selector = new TransformSelector(_transforms);
			}

			_onBeginning?.Invoke();

			PerformanceCounter.Start();
			_stopwatch.Start();
		}
		internal void End(int threadCount, double density)
		{
			CurrentDensity += density;

			_stopwatch.Stop();
			_onFinalize?.Invoke();

			Log?.Write($"Processed histogram using {threadCount} batch(es) and a density of {density} iterations/pixel");

			if (Statistics.IsEnabled)
			{
				var hcs = Statistics.TranformHitCounts.Sum();

				Log?.Write("Transform hit statistics:\r\n  " + string.Join("\r\n  ", Statistics.TranformHitCounts
					.Select((x, i) => $"Transform #{i + 1}: {x} ({100.0 * x / hcs:###,###,###,###,###,##0.00}%)")));

				Log?.Write("Peaks:\r\n"
					+ $"  Low:  ( X: {Statistics.MinimumVertexPosition.X:###,###,###,###,###,##0.000000} | Y: {Statistics.MinimumVertexPosition.Y:###,###,###,###,###,##0.000000} )\r\n"
					+ $"  High: ( X: {Statistics.MaximumVertexPosition.X:###,###,###,###,###,##0.000000} | Y: {Statistics.MaximumVertexPosition.Y:###,###,###,###,###,##0.000000} )");
			}

			if (PerformanceCounter.IsEnabled)
			{
				var sts = PerformanceCounter.GetMeasureDataPoint().Sum(x => x.SpentTime);

				Log?.Write($"Average iterations / second: {PerformanceCounter.GetIterationsPerSecond():###,###,###,###,###,##0.000}");

				Log?.Write("Time spent in calculation phases:\r\n  " + string.Join("\r\n  ", PerformanceCounter.GetMeasureDataPoint()
					.OrderByDescending(x => x.SpentTime)
					.Select(x => $"{x.Name}: {x.SpentTime * 1000:###,###,###,###,###,##0}ms ({100.0 * x.SpentTime / sts:###,###,###,###,###,##0.00}%)")));
			}

			Log?.Write($"Total time spent in calculation: {_stopwatch.ElapsedMilliseconds:###,###,###,###,###,##0}ms");
		}

		internal CalculationState CreateThreadState()
		{
			var calculationState = new CalculationState(this);

			calculationState.Input = new Vertex(
				Random() * 2 - 1,
				Random() * 2 - 1,
				Random() * 2 - 1);

			NextTransform(calculationState);

			return calculationState;
		}

		internal void BeginIteration([NotNull] CalculationState state)
		{
			var trans = state.Transform;

			state.Input = trans.Matrix.Multiply(state.Input).Copy(c: state.Input.C * trans.CmapCorners[0] + trans.CmapCorners[1]);
			state.Output = new Vertex(0, 0, state.Input.C);
		}
		internal void EndIteration([NotNull] CalculationState state)
		{
			NextTransform(state);

			Statistics.Update(state);

			state.Input = state.Output.ResetNaNs();
			state.IterationNumber++;

			PerformanceCounter.Count();
		}
		internal void Plot([NotNull] CalculationState state)
		{
			var pixelX = (int)((1 + state.Output.X) * _backBuffer.Width * 0.5f);
			var pixelY = (int)((2 - (1 + state.Output.Y)) * _backBuffer.Height * 0.5f);

			if (pixelX >= 0 && pixelY >= 0 && pixelX < _backBuffer.Width && pixelY < _backBuffer.Height)
			{
				var caddr = (int)Math.Max(0, Math.Min(state.Output.C * (_colors.Length - 1), _colors.Length - 1));
				var addr = _backBuffer.Width * pixelY + pixelX;
				var col = _colors[caddr];

				lock (_histogramLock)
				{
					var p = Histogram[addr];

					p.Red += col.R;
					p.Green += col.G;
					p.Blue += col.B;
					p.Count++;

					Histogram[addr] = p;
				}
			}
		}

		private void NextTransform(CalculationState calculationState)
		{
			calculationState.TransformIndex = _selector.NextIndex(this);
			calculationState.Transform = _transforms[calculationState.TransformIndex];

			Statistics.SetTransform(calculationState.TransformIndex);
		}

		IIteratorContextConfiguration IIteratorContextConfiguration.Log(ILogWriter log)
		{
			Log = log;
			return this;
		}
		IIteratorContextConfiguration IIteratorContextConfiguration.OnBeginning(Action action)
		{
			_onBeginning = action;
			return this;
		}
		IIteratorContextConfiguration IIteratorContextConfiguration.OnFinalize(Action action)
		{
			_onFinalize = action;
			return this;
		}
		IIteratorContextConfiguration IIteratorContextConfiguration.PerformanceCounter(Action<IIterationPerformanceCounterConfiguration> configuration)
		{
			configuration?.Invoke(PerformanceCounter);
			return this;
		}
		IIteratorContextConfiguration IIteratorContextConfiguration.Statistics(Action<IIterationStatisticsConfiguration> configuration)
		{
			configuration?.Invoke(Statistics);
			return this;
		}
	}
}