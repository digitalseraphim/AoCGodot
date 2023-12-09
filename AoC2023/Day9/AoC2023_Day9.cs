using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AoCGodot;
public partial class AoC2023_Day9 : BaseChallengeScene
{
	List<List<int>> sequences = new();

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	private void DoPart1()
	{
		int total = 0;
		int total2 = 0;

		foreach(var seq in sequences){
			int i = -1;
			int[] diff = seq.ToArray();
			List<int> last_values = new(){
				seq.Last()
			};
			List<int> first_values = new(){
				seq.First()
			};
			do{
				diff = diff.Zip(diff.Skip(1), (a,b)=>b-a).ToArray();
				last_values.Add(diff.Last());
				first_values.Add(i*diff.First());
				i*=-1;
			}while(!diff.All((a)=>a==0));
			total += last_values.Sum();
			total2 += first_values.Sum();

			GD.Print(first_values.Aggregate(new List<String>(), (sl, j)=>{sl.Add(j.ToString());return sl;}).ToArray().Join("/"));
		}

		resultsPanel.SetPart1Result(total.ToString());
		resultsPanel.SetPart2Result(total2.ToString());
	}

	private void DoPart2()
	{

	}

	private void ParseData(string[] data){
		sequences.Clear();
		foreach(var s in data){
			sequences.Add(s.Split(" ").Aggregate(new List<int>(), (l, s)=>{l.Add(s.ToInt()); return l;}));
		}
	}

}
