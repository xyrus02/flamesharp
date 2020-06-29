using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp.ObjectModel
{
	[PublicAPI]
	public class TransformModel
	{
		private double _colorOffset;
		private double _colorSpread;
		private double _probability;

		[NotNull]
		public MatrixModel Matrix { get; private set; } = new MatrixModel();

		[NotNull]
		public List<VariationModel> Variations { get; private set; } = new List<VariationModel>();

		public double Probability
		{
			get => _probability;
			set => _probability = value;
		}
		public double ColorOffset
		{
			get => _colorOffset;
			set
			{
				_colorOffset = value;
				UpdateCmapCorners();
			}
		}
		public double ColorSpread
		{
			get => _colorSpread;
			set
			{
				_colorSpread = value;
				UpdateCmapCorners();
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext streamingContext)
		{
			UpdateCmapCorners();

			if (_probability <= 0)
			{
				_probability = 1.0;
			}
		}

		private void UpdateCmapCorners()
		{
			var c0 = Math.Max(0, Math.Min(_colorOffset, 1));
			var c1 = Math.Max(-1, Math.Min(_colorSpread, 1));

			CmapCorners = new[]
			{
				(1 + c1) * 0.5,
				c0 * (1 - c1) * 0.5
			};
		}

		[NotNull]
		internal double[] CmapCorners { get; private set; } = {1,0};
	}
}