using System;
using System.Threading.Tasks;

using ThereWillBeGame.Samples;

namespace ThereWillBeGame
{
	public static class Program
	{
		private static SampleWorld _world;
		private static Game _game;

		private static int _seconds;
		private static double _milliseconds;

		public static async Task Main()
		{
			Console.Title = _seconds.ToString();

			_world = new SampleWorld(200, 40, 80, 20);
			_game = new Game(120, 30);

			_game.OnRedraw += OnRedraw;
			_game.OnTick += OnTick;
			_game.OnKeyAvailable += OnKeyAvailable;

			_game.Initialize();
			await _game.Run();
		}

		/// <summary>
		/// Вызывается каждый раз, когда нажата кнопка.
		/// </summary>
		/// <param name="info">Содержит информацию о нажатой кнопке.</param>
		private static void OnKeyAvailable(ConsoleKeyInfo info)
		{
			switch (info.Key)
			{
				case ConsoleKey.UpArrow:
					_world.MovePlayerUp();
					break;
				case ConsoleKey.DownArrow:
					_world.MovePlayerDown();
					break;
				case ConsoleKey.LeftArrow:
					_world.MovePlayerLeft();
					break;
				case ConsoleKey.RightArrow:
					_world.MovePlayerRight();
					break;
				case ConsoleKey.Escape:
					Environment.Exit(0);
					return;
			}
		}

		/// <summary>
		/// Вызывается каждый раз, когда игровой мир обновляется.
		/// </summary>
		/// <param name="milliseconds">Количество миллисекунд с последнего обновления.</param>
		private static void OnTick(double milliseconds)
		{
			_milliseconds += milliseconds;
			while (_milliseconds > 1000d)
			{
				_milliseconds -= 1000d;
				_seconds++;
				Console.Title = _seconds.ToString();
			}
		}

		/// <summary>
		/// Вызывается каждый раз, когда содержимое консоли будет перерисовано.
		/// </summary>
		/// <param name="array">Буфер, который будет отрисован на экране.</param>
		/// <param name="milliseconds">Количество миллисекунд с последнего вызова перерисовки.</param>
		private static void OnRedraw(char[] array, double milliseconds)
		{
			var rows = 0;
			$"Частота кадров: {_game.FPS}".ToCharArray().CopyTo(array, rows++);
			foreach (var r in _world.Map)
			{
				var index = _game.Columns * rows++;
				r.CopyTo(array, index);
			}
		}
	}
}
