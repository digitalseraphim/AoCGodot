using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AoCGodot;
public partial class AoC2024Day18 : BaseChallengeScene
{
	Map<int> memory;
	List<Pos> incoming;
	int part1ProcessNum;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private void DoPart1()
	{
		for(int i = 0; i < part1ProcessNum; i++){
			memory.SetValueAt(incoming[i], 0xffff);
		}

		int cost = Util.LowestPathCost(memory, memory.UL, memory.LR, -1, -1);

		resultsPanel.SetPart1Result(cost);
	}

	private void DoPart2()
	{
		int good = 0;
		int bad = incoming.Count - 1;

		while(good != bad-1){
			int v = (good+bad)/2;
			Map<int> m = new(memory);

			for(int i = 0; i < v; i++){
				m.SetValueAt(incoming[i], 0xffff);
			}

			int cost = Util.LowestPathCost(m, m.UL, m.LR, -1, -1);
			GD.Print($"{v} - {cost}");
			if(cost > 70*70){
				GD.Print("bad");
				bad = v;
			}else{
				GD.Print("good");
				good = v;
			}
		}

		resultsPanel.SetPart2Result(incoming[bad-1].ToString());
	}

	private void ParseData(string[] data)
	{
		if (challengeDataPanel.IsChallengeData())
		{
			memory = new(71, 71, 1);
			part1ProcessNum = 1024;
		}
		else
		{
			memory = new(7, 7, 1);
			part1ProcessNum = 12;
		}
		incoming = data.Select((x) => x.Split(",").Select(Int32.Parse))
					   .Select((x) => new Pos(x.First(), x.Last())).ToList();
	}

}
