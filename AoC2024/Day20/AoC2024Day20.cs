using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;

namespace AoCGodot;
public partial class AoC2024Day20 : BaseChallengeScene
{
	Map<char> race;
	Pos start;
	Pos end;
	Dictionary<Pos, int> costs = new();

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	
	private void DoPart1()
	{
		int minSavings;
		int count = 0;
		Dictionary<int, int> savingsCounts = new();
		costs = new();

		if(challengeDataPanel.IsChallengeData()){
			minSavings = 100;
		}else{
			minSavings = 1;
		}

		GD.Print($"start {start}, end {end}");

		Pos p = start;
		int cost = 0;
		costs[p] = cost++;
		while(!p.Equals(end)){
			foreach(Pos n in p.Neighbors){
				if(race.ValueAt(n) != '#' && !costs.ContainsKey(n)){
					costs[n] = cost++;
					p = n;
					break;
				}
			}
			if(!challengeDataPanel.IsChallengeData()){
				GD.Print($"pos = {p}");
			}
		}
		GD.Print($"total cost: " + cost);

		foreach(var r in costs){
			p = r.Key;
			int pc = r.Value;

			foreach(Direction d in Direction.ALL){
				var p2 = p.AfterMove(d,2);
				int savings = costs.GetValueOrDefault(p2) - pc - 2;
				if(costs.ContainsKey(p2) && savings >= minSavings){
					if(!challengeDataPanel.IsChallengeData()){
						GD.Print($"cheat from {p} to {p2} saves {savings}");
					}
					count++;
					int old = savingsCounts.GetOrCreate(savings, ()=>0);
					savingsCounts[savings] = old+1;
				}
			}
		}

		foreach(var s in savingsCounts){
			GD.Print($"{s.Value} cheats saved {s.Key}");
		}
		
		resultsPanel.SetPart1Result(count);
	}

	private void DoPart2()
	{
		int minSavings;
		int count = 0;
		Dictionary<int, int> savingsCounts = new();
		GD.Print("Part 2");
		if(challengeDataPanel.IsChallengeData()){
			minSavings = 100;
		}else{
			minSavings = 50;
		}

		foreach(var r in costs){
			var p = r.Key;
			int pc = r.Value;

			foreach(var r2 in costs){
				var p2 = r2.Key;
				int pc2 = r2.Value;
				int d = p2.Distance(p);
				if(d <= 20){
					int savings = costs.GetValueOrDefault(p2) - pc - d;
					if(costs.ContainsKey(p2) && savings >= minSavings){
						if(!challengeDataPanel.IsChallengeData()){
							GD.Print($"cheat from {p} to {p2} saves {savings}");
						}
						int old = savingsCounts.GetOrCreate(savings, ()=>0);
						savingsCounts[savings] = old+1;
						count++;
					}
				}
			}
		}
		foreach(var s in savingsCounts){
			GD.Print($"{s.Value} cheats saved {s.Key}");
		}

		resultsPanel.SetPart2Result(count);
	}

	private void ParseData(string[] data){
		race = new(Util.ParseCharMap(data));
		var se = race.FindStartAndEnd();
		start = se.Item1;
		end = se.Item2;
	}

}
