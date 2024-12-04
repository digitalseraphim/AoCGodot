using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day2 : BaseChallengeScene
{
	List<List<int>> reports;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	private void DoPart1()
	{
		int count = 0;
		foreach(var report in reports){
			if(check(report, null)){
				count ++;
			}
		}
		resultsPanel.SetPart1Result(count);
	}

	private bool check(List<int> report, string comment){
		bool good = true;
		bool increasing = false;

		if(comment != null){
			GD.Print(comment);
		}

		for(int i = 0; i < report.Count -1; i++){
			int diff = report[i+1] - report[i];

			if (Math.Abs(diff) < 1 || Math.Abs(diff) > 3){
					good = false;
					break;
			}

			if(i == 0){
				increasing = diff > 0;
			}else{
				if (increasing != diff > 0){
					good = false;
					break;
				}
			}
		}

		return good;
	}

	private void DoPart2()
	{
		int count = 0;
		foreach(var report in reports){
			for(int r = 0; r < report.Count; r++){
				if(check(report.Where( (x,i) => i != r).ToList(), "skip " + r)){
					count++;
					break;
				}
			}
		}
		resultsPanel.SetPart2Result(count);
	}

	private void ParseData(string[] data){
		reports = new();
		foreach(var s in data){
			var parts = s.Split(" ", StringSplitOptions.RemoveEmptyEntries);
			List<int> list = parts.Select(Int32.Parse).ToList();
			reports.Add(list);
		}
	}

}
