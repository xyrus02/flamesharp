using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Net.Ktrix.Flamesharp
{
	[PublicAPI]
	public class ColorMap
	{
		[JsonIgnore]
		private Rgb[] _controlPoints;

		[JsonIgnore]
		private Func<double, Rgb> _func;

		internal ColorMap() { }

		public ColorMap([NotNull] Func<double, Rgb> func)
		{
			_func = func ?? throw new ArgumentNullException(nameof(func));
		}
		public ColorMap([NotNull] params Rgb[] controlPoints)
		{
			_controlPoints = controlPoints ?? throw new ArgumentNullException(nameof(controlPoints));
		}

		[NotNull]
		public Rgb[] Render(int length)
		{
			if (length <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(length));
			}

			if (_controlPoints != null)
			{
				return RenderCp(length).ToArray();
			}

			if (_func != null)
			{
				return RenderLambda(length).ToArray();
			}

			Debug.Assert(false, "Unsupported color map render mode");

			return new Rgb[length];
		}

		public void SetControlPoints([NotNull] Rgb[] controlPoints)
		{
			_controlPoints = controlPoints ?? throw new ArgumentNullException(nameof(controlPoints));
			_func = null;
		}
		public void SetFunction([NotNull] Func<double, Rgb> func)
		{
			_func = func ?? throw new ArgumentNullException(nameof(func));
			_controlPoints = null;
		}

		private IEnumerable<Rgb> RenderLambda(int length)
		{
			for (var i = 0; i < length; i++)
			{
				yield return _func((double)i / (length - 1));
			}
		}
		private IEnumerable<Rgb> RenderCp(int length)
		{
			if (_controlPoints.Length == length)
			{
				for (var i = 0; i < length; i++)
				{
					yield return _controlPoints[i];
				}

				yield break;
			}

			if (_controlPoints.Length == 1)
			{
				for (var i = 0; i < length; i++)
				{
					yield return _controlPoints[0];
				}

				yield break;
			}

			if (_controlPoints.Length == 2)
			{
				for (var i = 0; i < length; i++)
				{
					yield return Lerp(_controlPoints[0], _controlPoints[1], (double)i / (length - 1));
				}

				yield break;
			}

			if (_controlPoints.Length < length)
			{
				var c = 0;

				var subdivision = new[]{0}
					.Concat(Enumerable.Range(1, _controlPoints.Length - 2).Select(x => x * length / (_controlPoints.Length - 1)))
					.Concat(new[]{length - 1})
					.ToArray();

				var previous = subdivision[c];
				var next = subdivision[c + 1];

				for (var i = 0; i < length - 1; i++)
				{
					if (i == previous)
					{
						yield return _controlPoints[c];
						continue;
					}

					if (i == next)
					{
						previous = subdivision[++c];
						next = subdivision[c+1];

						yield return _controlPoints[c];
						continue;
					}

					yield return Lerp(_controlPoints[c], _controlPoints[c + 1], (double) i / (next - previous - 1));
				}

				yield break;
			}

			if (_controlPoints.Length > length)
			{
				var leap = (int)Math.Floor(_controlPoints.Length / (double)length);

				for (var i = 0; i < length - 1; i++)
				{
					yield return _controlPoints[i * leap];
				}
			}
		}

		private static Rgb Lerp(Rgb a, Rgb b, double c) => new Rgb(
			(int) Math.Max(0, Math.Min(a.R + (b.R - a.R) * c, 255)),
			(int) Math.Max(0, Math.Min(a.G + (b.G - a.G) * c, 255)),
			(int) Math.Max(0, Math.Min(a.B + (b.B - a.B) * c, 255)));
	}
}