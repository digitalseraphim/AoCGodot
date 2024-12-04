using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AoCGodot;
public partial class AoC2023_Day22 : BaseChallengeScene
{
	List<Brick> Bricks = new();
	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	private void DoPart1()
	{
		Dictionary<int, List<Brick>> tower = new();
		HashSet<Brick> couldBreak = new();
		HashSet<Brick> cantBreak = new();
		HashSet<Brick> supportingNothing = new(Bricks);

		int TowerHeight = 1;
		int TowerWidth = 1;
		int TowerDepth = 1;

		foreach (Brick b in Bricks)
		{
			GD.Print(b);
			TowerHeight = Math.Max(TowerHeight, b.End2.Z);
			TowerWidth = Math.Max(TowerWidth, b.End2.X);
			TowerDepth = Math.Max(TowerDepth, b.End2.Y);

			for (int z = b.End1.Z; z <= b.End2.Z; z++)
			{
				// GD.Print("adding to layer ", z);
				GetLayer(tower, z).Add(b);
			}
		}

		GD.Print(TowerWidth);
		GD.Print(TowerDepth);
		GD.Print(TowerHeight);

		TowerWidth++;
		TowerDepth++;
		TowerHeight++;

		// start at level 2 becuase 0 is ground, and 1 would be sitting on the ground already
		for (int z = 2; z < TowerHeight; z++)
		{
			List<Brick> layer = new(GetLayer(tower, z));
			foreach (Brick b in layer)
			{
				// GD.Print("brick = ", b);
				if (b.End1.Z != z)
				{
					// GD.Print("not lowest layer");
					continue;
				}

				int z2 = z;
				Brick fall = b.AfterFall1();
				HashSet<Brick> supporting = new();
				// while (z2 > 0 && !GetLayer(tower, z2).Any((bb) => bb != b && bb.OverlapsOnXY(fall)))
				while (z2 > 0 && supporting.Count == 0)
				{
					z2--;
					// GD.Print("z2 = ", z2);
					supporting = GetLayer(tower, z2).Aggregate(supporting,
						(hs, bb) =>
						{
							// GD.Print("b = ", b, " bb = ", bb, " fall = ", fall, " overlap: ", bb.OverlapsOnXY(fall));
							if (bb != b && bb.OverlapsOnXY(fall))
							{
								supportingNothing.Remove(bb);
								hs.Add(bb);
							}
							return hs;
						}
					);
					fall = fall.AfterFall1();
				}
				if (supporting.Count > 1)
				{
					foreach (Brick bbb in supporting)
					{
						couldBreak.Add(bbb);
					}
				}
				else if (supporting.Count == 1)
				{
					cantBreak.Add(supporting.First());
					supporting.First().WouldBreakIfBroken.Add(b);
				}
				b.SupportedBy = new(supporting);

				z2++; //lift one to before above failed
				if (z2 < z)
				{
					for (int zz = b.End1.Z; zz <= b.End2.Z; zz++)
					{
						GetLayer(tower, zz).Remove(b);
					}

					b.Fall(z2);

					for (int zz = b.End1.Z; zz <= b.End2.Z; zz++)
					{
						GetLayer(tower, zz).Add(b);
					}
				}
			}
		}

		// Map<char> XZ = new(TowerHeight, TowerWidth, '.');
		// Map<char> YZ = new(TowerHeight, TowerDepth, '.');

		// char bc = 'A';
		// foreach (Brick b in Bricks)
		// {
		// 	Vector3I db = new[] { Vector3I.Right, Vector3I.Up, Vector3I.Back }[(int)b.Axis];
		// 	for (Vector3I i = b.End1; i[(int)b.Axis] <= b.End2[(int)b.Axis]; i += db)
		// 	{
		// 		Pos xz = new(i.X, i.Z);
		// 		Pos yz = new(i.Y, i.Z);

		// 		if (XZ.ValueAt(xz) == bc)
		// 		{
		// 			//do nothing
		// 		}
		// 		else if (XZ.ValueAt(xz) == '.')
		// 		{
		// 			XZ.SetValueAt(xz, bc);
		// 		}
		// 		else
		// 		{
		// 			XZ.SetValueAt(xz, '?');
		// 		}

		// 		if (YZ.ValueAt(yz) == bc)
		// 		{
		// 			//do nothing
		// 		}
		// 		else if (YZ.ValueAt(yz) == '.')
		// 		{
		// 			YZ.SetValueAt(yz, bc);
		// 		}
		// 		else
		// 		{
		// 			YZ.SetValueAt(yz, '?');
		// 		}
		// 	}
		// 	bc++;
		// }

		// GD.Print(XZ.ToUpsidedownString());
		// GD.Print();
		// GD.Print(YZ.ToUpsidedownString());

		foreach (Brick b in supportingNothing)
		{
			// GD.Print("SN - " + b);
			couldBreak.Add(b);
		}

		foreach (Brick b in cantBreak)
		{
			couldBreak.Remove(b);
		}

		resultsPanel.SetPart1Result(couldBreak.Count);

		int count = 0;

		for (int z = 1; z < TowerHeight; z++)
		{
			List<Brick> layer = GetLayer(tower, z);
			foreach (Brick b in layer)
			{
				// GD.Print("brick = ", b);
				if (b.End1.Z != z)
				{
					// GD.Print("not lowest layer");
					continue;
				}
				if (b.WouldBreakIfBroken.Count > 0)
				{
					// GD.Print("has bricks that would be broken");
					HashSet<Brick> broken = new(b.WouldBreakIfBroken);
					// GD.Print("    ", broken.Join(", "));
					for (int z2 = z + 1; z2 < TowerHeight; z2++)
					{
						List<Brick> layer2 = GetLayer(tower, z2);
						foreach (Brick b2 in layer2)
						{
							// GD.Print("brick2 = ", b2);
							if (b2.End1.Z != z2)
							{
								// GD.Print("not lowest layer");
								continue;
							}
							// GD.Print("broken: ", broken.Join(", "));
							// GD.Print("    sb: ", b2.SupportedBy.Join(", "));
							if (broken.IsSupersetOf(b2.SupportedBy))
							{
								// GD.Print("adding ", b2);
								broken.Add(b2);
							}
						}
					}
					count += broken.Count;
				}
			}
		}

		resultsPanel.SetPart2Result(count);
	}

