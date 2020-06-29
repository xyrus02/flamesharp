using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp.Data
{
	[PublicAPI]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public struct MemorySize
	{
		private readonly long mBytes;

		public MemorySize(long valueInBytes)
		{
			mBytes = valueInBytes;
		}
		public long Bytes => mBytes;

		public override int GetHashCode() => mBytes.GetHashCode();
		public override bool Equals(object obj) => (obj is MemorySize size) && size.mBytes.Equals(mBytes);
		public override string ToString() => mBytes.ToString();

		// Basis 2
		public long KiB => Bytes / 1024;
		public long MiB => KiB / 1024;
		public long GiB => MiB / 1024;
		public long TiB => GiB / 1024;

		// Basis 10
		public long KB => Bytes / 1000;
		public long MB => KB / 1000;
		public long GB => MB / 1000;
		public long TB => GB / 1000;
	}
}
