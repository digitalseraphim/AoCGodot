using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2023_Day18 : BaseChallengeScene
{
	List<Tuple<Direction, int, Direction, long>> instructions = new();
	Map<char> map = null;
	Pos start = null;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	private void DoPart1()
	{
		Dictionary<Direction, Dictionary<Direction, char>> continue_chars = new()
		{
			[Direction.UP] = new()
			{
				[Direction.LEFT] = '7',
				[Direction.RIGHT] = 'F',
				[Direction.UP] = '|'
			},
			[Direction.DOWN] = new()
			{
				[Direction.LEFT] = 'J',
				[Direction.RIGHT] = 'L',
				[Direction.DOWN] = '|'
			},
			[Direction.LEFT] = new()
			{
				[Direction.UP] = 'L',
				[Direction.DOWN] = 'F',
				[Direction.LEFT] = '-'
			},
			[Direction.RIGHT] = new()
			{
				[Direction.UP] = 'J',
				[Direction.DOWN] = '7',
				[Direction.RIGHT] = '-'
			}
		};

		Pos p = start;
		var last = instructions.Last();
		var last_replaced_char = '.';
		var totalDug = 0;
		foreach (var inst in instructions)
		{
			map.SetValueAt(p, continue_chars[last.Item1][inst.Item1]);
			for (int i = 0; i < inst.Item2; i++)
			{
				p = p.AfterMove(inst.Item1);
				last_replaced_char = map.SetValueAt(p, inst.Item1.IsHorizontal() ? '-' : '|');
			}
			last = inst;
			totalDug += inst.Item2;
		}

		map.SetValueAt(start, last_replaced_char);
		GD.Print(map);
		int count = Util.CountInternal(map) + totalDug;

		resultsPanel.SetPart1Result(count.ToString());
	}

	private void DoPart2()
	{
		long count = 0;
		long sweep = 0;

		if (instructions.First().Item3.IsVertical())
		{
			var last_inst = instructions.Last();
			instructions.RemoveAt(instructions.Count - 1);
			instructions.Insert(0, last_inst);
		}
		long path = 0;
		foreach (var insts in instructions.Chunk(2))
		{
			var inst1 = insts.First();
			var inst2 = insts.Last();

			var dir1 = inst1.Item3;
			var dir2 = inst2.Item3;
			var dist1 = inst1.Item4;
			var dist2 = inst2.Item4;

			if (dir1 == Direction.RIGHT)
			{
				sweep += dist1;
				if (dir2 == Direction.DOWN)
				{
					count += sweep * dist2;
				}
				else
				{
					count -= sweep * dist2;
				}
			}
			else
			{
				sweep -= dist1;
				if (dir2 == Direction.DOWN)
				{
					count += sweep * dist2;
				}
				else
				{
					count -= sweep * dist2;
				}
			}
			path += dist1 + dist2;
		}

		count += path/2;
		count += 1;
		resultsPanel.SetPart2Result(count.ToString());
	}

	private void DoPart2_2()
	{
		long count = 0;
		LPos p1 = new(0,0);

		long vertical_min = long.MaxValue;
		long vertical_max = long.MinValue;
		long horiz_min = long.MaxValue;
		long horiz_max = long.MinValue;

		// instructions.Add(instructions.First());

		for(int i = 0; i < instructions.Count; i++){
			var inst = instructions[i];
			Direction d = inst.Item3;
			long dist = inst.Item4;
			LPos p2 = p1.AfterMove(d, dist);
			count += p1.X*p2.Y - p2.X*p1.Y + dist;
			p1 = p2;

			vertical_min = Math.Min(vertical_min, p2.Y);
			vertical_max = Math.Max(vertical_max, p2.Y);
			horiz_min = Math.Min(horiz_min, p2.X);
			horiz_max = Math.Max(horiz_max, p2.X);
			GD.Print(p2);
		}
		count /= 2;

		count += 1;

		resultsPanel.SetPart2Result(count.ToString());
	}


	private void ParseData(string[] data)
	{
		RegEx regEx = RegEx.CreateFromString("(.) ([0-9]+) \\(#(.*)\\)");
		instructions.Clear();
		int vertical_min = int.MaxValue;
		int vertical_max = int.MinValue;
		int horiz_min = int.MaxValue;
		int horiz_max = int.MinValue;

		int vert_pos = 0;
		int horiz_pos = 0;
		Direction[] dirs = { Direction.RIGHT, Direction.DOWN, Direction.LEFT, Direction.UP };
		foreach (string s in data)
		{
			var m = regEx.Search(s);
			Direction d = null;
			int val = m.Strings[2].ToInt();

			switch (m.Strings[1][0])
			{
				case 'U':
					d = Direction.UP;
					vert_pos -= val;
					break;
				case 'D':
					d = Direction.DOWN;
					vert_pos += val;
					break;
				case 'L':
					d = Direction.LEFT;
					horiz_pos -= val;
					break;
				case 'R':
					d = Direction.RIGHT;
					horiz_pos += val;
					break;
			}

			Direction d2 = dirs[m.Strings[3].Last() - '0'];
			string hex = m.Strings[3];
			long val2 = long.Parse(new string(hex[0..(hex.Length - 1)]), System.Globalization.NumberStyles.HexNumber);

			var inst = new Tuple<Direction, int, Direction, long>(d, val, d2, val2);
			instructions.Add(inst);

			vertical_min = Math.Min(vertical_min, vert_pos);
			vertical_max = Math.Max(vertical_max, vert_pos);
			horiz_min = Math.Min(horiz_min, horiz_pos);
			horiz_max = Math.Max(horiz_max, horiz_pos);
		}

		map = new(vertical_max - vertical_min + 1, horiz_max - horiz_min + 1, '.');
		start = new(-horiz_min, -vertical_min);
	}

}
