using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoCGodot;
public partial class AoC2024Day13 : BaseChallengeScene
{
	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	List<PrizeMachine> prizeMachines;

	private void DoPart1()
	{
		int tokens = 0;

		foreach (var machine in prizeMachines)
		{
			var result = machine.Solve1();
			if(result != null){
				tokens += result.Item1*3 + result.Item2;
			}
		}

		resultsPanel.SetPart1Result(tokens);
	}

	private void DoPart2()
	{
		long tokens = 0;

		foreach (var machine in prizeMachines)
		{
			var result = machine.Solve2();
			if(result != null){
				tokens += result.Item1*3 + result.Item2;
			}
		}

		resultsPanel.SetPart2Result(tokens);
	}

	class PrizeMachine
	{
		public int AX, AY;
		public int BX, BY;
		public int PX, PY;

		readonly Regex rButton = new("Button (A|B): X\\+([0-9]+), Y\\+([0-9]+)");
		readonly Regex rPrize = new("Prize: X=([0-9]+), Y=([0-9]+)");

		public PrizeMachine(string[] data)
		{
			Match m = rButton.Match(data[0]);
			AX = m.Groups[2].Value.ToInt();
			AY = m.Groups[3].Value.ToInt();

			m = rButton.Match(data[1]);
			BX = m.Groups[2].Value.ToInt();
			BY = m.Groups[3].Value.ToInt();

			m = rPrize.Match(data[2]);
			PX = m.Groups[1].Value.ToInt();
			PY = m.Groups[2].Value.ToInt();
		}

		public Tuple<int, int> Solve1()
		{
			int b = ((PY * AX) - (PX * AY)) / ((AX * BY) - (AY * BX));
			int a = (PX - (b * BX)) / AX;

			if(a*AX + b*BX == PX && a*AY + b*BY == PY){
				return new(a,b);
			}

			return null;
		}

		public Tuple<long, long> Solve2()
		{
			long PX2 = 10000000000000 + PX;
			long PY2 = 10000000000000 + PY;

			long b = ((PY2 * AX) - (PX2 * AY)) / ((AX * BY) - (AY * BX));
			long a = (PX2 - (b * BX)) / AX;

			if(a*AX + b*BX == PX2 && a*AY + b*BY == PY2){
				return new(a,b);
			}

			return null;
		}
	}

	private void ParseData(string[] data)
	{
		prizeMachines = data.Chunk(4).Select((x) => new PrizeMachine(x)).ToList();
	}

}
