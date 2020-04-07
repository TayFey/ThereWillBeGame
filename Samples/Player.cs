namespace ThereWillBeGame.Samples
{
	public sealed class Player : IDrawableEntity
	{
		public char Icon => '@';

		public int X { get; set; }
		public int Y { get; set; }
	}
}
