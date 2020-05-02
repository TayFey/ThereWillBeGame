namespace ThereWillBeGame.Samples
{
	public sealed class Heart : IDrawableEntity
	{
		public Heart(int x, int y)
		{
			X = x;
			Y = y;
		}

		public char Icon => '*';
		public int X { get; set; }
		public int Y { get; set; }
	}
}
