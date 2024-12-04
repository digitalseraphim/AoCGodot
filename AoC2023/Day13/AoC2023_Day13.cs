using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace AoCGodot;
public partial class AoC2023_Day13 : BaseChallengeScene
{
	List<List<string>> areas = new();
	List<List<string>> tareas = new();
	List<int> h_m_lines = new();
	List<int> v_m_lines = new();
	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	private void DoPart1()
	{
		int total = 0;
		for (int i = 0; i < areas.Count; i++)
		{
			List<string> area = areas[i];
			int v1 = checkHorizontal(area);
			List<string> tarea = tareas[i];
			int v2 = 100 * checkHorizontal(tarea);
			h_m_lines.Add(v1);
			v_m_lines.Add(v2);
			int v =v1+v2;
			if(v == 0){
				GD.Print("got zero for:");
				GD.Print(area.ToArray().Join("\n"));
			}else{
				GD.Print(v);
			}
			total += v;
		}
		resultsPanel.SetPart1Result(total.ToString());
	}

	int checkHorizontal(List<string> area)
	{
		HashSet<int> possib = new();
		for (int i = 1; i < area[0].Length; i++)
		{
			possib.Add(i);
		}

		foreach (string line in area)
		{
			int len = line.Length;
			string rev = new string(line.Reverse().ToArray());

			foreach (int i in possib)
			{
				string s1 = line[Math.Max(0, 2 * i - len)..i];
				string s2 = rev[Math.Max(0, len - 2 * i)..(len - i)];

				if (s1 != s2)
				{
					possib.Remove(i);
				}
			}
			if (possib.Count == 0)
			{
				return 0;
			}
		}

		return possib.First();
	}

	int stringDist(string s1, string s2){
		int diff = 0;
		for(int i = 0; i < s1.Length; i++){
			if(s1[i] != s2[i]){
				diff++;
			}
		}
		return diff;
	}

	int checkHorizontalDist(List<string> area)
	{
		HashSet<int> possib = new();
		for (int i = 1; i < area[0].Length; i++)
		{
			possib.Add(i);
		}

		Dictionary<int, int> tracker = new();

		foreach (string line in area)
		{
			int len = line.Length;
			string rev = new string(line.Reverse().ToArray());

			foreach(int i in possib)
			{
				string s1 = line[Math.Max(0, 2 * i - len)..i];
				string s2 = rev[Math.Max(0, len - 2 * i)..(len - i)];
				int dist = stringDist(s1,s2);

				tracker[i] = tracker.GetValueOrDefault(i,0) + dist;
				if(tracker[i] > 1){
					possib.Remove(i);
				}
			}
			if (possib.Count == 0)
			{
				return 0;
			}
		}

		int ret = 0;
		foreach(int i in possib){
			GD.Print("i = ", i, " v = ", tracker[i]);
			if(tracker[i] == 1){
				ret = i;
			}
		}

		return ret;
	}

	private void DoPart2()
	{
		int total = 0;
		for (int i = 0; i < areas.Count; i++)
		{
			List<string> area = areas[i];
			int v1 = checkHorizontalDist(area);
			List<string> tarea = tareas[i];
			int v2 = 100 * checkHorizontalDist(tarea);
			h_m_lines.Add(v1);
			v_m_lines.Add(v2);
			int v =v1+v2;
			if(v == 0){
				GD.Print("got zero for:");
				GD.Print(area.ToArray().Join("\n"));
			}else{
				GD.Print(v);
			}
			total += v;
		}
		resultsPanel.SetPart2Result(total.ToString());
	}

	private void ParseData(string[] data)
	{
		areas.Clear();
		tareas.Clear();

		List<string> l = new();
		foreach (string line in data)
		{
			if (line.Length == 0)
			{
				areas.Add(l);
				l = new();
			}
			else
			{
				l.Add(line);
			}
		}

		if (l.Count > 0)
		{
			areas.Add(l);
		}

		foreach (List<String> area in areas)
		{
			List<List<char>> transposed = new();
			for (int i = 0; i < area.First().Length; i++)
			{
				transposed.Add(new());
			}
			for (int i = 0; i < area.Count; i++)
			{
				string line = area[i];
				for (int j = 0; j < line.Length; j++)
				{
					transposed[j].Add(line[j]);
				}
			}
			var t = transposed.Aggregate(new List<string>(), (a, l) => { a.Add(new string(l.ToArray())); return a; });
			tareas.Add(t);
		}

	}

}
