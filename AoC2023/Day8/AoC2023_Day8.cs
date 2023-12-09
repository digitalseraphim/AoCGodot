using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AoCGodot;
public partial class AoC2023_Day8 : BaseChallengeScene
{
	string instructions;
	Dictionary<string, Dictionary<char, string>> nodes = new();
	HashSet<string> ANodes = new();
	HashSet<string> ZNodes = new();

	public override void DoRun(string[] data)
	{
		ParseData(data);
		// DoPart1();
		// DoPart2();
		DoPart2_2();
	}
	private void DoPart1()
	{
		int steps = 0;

		string node = "AAA";

		while (node != "ZZZ")
		{
			node = nodes[node][instructions[steps % instructions.Length]];
			steps++;
		}

		resultsPanel.SetPart1Result(steps.ToString());
	}

	private void DoPart2()
	{
		IEnumerable<string> curNodes = new HashSet<string>(ANodes);
		long steps = 0;
		while (!curNodes.All(ZNodes.Contains))
		{
			curNodes = curNodes.Aggregate(new HashSet<string>(),
				(hs, n) =>
				{
					hs.Add(nodes[n][instructions[(int)(steps % instructions.Length)]]);
					return hs;
				});
			GD.Print(curNodes.ToArray().Join("/"));
			Thread.Sleep(10);
			steps++;
		}
		resultsPanel.SetPart1Result(steps.ToString());
	}
	private void DoPart2_2()
	{
		Dictionary<string, long> loopsizes = new();
		foreach (string s in ANodes)
		{
			long steps = 0;
			int endinz = 0;
			long zat = 0;
			string node = s;
			string znode = "";
			long loopsize = 0;
			Dictionary<string, long> visited = new();
			do
			{
				int istep = (int)(steps % instructions.Length);
				steps++;

				char inst = instructions[istep];
				string newNode = nodes[node][inst];
				string newNode2 = newNode + istep.ToString();
				// GD.Print(s, " ", newNode, " ", steps);
				if (visited.ContainsKey(newNode2))
				{
					// GD.Print(s, " ", newNode, " ", visited[newNode2], " ", steps);
					loopsize = steps - visited[newNode2];
					loopsizes.Add(s,loopsize);
					break;
				}
				else
				{
					visited.Add(newNode2, steps);
				}
				if (newNode.EndsWith("Z"))
				{
					endinz++;
					zat = steps;
					znode = newNode;
				}
				node = newNode;
				// Thread.Sleep(100);
			} while (!node.EndsWith("A"));
			GD.Print("loopsize = ", loopsize, "  endinz = ", endinz, " zat = ", zat, " ", znode, " ", nodes[znode].Values.ToArray().Join("/"));

		}



		// long max = 0;
		// bool notequal = true;

		// foreach(var v in loopsizes.Values){
		// 	v.Multiply(8745927);
		// }

		// while(notequal){
		// 	notequal = false;
		// 	foreach(var node in loopsizes.Keys){
		// 		LoopSize elem = loopsizes[node];
		// 		if(elem.Zat > max){
		// 			max = elem.Zat;
		// 			notequal = true;
		// 			GD.Print("max ", max);
		// 		}else if(elem.Zat < max){
		// 			// while(elem.Zat < max){
		// 			//elem.Increase();
		// 			long diff = (max - elem.Zat)/elem.Loopsize;
		// 			GD.Print("diff ", diff);
		// 			elem.Zat += (diff+1)*elem.Loopsize;
		// 			// }
		// 			notequal = true;
		// 			if(elem.Zat > max){
		// 				max = elem.Zat;
		// 				GD.Print("max ", max);
		// 			}
		// 		}
		// 	}
		// 	Thread.Sleep(10);
		// }

		long max = LCM(loopsizes.Values.ToArray());

		resultsPanel.SetPart2Result(max.ToString());
	}
	RegEx NodeRegEx = RegEx.CreateFromString("(.*) = \\((.*), (.*)\\)");

	static long LCM(long[] numbers)
	{
		return numbers.Aggregate(lcm);
	}
	static long lcm(long a, long b)
	{
		return Math.Abs(a * b) / GCD(a, b);
	}
	static long GCD(long a, long b)
	{
		return b == 0 ? a : GCD(b, a % b);
	}

	private void ParseData(string[] data)
	{
		instructions = data[0];


		nodes.Clear();
		ANodes.Clear();
		ZNodes.Clear();

		for (int i = 2; i < data.Length; i++)
		{
			RegExMatch m = NodeRegEx.Search(data[i]);
			nodes.Add(m.Strings[1], new()
			{
				['L'] = m.Strings[2],
				['R'] = m.Strings[3]
			});
			if (m.Strings[1].EndsWith('A'))
			{
				ANodes.Add(m.Strings[1]);
			}
			if (m.Strings[1].EndsWith('Z'))
			{
				ZNodes.Add(m.Strings[1]);
			}

		}
	}

	class LoopSize{
		public long Loopsize{
			get;set;
		}
		public long Zat{
			get;set;
		}

		public LoopSize(long loopsize, long zat){
			Loopsize = loopsize;
			Zat = zat;
		}

		public void Increase(){
			Zat += Loopsize;
		}

		public void Multiply(long v){
			Zat += v*Loopsize;
		}
	}

}
