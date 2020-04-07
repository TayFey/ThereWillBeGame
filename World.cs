using System;
using System.Collections.Generic;
using System.Linq;

namespace ThereWillBeGame
{
	public sealed class World
	{
		public readonly int Columns, Rows, ViewportColumns, ViewportRows;

		private readonly Player _player;
		private readonly char[] _map;

		private int _viewportY, _viewportX;

		/// <summary>
		/// Создаёт экземпляр нового игрового мира с новым экземплярои игрока.
		/// </summary>
		/// <param name="columns">Количество столбцов в игровом мире.</param>
		/// <param name="rows">Количество строк в игровом мире.</param>
		/// <param name="viewportColumns">Ширина видимой области.</param>
		/// <param name="viewportRows">Высота видимой области.</param>
		public World(int columns, int rows, int viewportColumns, int viewportRows)
		{
			if (columns < 3)
				throw new ArgumentOutOfRangeException(nameof(columns));
			if (rows < 3)
				throw new ArgumentOutOfRangeException(nameof(rows));
			if (viewportColumns < 0)
				throw new ArgumentOutOfRangeException(nameof(viewportColumns));
			if (viewportRows < 0)
				throw new ArgumentOutOfRangeException(nameof(viewportRows));

			ViewportColumns = viewportColumns;
			ViewportRows = viewportRows;
			Columns = columns;
			Rows = rows;

			_map = new char[Columns * Rows];

			// Для примера заполним игровой мир точками и сделаем ограду из непроходимых «решёточек»:
			for (var y = 0; y < Rows; y++)
			{
				for (var x = 0; x < Columns; x++)
				{
					if (y == 0 || y == Rows - 1 || x == 0 || x == Columns - 1)
					{
						this[y, x] = '#';
					}
					else
					{
						this[y, x] = '.';
					}
				}
			}

			// Поставим игрока в центр игрового мира:
			_player = new Player
			{
				X = (int) Math.Floor(columns / 2d),
				Y = (int) Math.Floor(rows / 2d),
			};

			// Подвидем «область видимости», чтобы игрок находился в центре:
			CenterViewportAroundPlayer();
		}

		/// <summary>
		/// Содержит массив строк в видимой области игрового мира.
		/// </summary>
		public IEnumerable<char[]> Map
		{
			get
			{
				for (var rows = 0; rows < Rows; rows++)
				{
					// Строка вне «зоны видимости», пропускаем:
					if (rows < _viewportY || rows > (_viewportY + ViewportRows))
					{
						continue;
					}

					var line = _map.Skip(Columns * rows).Take(Columns).ToArray();

					// Игрок на этой строке? Поместим его иконку поверх всего:
					if (_player.Y.Equals(rows))
					{
						line[_player.X] = _player.Icon;
					}

					// Показываем только видимую часть строки:
					yield return line.Skip(_viewportX).Take(ViewportColumns).ToArray();
				}
			}
		}

		/// <summary>
		/// Позволяет обращаться к конкретному символу в игровом мире без преобразования координат.
		/// </summary>
		/// <param name="y">Строка.</param>
		/// <param name="x">Столбец.</param>
		/// <returns>Символ по заданным координатам.</returns>
		private char this[int y, int x]
		{
			get => _map[x + Columns * y];
			set => _map[x + Columns * y] = value;
		}

		/// <summary>
		/// Если возможно, переместить игрока вверх.
		/// </summary>
		public void MovePlayerUp()
		{
			TryNewCoordinates(_player.Y - 1, _player.X);
		}

		/// <summary>
		/// Если возможно, переместить игрока вниз.
		/// </summary>
		public void MovePlayerDown()
		{
			TryNewCoordinates(_player.Y + 1, _player.X);
		}

		/// <summary>
		/// Если возможно, переместить игрока влево.
		/// </summary>
		public void MovePlayerLeft()
		{
			TryNewCoordinates(_player.Y, _player.X - 1);
		}

		/// <summary>
		/// Если возможно, переместить игрока вправо.
		/// </summary>
		public void MovePlayerRight()
		{
			TryNewCoordinates(_player.Y, _player.X + 1);
		}

		private void TryNewCoordinates(int y, int x)
		{
			if (!IsAvailableToPlayer(y, x))
			{
				return;
			}

			_player.X = x;
			_player.Y = y;

			// После каждого удачного движения персонажа сдвигаем «область видимости»:
			CenterViewportAroundPlayer();
		}

		private bool IsAvailableToPlayer(int y, int x)
		{
			if (y < 0 || y > Rows || x < 0 || x > Columns)
			{
				return false;
			}

			// Единственный «проходимый» символ в игровом мире это точка:
			return this[y, x] == '.';
		}

		private void CenterViewportAroundPlayer()
		{
			CenterViewportAround(_player.Y, _player.X);
		}

		private void CenterViewportAround(int y, int x)
		{
			_viewportX = x - (ViewportColumns / 2);

			// Область оказалась левее, чем край игрового мира:
			if (_viewportX < 0)
			{
				_viewportX = 0;
			}

			// Область оказалась правее, чем край игрового мира:
			if (_viewportX > Columns - ViewportColumns)
			{
				_viewportX = Columns - ViewportColumns;
			}

			_viewportY = y - (ViewportRows / 2);

			// Область оказалась выше, чем край игрового мира:
			if (_viewportY < 0)
			{
				_viewportY = 0;
			}

			// Область оказалась нижу, чем край игрового мира:
			if (_viewportY > Rows - ViewportRows)
			{
				_viewportY = Rows - ViewportRows;
			}
		}
	}
}
