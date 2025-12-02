namespace NLog.MessageTemplates;

/// <summary>
/// Combines Literal and Hole
/// </summary>
internal struct LiteralHole
{
	/// <summary>Literal</summary>
	public Literal Literal;

	/// <summary>Hole</summary>
	/// <remarks>Uninitialized when <see cref="F:NLog.MessageTemplates.Literal.Skip" /> = 0.</remarks>
	public Hole Hole;

	public bool MaybePositionalTemplate
	{
		get
		{
			if (Literal.Skip != 0 && Hole.Index != -1)
			{
				return Hole.CaptureType == CaptureType.Normal;
			}
			return false;
		}
	}

	public LiteralHole(in Literal literal, in Hole hole)
	{
		Literal = literal;
		Hole = hole;
	}
}