	private static List<Brick> GetLayer(Dictionary<int, List<Brick>> tower, int z)
	{
		if (!tower.TryGetValue(z, out List<Brick> layer))
		{
			layer = new();
			tower[z] = layer;
		}
		return layer;
	}

	private void DoPart2()
	{
		// resultsPanel.SetPart2Result("result");
	}

	private void ParseData(string[] data)
	{
		Bricks.Clear();

		foreach (string s in data)
		{
			Bricks.Add(Brick.Parse(s));
		}
	}

	class Brick : IComparable<Brick>, IEquatable<Brick>
	{
		private static int IDCOUNT = 0;
		private int Id = IDCOUNT++;
		public Vector3I End1
		{
			get; private set;
		}
		public Vector3I End2
		{
			get; private set;
		}

		public Vector3I.Axis Axis
		{
			get;
		}

		public HashSet<Brick> WouldBreakIfBroken { get; } = new();
		public HashSet<Brick> SupportedBy { get; set; } = new();

		public Brick(Vector3I end1, Vector3I end2)
		{
			if (end1.Z < end2.Z)
			{
				End1 = end1;
				End2 = end2;
			}
			else if (end2.Z < end1.Z)
			{
				End1 = end2;
				End2 = end1;
			}
			// Z ==
			else if (end1.X < end2.X)
			{
				End1 = end1;
				End2 = end2;
			}
			else if (end2.X < end1.X)
			{
				End1 = end2;
				End2 = end1;
			}
			else if (end1.Y < end2.Y)
			{
				End1 = end1;
				End2 = end2;
			}
			else
			{
				End1 = end2;
				End2 = end1;
			}
			Axis = (End2 - End1).MaxAxisIndex();
		}

		public static Brick Parse(string data)
		{
			string[] parts = data.Split("~");
			Vector3I end1 = ParsePart(parts[0]);
			Vector3I end2 = ParsePart(parts[1]);
			return new(end1, end2);
		}

		private static Vector3I ParsePart(string p)
		{
			string[] parts = p.Split(",");
			return new(parts[0].ToInt(), parts[1].ToInt(), parts[2].ToInt());
		}

		public bool Contains(Vector3I block)
		{
			return Util.IsBetween(End1.X, End2.X, block.X) &&
					Util.IsBetween(End1.Y, End2.Y, block.Y) &&
					Util.IsBetween(End1.Z, End2.Z, block.Z);
		}

		public bool OverlapsOnXY(Brick other)
		{
			return OverlapsOnAxis(other, (int)Vector3I.Axis.X) && OverlapsOnAxis(other, (int)Vector3I.Axis.Y);
		}

		public bool OverlapsOnAxis(Brick other, int axis)
		{
			return Util.IsBetween(End1[axis], End2[axis], other.End1[axis]) ||
				Util.IsBetween(End1[axis], End2[axis], other.End2[axis]) ||
				Util.IsBetween(other.End1[axis], other.End2[axis], End1[axis]) ||
				Util.IsBetween(other.End1[axis], other.End2[axis], End2[axis]);
		}

		private static bool PointBetweenOnXY(Vector3I A, Vector3I B, Vector3I test)
		{
			return Util.IsBetween(A.X, B.X, test.X) || Util.IsBetween(A.Y, B.Y, test.Y);
		}

		public void Fall(int newZ)
		{
			End2 = new(End2.X, End2.Y, End2.Z - (End1.Z - newZ));
			End1 = new(End1.X, End1.Y, newZ);
		}

		public Brick AfterFall1()
		{
			return new(new(End1.X, End1.Y, End1.Z - 1), new(End2.X, End2.Y, End2.Z - 1));
		}

		public override string ToString()
		{
			return Id + "[" + End1 + ", " + End2 + "]";
		}
		public override int GetHashCode()
		{
			return Id;
		}

		public int CompareTo(Brick other)
		{
			return Id.CompareTo(other.Id);
		}

		public bool Equals(Brick other)
		{
			return Id.Equals(other.Id);
		}
	}

}
