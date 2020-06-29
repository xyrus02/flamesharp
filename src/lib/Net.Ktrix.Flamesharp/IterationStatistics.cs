using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Net.Ktrix.Flamesharp.ObjectModel;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public class IterationStatistics : IterationComponent, IIterationStatisticsConfiguration
	{
		private readonly object _stLock = new object();

		private int[] _hitCounters;
		private Vertex _max;
		private Vertex _min;

		internal IterationStatistics()
		{
			_min = new Vertex(double.NaN, double.NaN);
			_max = new Vertex(double.NaN, double.NaN);
		}

		public Vertex MinimumVertexPosition
		{
			get
			{
				lock (_stLock)
				{
					return _min;
				}
			}
		}
		public Vertex MaximumVertexPosition
		{
			get
			{
				lock (_stLock)
				{
					return _max;
				}
			}
		}

		[NotNull]
		public IEnumerable<int> TranformHitCounts
		{
			get
			{
				lock (_stLock)
				{
					return _hitCounters.ToArray();
				}
			}
		}

		internal void Reset([NotNull] AttractorModel attractor)
		{
			if (!IsEnabled)
			{
				return;
			}

			lock (_stLock)
			{
				_hitCounters = new int[attractor.Transforms.Count];
				_min = new Vertex(double.NaN, double.NaN);
				_max = new Vertex(double.NaN, double.NaN);
			}
		}
		internal void SetTransform(int index)
		{
			if (!IsEnabled)
			{
				return;
			}

			lock (_stLock)
			{
				_hitCounters[index]++;
			}
		}
		internal void Update([NotNull] CalculationState state)
		{
			if (!IsEnabled)
			{
				return;
			}

			if (double.IsNaN(state.Output.X) || double.IsNaN(state.Output.Y))
			{
				return;
			}

			lock (_stLock)
			{
				_min = new Vertex(double.IsNaN(_min.X) ? state.Output.X : Math.Min(state.Output.X, _min.X), double.IsNaN(_min.Y) ? state.Output.Y : Math.Min(state.Output.Y, _min.Y));
				_max = new Vertex(double.IsNaN(_max.X) ? state.Output.X : Math.Max(state.Output.X, _max.X), double.IsNaN(_max.Y) ? state.Output.Y : Math.Max(state.Output.Y, _max.Y));
			}
		}

		IIterationStatisticsConfiguration IIteratorComponentConfiguration<IIterationStatisticsConfiguration>.Enabled()
		{
			IsEnabled = true;
			return this;
		}
		IIterationStatisticsConfiguration IIteratorComponentConfiguration<IIterationStatisticsConfiguration>.Disabled()
		{
			IsEnabled = false;
			return this;
		}
	}
}