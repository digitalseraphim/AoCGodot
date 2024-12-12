using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day10 : BaseChallengeScene
{
	Map<int> map;
	List<Pos> trailheads;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	
	HashSet<Pos> Check(Pos p, int val){
		HashSet<Pos> ret = new();
		foreach(Pos c in p.Neighbors.Where(x => map.IsInMap(x) && map.ValueAt(x) == val)){
			if(val == 9){
				ret.Add(c);
			}else{
				ret.UnionWith(Check(c, val+1));
			}
		}
		return ret;
	}

	int Check2(Pos p, int val){
		int count = 0;
		foreach(Pos c in p.Neighbors.Where(x => map.IsInMap(x) && map.ValueAt(x) == val)){
			if(val == 9){
				count++;
			}else{
				count += Check2(c, val+1);
			}
		}
		return count;
	}

	private void DoPart1()
	{
		int count = 0;

		count = trailheads.Aggregate(0, (v,p)=>v+Check(p,1).Count);

		// foreach(Pos head in trailheads){
		// 	HashSet<Pos> hs = Check(head, 1);
		// 	count += hs.Count;
		// }

		resultsPanel.SetPart1Result(count);
	}

	private void DoPart2()
	{
		int count = 0;

		count = trailheads.Aggregate(0, (v,p)=>v+Check2(p,1));

		// foreach(Pos head in trailheads){
		// 	count += Check2(head, 1);
		// }

		resultsPanel.SetPart2Result(count);
	}

	private void ParseData(string[] data){
		map = new(Util.ParseIntMap(data));
		trailheads = map.Where((pos)=>map.ValueAt(pos) == 0).ToList();
	}

}
