using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day11 : BaseChallengeScene
{
	List<long> stones;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private List<long> ProcessStone(List<long> incoming, long stone)
	{
		string s = stone.ToString();

		if (stone == 0)
		{
			incoming.Add(1);
		}
		else if (s.Length % 2 == 0)
		{
			incoming.AddRange(s.Chunk(s.Length / 2).Select(x => Int64.Parse(new string(x))));
		}
		else
		{
			incoming.Add(stone * 2024);
		}

		return incoming;
	}


	private void DoPart1()
	{
		List<long> myStones = stones;

		for (long i = 0; i < 25; i++)
		{
			myStones = myStones.Aggregate(new List<long>(), ProcessStone);
		}

		resultsPanel.SetPart1Result(myStones.Count);
	}

	class CountCache
	{
		readonly Dictionary<long, List<long>> cache = new();

		public long GetCount(long stone, int remaining)
		{
			if(cache.ContainsKey(stone)){
				return cache[stone][remaining-1];
			}
			return 0;
		}

		public void SetCount(long stone, int remaining, long count)
		{
			cache.GetOrCreate(stone, ()=>Enumerable.Repeat<long>(0, 75).ToList())[remaining-1] = count;
		}
	}

	readonly CountCache countCache = new();

	long ProcessStone2(long stone, int remaining)
	{
		List<long> children = new();
		string s = stone.ToString();
		long count = 0;
		long cachedCount;

		if(remaining == 0){
			return 1;
		}

		cachedCount = countCache.GetCount(stone, remaining);

		if(cachedCount > 0)
		{
			return cachedCount;
		}
		else if (stone == 0)
		{
			children.Add(1);
		}
		else if (s.Length % 2 == 0)
		{
			children.AddRange(s.Chunk(s.Length / 2).Select(x => Int64.Parse(new string(x))));
		}
		else
		{
			children.Add(stone * 2024);
		}

		foreach (var child in children)
		{
			count += ProcessStone2(child, remaining - 1);
		}

		countCache.SetCount(stone, remaining, count);

		return count;
	}


	private void DoPart2()
	{
		long count = 0;
		int NumBlinks = 75;

		foreach(long stone in stones){
			count += ProcessStone2(stone, NumBlinks);
		}

		resultsPanel.SetPart2Result(count);
	}

	private void ParseData(string[] data)
	{
		stones = data[0].Split(" ").Select(Int64.Parse).ToList();
	}

}
