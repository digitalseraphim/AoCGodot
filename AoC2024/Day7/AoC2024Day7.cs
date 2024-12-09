using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoCGodot;
public partial class AoC2024Day7 : BaseChallengeScene
{
	class TestData: List<KeyValuePair<long, List<long>>>{}

	TestData tests;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	bool Check2(long result, List<long> values, bool check_concat){
		long v = values.First();
		var rest = values.GetRange(1, values.Count-1);

		GD.Print($"Check2 - {result}, {v}, {rest.Join(",")}");
		if(rest.Any()){
			// more to process, check mult first
			if(result % v == 0){
				GD.Print("Mult");
				if(Check2(result/v, rest, check_concat)){
					GD.Print("1 return true");
					return true;
				}
			}
			// if that fails, check add
			GD.Print("Add");
			if(Check2(result - v, rest, check_concat)){
				GD.Print("3 return true");
				return true;
			}
			// if that fails check concat
			var res_string = result.ToString();
			var v_string = v.ToString();

			if(check_concat && result != v && res_string.EndsWith(v_string)){
				var s = res_string.Substr(0, res_string.Length - v_string.Length);
				long newresult = Int64.Parse(s);
				GD.Print("Concat");
				return Check2(newresult, rest, check_concat);
			}
		}

		// return false if any values left, otherwise return true if result matches the first value
		return !rest.Any() && result == v;
	}



	private void DoPart1()
	{
		BigInteger sum = 0;

		foreach(var test in tests){
			var test_result = test.Key;
			var test_values = new List<long>(test.Value);
			test_values.Reverse();
			if(Check2(test_result, test_values, false)){
				sum += test_result;
			}
		}

		resultsPanel.SetPart1Result(sum.ToString());
	}

	private void DoPart2()
	{
		BigInteger sum= 0;

		foreach(var test in tests){
			var test_result = test.Key;
			var test_values = new List<long>(test.Value);
			test_values.Reverse();
			if(Check2(test_result, test_values, true)){
				sum += test_result;
			}
		}

		resultsPanel.SetPart2Result(sum.ToString());
	}

	private void ParseData(string[] data){
		tests = new();

		foreach(var s in data){
			var parts = new Queue<string>(s.Split(new char[]{':',' '}, 
							StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries));
			var value = Int64.Parse(parts.Dequeue());
			var rest = parts.Select(Int64.Parse).ToList();
			tests.Add(new(value, rest));
		}
	}

}
