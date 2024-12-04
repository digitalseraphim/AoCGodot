using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace AoCGodot;
public partial class AoC2023_Day15 : BaseChallengeScene
{
	string[] instructions;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	private void DoPart1()
	{
		byte h = 0;
		long sum = 0;
		foreach (string s in instructions)
		{
			h = doHash(s);
			GD.Print(h);
			sum += h;
		}

		resultsPanel.SetPart1Result(sum.ToString());
	}

	private void DoPart2()
	{
		Dictionary<byte, Box> boxes = new();

		foreach (string s in instructions)
		{
			int l = s.Length;

			if (s[l - 1] == '-')
			{
				string label = s[0..(l - 1)];
				byte h = doHash(label);
				if (boxes.ContainsKey(h))
				{
					boxes[h].RemoveLens(label);
				}
			}
			else
			{
				string[] parts = s.Split("=");
				byte h = doHash(parts[0]);
				int fl = int.Parse(parts[1]);
				Lens lens = new(parts[0], fl);
				if (!boxes.ContainsKey(h))
				{
					boxes[h] = new();
				}
				boxes[h].AddLens(lens);
			}
		}

		long total = 0;
		for (int i = 0; i < 256; i++)
		{
			byte b = (byte)i;
			if (boxes.ContainsKey(b))
			{
				long power = boxes[b].Power(b);
				GD.Print(string.Format("Box {0} = {1}", b, power));
				GD.Print(boxes[b]);
				total += power;
			}
		}

		resultsPanel.SetPart2Result(total.ToString());
	}

	private void ParseData(string[] data)
	{
		instructions = data[0].Split(",");
	}

	byte doHash(string s)
	{
		byte val = 0;
		foreach (char c in s)
		{
			val += (byte)c;
			val *= 17;
			GD.Print(" ", val);
		}
		return val;
	}

	class Lens
	{
		public string Label;
		public int FocalLength;

		public Lens(string label, int focalLength)
		{
			Label = label;
			FocalLength = focalLength;
		}

		public override string ToString()
		{
			return string.Format("[{0} {1}]", Label, FocalLength);
		}

		public long Power(byte boxNum, int index)
		{
			long v = boxNum + 1;
			v *= index + 1;
			v *= FocalLength;
			return v;
		}
	}

	class Box
	{
		readonly Dictionary<string, Lens> storage = new();
		readonly List<string> order = new();

		public void AddLens(Lens l)
		{
			if (!order.Contains(l.Label))
			{
				order.Add(l.Label);
			}
			storage[l.Label] = l;
		}

		public void RemoveLens(string name)
		{
			if (order.Contains(name))
			{
				order.Remove(name);
				storage.Remove(name);
			}
		}

		public override string ToString()
		{
			List<string> s = new();
			foreach (string l in order)
			{
				s.Add(storage[l].ToString());
			}
			return s.ToArray().Join(" ");
		}

		public long Power(byte boxNum)
		{
			long v = 0;
			for (int i = 0; i < order.Count; i++)
			{
				v += storage[order[i]].Power(boxNum, i);
			}
			return v;
		}
	}

}
