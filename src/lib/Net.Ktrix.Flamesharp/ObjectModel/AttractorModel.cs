using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Net.Ktrix.Flamesharp.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using XyrusWorx.IO;

namespace Net.Ktrix.Flamesharp.ObjectModel
{
	[PublicAPI]
	public class AttractorModel
	{
		private readonly RuntimeExpressionEvaluator<ColorMapExpressionContext, Rgb> _cmapExpression;
		private readonly ColorMapExpressionContext _cmapExpressionContext;

		public AttractorModel()
		{
			_cmapExpression = new RuntimeExpressionEvaluator<ColorMapExpressionContext, Rgb>();
			_cmapExpressionContext = new ColorMapExpressionContext();
		}

		[NotNull]
		[JsonIgnore]
		public ColorMap ColorMap { get; private set; } = new ColorMap(c => new Rgb(c, c, c));

		[CanBeNull]
		[JsonProperty("cmap")]
		public string ColorMapExpression { get; set; }

		[NotNull]
		public List<TransformModel> Transforms { get; private set; } = new List<TransformModel>();

		[NotNull]
		public static AttractorModel FromJson([NotNull] TextContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			using (var inputFile = container.Read())
			{
				return JsonConvert.DeserializeObject<AttractorModel>(inputFile.ReadToEnd(), new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver()
				});
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext streamingContext)
		{
			if (string.IsNullOrWhiteSpace(ColorMapExpression))
			{
				ColorMap = new ColorMap(c => new Rgb(c, c, c));
			}
			else
			{
				_cmapExpression.Expression = ColorMapExpression;
				ColorMap = new ColorMap(
					c =>
					{
						((IColorMapExpressionContextSetup)_cmapExpressionContext).SetOffset(c);
						return _cmapExpression.Evaluate(_cmapExpressionContext);
					});
			}

		}
	}
}
