using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using XyrusWorx;

namespace Net.Ktrix.Flamesharp.ObjectModel
{
	[PublicAPI]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
	public class MatrixModel
	{
		public string Data
		{
			get { return string.Format(CultureInfo.InvariantCulture, "{0:0.0#####} {1:0.0#####} {2:0.0#####} {3:0.0#####} {4:0.0#####} {5:0.0#####}", XX, XY, YX, YY, OX, OY); }
			set
			{
				if (Equals(Data, value))
				{
					return;
				}

				var tokens = Regex.Split((value ?? "").Trim(), @"[\s\r\n\t]+");
				var values = new double[6];

				for (var i = 0; i < tokens.Length && i < 6; i++)
				{
					values[i] = tokens[i].TryDeserialize<double>(CultureInfo.InvariantCulture);
				}

				XX = values[0];
				XY = values[1];
				YX = values[2];
				YY = values[3];
				OX = values[4];
				OY = values[5];
			}
		}

		[JsonIgnore, XmlIgnore]
		public double XX { get; set; } = 1.0;

		[JsonIgnore, XmlIgnore]
		public double XY { get; set; } = 0.0;

		[JsonIgnore, XmlIgnore]
		public double YX { get; set; } = 0.0;

		[JsonIgnore, XmlIgnore]
		public double YY { get; set; } = 1.0;

		[JsonIgnore, XmlIgnore]
		public double OX { get; set; } = 0.0;

		[JsonIgnore, XmlIgnore]
		public double OY { get; set; } = 0.0;

		public Vertex Multiply(Vertex vertex)
		{
			return new Vertex(
				XX * vertex.X + XY * vertex.X + OX,
				YX * vertex.Y + YY * vertex.Y + OY,
				vertex.C);
		}
	}
}