using Godot;

namespace AoCGodot;

public partial class ResultsPanel : HBoxContainer
{
	[Export]
	LineEdit part1Result;
	[Export]
	LineEdit part2Result;

	public void SetPart1Result(string s)
	{
		part1Result.Text = s;
	}

	public void SetPart2Result(string s)
	{
		part2Result.Text = s;
	}

	public void SetResultDeferred(int part, string result)
	{
		CallDeferred(part == 1?"SetPart1Result":"SetPart2Result", result);
	}
}
