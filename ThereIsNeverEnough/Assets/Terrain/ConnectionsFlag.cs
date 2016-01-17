/// <summary>
/// For roads on how they're connected
/// </summary>
[System.Flags]
public enum ConnectionsFlag
{
	None		= 0,
	Up			= 1,
	Down		= 2,
	Left		= 4,
	Right		= 8,
	UpRight		= 16,
	DownRight	= 32,
	UpLeft		= 64,
	DownLeft	= 128
}
