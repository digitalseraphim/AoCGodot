using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day23 : BaseChallengeScene
{
	Dictionary<string, HashSet<string>> connections;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	class HS : HashSet<string>
	{
		public override int GetHashCode()
		{
			return this.ToImmutableSortedSet().Join(",").GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.GetHashCode() == obj.GetHashCode();
		}
	}

	private void DoPart1()
	{
		HashSet<HashSet<string>> matches = new(HashSet<string>.CreateSetComparer());

		foreach (var t in connections.Keys.Where((s) => s.StartsWith('t')))
		{
			foreach (var p in connections[t].DifferentCombinations(2))
			{
				var f = p.First();
				var l = p.Last();
				if (connections.ContainsKey(f) && connections[f].Contains(l))
				{
					matches.Add(new() { t, f, l });
				}
			}
		}

		foreach (var m in matches)
		{
			GD.Print($"{m.Join(",")} - {m.GetHashCode()}");
		}

		resultsPanel.SetPart1Result(matches.Count);
	}

	private void DoPart2()
	{
		HashSet<HashSet<string>> cliques = new(HashSet<string>.CreateSetComparer());

		BronKerbosch1(new(), new(connections.Keys), new(), cliques);

		HashSet<string> biggestClique = null;
		foreach(var hs in cliques){
			if (biggestClique == null || hs.Count > biggestClique.Count)
			{
				biggestClique = hs;
			}
		}

		GD.Print($"Biggest - {biggestClique.Count} {biggestClique.Join(",")}");

		resultsPanel.SetPart2Result(biggestClique.ToImmutableSortedSet().Join(","));
	}

	void BronKerbosch1(HashSet<string> R,
					   HashSet<string> P,
					   HashSet<string> X,
					   HashSet<HashSet<string>> cliques)
	{
		if (!P.Any() && !X.Any())
		{
			cliques.Add(R);
			return;
		}
		foreach (var v in P.ToList())
		{
			HashSet<string> newP = new(P.Intersect(connections[v]));
			HashSet<string> newX = new(X.Intersect(connections[v]));
			BronKerbosch1(new HashSet<string>(R){v}, newP, newX, cliques);
        	P.Remove(v);
			X.Add(v);
		}
	}

	private void ParseData(string[] data)
	{
		connections = new();

		foreach (var d in data)
		{
			var s = d.Split("-");
			connections.GetOrCreate(s[0], () => new()).Add(s[1]);
			connections.GetOrCreate(s[1], () => new()).Add(s[0]);
		}
	}

}
