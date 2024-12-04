using AoCGodot;
using Godot;
using System;
using System.Text.RegularExpressions;

namespace AoCGodot;
public partial class AoC2024Day3 : BaseChallengeScene
{
	string[] lines;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private void DoPart1()
	{
		int sum = 0;
		Regex r = new("mul\\(([0-9]{1,3}),([0-9]{1,3})\\)");

		foreach(var line in lines ){
			var matches = r.Matches(line);
			foreach(Match m in matches){
				var v1 = m.Groups[1].Value.ToInt();
				var v2 = m.Groups[2].Value.ToInt();
				GD.Print($"{v1} * {v2}");
				sum += v1*v2;
			}
		}
		resultsPanel.SetPart1Result(sum);
	}

	private void DoPart2()
	{
		int sum = 0;
		Regex r = new("mul\\(([0-9]{1,3}),([0-9]{1,3})\\)|don't\\(\\)|do\\(\\)");
		var doMul = true;

		foreach(var line in lines ){
			var matches = r.Matches(line);
			foreach(Match m in matches){
				if(m.Groups[0].Value == "do()"){
					doMul = true;
				}else if(m.Groups[0].Value == "don't()"){
					doMul = false;
				}else if(doMul){
					var v1 = m.Groups[1].Value.ToInt();
					var v2 = m.Groups[2].Value.ToInt();
					GD.Print($"{v1} * {v2}");
					sum += v1*v2;
				}else{
					GD.Print($"skipping {m.Groups[0].Value}");
				}
			}
		}
		resultsPanel.SetPart2Result(sum);
	}

	private void ParseData(string[] data){
		lines = data;
	}

}
