using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day22 : BaseChallengeScene
{
	List<long> startingSecrets;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	
	static long Mix(long secret, long val){
		return secret ^ val;
	}

	static long Prune(long secret){
		return Util.Mod(secret, 16777216);
	}

	private void DoPart1()
	{
		long val = 0;

		foreach(long secret in startingSecrets){
			long s = secret;
			for(int i =0; i < 2000; i++){
				s = Prune(Mix(s, s*64));
				s = Prune(Mix(s, s/32));
				s = Prune(Mix(s, s*2048));
			}
			GD.Print(s);
			val += s;
		}

		resultsPanel.SetPart1Result(val);
	}

    class PriceChanges : Tuple<int, int, int, int>
    {
        public PriceChanges(int item1, int item2, int item3, int item4) : base(item1, item2, item3, item4)
        {
        }

		public PriceChanges Next(int item){
			return new(Item2, Item3, Item4, item);
		}
    }

	class Prices : Tuple<int, int, int, int, int>
    {
        public Prices(int item1, int item2, int item3, int item4, int item5) :
			 base(item1, item2, item3, item4, item5)
        {
        }

		public Prices Next(int item){
			return new(Item2, Item3, Item4, Item5, item);
		}

		public PriceChanges Changes(){
			return new(Item2-Item1, Item3-Item2, Item4-Item3, Item5-Item4);
		}
    }

    private void DoPart2()
	{
		Dictionary<PriceChanges, int> totals = new();
		foreach(long secret in startingSecrets){
			HashSet<PriceChanges> seen = new();
			Prices prices = new(0,0,0,0,0);
			
			long s = secret;
			for(int i = 0; i < 2000; i++){
				s = Prune(Mix(s, s*64));
				s = Prune(Mix(s, s/32));
				s = Prune(Mix(s, s*2048));

				prices = prices.Next((int)Util.Mod(s,10));
				if(i >= 4){
					PriceChanges pc = prices.Changes();
					if(seen.Contains(pc)){
						continue;
					}
					seen.Add(pc);
					totals[pc] = totals.GetValueOrDefault(pc, 0) + prices.Item5;
				}
			}
		}

		var m = totals.MaxBy((kvp)=>kvp.Value);

		GD.Print(m.Key);

		resultsPanel.SetPart2Result(m.Value);
	}

	private void ParseData(string[] data){
		startingSecrets = data.ToList().Select(Int64.Parse).ToList();
	}

}
