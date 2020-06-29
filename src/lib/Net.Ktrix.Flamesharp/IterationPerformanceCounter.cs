using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using XyrusWorx;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public class IterationPerformanceCounter : IterationComponent, IIterationPerformanceCounterConfiguration
	{
		private readonly object _cLock = new object();
		private readonly object _stLock = new object();

		private readonly List<double> _samples;
		private readonly ConcurrentDictionary<StringKey, MeasureDataPointTracker> _tracking;

		private long _startTime;
		private long _freq;
		private int _count;

		[DllImport(@"Kernel32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		[DllImport(@"Kernel32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool QueryPerformanceFrequency(out long lpFrequency);

		public IterationPerformanceCounter()
		{
			_tracking = new ConcurrentDictionary<StringKey, MeasureDataPointTracker>();
			_samples = new List<double>();
			_startTime = 0;
		}

		[Pure]
		public double GetIterationsPerSecond()
		{
			lock (_stLock)
			{
				if (_samples.Count == 0)
				{
					return 0;
				}

				return _samples.Average();
			}
		}

		[Pure, NotNull]
		public IEnumerable<MeasureDataPoint> GetMeasureDataPoint()
		{
			return from dataPoint in _tracking select new MeasureDataPoint(dataPoint.Key, dataPoint.Value.Time);
		}

		[NotNull]
		public IScope EnterMeasureScope(StringKey name)
		{
			if (!IsEnabled)
			{
				return new Scope();
			}

			void OnEnter()
			{
				var tracker = _tracking.GetOrAdd(name, new MeasureDataPointTracker(this));
				tracker.BeginMeasure();
			}
			void OnLeave()
			{
				_tracking[name].EndMeasure();
			}

			return new Scope(OnEnter, OnLeave).Enter();
		}

		internal void Start()
		{
			if (!IsEnabled)
			{
				return;
			}

			lock (_stLock)
			{
				if (_freq <= 0)
				{
					if (QueryPerformanceFrequency(out _freq) == false)
					{
						throw new NotSupportedException("Performance counters are not supported on this system.");
					}
				}

				QueryPerformanceCounter(out _startTime);
			}
		}
		internal void Count()
		{
			if (!IsEnabled)
			{
				return;
			}

			lock (_cLock)
			{
				_count++;

				QueryPerformanceCounter(out var time);

				var el = (time - _startTime) / (double)_freq;
				if (el >= 1)
				{
					if (_samples.Count < 15)
					{
						_samples.Add(_count / el);
					}
					else
					{
						_samples.RemoveAt(0);
						_samples.Add(_count / el);
					}

					Start();
					_count = 0;
				}
			}

		}
		internal void Reset()
		{
			if (!IsEnabled)
			{
				return;
			}

			lock (_stLock)
			{
				_tracking.Clear();
				_freq = 0;
				_startTime = 0;
				_count = 0;
			}
		}

		IIterationPerformanceCounterConfiguration IIteratorComponentConfiguration<IIterationPerformanceCounterConfiguration>.Enabled()
		{
			IsEnabled = true;
			return this;
		}
		IIterationPerformanceCounterConfiguration IIteratorComponentConfiguration<IIterationPerformanceCounterConfiguration>.Disabled()
		{
			IsEnabled = false;
			return this;
		}

		[PublicAPI]
		public struct MeasureDataPoint
		{
			public readonly StringKey Name;
			public readonly double SpentTime;

			internal MeasureDataPoint(StringKey name, double spentTime)
			{
				Name = name;
				SpentTime = spentTime;
			}
		}

		class MeasureDataPointTracker
		{
			private readonly IterationPerformanceCounter _counter;
			private long _startTime;
			private double _time;

			internal MeasureDataPointTracker(IterationPerformanceCounter counter)
			{
				_counter = counter;
			}

			public void BeginMeasure()
			{
				QueryPerformanceCounter(out var startTime);
				_startTime = startTime;
			}
			public void EndMeasure()
			{
				QueryPerformanceCounter(out var endTime);
				var delta = (endTime - _startTime) / (double)_counter._freq;
				_time += delta;
			}

			public double Time => _time;
		}
	}
}