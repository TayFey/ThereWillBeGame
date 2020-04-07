using System;
using System.Collections.Generic;
using System.Linq;

namespace ThereWillBeGame
{
	public abstract class World
	{
		public readonly int Columns, Rows, ViewportColumns, ViewportRows;
		protected readonly List<IDrawableEntity> _entities = new List<IDrawableEntity>();
		private readonly char[] _map;
		private int _viewportY, _viewportX;

		/// <summary>
		/// Создаёт экземпляр нового игрового мира.
		/// </summary>
		/// <param name="columns">Количество столбцов в игровом мире.</param>
		/// <param name="rows">Количество строк в игровом мире.</param>
		/// <param name="viewportColumns">Ширина видимой области.</param>
		/// <param name="viewportRows">Высота видимой области.</param>
		protected World(int columns, int rows, int viewportColumns, int viewportRows)
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
		}

		/// <summary>
		/// Видимая область игрового мира в виде последовательности строк.
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

					foreach (var entity in _entities.Where(e => e.Y.Equals(rows)))
					{
						line[entity.X] = entity.Icon;
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
		protected char this[int y, int x]
		{
			get => _map[x + Columns * y];
			set => _map[x + Columns * y] = value;
		}

		protected void CenterViewportAround(int y, int x)
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
