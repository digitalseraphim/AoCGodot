using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day16 : BaseChallengeScene
{
	Map<char> maze;
	Pos start;
	Pos end;
	List<RPath> allWinning;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private void DoPart1()
	{
		List<RPath> paths = new();
		Dictionary<Tuple<Pos, Direction>, int> visited = new();
		RPath winningPath = null;

		allWinning = new();

		paths.Add(new(start, Direction.RIGHT, 0, new()));

		GD.Print($"start = {start} end = {end}");

		while (paths.Any())
		{
			var rpath = paths.First();
			paths.RemoveAt(0);
			// GD.Print($"Checking path {rpath}");
			if (rpath.current.Equals(end))
			{
				if (winningPath == null || winningPath.cost > rpath.cost)
				{
					winningPath = rpath;
					allWinning = new() { rpath };
				}
				else if (winningPath.cost == rpath.cost)
				{
					allWinning.Add(rpath);
				}

				continue;
			}

			foreach (var next in rpath.NextOptions(maze))
			{
				var pd = next.PosAndDir();
				if (visited.ContainsKey(pd))
				{
					if (visited[pd] < next.cost)
					{
						continue;
					}
				}
				visited[pd] = next.cost;
				Util.AddSorted(next, paths);
				// GD.Print($"adding {next}");
			}
		}

		resultsPanel.SetPart1Result(winningPath.cost);
	}

	private void DoPart2()
	{
		HashSet<Pos> winningPaths = new(){start, end};

		foreach(var w in allWinning){
			GD.Print(w);
			winningPaths.UnionWith(w.history);
		}

		resultsPanel.SetPart2Result(winningPaths.Count);
	}

	private void ParseData(string[] data)
	{
		maze = new(Util.ParseCharMap(data));
		foreach (Pos p in maze)
		{
			if (maze.ValueAt(p) == 'S')
			{
				start = p;
			}
			else if (maze.ValueAt(p) == 'E')
			{
				end = p;
			}
		}
	}

	class RPath : IComparable<RPath>
	{
		public Pos current;
		public Direction facing;
		public int cost;
		public List<Pos> history;

		public RPath(Pos cur, Direction face, int c, List<Pos> h)
		{
			current = cur;
			facing = face;
			cost = c;
			history = h;
		}

		public List<RPath> NextOptions(Map<char> m)
		{
			List<RPath> options = new();

			foreach (var pd in current.NeighborsWithDirs)
			{
				if (m.ValueAt(pd.Item1) != '#')
				{
					int turnCost;
					if (pd.Item2 == facing)
					{
						turnCost = 0;
					}
					else if (pd.Item2 == facing.TurnCCW() || pd.Item2 == facing.TurnCW())
					{
						turnCost = 1000;
					}
					else
					{
						turnCost = 2000;
					}
					options.Add(new(pd.Item1, pd.Item2, cost + turnCost + 1, history.Append(current).ToList()));
				}
			}

			return options;
		}

		public Tuple<Pos, Direction> PosAndDir()
		{
			return new(current, facing);
		}

		public override string ToString()
		{
			return $"{current} {facing} {cost} {history.Count}";
		}

		public int CompareTo(RPath other)
		{
			if (current == other.current && facing == other.facing && cost == other.cost)
			{
				return 0;
			}

			if (cost < other.cost)
			{
				return -1;
			}
			else if (cost > other.cost)
			{
				return 1;
			}
			else
			{
				int idx = Array.IndexOf(Direction.ALL, facing);
				int otheridx = Array.IndexOf(Direction.ALL, other.facing);
				if (idx < otheridx)
				{
					return -1;
				}
				else if (idx > otheridx)
				{
					return 1;
				}
				else
				{
					int v1 = current.X * 1000 + current.Y;
					int v2 = other.current.X * 1000 + other.current.Y;
					return v1.CompareTo(v2);
				}
			}
		}
	}

}
