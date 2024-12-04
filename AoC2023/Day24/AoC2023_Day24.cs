using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;

namespace AoCGodot;
public partial class AoC2023_Day24 : BaseChallengeScene
{
	List<Hail> Hailstones { get; } = new();
	bool test = false;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private void DoPart1()
	{
		double xytmin;
		double xytmax;
		int count = 0;

		if (test)
		{
			xytmin = 7;
			xytmax = 27;
		}
		else
		{
			xytmin = 200000000000000;
			xytmax = 400000000000000;
		}

		/*

			s = Adx * (By0 - Ay0) - Ady * (Bx0 - Ax0)
				-----------------------------------------
				(Ady*Bdx - Adx*Bdy)
		*/


		foreach (Tuple<Hail, Hail> hh in Hailstones.GetCombinations())
		{
			Hail A = hh.Item1;
			Hail B = hh.Item2;

			double s = (A.DX * (B.Y - A.Y) - A.DY * (B.X - A.X)) / (A.DY * B.DX - A.DX * B.DY);

			if (s < 0)
			{
				continue;
			}

			double BXS = B.X + B.DX * s;
			double BYS = B.Y + B.DY * s;

			double t = (BXS - A.X) / A.DX;


			GD.Print(hh, " ", s, " ", t, " ", BXS, " ", BYS);

			if (t < 0)
			{
				continue;
			}

			if (Util.IsBetween(xytmin, xytmax, BXS) &&
				Util.IsBetween(xytmin, xytmax, BYS))
			{
				GD.Print("yes");
				count++;
			}
		}

		resultsPanel.SetPart1Result(count);
	}

	private void DoPart13()
	{
		double xytmin;
		double xytmax;
		int count = 0;

		if (test)
		{
			xytmin = 7;
			xytmax = 27;
		}
		else
		{
			xytmin = 200000000000000;
			xytmax = 400000000000000;
		}

		foreach (Tuple<Hail, Hail> hh in Hailstones.GetCombinations())
		{
			double R = (hh.Item1.X * hh.Item1.DY - hh.Item1.Y * hh.Item1.DX +
						hh.Item2.X * hh.Item1.DY - hh.Item2.Y * hh.Item1.DX) /
						(hh.Item2.DY * hh.Item1.DX - hh.Item1.DY * hh.Item2.DX);
			double S = (hh.Item1.X - hh.Item2.X + R * hh.Item2.X) / hh.Item1.DX;

			if ((R - hh.Item1.X) / hh.Item1.DX < 0 || (S - hh.Item1.Y) / hh.Item1.DY < 0)
			{
				continue;
			}

			if (Util.IsBetween(xytmin, xytmax, R) &&
				Util.IsBetween(xytmin, xytmax, S))
			{
				GD.Print("yes");
				count++;
			}
		}

		resultsPanel.SetPart1Result(count);
	}

	private void DoPart1_old()
	{
		double xytmin;
		double xytmax;
		int count = 0;

		if (test)
		{
			xytmin = 7;
			xytmax = 27;
		}
		else
		{
			xytmin = 200000000000000;
			xytmax = 400000000000000;
		}

		foreach (Tuple<Hail, Hail> hh in Hailstones.GetCombinations())
		{
			double tx = (hh.Item1.X - hh.Item2.X) / (hh.Item2.DX - hh.Item1.DX);
			double ty = (hh.Item1.Y - hh.Item2.Y) / (hh.Item2.DY - hh.Item1.DY);

			if (tx < 0 || ty < 0)
			{
				continue;
			}

			double XT1 = hh.Item1.X + hh.Item1.DX * tx;
			double YT1 = hh.Item1.Y + hh.Item1.DY * ty;
			double XT2 = hh.Item2.X + hh.Item2.DX * tx;
			double YT2 = hh.Item2.Y + hh.Item2.DY * ty;

			GD.Print(hh, " ", tx, " ", ty, " ", XT1, " ", YT1, " ", XT2, " ", YT2);

			if (Util.IsBetween(xytmin, xytmax, XT1) &&
				Util.IsBetween(xytmin, xytmax, YT1) &&
				Util.IsBetween(xytmin, xytmax, XT2) &&
				Util.IsBetween(xytmin, xytmax, YT2))
			{
				GD.Print("yes");
				count++;
			}
		}

		resultsPanel.SetPart1Result(count);
	}


