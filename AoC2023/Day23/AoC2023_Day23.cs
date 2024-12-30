using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2023_Day23 : BaseChallengeScene
{
	Map<char> Trails = null;

	class Walk : List<Pos>, IComparable<Walk>
	{
		Pos target;

		public Pos Current
		{
			get
			{
				return this.Last();
			}
		}

		public Walk(Pos p, Pos t)
		{
			Add(p);
			target = t;
		}

		public Walk(Walk other, Pos newCurrent) : base(other)
		{
			Add(newCurrent);
			target = other.target;
		}

		public int H{
			get{
				return Count + target.Distance(Current);
			}
		}

        public int CompareTo(Walk other)
        {
            return H.CompareTo(other.H);
        }
    }

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private void DoPart1()
	{
		Pos end = Trails.LR.AfterMove(Direction.LEFT, 1);
		List<Walk> toProcess = new(){
			new(new Pos(1, 0), end)
		};
		Walk best = null;

		string slides = "^>v<";

		while (toProcess.Count > 0)
		{
			Walk w = toProcess.First();
			toProcess.RemoveAt(0);

			HashSet<Pos> next;

			char c;
			bool failedSlide = false;

			while (slides.Contains(c = Trails.ValueAt(w.Current)))
			{
				Pos afterSlide = w.Current.AfterMove(Direction.ALL[slides.IndexOf(c)]);
				if (!w.Contains(afterSlide))
				{
					w.Add(afterSlide);
				}
				else
				{
					// can't continue this way, bail out
					failedSlide = true;
					break;
				}
			}
			if (failedSlide)
			{
				continue;
			}

			next = w.Current.Neighbors.Aggregate(new HashSet<Pos>(), (hs, n) =>
			{
				// GD.Print("n - ", n, " ", Trails.IsInMap(n), " ", w.Contains(n), " ", Trails.IsInMap(n) ? Trails.ValueAt(n) : "X");

				if (Trails.IsInMap(n) && !w.Contains(n) && Trails.ValueAt(n) != '#')
				{
					// GD.Print("adding");
					hs.Add(n);
				}

				return hs;
			});

			foreach (Pos p in next)
			{
				Walk n = new(w, p);
				if (p.Equals(end))
				{
					GD.Print("found end ", n.Count);
					if (best == null || n.Count > best.Count)
					{
						best = n;
					}
				}
				else
				{
					toProcess.Add(n);
				}
			}
		}

		// foreach (Pos p in best)
		// {
		// 	Trails.SetValueAt(p, 'O');
		// }

		// GD.Print(Trails.ToString());

		resultsPanel.SetPart1Result(best == null ? -1 : best.Count-1);
	}

	private void DoPart2()
	{
		Pos end = Trails.LR.AfterMove(Direction.LEFT, 1);
		List<Walk> toProcess = new(){
			new(new Pos(1, 0), end)
		};
		Walk best = null;
		Dictionary<Pos, int> LongestRouteTo = new();

		// string slides = "^>v<";

		// answer is 6434

		while (toProcess.Count > 0)
		{
			Walk w = toProcess.Last();
			toProcess.RemoveAt(toProcess.Count-1);

			HashSet<Pos> next;

			next = w.Current.Neighbors.Aggregate(new HashSet<Pos>(), (hs, n) =>
			{
				// GD.Print("n - ", n, " ", Trails.IsInMap(n), " ", w.Contains(n), " ", Trails.IsInMap(n) ? Trails.ValueAt(n) : "X");

				if (Trails.IsInMap(n) && !w.Contains(n) && Trails.ValueAt(n) != '#')
				{
					// GD.Print("adding");
					hs.Add(n);
				}

				return hs;
			});

			foreach (Pos p in next)
			{
				Walk n = new(w, p);

				// if(LongestRouteTo.ContainsKey(p)){
				// 	if(n.Count < LongestRouteTo[p]){
				// 		continue;
				// 	}
				// }
				// LongestRouteTo[p] = n.Count;

				if (p.Equals(end))
				{
					GD.Print("found end ", n.Count);
					if (best == null || n.Count > best.Count)
					{
						best = n;
					}
				}
				else
				{
					toProcess.AddSorted(n);
				}
			}
		}

		// foreach (Pos p in best)
		// {
		// 	Trails.SetValueAt(p, 'O');
		// }

		// GD.Print(Trails.ToString());

		resultsPanel.SetPart2Result(best == null ? -1 : best.Count-1);
	}

	private void ParseData(string[] data)
	{
		Trails = new(Util.ParseCharMap(data));
	}

}
