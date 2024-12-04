using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AoCGodot;
public partial class AoC2023_Day19 : BaseChallengeScene
{
	Dictionary<string, Workflow> Workflows = new();
	List<Part> Parts = new();

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private void DoPart1()
	{
		long total = 0;

		foreach (Part p in Parts)
		{
			string next = "in";

			while (next != "A" && next != "R")
			{
				next = Workflows[next].Test(p);
			}

			if (next == "A")
			{
				total += p.TotalRating;
			}
		}

		resultsPanel.SetPart1Result(total.ToString());
	}

	private void DoPart2()
	{
		long total = 0;

		List<Tuple<string, Part>> toProcess = new(){
			new("in", new())
		};

		while (toProcess.Count > 0)
		{
			Tuple<string, Part> tuple = toProcess.First();
			toProcess.RemoveAt(0);

			if (tuple.Item1 == "A")
			{
				total += tuple.Item2.NumPossibilities();
				continue;
			}
			else if (tuple.Item1 == "R")
			{
				continue;
			}

			Workflow wf = Workflows[tuple.Item1];
			Part p = tuple.Item2;

			foreach (Rule r in wf.Rules)
			{
				Tuple<Part, Part> newParts = p.WithUpdatedRankRange(r.Ranking, r.Comparison, r.Value);

				if (newParts.Item1.HasValidRanges())
				{
					toProcess.Add(new(r.Workflow, newParts.Item1));
				}
				if (newParts.Item2.HasValidRanges())
				{
					p = newParts.Item2;
				}
				else
				{
					p = null;
					break;
				}
			}

			if (p != null)
			{
				toProcess.Add(new(wf.Default, p));
			}
			continue;
		}

		resultsPanel.SetPart2Result(total.ToString());
	}

	private void ParseData(string[] data)
	{
		int i;
		Workflows.Clear();
		Parts.Clear();
		for (i = 0; i < data.Length && data[i].Length > 0; i++)
		{
			Workflow wf = new Workflow(data[i]);
			Workflows.Add(wf.Name, wf);
		}
		i++;
		for (; i < data.Length; i++)
		{
			Parts.Add(new Part(data[i]));
		}
	}

	class Workflow
	{
		public string Name { get; }
		public readonly List<Rule> Rules = new();
		public string Default { get; }

		public Workflow(string data)
		{
			string[] parts = data.Split(new char[] { '{', '}', ',' }, StringSplitOptions.RemoveEmptyEntries);
			Name = parts[0];
			Default = parts.Last();
			foreach (string s in parts[1..(parts.Length - 1)])
			{
				Rules.Add(new Rule(s));
			}
		}

		public string Test(Part p)
		{
			foreach (Rule r in Rules)
			{
				string next = r.Test(p);
				if (next != null)
				{
					return next;
				}
			}

			return Default;
		}
	}

	class Rule
	{
		private Predicate<Part> Predicate { get; }
		public string Workflow { get; }
		public string Ranking { get; }
		public string Comparison { get; }
		public long Value { get; }

		readonly RegEx RuleRegEx = RegEx.CreateFromString("(.*)([<>])(.*)");

		public Rule(string data)
		{
			string[] parts = data.Split(':');
			var m = RuleRegEx.Search(parts[0]);

			Ranking = m.Strings[1];
			Comparison = m.Strings[2];
			Value = long.Parse(m.Strings[3]);

			if (Comparison == ">")
			{
				Predicate = GT;
			}
			else
			{
				Predicate = LT;
			}

			Workflow = parts[1];
		}

		public bool GT(Part p)
		{
			return p[Ranking] > Value;
		}

		public bool LT(Part p)
		{
			return p[Ranking] < Value;
		}

		public string Test(Part p)
		{
			if (Predicate(p))
			{
				return Workflow;
			}
			return null;
		}
	}

	class Part
	{
		private readonly Dictionary<string, long> Values = new();

		private readonly Dictionary<string, RankingRange> Ranges = new()
		{
			["x"] = new(),
			["m"] = new(),
			["a"] = new(),
			["s"] = new()
		};

		public Part()
		{
		}

		public Part(string data)
		{
			foreach (string valstring in data[1..(data.Length - 1)].Split(","))
			{
				string[] parts = valstring.Split("=");
				Values[parts[0]] = long.Parse(parts[1]);
			}
		}

		public Part(Part other)
		{
			foreach (var kvp in other.Values)
			{
				Values[kvp.Key] = kvp.Value;
			}
			foreach (var kvp in other.Ranges)
			{
				Ranges[kvp.Key] = kvp.Value;
			}
		}

		public long this[string s]
		{
			get
			{
				return Values[s];
			}
		}

		public long TotalRating
		{
			get
			{
				return Values.Values.Sum();
			}
		}

		public Tuple<Part, Part> WithUpdatedRankRange(string rank, string comp, long val)
		{
			Part passPart = new(this);
			Part failPart = new(this);
			Tuple<RankingRange, RankingRange> newRanges;

			if (comp == ">")
			{
				newRanges = Ranges[rank].GT(val);
			}
			else
			{
				newRanges = Ranges[rank].LT(val);
			}

			passPart.Ranges[rank] = newRanges.Item1;
			failPart.Ranges[rank] = newRanges.Item2;

			return new(passPart, failPart);
		}

		public bool HasValidRanges()
		{
			return Ranges.All((r) => { return r.Value.Max >= r.Value.Min; });
		}

		public long NumPossibilities()
		{
			return Ranges.Aggregate(1L, (t, kvp) => { return t * (kvp.Value.Max - kvp.Value.Min + 1); });
		}

		public override string ToString()
		{
			string s = "{" + Ranges["x"] + ", " + Ranges["m"] + ", " + Ranges["a"] + ", " + Ranges["s"] + "}";

			return s;
		}
	}

	class RankingRange
	{
		public long Min
		{
			get; set;
		}
		public long Max
		{
			get; set;
		}

		public RankingRange() : this(1, 4000)
		{
		}

		private RankingRange(long min, long max)
		{
			Min = min;
			Max = max;
		}

		public Tuple<RankingRange, RankingRange> GT(long val)
		{
			long newMin = Math.Max(Min, val + 1);
			long newMax = Math.Min(Max, val);
			return new(new(newMin, Max), new(Min, newMax));
		}

		public Tuple<RankingRange, RankingRange> LT(long val)
		{
			long newMax = Math.Min(Max, val - 1);
			long newMin = Math.Max(Min, val);
			return new(new(Min, newMax), new(newMin, Max));
		}

		public override string ToString()
		{
			return "[" + Min + "," + Max + "]";
		}
	}
}
