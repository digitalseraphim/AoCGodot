using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace AoCGodot;
public partial class AoC2023_Day5 : BaseChallengeScene
{
    SortedSet<ValRange> seeds = new();
    SortedSet<ValRange> seeds2 = new();
    SortedSet<ConvMap> seedToSoil = null;

    SortedSet<ConvMap> soilToFertilizer = null;

    SortedSet<ConvMap> fertilizerToWater = null;

    SortedSet<ConvMap> waterToLight = null;

    SortedSet<ConvMap> lightToTemperature = null;

    SortedSet<ConvMap> temperatureToHumidity = null;

    SortedSet<ConvMap> humidityToLocation = null;

    public override void DoRun(string[] data)
    {
        ParseData(data);
        // DoPart1();
        DoPart2();
    }

    private void DoPart1()
    {
        foreach (var a in seeds)
        {
            GD.Print(a);
        }
        GD.Print("\ncalling seedToSoil ");
        SortedSet<ValRange> v = LookupFunc(seeds, seedToSoil);
        GD.Print("\ncalling soilToFertilizer ");
        v = LookupFunc(v, soilToFertilizer);
        GD.Print("\ncalling fertilizerToWater ");
        v = LookupFunc(v, fertilizerToWater);
        GD.Print("\ncalling waterToLight ");
        v = LookupFunc(v, waterToLight);
        GD.Print("\ncalling lightToTemperature ");
        v = LookupFunc(v, lightToTemperature);
        GD.Print("\ncalling temperatureToHumidity ");
        v = LookupFunc(v, temperatureToHumidity);
        GD.Print("\ncalling humidityToLocation ");
        v = LookupFunc(v, humidityToLocation);

        foreach (var a in v)
        {
            GD.Print(a);
        }

        resultsPanel.SetPart1Result(v.First().Start.ToString());
    }

    private void DoPart2()
    {
        GD.Print("\ncalling seedToSoil ");
        SortedSet<ValRange> v = LookupFunc(seeds2, seedToSoil);
        GD.Print("\ncalling soilToFertilizer ");
        v = LookupFunc(v, soilToFertilizer);
        GD.Print("\ncalling fertilizerToWater ");
        v = LookupFunc(v, fertilizerToWater);
        GD.Print("\ncalling waterToLight ");
        v = LookupFunc(v, waterToLight);
        GD.Print("\ncalling lightToTemperature ");
        v = LookupFunc(v, lightToTemperature);
        GD.Print("\ncalling temperatureToHumidity ");
        v = LookupFunc(v, temperatureToHumidity);
        GD.Print("\ncalling humidityToLocation ");
        v = LookupFunc(v, humidityToLocation);
        resultsPanel.SetPart2Result(v.First().Start.ToString());
    }

    private void ParseData(string[] data)
    {
        ClearAndParseSeeds(data[0]);
        // seed-to-soil map starts at [3]
        Tuple<Int64, SortedSet<ConvMap>> ret = ParseMap(data, 3);
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
        List<Int64> ss = new();

        seeds.Clear();
        seeds2.Clear();
        string[] strNums = l.Split(": ")[1].Split(" ");
        foreach (string s in strNums)
        {
            Int64 v = Int64.Parse(s);
            GD.Print("parse Seed - ", s);
            ss.Add(v);
            seeds.Add(new(v, 1));
        }
        foreach (var s in ss.Chunk(2))
        {
            seeds2.Add(new(s[0], s[1]));
        }
    }

    static Tuple<Int64, SortedSet<ConvMap>> ParseMap(string[] data, Int64 line)
    {
        Int64 l = line;
        SortedSet<ConvMap> ret = new();

        while (l < data.Length && data[l].Trim().Length > 0)
        {
            string[] sa = data[l].Trim().Split(" ");
            Int64 dest = Int64.Parse(sa[0]);
            Int64 source = Int64.Parse(sa[1]);
            Int64 len = Int64.Parse(sa[2]);

            ret.Add(new(source, dest, len));

            l++;
        }

        return Tuple.Create(l, ret);
    }

    static SortedSet<ValRange> LookupFunc(IEnumerable<ValRange> value_range,
        SortedSet<ConvMap> map)
    {
        SortedSet<ValRange> ret = new();

        SortedSet<ValRange> vr = new SortedSet<ValRange>(value_range);
        SortedSet<ConvMap> m = new SortedSet<ConvMap>(map);

        while (vr.Count > 0 && m.Count > 0)
        {
            ValRange valRange = vr.First();
            ConvMap convMap = m.First();
            vr.Remove(valRange);

            var result = convMap.ConvValRange(valRange);

            foreach (var r in result.Item1)
            {
                ret.Add(r);
            }

            if(result.Item1.Count == 0){
                m.Remove(convMap);
            }

            if(result.Item2.Count > 0)
            {
                GD.Print("Have unmodified");
                foreach(var r in result.Item2)
                {
                    GD.Print("  ", r);
                    vr.Add(r);
                }
            }
        }
        if (vr.Count > 0)
        {
            GD.Print("have vr left over");
            foreach (var r in vr)
            {
                GD.Print("  ", r);
                ret.Add(r);
            }
        }

        return ret;
    }

