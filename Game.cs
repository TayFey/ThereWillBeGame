using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThereWillBeGame
{
	public sealed class Game
	{
		public readonly int Columns, Rows;

		public Game(int columns, int rows)
		{
			if (columns < 0)
				throw new ArgumentOutOfRangeException(nameof(columns));
			if (rows < 0)
				throw new ArgumentOutOfRangeException(nameof(rows));

			Columns = columns;
			Rows = rows;
		}

		public event Action<char[], double> OnRedraw;
		public event Action<double> OnTick;
		public event Action<ConsoleKeyInfo> OnKeyAvailable;

		public int FPS { get; private set; }
		private static double GlobalMilliseconds => DateTime.UtcNow.TimeOfDay.TotalMilliseconds;

		public void Initialize()
		{
			Console.SetWindowSize(Columns, Rows + 1);
			Console.SetBufferSize(Columns, Rows + 1);
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.White;
			Console.InputEncoding = Console.OutputEncoding = Encoding.Unicode;
			Console.CursorVisible = false;
		}

		public async Task Run()
		{
			var refresher = new Thread(Refresh);
			refresher.Start();
			await using var writer = GetConsoleStreamWriter();
			Console.SetOut(writer);
			await RenderingLoop(writer);
		}

		private static StreamWriter GetConsoleStreamWriter()
		{
			return new StreamWriter(Console.OpenStandardOutput(), new UnicodeEncoding(false, false))
			{
				AutoFlush = false,
			};
		}

		private async Task RenderingLoop(TextWriter writer)
		{
			var milliseconds = GlobalMilliseconds;
			while (true)
			{
				var previousFrameMilliseconds = GlobalMilliseconds - milliseconds;
				FPS = (int) Math.Floor(1000d / previousFrameMilliseconds);
				milliseconds = GlobalMilliseconds;
				var array = new char[Columns * Rows];
				OnRedraw?.Invoke(array, previousFrameMilliseconds);
				await writer.WriteAsync(array);
				await writer.FlushAsync();
				Console.SetCursorPosition(0, 0);
			}
		}

		private void Refresh()
		{
			var milliseconds = GlobalMilliseconds;
			while (true)
			{
				var previousFrameMilliseconds = GlobalMilliseconds - milliseconds;
				milliseconds = GlobalMilliseconds;
				OnTick?.Invoke(previousFrameMilliseconds);

				if (!Console.KeyAvailable)
				{
					continue;
				}

				var info = Console.ReadKey(true);
				OnKeyAvailable?.Invoke(info);
			}
		}
	}
}
