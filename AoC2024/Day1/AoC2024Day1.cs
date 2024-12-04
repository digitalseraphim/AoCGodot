using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;

namespace AoCGodot;
public partial class AoC2024Day1 : BaseChallengeScene
{
	List<int> list1;
	List<int> list2;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private void DoPart1()
	{
		int sum = 0;

		for(var i = 0; i < list1.Count; i++){
			sum += Math.Abs(list1[i] - list2[i]);
		}

		resultsPanel.SetPart1Result(sum);
	}

	private void DoPart2()
	{
		int sum = 0;

		for(var i = 0; i < list1.Count; i++){
			var num = list2.FindAll((x) => x == list1[i]).Count;

			sum += list1[i] * num;
		}


		resultsPanel.SetPart2Result(sum);
	}

	private void ParseData(string[] data){
		list1 = new();
		list2 = new();

		foreach(var s in data){
			var parts = s.Split(" ", StringSplitOptions.RemoveEmptyEntries);
			GD.Print("Parts: " + parts);
			list1.Add(parts[0].ToInt());
			list2.Add(parts[1].ToInt());
		}
		list1.Sort();
		list2.Sort();
	}

}
