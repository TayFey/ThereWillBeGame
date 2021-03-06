﻿using System;

namespace ThereWillBeGame.Samples
{
	/// <summary>
	/// Пример игрового мира, который позволяет ходить по квадратной карте.
	/// </summary>
	public sealed class SampleWorld : World
	{
		private readonly Player _player;

		public SampleWorld(int columns, int rows, int viewportColumns, int viewportRows) : base(columns, rows, viewportColumns, viewportRows)
		{
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

			// Добавим игрока в список объектов, которые поддерживают отрисовку, чтобы он был виден на карте:
			_entities.Add(_player);
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
	}
}
