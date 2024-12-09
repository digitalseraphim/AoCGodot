using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AoCGodot;
public partial class AoC2024Day6 : BaseChallengeScene
{
	int rows;
	int columns;

	Map<char> map;
	Pos start;
	Direction startdir;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	private void DoPart1()
	{
		int count = 1;
		var pos = start;
		var dir = startdir;
		Map<char> tmpMap = new(map);
		tmpMap.SetValueAt(pos, 'X');
		while (true)
		{
			var newpos = pos.AfterMove(dir);
			if (!tmpMap.IsInMap(newpos))
			{
				break;
			}
			if (tmpMap.ValueAt(newpos) == '#')
			{
				dir = dir.TurnCW();
			}
			else
			{
				pos = newpos;
				if (tmpMap.ValueAt(newpos) == '.')
				{
					tmpMap.SetValueAt(newpos, 'X');
					count++;
				}
			}
		}

		resultsPanel.SetPart1Result(count);
	}


	private bool CheckLoop(Pos pos, Direction dir, Map<char> map, Map<int> dmap)
	{
		Map<int> tmpMap = new(dmap);

		while (true)
		{
			tmpMap.SetValueAt(pos, tmpMap.ValueAt(pos) | 1 << Array.IndexOf(Direction.ALL, dir));
			// GD.Print(tMap.ToString());
			var newpos = pos.AfterMove(dir);
			if (!map.IsInMap(newpos))
			{
				return false;
			}
			if (map.ValueAt(newpos) == '#')
			{
				dir = dir.TurnCW();
				tmpMap.SetValueAt(pos, tmpMap.ValueAt(pos) | 1 << Array.IndexOf(Direction.ALL, dir));
			}
			else
			{
				if ((tmpMap.ValueAt(newpos) & (1 << Array.IndexOf(Direction.ALL, dir))) != 0)
				{
					// if we are going the same way as we have before at the new pos, we are looping
					return true;
				}
				pos = newpos;
			}
		}
	}


	private void DoPart2()
	{
		int count = 0;
		var pos = start;
		var dir = startdir;
		Map<int> tmpMap = new(map.Width, map.Height, 0);
		Map<char> tMap = new(map);
		HashSet<Pos> positions = new();
		string dirs = " ↑→↗↓|↘$←↖-%↙&@+";

		while (true)
		{
			tmpMap.SetValueAt(pos, tmpMap.ValueAt(pos) | dir.Bit());
			tMap.SetValueAt(pos, dirs[tmpMap.ValueAt(pos)]);
			var newpos = pos.AfterMove(dir);
			
			if (!map.IsInMap(newpos))
			{
				break;
			}

			if (map.ValueAt(newpos) == '#')
			{
				var odir = dir;
				dir = dir.TurnCW();
				tmpMap.SetValueAt(pos, tmpMap.ValueAt(pos) | dir.Bit());
				tMap.SetValueAt(pos, dirs[tmpMap.ValueAt(pos)^odir.Bit()]);
				continue;
			}
			
			// Check if we can force loop here
			// can only place a new block where there isn't one already, and we haven't been there before
			if (tmpMap.ValueAt(newpos) == 0 && map.ValueAt(newpos) == '.')
			{
				Map<char> newmap = new(map);
				newmap.SetValueAt(newpos, '#');
				if (CheckLoop(pos, dir.TurnCW(), newmap, tmpMap))
				{
					// Map<char> t2Map = new(tMap);
					// t2Map.SetValueAt(newpos, 'O');
					// GD.Print(t2Map);
					count++;
					positions.Add(newpos);
				}
			}
			pos = newpos;
		}

		GD.Print($"count = {count}, set = {positions.Count}, diff = {count - positions.Count}");
		// foreach(var p in positions){
		// 	GD.Print(p);
		// }
		resultsPanel.SetPart2Result(positions.Count);
	}

/*
too high:
2214
2130
2126

wrong:
1918
*/


	private void DoPart2_old()
	{
		int count = 0;
		var pos = start;
		var dir = startdir;
		Map<int> tmpMap = new(map.Width, map.Height, 0);
		Map<char> tMap = new(map);
		string dirs = " ↑→↗↓|↘$←↖-%↙&@+";

		while (true)
		{
			tmpMap.SetValueAt(pos, tmpMap.ValueAt(pos) | 1 << Array.IndexOf(Direction.ALL, dir));
			tMap.SetValueAt(pos, dirs[tmpMap.ValueAt(pos)]);
			// GD.Print(tMap.ToString());
			var newpos = pos.AfterMove(dir);
			if (!map.IsInMap(newpos))
			{
				break;
			}
			if (map.ValueAt(newpos) == '#')
			{
				dir = dir.TurnCW();
				tmpMap.SetValueAt(pos, tmpMap.ValueAt(pos) | 1 << Array.IndexOf(Direction.ALL, dir));
				tMap.SetValueAt(pos, dirs[tmpMap.ValueAt(pos)]);
			}
			else
			{
				pos = newpos;
				if ((tmpMap.ValueAt(newpos) & (1 << Array.IndexOf(Direction.ALL, dir.TurnCW()))) != 0)
				{
					// if we can turn right and go in a direction we went before, then we can loop
					Map<char> t2Map = new(tMap);
					t2Map.SetValueAt(newpos.AfterMove(dir), 'O');
					GD.Print(t2Map);
					count++;
				}
			}
		}

		resultsPanel.SetPart2Result(count);
	}



	/* 	{
			int count = 0;
			List<Pos> obstacles = new();
			Dictionary<int, List<Pos>> obs_by_row = new();
			Dictionary<int, List<Pos>> obs_by_col= new();

			foreach(Pos p in map){
				if(map.ValueAt(p) == '#'){
					obstacles.Add(p);
					obs_by_row.GetOrCreate(p.X,()=>new()).Add(p);
					obs_by_col.GetOrCreate(p.Y,()=>new()).Add(p);
				}
			}

			foreach(Pos ul in obstacles){
				Pos ur, lr, ll;
				// check for one in next row and prev column
				var nextRow = obs_by_row[ul.X+1].Where((x)=>x.Y>ul.Y);
				var prevCol = obs_by_col[ul.Y-1].Where((x)=>x.X>ul.X);
				var haveNextRow = false;
				var havePrevCol = false;

				haveNextRow = nextRow.Any();
				havePrevCol = prevCol.Any();

				// if we have both, we are good, otherwise, need to check based on what we do have
				if(haveNextRow && havePrevCol){
					// with both, can make loop with any in previous column
					// but need to check paths

					count += prevCol.Count();
				}else if(haveNextRow){
					// we have at least one in the next row
					// get the closest one, as that's the only one that we can use
					// check the previous column of that one, if there is anything, we can use it
					var nr = nextRow.MinBy((x)=>x.Y);
					prevCol = obs_by_col[nr.Y-1].Where((x)=>x.X>nr.X);
					if(prevCol.Any()){
						count++;
					}
				}else if(havePrevCol){

				}
			}

			resultsPanel.SetPart2Result("result");
		}
	 */

	private void ParseData(string[] data)
	{
		string dirs = "^v<>";
		map = new(Util.ParseCharMap(data));
		foreach (var p in map)
		{
			char c = map.ValueAt(p);
			int idx = dirs.IndexOf(c);
			if (idx >= 0)
			{
				startdir = Direction.ALL[idx];
				start = p;
				break;
			}
		}
	}

}
