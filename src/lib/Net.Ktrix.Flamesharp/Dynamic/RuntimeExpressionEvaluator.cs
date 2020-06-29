using System;
using System.IO;
using ExpressionEvaluator;
using JetBrains.Annotations;
using XyrusWorx;

namespace Net.Ktrix.Flamesharp.Dynamic
{
	[PublicAPI]
	public sealed class RuntimeExpressionEvaluator<TContext, TResult>
	{
		private Func<TContext, object> _compiledExpression;
		private string _expression;

		public string Expression
		{
			get { return _expression; }
			set
			{
				_expression = value?.Trim();
				_compiledExpression = null;
			}
		}
		public TResult Evaluate([NotNull] TContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (string.IsNullOrWhiteSpace(Expression))
			{
				return default;
			}

			if (_compiledExpression == null)
			{
				try
				{
					var compiler = new CompiledExpression(Expression);
					var func = compiler.ScopeCompile<TContext>();

					_compiledExpression = func;
				}
				catch (Exception exception)
				{
					throw new InvalidDataException($"Failed to process expression: \"{Expression}\". {exception.GetOriginalMessage()}", exception);
				}
			}

			var obj = _compiledExpression(context);
			if (obj is TResult tr)
			{
				return tr;
			}

			return default;
		}
	}
}
