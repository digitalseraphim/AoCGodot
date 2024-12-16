using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoCGodot;
public partial class AoC2024Day14 : BaseChallengeScene
{
	MapDraw M;
	// HScrollBar sb;
	Dictionary<char, Color> colormap = new();
	Color black;

	public void WhenReady(){
		M = (MapDraw)FindChild("MapDraw");
		black = new Color("black");
		colormap['#'] = new Color("green");
		colormap['@'] = new Color("red");
		// sb = (HScrollBar)FindChild("FrameNum");
	}

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	List<Robot> Robots;

	private void DoPart1()
	{
		int countUL = 0;
		int countUR = 0;
		int countLL = 0;
		int countLR = 0;

		int width, height;

		if (challengeDataPanel.IsChallengeData())
		{
			width = 101;
			height = 103;
		}
		else
		{
			width = 11;
			height = 7;
		}


		foreach (var r in Robots)
		{
			var p = r.PosAfterMove(100, width, height);

			bool left = p.X < (width - 1) / 2;
			bool right = p.X > (width - 1) / 2;

			bool upper = p.Y < (height - 1) / 2;
			bool lower = p.Y > (height - 1) / 2;

			if (left)
			{
				if (upper)
				{
					countUL++;
				}
				else if (lower)
				{
					countLL++;
				}
			}
			else if (right)
			{
				if (upper)
				{
					countUR++;
				}
				else if (lower)
				{
					countLR++;
				}
			}

		}


		resultsPanel.SetPart1Result(countUL * countLL * countLR * countUR);
	}

	private void DoPart2()
	{
		OnFrameChange(0);
		// resultsPanel.SetPart2Result("check treeSearch.txt");
	}

	private void OnFrameChange(float frame){
		int width, height;

		if (challengeDataPanel.IsChallengeData())
		{
			width = 101;
			height = 103;
		}
		else
		{
			width = 11;
			height = 7;
		}
		// Pos BottomCenter = new( (width - 1) / 2, height - 3);

		GD.Print($"FrameChange {(int)(frame+.1)}");
		if(Robots == null){
			return;
		}

		// StreamWriter fs = new("treeSearch.txt");

		int i = 33 + ((int)(frame+.1) * width);
		resultsPanel.SetPart2Result(i);
		Map<char> m = new(height, width, ' ');
		Random rand = new();
		foreach (var r in Robots)
		{
			var p = r.PosAfterMove(i, width, height);
			if(rand.Next(5) == 0){
				m.SetValueAt(p, '@');
			}else{
				m.SetValueAt(p, '#');
			}
		}

		M.SetMap(m, colormap, black);

		// if (m.ValueAt(BottomCenter) == '#'
		//    && m.ValueAt(BottomCenter.AfterMove(Direction.LEFT)) == '#'
		//    && m.ValueAt(BottomCenter.AfterMove(Direction.RIGHT)) == '#')
		// {
		// 	fs.WriteLine($"Second: {i}");
		// 	fs.WriteLine(m.ToString());
		// }
		// }
		// fs.Close();


	}

	class Robot
	{
		public Pos P;
		public Pos V;

		readonly Regex r = new("p=([0-9]+),([0-9]+) v=(-?[0-9]+),(-?[0-9]+)");

		public Robot(string data)
		{
			Match m = r.Match(data);

			P = new(m.Groups[1].Value.ToInt(), m.Groups[2].Value.ToInt());
			V = new(m.Groups[3].Value.ToInt(), m.Groups[4].Value.ToInt());
		}

		public Robot(Robot other)
		{
			P = new(other.P);
			V = new(other.V);
		}

		public Pos PosAfterMove(int num, int width, int height)
		{
			int PX = (P.X + V.X * num) % width;
			int PY = (P.Y + V.Y * num) % height;

			if (PX < 0)
			{
				PX += width;
			}

			if (PY < 0)
			{
				PY += height;
			}
			return new(PX, PY);
		}
	}


	private void ParseData(string[] data)
	{
		Robots = data.Aggregate(new List<Robot>(), (l, x) => { l.Add(new(x)); return l; });
	}


}
