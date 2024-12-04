using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;

namespace AoCGodot;
public partial class AoC2023_Day12 : BaseChallengeScene
{
	List<Tuple<string, List<int>>> springs = new();

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		GD.Print();
		DoPart2();
	}

	// int[] num_partitions = { 1, 1, 2, 3, 5, 7, 11, 15, 22, 30, 42, 56, 77, 101, 135, 176, 231, 297, 385, 490, 627, 792,
	// 						1002, 1255, 1575, 1958, 2436, 3010, 3718, 4565, 5604 };

	// private void DoPart1()
	// {
	// 	int total = 0;
	// 	foreach (var r in springs)
	// 	{
	// 		var map = r.Item1;
	// 		var counts = r.Item2;

	// 		int p = countPossibilities(map[map.IndexOfAny("?#".ToArray())..], counts);

	// 		total += p;
	// 	}


	// 	resultsPanel.SetPart1Result(total.ToString());

	// }

	// private int countPossibilities(string map, IEnumerable<int> counts, int recur = 0)
	// {
	// 	////GD.Print(recur, " countPossibilities - ", map, " ", counts.ToArray().Join("/"));
	// 	if (counts.Count() == 0)
	// 	{
	// 		////GD.Print(recur, " no counts");
	// 		if(map.Contains('#')){
	// 			////GD.Print(recur, " would skip required, ret 0");
	// 			return 0;
	// 		}
	// 		////GD.Print(recur, " ret 1");
	// 		return 1;
	// 	}
	// 	int first_count = counts.First();
	// 	string regex_str = "^([?#]{" + first_count + "})(.?)";
	// 	RegEx r = RegEx.CreateFromString(regex_str);
	// 	int count_total = counts.Sum();
	// 	int min_len = count_total + (counts.Count() - 1);
	// 	int count = 0;
	// 	int end = map.Length - min_len;
	// 	int first_hash = map.IndexOf("#");
	// 	if(first_hash >= 0){
	// 		end = Math.Min(first_hash, end);
	// 	}
	// 	////GD.Print(recur, " min_len ", min_len, " first_hash ", first_hash, " end ", end);

	// 	for (int i = 0; i <= end; i++)
	// 	{
	// 		////GD.Print(recur, " check ", map[i..], " ", regex_str);
	// 		RegExMatch m = r.Search(map[i..]);
	// 		if (m != null)
	// 		{
	// 			int next_start = i + first_count + 1;
	// 			////GD.Print(recur, " strs ", m.Strings.Join("/"));
	// 			if(m.Strings[2] == "#"){
	// 				//this match requires skipping a required match after
	// 				////GD.Print(recur, " would skip #");
	// 				continue;
	// 			}

	// 			while (next_start < map.Length && map[next_start] == '.')
	// 			{
	// 				next_start++;
	// 			}

	// 			if (next_start >= map.Length)
	// 			{
	// 				count++;
	// 			}
	// 			else
	// 			{
	// 				count += countPossibilities(map[next_start..], counts.Skip(1), recur + 1);
	// 			}
	// 		}
	// 		else
	// 		{
	// 			////GD.Print(recur, " null");
	// 		}
	// 	}
	// 	////GD.Print(recur, " returning ", count);
	// 	return count;
	// }

	const char black = '#';
	const char white = '.';
	const string black_or_unknown = "#?";
	const string white_or_unknown = ".?";


	// private long S(int i, int j, string map, int[] d)
	// {
	// 	GD.Print("S(", i, ", ", j, ")");

	// 	if (i < 0 || j < 0)
	// 	{
	// 		GD.Print("ret2 - ", 0);
	// 		return 0;
	// 	}
	// 	if (i == 0 || j == 0)
	// 	{
	// 		if (j > 0)
	// 		{
	// 			GD.Print("ret1a - 0");
	// 			return 0;
	// 		}
	// 		GD.Print("ret1b - 1");
	// 		return 1;
	// 	}


	// 	// if (j < d.Length - 1)
	// 	// {
	// 	// 	//GD.Print(map);
	// 	// 	//GD.Print(String.Format("{0," + i + d[j + 1] + 2 + "}", "^"));
	// 	// }
	// 	// if ((j < d.Length - 1) && (map[i + d[j + 1] + 1] == '#'))
	// 	// {
	// 	// 	return 0;
	// 	// }


	// 	/* debugs */
	// 	if (white_or_unknown.Contains(map[i - 1]))
	// 	{
	// 		GD.Print("  S(", i - 1, ",", j, ")");
	// 	}
	// 	if (black_or_unknown.Contains(map[i - 1]))
	// 	{
	// 		int a = i;
	// 		int b = d[j - 1];
	// 		int c = (j > 1) ? 1 : 0;

	// 		//GD.Print("a - b - c ", a, " - ", b, " - ", c, " = ", a - b - c);
	// 		//GD.Print("  S(", a, " - ", b, " - ", c, ",", j - 1, ")");
	// 		GD.Print("  S(", a - b - c, ",", j - 1, ")");
	// 	}

	// 	/* Actual calculation */
	// 	int ret = 0;

	// 	if (white_or_unknown.Contains(map[i - 1]))
	// 	{
	// 		int v = S(i - 1, j, map, d);
	// 		ret += v;
	// 	}
	// 	if (black_or_unknown.Contains(map[i - 1]))
	// 	{
	// 		int a = i;
	// 		int b = d[j - 1];
	// 		int c = (j > 1) ? 1 : 0;

	// 		if (b > a)
	// 		{
	// 			GD.Print("S(", i, ", ", j, ") b>a - 0");
	// 			return 0;
	// 		}
	// 		GD.Print("b = ", b, " txt = ", new string(map[(a - b)..(a)]));
	// 		if (c == 1)
	// 		{
	// 			GD.Print("   gap = ", map[a - b - c]);
	// 		}
	// 		// if the range we are skipping contains white, or the potential white space we need is already black, fail
	// 		if (map[(a - b)..a].Contains(white) || (c == 1 && map[a - b - 1] == black))
	// 		{
	// 			GD.Print("S(", i, ", ", j, ") ret2 - ", ret);
	// 			return ret;
	// 		}

	// 		//GD.Print("a - b - c ", a, " - ", b, " - ", c, " = ", a - b - c);
	// 		int v = S(a - b - c, j - 1, map, d);

	// 		ret += v;
	// 	}
	// 	GD.Print("S(", i, ", ", j, ") ", "ret - ", ret);
	// 	return ret;
	// }

	[Export]
	bool debug = false;
	[Export]
	bool usecache = true;

	Dictionary<Tuple<int, int>, long> cache = new();

	private long S2(int i, int j, string map, int[] d)
	{
		long ret = 0;
		Tuple<int, int> t = new(i, j);
		if (debug)
		{
			GD.Print("S(", i, ", ", j, ") ", map[0..i], " ", d[0..j].Join(","));
		}

		if (usecache && cache.ContainsKey(t))
		{
			long v = cache[t];
			if (debug)
			{
				GD.Print("  cached value - ", v);
			}
			return v;
		}

		// Base cases:
		// if i or j are zero or lower, the array accesses will fail.
		// i == 0 or lower means there's no input left, so if there are any
		// sizes left, fail
		if (i <= 0 && j > 0)
		{
			if (debug)
			{
				GD.Print("  Early fail, i <= 0, j > 0");
			}
			cache[t] = 0;
			return 0;
		}

		// if j == 0, there are no sizes left, but there is room left,
		// fail if there are any spots that need to be black
		if (j == 0)
		{
			if (map[0..i].Contains(black))
			{
				// there is at least one black, fail
				if (debug)
				{
					GD.Print("  Early fail, j > 0, black in range");
				}
				cache[t] = 0;
				return 0;
			}
			if (debug)
			{
				GD.Print("  Base state, return 1");
			}
			cache[t] = 1;
			return 1;
		}

		if (d[0..j].Sum() + j - 1 > i)
		{
			// minimum length needed isn't available, so fail
			if (debug)
			{
				GD.Print("  min length fail ", d[0..j].Sum() + j - 1, " > ", i);
			}
			cache[t] = 0;
			return 0;
		}


		if (debug)
		{
			if (white_or_unknown.Contains(map[i - 1]))
			{
				GD.Print(" S(", i - 1, ",", j, ")");
			}

			if (black_or_unknown.Contains(map[i - 1]))
			{
				int gap_val = (j > 1) ? 1 : 0;

				GD.Print(" S(", i - d[j - 1] - gap_val, ",", j - 1, ")");
			}
		}

		if (white_or_unknown.Contains(map[i - 1]))
		{
			//if map[i-1] is white or unknown, count how many if white
			ret += S2(i - 1, j, map, d);
		}

		if (black_or_unknown.Contains(map[i - 1]))
		{
			// if map[i-1] is black or unknown, count how many if black
			// this means the map[i-1] must be part of d[j-1]
			int gap_val = (j > 1) ? 1 : 0;

			if (map[(i - d[j - 1])..i].Contains(white))
			{
				// if one of the values we skip is white, fail out
				if (debug)
				{
					GD.Print("  S(", i, ", ", j, ") black, try to skip white");
				}
			}
			else if (j > 1 && map[i - d[j - 1] - 1] == black)
			{
				// the gap needed is black, fail
				if (debug)
				{
					GD.Print("  S(", i, ", ", j, ") black, gap val black");
				}
			}
			else
			{
				// d[j] can fit here, so calculate how many options at j-1
				ret += S2(i - d[j - 1] - gap_val, j - 1, map, d);
			}
		}

		if (debug)
		{
			GD.Print("  S(", i, ", ", j, ") return ", ret);
		}
		cache[t] = ret;
		return ret;
	}


	private void DoPart1()
	{
		long total = 0;

		foreach (var r in springs)
		{
			var map = r.Item1;
			var counts = r.Item2;
			GD.Print("map = ", map);
			GD.Print("counts = ", counts.ToArray().Join(", "));
			var v = S2(map.Length, counts.Count, map, counts.ToArray());
			GD.Print("v = ", v);
			total += v;
		}

		resultsPanel.SetPart1Result(total.ToString());
	}


	private void DoPart2()
	{
		long total = 0;
		foreach (var r in springs)
		{
			var map = r.Item1;
			var counts = r.Item2;

			List<int> new_counts = new();
			new_counts.AddRange(counts);

			string new_map = map;

			for (int i = 1; i < 5; i++)
			{
				new_map += "?" + map;
				new_counts.AddRange(counts);
			}
			GD.Print("new_map = ", new_map);
			GD.Print("new_counts = ", new_counts.ToArray().Join(", "));
			cache.Clear();
			long p = S2(new_map.Length, new_counts.Count, new_map, new_counts.ToArray());
			GD.Print("p = ", p);
			total += p;
		}
		resultsPanel.SetPart2Result(total.ToString());
	}

	private void ParseData(string[] data)
	{
		springs.Clear();
		cache.Clear();
		foreach (string s in data)
		{
			string[] splt = s.Split(" ");
			// deduplicate '.'s
			string map = splt[0];//.Split(".", StringSplitOptions.RemoveEmptyEntries).Join(".");
			List<int> counts = splt[1].Split(",").Aggregate(new List<int>(), (l, v) => { l.Add(v.ToInt()); return l; });
			springs.Add(new(map, counts));
		}
	}

}
