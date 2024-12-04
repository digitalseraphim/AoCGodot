using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;

namespace AoCGodot;
public partial class AoC2023_Day17 : BaseChallengeScene
{

	Map<int> map;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	private void DoPart1()
	{
		int v = Util.LowestPathCost(map, map.UL, map.LR, 1, 3);

		resultsPanel.SetPart1Result(v.ToString());
	}

	private void DoPart2()
	{
		int v = Util.LowestPathCost(map, map.UL, map.LR, 4, 10);

		resultsPanel.SetPart2Result(v.ToString());
	}

	private void ParseData(string[] data)
	{
		map = new(Util.ParseIntMap(data));
	}



}