	private void DoPart1old()
	{
		Dictionary<Hail, Tuple<double, double>> crossingTimes = new();
		double xytmin;
		double xytmax;

		if (test)
		{
			xytmin = 7;
			xytmax = 27;
		}
		else
		{
			xytmin = 200000000000000;
			xytmax = 400000000000000;
		}

		foreach (Hail h in Hailstones)
		{
			double xt1 = (xytmin - h.X) / h.DX;
			double xt2 = (xytmax - h.X) / h.DX;
			double yt1 = (xytmin - h.Y) / h.DY;
			double yt2 = (xytmax - h.Y) / h.DY;

			if ((xt1 < 0 && xt2 < 0) || (yt1 < 0 && yt2 < 0))
			{
				// intersection with area happened in the past or not at all
				continue;
			}

			(xt1, xt2) = (Math.Max(Math.Min(xt2, xt1), 0), Math.Max(xt1, xt2));
			(yt1, yt2) = (Math.Max(Math.Min(yt2, yt1), 0), Math.Max(yt1, yt2));
			if (Util.IsBetween(xt1, xt2, yt1) || Util.IsBetween(xt1, xt2, yt2))
			{
				//time ranges overlap
				crossingTimes.Add(h, new(Math.Max(xt1, yt1), Math.Min(xt2, yt2)));
			}
		}

		long count = 0;

		foreach (Tuple<Hail, Hail> hh in crossingTimes.Keys.GetCombinations())
		{
			Tuple<double, double> t1 = crossingTimes[hh.Item1];
			Tuple<double, double> t2 = crossingTimes[hh.Item2];
			GD.Print(hh);
			GD.Print("t1 ", t1, " t2 ", t2);
			if (Util.IsBetween(t1.Item1, t1.Item2, t2.Item1) || Util.IsBetween(t1.Item1, t1.Item2, t2.Item1))
			{
				//they cross the box at the same time.
				double start_time = Math.Max(t1.Item1, t2.Item1);
				double end_time = Math.Min(t1.Item2, t2.Item2);

				//calculate positions at start_time
				// double h1x = hh.Item1.X + hh.Item1.DX*start_time;
				// double h1y = hh.Item1.Y + hh.Item1.DY*start_time;
				// double h2x = hh.Item2.X + hh.Item2.DX*start_time;
				// double h2y = hh.Item2.Y + hh.Item2.DY*start_time;

				// double tx = (h1x-h2x)/(hh.Item2.DX-hh.Item1.DX);
				// double ty = (h1y-h2y)/(hh.Item2.DY-hh.Item1.DY);
				// GD.Print("tx = ", tx, " ty = ", ty);
				// if(Util.IsBetween(0, end_time-start_time, tx) &&
				// 		Util.IsBetween(0, end_time-start_time, ty)){
				// 	if(Math.Abs(tx - ty) < 0.00000000001){
				// 		count ++;
				// 	}else{
				// 		GD.Print(Math.Abs(tx-ty));
				// 	}
				// }


				// double h1x = hh.Item1.X + hh.Item1.DX*start_time;
				// double h1y = hh.Item1.Y + hh.Item1.DY*start_time;
				// double h2x = hh.Item2.X + hh.Item2.DX*start_time;
				// double h2y = hh.Item2.Y + hh.Item2.DY*start_time;

				double tx = (hh.Item1.X - hh.Item2.X) / (hh.Item2.DX - hh.Item1.DX);
				double ty = (hh.Item1.Y - hh.Item2.Y) / (hh.Item2.DY - hh.Item1.DY);
				GD.Print("tx = ", tx, " ty = ", ty);
				if (Util.IsBetween(start_time, end_time, tx) &&
						Util.IsBetween(start_time, end_time, ty))
				{
					//if(Math.Abs(tx - ty) < 0.00000000001){
					count++;
					// }else{
					// 	GD.Print(Math.Abs(tx-ty));
					// }
				}
			}
		}
		resultsPanel.SetPart1Result(count);
	}

	private void DoPart2()
	{
		resultsPanel.SetPart2Result("result");
	}

	private void ParseData(string[] data)
	{
		Hailstones.Clear();
		foreach (string s in data)
		{
			Hailstones.Add(new(s));
		}
	}

	class Hail
	{
		public double X { get; }
		public double Y { get; }
		public double Z { get; }
		public double DX { get; }
		public double DY { get; }
		public double DZ { get; }

		public Hail(string s)
		{
			string[] parts = s.Split(new[] { ",", "@" }, StringSplitOptions.TrimEntries);
			X = double.Parse(parts[0]);
			Y = double.Parse(parts[1]);
			Z = double.Parse(parts[2]);
			DX = double.Parse(parts[3]);
			DY = double.Parse(parts[4]);
			DZ = double.Parse(parts[5]);
		}

		public Tuple<long, long> InterceptXY(Hail other)
		{
			if ((double)DX / (double)DY == (double)other.DX / (double)other.DY)
			{
				// Parallel lines
				GD.Print("got parallel");
			}
			else
			{
				// X + t*DX = other.X + u*other.DX  |  X-other.X = u*other.DX - t*DX
				// Y + t*DY = other.Y + u*other.DY  |  Y-other.Y = u*other.DY - t*DY


			}
			return null;
		}

		public override string ToString()
		{
			return String.Format("[{0},{1} @ {2},{3}]", X, Y, DX, DY);
		}

	}
}
