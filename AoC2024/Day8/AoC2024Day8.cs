using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day8 : BaseChallengeScene
{
	Map<char> map;
	Dictionary<char, List<Pos>> antennas;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private void DoPart1()
	{
		HashSet<Pos> antinodes = new();

		foreach(var pair in antennas){
			List<Pos> positions = pair.Value;
			foreach(var v in positions.Permutations()){
				if(v.Item1 == v.Item2){
					continue;
				}
				GD.Print($"{pair.Key} - {v.Item1} {v.Item2}");
				Pos an = new(2*v.Item1.X - v.Item2.X, 2*v.Item1.Y - v.Item2.Y);
				GD.Print(an);
				if(map.IsInMap(an)){
					antinodes.Add(an);
				}
			}
		}

		resultsPanel.SetPart1Result(antinodes.Count);
	}

	private void DoPart2()
	{
		HashSet<Pos> antinodes = new();

		foreach(var pair in antennas){
			List<Pos> positions = pair.Value;
			foreach(var v in positions.Permutations()){
				if(v.Item1 == v.Item2){
					antinodes.Add(v.Item1);
					continue;
				}
				GD.Print($"{pair.Key} - {v.Item1} {v.Item2}");
				int dx = v.Item2.X - v.Item1.X;
				int dy = v.Item2.Y - v.Item1.Y;

				int gcd = Math.Abs((int)Util.GCD(dx, dy));

				dx /= gcd;
				dy /= gcd;

				for(int i = 0; ; i++){
					Pos an = new(v.Item1.X - dx*i, v.Item1.Y - dy*i);
					GD.Print(an);
					if(map.IsInMap(an)){
						antinodes.Add(an);
						if(map.ValueAt(an) == '.'){
							map.SetValueAt(an, '#');
						}
					}else{
						break;
					}
				}
			}
		}

		GD.Print(map);

		resultsPanel.SetPart2Result(antinodes.Count);
	}

	private void ParseData(string[] data){
		map = new(Util.ParseCharMap(data));
		antennas = map.Where((x) => map.ValueAt(x) != '.')
					  .GroupBy(x=>map.ValueAt(x))
					  .ToDictionary(x=>x.Key, x=>x.ToList());
	}

}
