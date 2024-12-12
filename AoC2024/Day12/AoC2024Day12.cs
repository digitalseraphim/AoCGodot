using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day12 : BaseChallengeScene
{
	Map<char> gardens;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	List<List<Pos>> areas;

	private void DoPart1()
	{
		int count = 0;
		Map<char> g = new(gardens);

 		areas = new();

		foreach (Pos p in g)
		{
			char v = g.ValueAt(p);
			Queue<Pos> toProc = new();
			List<Pos> area;
			toProc.Enqueue(p);

			if (v == '.')
			{
				continue;
			}

			area = new() { p };

			while (toProc.Any())
			{
				// GD.Print($"top - {v} - {toProc.Join(" ")}");
				var p2 = toProc.Dequeue();
				g.SetValueAt(p2, '.');

				foreach (var n in p2.Neighbors)
				{
					if (g.IsInMap(n) && g.ValueAt(n) == v && !area.Contains(n))
					{
						toProc.Enqueue(n);
						area.Add(n);
					}
				}
			}
			// GD.Print($"Add {area.Join(" ")}");
			areas.Add(area);
		}

		foreach(var area in areas){
			int fence = 0;
			var v = gardens.ValueAt(area.First());
			
			foreach(var p in area){
				fence += p.Neighbors.Count((x)=>!gardens.IsInMap(x)||gardens.ValueAt(x)!=v);
			}
			// GD.Print($"{v} {area.Count} {fence}");
			count += fence * area.Count;
		}

		resultsPanel.SetPart1Result(count);
	}

	private void DoPart2()
	{
		int price = 0;

		foreach(var area in areas){
			int sides = 0;
			var v = gardens.ValueAt(area[0]);
			Dictionary<int, List<int>> row_changes = new();
			Dictionary<int, List<int>> col_changes = new();

			foreach(Pos p in area){
				var n = p.AfterMove(Direction.UP);
				var s = p.AfterMove(Direction.DOWN);
				var e = p.AfterMove(Direction.RIGHT);
				var w = p.AfterMove(Direction.LEFT);

				var nv = gardens.IsInMap(n) ? gardens.ValueAt(n): ' ';
				var sv = gardens.IsInMap(s) ? gardens.ValueAt(s): ' ';
				var ev = gardens.IsInMap(e) ? gardens.ValueAt(e): ' ';
				var wv = gardens.IsInMap(w) ? gardens.ValueAt(w): ' ';

				//save the lower value of any changes, tweaked to preserve direction of change

				if(nv != v){
					col_changes.GetOrCreate(n.X, ()=>new()).Add(n.Y*10+1);
				}
				if(sv != v){
					col_changes.GetOrCreate(p.X, ()=>new()).Add(p.Y*10-1);
				}

				if(wv != v){
					row_changes.GetOrCreate(w.Y, ()=>new()).Add(w.X*10+1);
				}
				if(ev != v){
					row_changes.GetOrCreate(p.Y, ()=>new()).Add(p.X*10-1);
				}
			}

			List<int> cols = new(col_changes.Keys);
			cols.Sort();
			List<int> rows = new(row_changes.Keys);
			rows.Sort();
			int maxCols = cols.Last();
			int maxRows = rows.Last();

			foreach(int c in cols){
				var cc = col_changes[c];
				while(cc.Any()){
					sides++;
					int val = cc.First();
					for(int i = c; i <= maxCols; i++){
						if(!col_changes[i].Remove(val)){
							break;
						}
					}
				}
			}

			foreach(int r in rows){
				var rc = row_changes[r];
				while(rc.Any()){
					sides++;
					int val = rc.First();
					for(int i = r; i <= maxRows; i++){
						if(!row_changes[i].Remove(val)){
							break;
						}
					}
				}
			}

			price += area.Count * sides;

			GD.Print($"{v} - {sides}");
		}

		resultsPanel.SetPart2Result(price);
	}

	private void ParseData(string[] data)
	{
		gardens = new(Util.ParseCharMap(data));
	}

}
