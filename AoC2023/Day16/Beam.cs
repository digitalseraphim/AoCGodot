using System.Collections.Generic;

namespace AoCGodot;

public class Beam
{
	public Pos Position
	{
		get;
	}
	public Direction Direction
	{
		get;
	}

	public Beam(Pos p, Direction d)
	{
		Position = p;
		Direction = d;
	}

	public void SplitInto(List<Beam> list)
	{
		TurnCCWInto(list);
		TurnCWInto(list);
	}

	public void TurnCCWInto(List<Beam> list)
	{
		Direction ccw = Direction.TurnCCW();
		list.Insert(0, new(Position.AfterMove(ccw), ccw));
	}

	public void TurnCWInto(List<Beam> list)
	{
		Direction cw = Direction.TurnCW();
		list.Insert(0, new(Position.AfterMove(cw), cw));
	}

	public void GoStraightInto(List<Beam> list)
	{
		list.Insert(0, new(Position.AfterMove(Direction), Direction));
	}

	public override string ToString()
	{
		return Position + " " + Direction;
	}
	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

}
