using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace AoCGodot;
public partial class AoC2023_Day11 : BaseChallengeScene
{
	Universe u = null;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	private void DoPart1()
	{
		int total = 0;
		for (int i = 0; i < u.galaxies.Count - 1; i++)
		{
			for (int j = i + 1; j < u.galaxies.Count; j++)
			{
				total += u.galaxies.ElementAt(i).Distance(u.galaxies.ElementAt(j), u, 1);
			}
		}

		resultsPanel.SetPart1Result(total.ToString());
	}

	private void DoPart2()
	{
		long total = 0;
		for (int i = 0; i < u.galaxies.Count - 1; i++)
		{
			for (int j = i + 1; j < u.galaxies.Count; j++)
			{
				total += u.galaxies.ElementAt(i).Distance(u.galaxies.ElementAt(j), u, 999999);
			}
		}

		resultsPanel.SetPart2Result(total.ToString());
	}

	private void ParseData(string[] data)
	{
		u = new(data);
	}

	class Universe
	{
		public HashSet<Loc> galaxies = new();

		public HashSet<int> emptyRows = new();
		public HashSet<int> emptyCols = new();

		public Universe(string[] data)
		{
			for (int c = 0; c < data[0].Length; c++)
			{
				emptyCols.Add(c);
			}
			for (int r = 0; r < data.Length; r++)
			{
				int c = data[r].IndexOf("#");
				if (c == -1)
				{
					emptyRows.Add(r);
				}
				else
				{
					for (; c < data[0].Length; c++)
					{
						if (data[r][c] == '#')
						{
							galaxies.Add(new(r, c));
							emptyCols.Remove(c);
						}
					}
				}
			}
		}
	}

	class Loc
	{
		int Row
		{
			get;
		}
		int Col
		{
			get;
		}

		public Loc(int row, int col)
		{
			Row = row;
			Col = col;
		}

		public int Distance(Loc other, Universe u, int expansion = 1)
		{
			int top_row = Math.Min(Row, other.Row);
			int bot_row = Math.Max(Row, other.Row);
			int dr = bot_row - top_row;

			int left_col = Math.Min(Col, other.Col);
			int right_col = Math.Max(Col, other.Col);

			int dc = right_col - left_col;

			int er = u.emptyRows.Where((x) => x > top_row && x < bot_row).Count();
			int ec = u.emptyCols.Where((x) => x > left_col && x < right_col).Count();

			return dr + dc + ((er + ec) * expansion);
		}

	}

}
