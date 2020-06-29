using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace Net.Ktrix.Flamesharp.Cli
{
	sealed class PreviewControl : UserControl
	{
		private readonly BackBuffer _backBuffer;
		private readonly Bitmap _frontBuffer;

		public PreviewControl([NotNull] BackBuffer backBuffer, [NotNull] Bitmap frontBuffer)
		{
			if (backBuffer == null)
			{
				throw new ArgumentNullException(nameof(backBuffer));
			}

			if (frontBuffer == null)
			{
				throw new ArgumentNullException(nameof(frontBuffer));
			}

			_backBuffer = backBuffer;
			_frontBuffer = frontBuffer;
		}

		public Action RenderCallback { get; set; }
		public Action<int, LogDensityStatistics> SampleCallback { get; set; }

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.DrawImage(_frontBuffer, new Rectangle(0, 0, ClientSize.Width, ClientSize.Height), 0, 0, _frontBuffer.Width, _frontBuffer.Height, GraphicsUnit.Pixel);
		}

		public void RenderLoop(Func<bool> cancel)
		{
			var sample = 0;

			while (!cancel())
			{
				RenderCallback?.Invoke();
				var stats = _backBuffer.WriteTo(_frontBuffer);
				SampleCallback?.Invoke(sample++, stats);
				Invalidate();
				Thread.Sleep(10);
			}
		}
	}
}