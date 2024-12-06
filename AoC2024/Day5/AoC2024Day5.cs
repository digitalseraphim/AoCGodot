using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AoCGodot;
public partial class AoC2024Day5 : BaseChallengeScene
{
	Dictionary<int,List<int>> Rules;
	List<List<int>> Updates;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		int sum = 0;
		int sum2 = 0;
		foreach(var update in Updates){
			if(Check(update)){
				sum += update[(update.Count-1)/2];
			}else{
				sum2 += Fix(update);
			}
		}
		resultsPanel.SetPart1Result(sum);
		resultsPanel.SetPart2Result(sum2);
	}

	bool Check(List<int> update){
		for(int i1 = 0; i1 < update.Count; i1++){
			var v1 = update[i1];
			if(Rules.ContainsKey(v1)){
				var Rule = Rules[update[i1]];
				foreach (var v2 in Rule){
					var idx2 = update.IndexOf(v2);
					if(idx2 == -1){
						continue;
					}
					if(idx2 < i1){
						return false;
					}
				}
			}
		}
		return true;
	}

	int Fix(List<int> update){
		var didFix = true;
		while(didFix){
			didFix = false;
			for(int i1 = 0; i1 < update.Count; i1++){
				var v1 = update[i1];
				if(Rules.ContainsKey(v1)){
					var Rule = Rules[update[i1]];
					foreach (var v2 in Rule){
						var idx2 = update.IndexOf(v2);
						if(idx2 == -1){
							continue;
						}
						if(idx2 < i1){
							didFix = true;
							GD.Print($"moving {v1} from {i1} to {idx2} before {v2}");
							update.Remove(v1);
							update.Insert(idx2, v1);
						}
					}
				}
			}
		}
		GD.Print($"returning {update[(update.Count-1)/2]} from {String.Join(",", update)}");
		return update[(update.Count-1)/2];
	}


	private void ParseData(string[] data){
		int i = 0;
		Rules = new();
		for(; i < data.Length; i++){
			if(data[i] == ""){
				break;
			}
			var parts = data[i].Split("|");
			var p1 = parts[0].ToInt();
			var p2 = parts[1].ToInt();

			Rules.GetOrCreate(p1, () => new()).Add(p2);
		}

		i++;

		Updates = new();
		for(; i < data.Length; i++){
			var parts = data[i].Split(",", StringSplitOptions.RemoveEmptyEntries);
			Updates.Add(parts.Select(Int32.Parse).ToList());
		}
	}

}
