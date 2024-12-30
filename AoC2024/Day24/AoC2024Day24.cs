using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day24 : BaseChallengeScene
{
	Dictionary<string, int> Values;
	Queue<Tuple<string, string, string, string>> Gates;
	Dictionary<string, Tuple<string, string, string>> OutputGates;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
		DoPart2old();
	}

	static long RunGates(Queue<Tuple<string, string, string, string>> gates, Dictionary<string, int> values)
	{

		while (gates.Any())
		{
			var g = gates.Dequeue();

			if (values.ContainsKey(g.Item1) && values.ContainsKey(g.Item3))
			{
				var v1 = values[g.Item1];
				var v2 = values[g.Item3];
				switch (g.Item2)
				{
					case "AND":
						values[g.Item4] = v1 & v2;
						break;
					case "OR":
						values[g.Item4] = v1 | v2;
						break;
					case "XOR":
						values[g.Item4] = v1 ^ v2;
						break;
				}
			}
			else
			{
				gates.Enqueue(g);
			}
		}

		long val = 0;
		for (int i = 0; i < 64; i++)
		{
			var s = $"z{i:D2}";
			if (values.ContainsKey(s))
			{
				val |= (long)values[s] << i;
			}
			else
			{
				break;
			}
		}

		return val;
	}

	private void DoPart1()
	{
		long val = RunGates(new(Gates), new(Values));

		resultsPanel.SetPart1Result(val);
	}

	private bool AreEquiv(Tuple<string, string, string> a, Tuple<string, string, string> b)
	{
		if (a == null || b == null)
		{
			return false;
		}

		if (a.Equals(b))
		{
			return true;
		}

		if (a.Item2 != b.Item2)
		{
			return false;
		}

		if ((a.Item1 == b.Item1 && a.Item3 == b.Item3) ||
		   (a.Item1 == b.Item3 && a.Item3 == b.Item1))
		{
			return true;
		}

		var ea1 = OutputGates.GetValueOrDefault(a.Item1);
		var ea3 = OutputGates.GetValueOrDefault(a.Item3);
		var eb1 = OutputGates.GetValueOrDefault(b.Item1);
		var eb3 = OutputGates.GetValueOrDefault(b.Item3);

		return ((a.Item1 == b.Item1 || AreEquiv(ea1, eb1)) &&
				(a.Item3 == b.Item3 || AreEquiv(ea3, eb3))) ||
			   ((a.Item1 == b.Item3 || AreEquiv(ea1, eb3)) &&
				(a.Item3 == b.Item1 || AreEquiv(ea3, eb1)));
	}

	HashSet<string> cantfind = new();

	string FindGate(Tuple<string, string, string> v)
	{
		string ret = null;
		foreach (var g in OutputGates)
		{
			if (g.Value.Item2 != v.Item2)
			{
				continue;
			}
			if ((g.Value.Item1 == v.Item1 && g.Value.Item3 == v.Item3) ||
				(g.Value.Item1 == v.Item3 && g.Value.Item3 == v.Item1))
			{
				if (ret != null)
				{
					GD.Print($"{ret}, {g.Key}");
				}
				ret = g.Key;
			}
		}
		if (ret == null)
		{
			GD.Print($"Couldn't find {v}");
			if (v.Item1 != null && v.Item3 != null && (v.Item1 != v.Item3))
			{

			}
		}
		return ret;
	}

	private void SwapGates(string g1, string g2)
	{
		var tmp = OutputGates[g1];
		OutputGates[g1] = OutputGates[g2];
		OutputGates[g2] = tmp;

	}

	private void DoPart2()
	{
		for (int i = 2; i < 45; i++)
		{
			var zn1 = $"z{i - 1:D2}";
			var zn2 = $"z{i:D2}";

			if (!OutputGates.ContainsKey(zn2))
			{
				break;
			}

			GD.Print($"{zn1} => {zn2}");

			var t1 = OutputGates[zn1];

			// A xor B => ((A AND B) OR (x-1 AND y-1)) XOR (x0 XOR y0)
			//            (C         OR D)             XOR  E
			//            F XOR E


			var C = FindGate(new(t1.Item1, "AND", t1.Item3));
			var D = FindGate(new($"x{i - 1:D2}", "AND", $"y{i - 1:D2}"));
			var F = FindGate(new(C, "OR", D));
			var E = FindGate(new($"x{i:D2}", "XOR", $"y{i:D2}"));
			var newZ = FindGate(new(F, "XOR", E));

			if (newZ != null && zn2 != null && newZ != zn2)
			{
				GD.Print($"   {newZ} != {zn2}");
				cantfind.Add(newZ);
				cantfind.Add(zn2);
				SwapGates(newZ, zn2);
			}
			else if (newZ == null)
			{
				var t2 = OutputGates[zn2];

				if (t2.Item1 == F && t2.Item3 != E)
				{
					cantfind.Add(t2.Item3);
					cantfind.Add(E);
					SwapGates(t2.Item3, E);
				}
				else if (t2.Item1 == E && t2.Item3 != F)
				{
					cantfind.Add(t2.Item3);
					cantfind.Add(F);
					SwapGates(t2.Item3, F);
				}
				else if (t2.Item3 == F && t2.Item1 != E)
				{
					cantfind.Add(t2.Item1);
					cantfind.Add(E);
					SwapGates(t2.Item1, E);
				}
				else if (t2.Item3 == E && t2.Item1 != F)
				{
					cantfind.Add(t2.Item1);
					cantfind.Add(F);
					SwapGates(t2.Item1, F);
				}
				else
				{
					GD.Print("Don't know why");
				}
			}

		}


		var l = cantfind.ToList();
		l.Sort();

		GD.Print(l.Join(","));
		resultsPanel.SetPart2Result(l.Join(","));
	}


	private void DoPart2old()
	{
		int x = 0;
		int y = 0;

		for (; x < 64; x++)
		{
			if (!Values.ContainsKey($"x{x:D2}"))
			{
				break;
			}
		}

		for (; y < 64; y++)
		{
			if (!Values.ContainsKey($"y{y:D2}"))
			{
				break;
			}
		}

		GD.Print($"x = {x} y = {y}");

		for (int r = 0; r < x; r++)
		{
			Dictionary<string, int> values = new();
			Queue<Tuple<string, string, string, string>> gates = new(Gates);

			for (int xx = 0; xx < x; xx++)
			{
				values[$"x{xx:D2}"] = (xx == r) ? 1 : 0;
			}
			for (int yy = 0; yy < y; yy++)
			{
				values[$"y{yy:D2}"] = 0;
			}

			long result = RunGates(gates, values);

			if (result != (1L << r))
			{
				GD.Print($"x{r:D2} {result} != {1L << r}");
			}
		}

		/*
		for (int r = 0; r < y; r++)
		{
			Dictionary<string, int> values = new();
			Queue<Tuple<string, string, string, string>> gates = new(Gates);

			for (int xx = 0; xx < x; xx++)
			{
				values[$"x{xx:D2}"] = 0;
			}
			for (int yy = 0; yy < y; yy++)
			{
				values[$"y{yy:D2}"] = (yy == r) ? 1 : 0;
			}

			long result = RunGates(gates, values);

			if (result != (1L << r))
			{
				GD.Print($"y{r:D2} {result} != {1L << r}");
			}
		}


		resultsPanel.SetPart2Result("result");
		*/
	}

	private void ParseData(string[] data)
	{
		Values = new();
		Gates = new();
		OutputGates = new();

		int i;

		for (i = 0; i < data.Length; i++)
		{
			if (data[i] == "")
			{
				break;
			}
			var v = data[i].Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
			Values[v[0]] = Int32.Parse(v[1]);
		}

		i++;

		for (; i < data.Length; i++)
		{
			var v = data[i].Split(new char[] { ' ', '-', '>' }, StringSplitOptions.RemoveEmptyEntries);
			Gates.Enqueue(new(v[0], v[1], v[2], v[3]));
			OutputGates[v[3]] = new(v[0], v[1], v[2]);
		}

		for (int k = 0; k < 7; k++)
		{
			var zn = $"z{k:D2}";
			if (!OutputGates.ContainsKey(zn))
			{
				break;
			}
			var z = OutputGates[zn];
			List<string> s = new() { z.Item1, z.Item2, z.Item3 };
			HashSet<string> ops = new() { "AND", "OR", "XOR", "(", ")" };

			GD.Print($"{zn} - {s.Join(" ")}");

			for (int j = 0; j < s.Count;)
			{
				if (s[j].StartsWith("x") || s[j].StartsWith("y") || s[j].StartsWith("z") || ops.Contains(s[j]))
				{
					j++;
					continue;
				}
				var v = OutputGates[s[j]];
				s.RemoveAt(j);
				s.InsertRange(j, new List<string>() { "(", v.Item1, v.Item2, v.Item3, ")" });
				j++;
				GD.Print($"{zn} - {s.Join(" ")}");
			}

			GD.Print($"{zn} - {s.Join(" ")}");
		}
	}

}
