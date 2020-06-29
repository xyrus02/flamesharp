using JetBrains.Annotations;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Net.Ktrix.Flamesharp.ObjectModel;
using XyrusWorx;
using XyrusWorx.IO;
using XyrusWorx.Runtime;

namespace Net.Ktrix.Flamesharp.Cli
{
	class App : ConsoleApplication
	{
		private double _totalDensity;

		public App()
		{
			ServiceLocator.Default.RegisterSingleton<VariationRegistry>();
			ServiceLocator.Default.Resolve<VariationRegistry>().Register(typeof(VariationRegistry).Assembly);
		}

		[UsedImplicitly, CommandLineValues]
		[CommandLineAnnotation(ValueLabel = "<formula-file>")]
		public string FormulaFile { get; private set; }

		[UsedImplicitly, CommandLineSwitch("trace", ShortForm = "t")]
		public bool Trace { get; private set; }

		[UsedImplicitly, CommandLineSwitch("single-threading", ShortForm = "st")]
		public bool ForceSingleThreading { get; private set; }

		[UsedImplicitly, CommandLineProperty("resolution", ShortForm = "sz")]
		public int Resolution{ get; private set; }

		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		protected override IResult Execute(CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(FormulaFile))
			{
				WriteHelp();
				return Result.Success;
			}

			Log.Write("Preparing...");

			var formulaFileName = Path.GetFileName(FormulaFile);
			var directory = new FileSystemStore(Path.GetDirectoryName(FormulaFile) ?? throw new FileNotFoundException("Unable to access formula file", FormulaFile));

			var size = Resolution <= 0 ? 512 : Resolution;

			using (var backBuffer = new BackBuffer(size, size))
			{
				using var bitmap = new Bitmap(backBuffer.Width, backBuffer.Height, PixelFormat.Format32bppArgb);
				using var diagnostics = new DiagnosticsWindow();
				using var preview = new PreviewControl(backBuffer, bitmap);
				
				var formula = AttractorModel.FromJson(directory.Open(formulaFileName).AsText());
				var iterator = new Iterator(backBuffer).Configure(config => config
					.Statistics(s => s.SetEnabled(Trace))
					.PerformanceCounter(s => s.SetEnabled(Trace))
					.OnFinalize(WriteHeader)
					.Log(Log));

				iterator.AllowMultithreading = !ForceSingleThreading;

				var n = 0;

				using (var bitmapG = Graphics.FromImage(bitmap))
				{
					bitmapG.FillRectangle(Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);
				}

				preview.SampleCallback = (sample, stats) =>
				{
					diagnostics.Invoke(new Action(
						() =>
						{
							diagnostics.brightness.Series[0].Points.AddXY(iterator.GetCurrentDensity(), stats.AverageBrightness);
							diagnostics.peakPoint.Series[0].Points.AddXY(iterator.GetCurrentDensity(), stats.HighAccumulator);
							diagnostics.peakPoint.Series[1].Points.AddXY(iterator.GetCurrentDensity(), stats.LowAccumulator);
						}));
				};

				preview.RenderCallback = () =>
				{
					var cd = iterator.GetCurrentDensity();
					var increment = cd < 10 ? 1 : GetDensity(++n) / 10.0;
					// ReSharper disable once MethodSupportsCancellation
					iterator.Iterate(formula, increment);
					_totalDensity = iterator.GetCurrentDensity();
				};

				preview.Dock = DockStyle.Fill;
				diagnostics.histogramPanel.Controls.Add(preview);

				var thread = new Thread(() => preview.RenderLoop(() => !preview.Visible))
				{
					IsBackground = true
				};

				diagnostics.Shown += (o,e) => thread.Start();
				diagnostics.ShowDialog();
			}

			return Result.Success;
		}

		private void WriteHeader()
		{
			Console.Clear();

			using (WithColor(ConsoleColor.White, ConsoleColor.DarkCyan))
			{
				var pad = Console.WindowWidth;

				Console.Write("".PadRight(pad));
				Console.Write($@" FlameSharp Analytics Utility - {Path.GetFileName(FormulaFile)}".PadRight(pad));

				using (WithColor(ConsoleColor.Yellow))
				{
					Console.Write($@" Total density: {_totalDensity:###,###,###,###,###,##0.00} iterations/pixel".PadRight(pad));
					Console.Write($@" Trace mode:    {(Trace ? "ON" : "OFF")}".PadRight(pad));
				}

				Console.Write("".PadRight(pad));
			}

			Console.WriteLine("");
		}

		private static int GetDensity(int pass)
		{
			if (pass <= 1)
			{
				return 1;
			}

			if (pass >= 30)
			{
				pass = 30;
			}

			return GetDensity(pass - 2) + GetDensity(pass - 1);
		}

		[UsedImplicitly]
		static void Main() => new App().Run();
	}
}
