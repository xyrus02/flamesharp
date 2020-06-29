using System;
using JetBrains.Annotations;
using Net.Ktrix.Flamesharp.ObjectModel;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public sealed class CalculationState
	{
		internal CalculationState([NotNull] IteratorContext context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public ulong IterationNumber { get; internal set; }
		public int TransformIndex { get; internal set; }
		public TransformModel Transform { get; internal set; }

		public Vertex Input { get; set; }
		public Vertex Output { get; set; }

		[NotNull]
		public IteratorContext Context { get; }
	}
}