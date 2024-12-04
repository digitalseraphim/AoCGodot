using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
		int value = 1;

		foreach (Pos p in Garden)
		{
			if (Garden.ValueAt(p) == 'S')
			{
				start = p;
				break;
			}
		}


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

		Dictionary<string, int> seen = new();
		Dictionary<Pos, int> numCopies = new();

		HashSet<Pos> toProcess = new()
		{
			start
		};

		// int iterations = 26501365;
		int iterations = 100;
		for (int i = 0; i < iterations; i++)
		{
			HashSet<Pos> next = new();
			foreach (Pos p in toProcess)
			{
				foreach (Pos n in p.Neighbors)
				{
					Pos n2 = n;
					if (!Garden.IsInMap(n))
					{
						n2 = new(Util.Mod(n.X, Garden.Width), Util.Mod(n.Y, Garden.Height));
					}

					if (Garden.ValueAt(n2) != '#')
					{
						next.Add(n);
					}
				}
			}
			toProcess = next;
			if (value < 0)
			{
				Map<char> Map2 = new(Garden);
				foreach (Pos p in toProcess)
				{
					Map2.SetValueAt(p, 'O');
				}
				// GD.Print(Map2);
				// GD.Print();
				string asString = Map2.ToString();
				if (seen.ContainsKey(asString))
				{
					int first = seen[asString];
					int loop_size = i - first;
					int extra = 26501365 % loop_size;
					GD.Print("i = ", i);
					GD.Print("first = ", first);
					GD.Print("loop size = ", loop_size);
					GD.Print("rest = ", extra);
					int num_loops = 26501365 / loop_size;
					num_loops -= 10;
					i += num_loops * loop_size;
					GD.Print("i = ", i);
					value = 1;
				}
				else
				{
					seen[asString] = i;
				}
			}
			GD.Print(i, " ", toProcess.Count);
		}

		resultsPanel.SetPart2Result(toProcess.Count);
	}

	private void ParseData(string[] data)
	{
		Garden = new(Util.ParseCharMap(data));
	}

}
