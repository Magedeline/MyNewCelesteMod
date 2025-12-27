namespace DesoloZantas.Core.Core.Others;

/// <summary>
/// A unique level/area/chapter identifier.
/// </summary>
public struct LevelSideId
{
	/// <summary>
	/// The SID (String Identifier) of the level/area/chapter
	/// </summary>
	public string LevelSid;
	/// <summary>
	/// The side type (A/B/C) of the level/area/chapter
	/// </summary>
	public AreaMode Side;
	public LevelSideId(Session session)
	{
		LevelSid = session.Area.GetSID();
		Side = session.Area.Mode;
	}
	public LevelSideId(Level level)
		: this(level.Session) { }
	public LevelSideId(Scene scene)
		: this((scene as Level).Session) { }
	public LevelSideId(AreaKey areakey)
	{
		LevelSid = areakey.GetSID();
		Side = areakey.Mode;
	}
}




