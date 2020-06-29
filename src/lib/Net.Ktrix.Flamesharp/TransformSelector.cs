using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Net.Ktrix.Flamesharp.ObjectModel;

namespace Net.Ktrix.Flamesharp
{
	class TransformSelector
	{
		private const int mTableSize = 1024;
		private readonly int[] _quantizedProbTable;

		public TransformSelector([NotNull] TransformModel[] transforms)
		{
			if (transforms == null)
			{
				throw new ArgumentNullException(nameof(transforms));
			}

			_quantizedProbTable = new int[mTableSize];

			QuantizeRelativeProbabilities(transforms);
		}

		public int NextIndex([NotNull] IteratorContext context)
		{
			var i = context.RandomInteger() % mTableSize;
			return _quantizedProbTable[i];
		}

		private void QuantizeRelativeProbabilities(IEnumerable<TransformModel> transforms)
		{
			var probs = transforms.Select(x => x.Probability).ToArray();
			var probSum = probs.Sum();

			if (Math.Abs(probSum) < double.Epsilon)
			{
				for (var i = 0; i < probs.Length; i++)
				{
					probs[i] = 1d / probs.Length;
				}
			}
			else
			{
				for (var i = 0; i < probs.Length; i++)
				{
					probs[i] = probs[i] / probSum;
				}
			}

			var ranking = probs.Select((prob, index) => new { Index = index, Probability = prob }).OrderBy(x => x.Probability);
			var j = 0;

			foreach (var item in ranking)
			{
				var q = (int)Math.Floor(item.Probability * mTableSize);

				for (var i = 0; i < q; i++)
				{
					_quantizedProbTable[i + j] = item.Index;
				}

				j += q;
			}
		}
	}
}