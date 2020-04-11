using System;
using System.Collections.Generic;
using System.Text;

namespace ThereWillBeGame.Samples
{
	public sealed class Heart : IDrawableEntity
	{
		public char Icon => '*';
		public int X { get; set; }
		public int Y { get; set; }

	}
}
