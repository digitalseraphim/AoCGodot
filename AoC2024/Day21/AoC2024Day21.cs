using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day21 : BaseChallengeScene
{
	string[] codes;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	public readonly Map<char> numMap = new(Util.ParseCharMap(new string[] { "789", "456", "123", " 0A" }));
	public readonly Map<char> dirMap = new(Util.ParseCharMap(new string[] { " ^A", "<v>" }));

	readonly Dictionary<char, Pos> numKeypad = new(){
		{'7', new(0,0)},
		{'8', new(1,0)},
		{'9', new(2,0)},
		{'4', new(0,1)},
		{'5', new(1,1)},
		{'6', new(2,1)},
		{'1', new(0,2)},
		{'2', new(1,2)},
		{'3', new(2,2)},
		{'0', new(1,3)},
		{'A', new(2,3)},
	};

	readonly Dictionary<char, Pos> dirKeypad = new(){
		{'^', new(1,0)},
		{'A', new(2,0)},
		{'<', new(0,1)},
		{'v', new(1,1)},
		{'>', new(2,1)},
	};

	string PosToDirs(Pos start, Pos end, Dictionary<char, Pos> keypad)
	{
		int dx = end.X - start.X;
		int dy = end.Y - start.Y;
		string s = "";

		if (keypad == numKeypad)
		{
			if (start.X == 0 && end.Y == 3)
			{
				s += new string('>', dx);
				dx = 0;
			}
			else if (start.Y == 3 && end.X == 0)
			{
				s += new string('^', -dy);
				dy = 0;
			}
		}
		else
		{
			if (start.X == 0 && end.Y == 0)
			{
				s += new string('>', dx);
				dx = 0;
			}
			else if (start.Y == 0 && end.X == 0)
			{
				s += new string('v', dy);
				dy = 0;
			}
		}

		if (dx > 0)
		{
			return s + new string(dy < 0 ? '^' : 'v', Math.Abs(dy)) + new string('>', Math.Abs(dx)) + 'A';
		}
		if (dy < 0)
		{
			return s + new string(dx < 0 ? '<' : '>', Math.Abs(dx)) + new string('^', Math.Abs(dy)) + 'A';
		}
		return s + new string('<', Math.Abs(dx)) + new string('v', Math.Abs(dy)) + 'A';
	}

	string Translate(string input, Dictionary<char, Pos> keypad, Pos start = null, Dictionary<Tuple<string,Pos>, string> cache = null)
	{
		string s = "";
		start ??= keypad['A'];
		GD.Print($"Translate({input}, keypad, {start}, cache)");
		Tuple<string, Pos> t = new(input, start);

		if (cache != null && cache.ContainsKey(t))
		{
			GD.Print($"cache: {cache[t]}");
			return cache[t];
		}

		foreach (char c in input)
		{
			Pos p2 = keypad[c];
			s += PosToDirs(start, p2, keypad);
			start = p2;
		}
		if(cache != null){
			cache[t] = s;
		}

		GD.Print(s);
		return s;
	}

	Dictionary<Tuple<string,Pos>,string> strcache = new();

	long Translate2(string input, int level, int max, Dictionary<string, long>[] numcaches){
		GD.Print($"Translate2({input}, {level}, {max}, cache)");
		if(level == max){
			GD.Print($"(max) level {level} input /{input}/ count - {input.Length}");
			return input.Length;
		}

		if(numcaches[level].ContainsKey(input)){
			GD.Print($"(cache) level {level} input /{input}/ count - {numcaches[level][input]}");
			return numcaches[level][input];
		}

		long count = 0;
		Pos start = dirKeypad['A'];
		
		for(int i = 0; i < input.Length; i++){
			string s = input.Substr(i,1);
			string next = Translate(s, dirKeypad, start, strcache);
			long v = Translate2(next, level+1, max, numcaches);
			count += v;
			start = dirKeypad[s[0]];
		}
		GD.Print($"level {level} input /{input}/ count - {count}");
		numcaches[level][input] = count;
		return count;
	}

	public static string Reverse(string input, Map<char> keymap, Dictionary<char, Pos> keypad)
	{
		Pos pos = keypad['A'];
		string s = "";

		foreach (char c in input)
		{
			switch (c)
			{
				case 'A':
					s += keymap.ValueAt(pos);
					break;
				case '<':
					pos = pos.AfterMove(Direction.LEFT);
					break;
				case '>':
					pos = pos.AfterMove(Direction.RIGHT);
					break;
				case '^':
					pos = pos.AfterMove(Direction.UP);
					break;
				case 'v':
					pos = pos.AfterMove(Direction.DOWN);
					break;
			}

			if (keymap.ValueAt(pos) == ' ')
			{
				GD.Print("ERROR");
			}
		}

		return s;
	}

	private void DoPart1()
	{
		long count = 0;

		Dictionary<string, long>[] caches = Enumerable.Range(0, 26).Select(x => new Dictionary<string, long>()).ToArray();
		foreach (string c in codes)
		{
			int num = Int32.Parse(c.AsSpan(0, c.Length - 1));
			string dirs = Translate(c, numKeypad, null);
			long len = Translate2(dirs, 0, 2, caches);
			count += len * num;
			GD.Print($"len {len} num {num} code {c}");
		}

		resultsPanel.SetPart1Result(count);
	}

	private void DoPart2()
	{
		long count = 0;
		GD.Print("PART 2");

		Dictionary<string, long>[] caches = Enumerable.Range(0, 26).Select(x => new Dictionary<string, long>()).ToArray();
		foreach (string c in codes)
		{
			int num = Int32.Parse(c.AsSpan(0, c.Length - 1));
			string dirs = Translate(c, numKeypad);
			long len = Translate2(dirs, 0, 25, caches);
			count += len * num;
			GD.Print($"len {len} num {num} code {c}");
		}
		resultsPanel.SetPart2Result(count);
	}

	private void ParseData(string[] data)
	{
		codes = data;
		strcache = new();
	}

}
