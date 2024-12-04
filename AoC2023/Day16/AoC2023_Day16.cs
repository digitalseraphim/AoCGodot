using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2023_Day16 : BaseChallengeScene
{
	char[][] map;


	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}



	private void DoPart1()
	{
		Beam lightBeam = new(new(0,0), Direction.RIGHT);

		int num_energized = TestLightBeam(lightBeam);

		resultsPanel.SetPart1Result(num_energized.ToString());
	}

	int TestLightBeam(Beam beam)
	{
		HashSet<string> visited = new();
		List<Beam> light_beams = new(){
			beam
		};
		int num_energized = 0;
		char[][] energized;
		int i;

		energized = new char[map.Length][];

		for (i = 0; i < map.Length; i++)
		{
			energized[i] = new char[map[i].Length];
			Array.Fill(energized[i], '.');
		}

		while (light_beams.Count > 0)
		{
			i++;
			var b = light_beams.First();
			light_beams.RemoveAt(0);
			if (visited.Contains(b.ToString()))
			{
				// GD.Print("Cache!");
				continue;
			}
			visited.Add(b.ToString());
			// GD.Print("visiting " + b);
			// GD.Print("  Backlog: ",  string.Join<Beam>(", ",light_beams));

			if (b.Position.X < 0
				|| b.Position.X >= map[0].Length
				|| b.Position.Y < 0
				|| b.Position.Y >= map.Length)
			{
				continue;
			}

			if (energized[b.Position.Y][b.Position.X] != '#')
			{
				energized[b.Position.Y][b.Position.X] = '#';
				num_energized++;
			}

			switch (map[b.Position.Y][b.Position.X])
			{
				case '.':
					{
						b.GoStraightInto(light_beams);
						break;
					}
				case '|':
					{
						if (b.Direction.IsHorizontal())
						{
							b.SplitInto(light_beams);
						}
						else
						{
							b.GoStraightInto(light_beams);
						}
						break;
					}
				case '-':
					{
						if (b.Direction.IsVertical())
						{
							b.SplitInto(light_beams);
						}
						else
						{
							b.GoStraightInto(light_beams);
						}
						break;
					}
				case '/':
					{
						if (b.Direction.IsHorizontal())
						{
							b.TurnCCWInto(light_beams);
						}
						else
						{
							b.TurnCWInto(light_beams);
						}
						break;
					}
				case '\\':
					{
						if (b.Direction.IsHorizontal())
						{
							b.TurnCWInto(light_beams);
						}
						else
						{
							b.TurnCCWInto(light_beams);
						}
						break;
					}
			}
			// PrintEnergized();
		}

		return num_energized;
	}

	void PrintEnergized(char[][] energized){
		foreach(char[] c in energized){
			GD.Print(new string(c));
		}
		GD.Print();
	}

	private void DoPart2()
	{
		int best = 0;

		for(int x = 0; x < map[0].Length; x++)
		{
			Beam lightBeam = new(new(x,0), Direction.DOWN);
			int num_energized = TestLightBeam(lightBeam);
			best = Math.Max(best, num_energized);

			lightBeam = new(new(x, map.Length-1), Direction.UP);
			num_energized = TestLightBeam(lightBeam);
			best = Math.Max(best, num_energized);
		}

		for(int y = 0; y < map.Length; y++)
		{
			Beam lightBeam = new(new(0,y), Direction.RIGHT);
			int num_energized = TestLightBeam(lightBeam);
			best = Math.Max(best, num_energized);

			lightBeam = new(new(map[0].Length-1, y), Direction.LEFT);
			num_energized = TestLightBeam(lightBeam);
			best = Math.Max(best, num_energized);
		}

		resultsPanel.SetPart2Result(best.ToString());
	}

	private void ParseData(string[] data)
	{
		map = new char[data.Length][];
		for (int i = 0; i < data.Length; i++)
		{
			map[i] = data[i].ToArray();
		}
	}

}
