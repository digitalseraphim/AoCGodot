using AoCGodot;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Dynamic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2023_Day6 : BaseChallengeScene
{
	HashSet<Race> races = new();
	Race singleRace = null;
	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private void DoPart1()
	{
		Int64 total = 1;

		foreach(Race r in races){
			Int64 i = 0;
			while((i < r.Time) && ((r.Time-i)*i) <= r.Distance){
				GD.Print("i ", i, " ", (r.Time - i), " " , (r.Time-i)*i, " ", r.Distance);
				i++;
			}
			Int64 j = r.Time;
			while((j > 0) && (((r.Time-j)*j) <= r.Distance)){
				GD.Print("j ", j, " ", (r.Time - j), " " , (r.Time-j)*j, " ", r.Distance);
				j--;
			}

			GD.Print("i = " + i + " j = " + j);
			total *= j-i+1;
		}

		resultsPanel.SetPart1Result(total.ToString());
	}

	private void DoPart2()
	{
        static long calc(long total, long t) => (total - t) * t;

		HashSet<BSRange> ranges = new();
		ranges.Add(new(0, singleRace.Time));
		Int64 highTime = 0;

		while(ranges.Count > 0){
			BSRange rr = ranges.First();
			GD.Print("rr - " + rr);
			if(calc(singleRace.Time, rr.Mid) > singleRace.Distance){
				highTime = rr.Mid;
				break;
			}
			ranges.Add(rr.LowerHalf);
			ranges.Add(rr.UpperHalf);

		}

        BSRange r = new(0, highTime);

		while(r.Upper != r.Lower){
			Int64 d = calc(singleRace.Time, r.Mid);
			if(d > singleRace.Distance){
				r = r.LowerHalf;
			}else if(d <= singleRace.Distance){
				r = r.UpperHalf;
			}
			GD.Print("r = " + r);
		}

		BSRange r2 = new(highTime, singleRace.Time);
		while(r2.Upper != r2.Lower){
			Int64 d = calc(singleRace.Time, r2.Mid);
			if(d < singleRace.Distance){
				r2 = r2.LowerHalf;
			}else if(d > singleRace.Distance){
				r2 = r2.UpperHalf;
			}
			GD.Print("r2 = " + r2);
		}

		Int64 total = r2.Lower - r.Lower ;
		resultsPanel.SetPart2Result(total.ToString());
	}

	class BSRange{
		public Int64 Lower{get;set;}
		public Int64 Upper{get;set;}
		public BSRange(Int64 lower, Int64 upper){
			Lower=lower;
			Upper=upper;
		}

		public Int64 Mid{
			get {
				return Lower + ((Upper - Lower)/2);
			}
		}

		public BSRange UpperHalf{
			get{
				return new(Mid+1, Upper);
			}
		}

		public BSRange LowerHalf{
			get{
				return new(Lower, Mid);
			}
		}

        public override string ToString()
        {
            return string.Format("BSR - {0} - {1}", Lower, Upper);
        }
    }

	private void ParseData(string[] data){
		races.Clear();
		Array<RegExMatch> time_matches = RegEx.CreateFromString("\\d+").SearchAll(data[0]);
		Array<RegExMatch> dist_matches = RegEx.CreateFromString("\\d+").SearchAll(data[1]);
		string longtime = "";
		string longdist = "";

		for(int i = 0; i < time_matches.Count; i++){
			Int64 time = Int64.Parse(time_matches.ElementAt(i).Strings[0]);
			Int64 dist = Int64.Parse(dist_matches.ElementAt(i).Strings[0]);
			longtime += time_matches.ElementAt(i).Strings[0];
			longdist += dist_matches.ElementAt(i).Strings[0];
			races.Add(new(time, dist));
		}

		singleRace = new(Int64.Parse(longtime), Int64.Parse(longdist));
	}

	class Race : Tuple<Int64, Int64>
	{
		public Race(Int64 time, Int64 distance) : base(time, distance)
		{
		}

		public Int64 Time
		{
			get { return Item1; }
		}

		public Int64 Distance
		{
			get { return Item2; }
		}
	}
}
