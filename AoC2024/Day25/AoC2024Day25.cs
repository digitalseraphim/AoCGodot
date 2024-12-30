using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day25 : BaseChallengeScene
{
	class Key
	{
		public int[] heights;

		public Key(string[] data)
		{
			Map<char> m = new(Util.ParseCharMap(data));
			heights = new int[5];
			Pos p = m.LL;

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 6; j++)
				{
					if (m.ValueAt(p.AfterMove(Direction.UP, j + 1)) == '.')
					{
						heights[i] = j;
						break;
					}
				}
				p = p.AfterMove(Direction.RIGHT);
			}
		}
	}

	class Lock
	{
		public int[] heights;

		public Lock(string[] data)
		{
			Map<char> m = new(Util.ParseCharMap(data));
			heights = new int[5];
			Pos p = m.UL;

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 6; j++)
				{
					if (m.ValueAt(p.AfterMove(Direction.DOWN, j + 1)) == '.')
					{
						heights[i] = j;
						break;
					}
				}
				p = p.AfterMove(Direction.RIGHT);
			}
		}
	}

	List<Key> Keys;
	List<Lock> Locks;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private void DoPart1()
	{
		int count = 0;
		foreach (var v in Keys.Product(Locks))
		{
			bool fits = true;
			for (int i = 0; i < 5; i++)
			{
				if (v.Item1.heights[i] + v.Item2.heights[i] > 5)
				{
					fits = false;
					break;
				}
			}
			if (fits)
			{
				count++;
			}
		}

		resultsPanel.SetPart1Result(count);
	}

	private void DoPart2()
	{
		resultsPanel.SetPart2Result("result");
	}

	private void ParseData(string[] data)
	{
		List<string[]> chunks = data.SplitOnBlanks();
		Keys = new();
		Locks = new();

		foreach (var c in chunks)
		{
			if (c[0][0] == '#')
			{
				Locks.Add(new(c));
				GD.Print($"Lock {Locks.Last().heights.Join(",")}");
			}
			else
			{
				Keys.Add(new(c));
				GD.Print($"Key {Keys.Last().heights.Join(",")}");
			}
		}
	}

}
