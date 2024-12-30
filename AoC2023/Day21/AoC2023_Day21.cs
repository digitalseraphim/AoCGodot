using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace AoCGodot;
public partial class AoC2023_Day21 : BaseChallengeScene
{
	Map<char> Garden;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	private void DoPart1()
	{
		Pos start = null;

		foreach (Pos p in Garden)
		{
			if (Garden.ValueAt(p) == 'S')
			{
				start = p;
				break;
			}
		}

		HashSet<Pos> toProcess = new(){
			start
		};

		// Map<char> Map2 = new(Garden);
		// foreach(Pos p in toProcess){
		// 	Map2.SetValueAt(p, '!');
		// }
		// char c = '0';
		// foreach(Pos p in start.Neighbors){
		// 	Map2.SetValueAt(p, c++);
		// }
		// GD.Print(Map2);
		// GD.Print();

		for (int i = 0; i < 64; i++)
		{
			HashSet<Pos> next = new();

			foreach (Pos p in toProcess)
			{
				foreach (Pos n in p.Neighbors)
				{
					if (Garden.IsInMap(n) && Garden.ValueAt(n) != '#')
					{
						next.Add(n);
					}
				}
			}
			toProcess = next;
			// Map2 = new(Garden);
			// foreach(Pos p in toProcess){
			// 	Map2.SetValueAt(p, 'O');
			// }
			// GD.Print(Map2);
			// GD.Print();
		}


		resultsPanel.SetPart1Result(toProcess.Count);
	}

	private void DoPart2()
	{
		Pos start = null;

		foreach (Pos p in Garden)
		{
			if (Garden.ValueAt(p) == 'S')
			{
				start = p;
				break;
			}
		}

		HashSet<Pos> toProcess = new() { start };

		int iterations = 26501365;
		// int iterations = 1000;

		int mod = iterations % Garden.Width;
		int div = iterations / Garden.Width;

		List<long> seq = new();

		GD.Print($"mod {mod}, div {div}");

		for (int i = 0; i < mod + (Garden.Width*3); i++)
		{
			// GD.Print($"Iteration: {i}");

			HashSet<Pos> next = new();

			foreach (Pos p in toProcess)
			{
				foreach (Pos n in p.Neighbors)
				{
					if (Garden.ValueAt(n.Mod(Garden)) != '#')
					{
						next.Add(n);
					}
				}
			}
			toProcess = next;
			if((i+1)%Garden.Width == mod){
				GD.Print($"Iteration: {i+1} / {next.Count}");
				seq.Add(next.Count);
			}
		}

		for(int i = seq.Count; i <= div; i++){
			long[] diff = seq.Where((x,i)=>i>seq.Count-5).ToArray();
			// GD.Print(diff.Join(","));
			List<long> last_values = new(){
				seq.Last()
			};
			do{
				diff = diff.Zip(diff.Skip(1), (a,b)=>b-a).ToArray();
				// GD.Print(" " + diff.Join(","));
				last_values.Add(diff.Last());
			}while(!diff.All((a)=>a==0));
			seq.Add(last_values.Sum());
		}


		// GD.Print($" dists - {dists.Join(",")}");

		// Map<char> m = new(Garden);
		// foreach(var p in seen){
		// 	m.SetValueAt(p, 'S');
		// }

		// GD.Print(m);

		// foreach(var p in Garden){
		// 	GD.Print($"{Divs[p]} {Mods[p]}");
		// }
		// Dictionary<Pos, int> counts = new();
		// foreach(var p in seen){
		// 	counts[p.Mod(Garden)] = counts.GetValueOrDefault(p.Mod(Garden)) + 1;
		// }

		// foreach(var p in counts){
		// 	GD.Print($"{p.Key} => {p.Value}");
		// }

		resultsPanel.SetPart2Result(seq.Last());
	}

	private void ParseData(string[] data)
	{
		Garden = new(Util.ParseCharMap(data));
	}

}
