public enum Directions
{
	None = 0,
	Up = 1,
	Down = 2,
	Left = 3,
	Right = 4
}

public static class DirectionsExtension
{
    public static Directions inverse(this Directions d)
    {
        switch (d)
        {
            case Directions.Up:
                return Directions.Down;
            case Directions.Down:
                return Directions.Up;
            case Directions.Left:
                return Directions.Right;
            case Directions.Right:
                return Directions.Left;
            default:
                return Directions.None;
        }
    }

	/// <summary>
	/// Returns the corresponding Connections Flag
	/// </summary>
	/// <param name="d"></param>
	/// <returns></returns>
	public static ConnectionsFlag ToConnect(this Directions d)
	{
		switch (d)
		{
			case Directions.Up:
				return ConnectionsFlag.Up;
			case Directions.Down:
				return ConnectionsFlag.Down;
			case Directions.Left:
				return ConnectionsFlag.Left;
			case Directions.Right:
				return ConnectionsFlag.Right;
			default:
				return ConnectionsFlag.None;
		}
	}
}