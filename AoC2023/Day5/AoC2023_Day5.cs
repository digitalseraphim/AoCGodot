using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace AoCGodot;
public partial class AoC2023_Day5 : BaseChallengeScene
{
    List<Int64> seeds = new();

    Func<Int64, Int64> seedToSoil = null;

    Func<Int64, Int64> soilToFertilizer = null;

    Func<Int64, Int64> fertilizerToWater = null;

    Func<Int64, Int64> waterToLight =null;

    Func<Int64, Int64> lightToTemperature = null;

    Func<Int64, Int64> temperatureToHumidity = null;

    Func<Int64, Int64> humidityToLocation = null;
    public override void DoRun(string[] data)
    {
        ParseData(data);
        DoPart1();
        DoPart2();
    }

    private void DoPart1()
    {
        Int64 minLoc = Int32.MaxValue;

        foreach (Int64 seed in seeds)
        {
            GD.Print("calling seedToSoil ", seed);
            Int64 v = seedToSoil(seed);
            GD.Print("calling soilToFertilizer ", v);
            v = soilToFertilizer(v);
            GD.Print("calling fertilizerToWater ", v);
            v = fertilizerToWater(v);
            GD.Print("calling waterToLight ", v);
            v = waterToLight(v);
            GD.Print("calling lightToTemperature ", v);
            v = lightToTemperature(v);
            GD.Print("calling temperatureToHumidity ", v);
            v = temperatureToHumidity(v);
            GD.Print("calling humidityToLocation ", v);
            v = humidityToLocation(v);

            if (v < minLoc)
            {
                minLoc = v;
            }
        }
        resultsPanel.SetPart1Result(minLoc.ToString());
    }

    private void DoPart2()
    {
        Int64 minLoc = Int32.MaxValue;

        foreach (IEnumerable<Int64> seed_range in seeds.Chunk(2))
        {
            for(Int64 i=0; i < seed_range.Last(); i++ ){
                Int64 seed =  seed_range.First() + i;
                GD.Print("calling seedToSoil ", seed);
                Int64 v = seedToSoil(seed);
                GD.Print("calling soilToFertilizer ", v);
                v = soilToFertilizer(v);
                GD.Print("calling fertilizerToWater ", v);
                v = fertilizerToWater(v);
                GD.Print("calling waterToLight ", v);
                v = waterToLight(v);
                GD.Print("calling lightToTemperature ", v);
                v = lightToTemperature(v);
                GD.Print("calling temperatureToHumidity ", v);
                v = temperatureToHumidity(v);
                GD.Print("calling humidityToLocation ", v);
                v = humidityToLocation(v);

                if (v < minLoc)
                {
                    minLoc = v;
                }
            }
        }
        resultsPanel.SetPart2Result(minLoc.ToString());
    }

    private void ParseData(string[] data)
    {
        ClearAndParseSeeds(data[0]);
        // seed-to-soil map starts at [3]
        Tuple<Int64, Func<Int64, Int64>> ret = ParseMap(data, 3);
        seedToSoil = ret.Item2;
        // each map starts 2 after the previous one ends
        ret = ParseMap(data, ret.Item1 + 2);
        soilToFertilizer = ret.Item2;
        ret = ParseMap(data, ret.Item1 + 2);
        fertilizerToWater = ret.Item2;
        ret = ParseMap(data, ret.Item1 + 2);
        waterToLight = ret.Item2;
        ret = ParseMap(data, ret.Item1 + 2);
        lightToTemperature = ret.Item2;
        ret = ParseMap(data, ret.Item1 + 2);
        temperatureToHumidity = ret.Item2;
        ret = ParseMap(data, ret.Item1 + 2);
        humidityToLocation = ret.Item2;
    }

    void ClearAndParseSeeds(string l)
    {
        seeds.Clear();
        string[] strNums = l.Split(": ")[1].Split(" ");
        foreach (string s in strNums)
        {
            GD.Print("parse Seed - ", s);
            seeds.Add(Int64.Parse(s));
        }
    }

    static Tuple<Int64, Func<Int64, Int64>> ParseMap(string[] data, Int64 line)
    {
        Int64 l = line;
        Func<Int64, Int64> f = null;

        while (l < data.Length && data[l].Trim().Length > 0)
        {
            string[] sa = data[l].Trim().Split(" ");
            Int64 dest = Int64.Parse(sa[0]);
            Int64 source = Int64.Parse(sa[1]);
            Int64 len = Int64.Parse(sa[2]);
            Func<Int64, Int64> f2 = f;

            f = (Int64 v) => LookupFunc(source, dest, len, f2, v);
            GD.Print(f);
            l++;
        }

        return Tuple.Create(l, f);
    }

    static Int64 LookupFunc(Int64 source_start, Int64 dest_start, Int64 len, Func<Int64,Int64> nextFunc, Int64 value){
        GD.Print("lookup - "+source_start + " " + value + " " + (source_start+len));
        if(value >= source_start && value < source_start + len){
            GD.Print("returning - ", dest_start + (value - source_start));
            return dest_start + (value - source_start);
        }
        if(nextFunc != null){
            GD.Print("recurse - ", value);
            return nextFunc(value);
        }
        GD.Print("returning value ", value);
        return value;
    }
}
