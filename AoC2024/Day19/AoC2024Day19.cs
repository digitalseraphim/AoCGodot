using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day19 : BaseChallengeScene
{
	List<string> towels;
	List<string> patterns;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private void DoPart1()
	{
		int count = 0;

		foreach (string pat in patterns)
		{
			// GD.Print(pat);
			var sublist = towels.Where(pat.Contains).ToList();
			// GD.Print(sublist.Join(" "));
			List<int> values = new();

			while (true)
			{
				var v = values.Select(i => sublist[i]).ToList().Join("");
				if (v == pat)
				{
					count++;
					break;
				}
				if (pat.StartsWith(v) && v.Length < pat.Length)
				{
					values.Add(0);
					continue;
				}
				else
				{
					int vv = 0;
					bool done = false;
					do
					{
						if (!values.Any())
						{
							done = true;
							break;
						}
						vv = values.Last() + 1;
						values.RemoveAt(values.Count - 1);
					} while (vv == sublist.Count);

					if (done)
					{
						break;
					}

					values.Add(vv);
				}
			}
		}

		GD.Print("done");
		resultsPanel.SetPart1Result(count);
	}


	static long Check(string pat, string prefix, List<string> options, Dictionary<string, long> cache)
	{
		long count = 0;
		if(cache.ContainsKey(prefix)){
			return cache[prefix];
		}
		foreach (string s in options)
		{
			var v = prefix + s;
			if (pat == v)
			{
				count++;
			}
			else if (pat.StartsWith(v))
			{
				count += Check(pat, v, options, cache);
			}
		}
		cache[prefix] = count;
		return count;
	}

	private void DoPart2()
	{
		long count = 0;

		foreach (string pat in patterns)
		{
			var sublist = towels.Where(pat.Contains).ToList();
			Dictionary<string, long> cache = new();
			count += Check(pat, "", sublist, cache);

		}

		GD.Print("done");
		resultsPanel.SetPart2Result(count);
	}

	private void ParseData(string[] data)
	{
		towels = data[0].Split(",", StringSplitOptions.TrimEntries).ToList();
		patterns = data.ToList().GetRange(2, data.Length - 2);
	}

}