    class ConvMap : Tuple<Int64, Int64, Int64>, IComparable<ConvMap>
    {
        public ConvMap(long source, long dest, long len) : base(source, dest, len)
        {
        }

        public Int64 Source
        {
            get { return Item1; }
        }
        public Int64 Dest
        {
            get { return Item2; }
        }
        public Int64 Len
        {
            get { return Item3; }
        }
        public Int64 SourceEnd
        {
            get { return Source + Len - 1; }
        }

        public Int64 DestEnd
        {
            get { return Dest + Len - 1; }
        }

        public Int64 ConvVal(Int64 val)
        {
            return val - Source + Dest;
        }

        public int Compare(Int64 val)
        {
            GD.Print("Compare ", val, " / ", Source, " - ", SourceEnd);
            if (val < Source)
            {
                GD.Print("ret -1");
                return -1;
            }
            if (val > SourceEnd)
            {
                GD.Print("ret 1");
                return 1;
            }
            GD.Print("ret 0");
            return 0;
        }

        public int CompareDest(Int64 val)
        {
            GD.Print("CompareDest ", val, " / ", Dest, " - ", DestEnd);
            if (val < Dest)
            {
                GD.Print("ret -1");
                return -1;
            }
            if (val > DestEnd)
            {
                GD.Print("ret 1");
                return 1;
            }
            GD.Print("ret 0");
            return 0;
        }


        public Tuple<SortedSet<ValRange>, SortedSet<ValRange>> ConvValRange(ValRange vr)
        {
            SortedSet<ValRange> modified = new();
            SortedSet<ValRange> unmodified = new();
            GD.Print("ConvValRange - ");
            GD.Print("  ", this);
            GD.Print("  ", vr);

            int c_start = Compare(vr.Start);
            int c_end = Compare(vr.End);
            GD.Print("c_start = ", c_start);
            GD.Print("c_end = ", c_end);

            ValRange v;
            if (c_end < 0)
            {
                //range is fully before, so just return the input, it won't be modified
                //by a later ConvMap, so treat as modified
                GD.Print("fully before");
                modified.Add(v = vr);
                GD.Print("adding m ", v);
            }
            else if(c_start > 0)
            {
                //range is fully after, so just return the input, it could be modified
                //by a later ConvMap, so treat as unmodified
                GD.Print("fully after");
                unmodified.Add(v=vr);
                GD.Print("adding u ", v);
            }
            else if (c_start == 0 && c_end == 0)
            {
                //fully inside
                GD.Print("fully inside");
                modified.Add(v = ValRange.FromStartEnd(ConvVal(vr.Start), ConvVal(vr.End)));
                GD.Print("adding m ", v);
            }
            else if (c_end == 0)
            {
                //starts before, ends in
                GD.Print("starts before, ends in");
                // both treated as modified
                modified.Add(v = ValRange.FromStartEnd(vr.Start, Source-1));
                GD.Print("adding m ", v);
                modified.Add(v = ValRange.FromStartEnd(Dest, ConvVal(vr.End)));
                GD.Print("adding m ", v);
            }
            else if(c_start == 0)
            {
                //starts in, ends after
                GD.Print("starts in, ends after");
                modified.Add(v = ValRange.FromStartEnd(ConvVal(vr.Start), DestEnd));
                GD.Print("adding m ", v);
                unmodified.Add(v = ValRange.FromStartEnd(SourceEnd+1, vr.End));
                GD.Print("adding u ", v);
            }
            else
            {
                GD.Print("starts before, ends after");
                modified.Add(v = ValRange.FromStartEnd(vr.Start, Source-1));
                GD.Print("adding m ", v);
                modified.Add(v = ValRange.FromStartEnd(Dest, DestEnd));
                GD.Print("adding m ", v);
                unmodified.Add(v = ValRange.FromStartEnd(SourceEnd+1, vr.End));
                GD.Print("adding u ", v);
            }

            return new(modified, unmodified);
        }

        public override string ToString()
        {
            return string.Format("Map Source {0} Dest {1} Len {2}", Source, Dest, Len);
        }

        public int CompareTo(ConvMap other)
        {
            return Source.CompareTo(other.Source);
        }

    }

    class ValRange : Tuple<Int64, Int64>, IComparable<ValRange>
    {

        public ValRange(long start, long len) : base(start, len)
        {
        }

        public Int64 Start
        {
            get { return Item1; }
        }
        public Int64 Len
        {
            get { return Item2; }
        }

        public Int64 End
        {
            get { return Start + Len - 1; }
        }

        public static ValRange FromStartEnd(Int64 start, Int64 end)
        {
            if(start == -1){
                GD.Print("Got a start of -1");
            }
            return new(start, end - start + 1);
        }

        public int CompareTo(ValRange other)
        {
            return Start.CompareTo(other.Start);
        }


        public override string ToString()
        {
            return string.Format("Range Start {0} Len {1}", Start, Len);
        }


    }
}
