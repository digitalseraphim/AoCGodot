using AoCGodot;
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;

public class Util
{

	public static bool IsBetween<T>(T A, T B, T test) where T : IComparable<T>
	{
		return (A.CompareTo(B) > 0) ?
			(A.CompareTo(test) >= 0 && test.CompareTo(B) >= 0) :
			(B.CompareTo(test) >= 0 && test.CompareTo(A) >= 0);
	}
	public static int Mod(int x, int m)
	{
		return ((x % m) + m) % m;
	}

	public static char[][] ParseCharMap(string[] data)
	{
		char[][] map = new char[data.Length][];
		for (int i = 0; i < data.Length; i++)
		{
			map[i] = data[i].ToArray();
		}
		return map;
	}

	public static int[][] ParseIntMap(string[] data)
	{
		int[][] map = new int[data.Length][];
		for (int i = 0; i < data.Length; i++)
		{
			map[i] = new int[data[i].Length];
			for (int j = 0; j < data[i].Length; j++)
			{
				map[i][j] = data[i][j] - '0';
			}
		}
		return map;
	}

	public static int AddSorted<T>(T value, List<T> list) where T : IComparable<T>
	{
		int position = list.BinarySearch(value);
		if (position < 0)
		{
			position = ~position;
		}
		list.Insert(position, value);
		return position;
	}


	public static int LowestPathCost(Map<int> map, Pos start, Pos target, int minStraightLine, int maxStraightLine)
	{
		List<Path> paths = new();
		HashSet<string> visited = new();
		Path startPath = new(Direction.DOWN, start, target, 0, 0, minStraightLine, maxStraightLine);
		paths.Add(startPath);

		visited.Add("" + startPath.Position + startPath.Dir + startPath.NumInStraightLine);

		int i = 0;
		while (paths.Count > 0 && i < 5)
		{
			//i++;
			Path p = paths.First();
			paths.RemoveAt(0);

			GD.Print(p);

			List<Path> next = p.NextOptions(map);
			foreach (Path n in next)
			{
				if (n.Position.Equals(target))
				{
					return n.Cost;
				}
				if (!visited.Contains("" + n.Position + n.Dir + n.NumInStraightLine))
				{
					GD.Print("  Adding: " + n);
					visited.Add("" + n.Position + n.Dir + n.NumInStraightLine);
					Util.AddSorted(n, paths);
				}
			}
		}
		return -1;
	}

	public static int CountInternal(Map<char> map)
	{
		int count = 0;
		int inout = 0;
		int pipe = 1;

		for (int Y = 0; Y <= map.LR.Y; Y++)
		{
			for (int X = 0; X <= map.LR.X; X++)
			{
				char c2 = map.ValueAt(X, Y);

				// GD.Print(c2, " - " + inout);
				if (c2 == '-')
				{
					// GD.Print("skip -");
				}
				else if (c2 == '|')
				{
					inout += pipe;
					pipe *= -1;
				}
				else if ("F".Contains(c2))
				{
					do
					{
						X++;
						c2 = map.ValueAt(X, Y);
					} while (!"J7".Contains(c2));
					if (c2 == 'J')
					{
						inout += pipe;
						pipe *= -1;
					}
				}
				else if ("L".Contains(c2))
				{
					do
					{
						X++;
						c2 = map.ValueAt(X, Y);
					} while (!"J7".Contains(c2));
					if (c2 == '7')
					{
						inout += pipe;
						pipe *= -1;
					}
				}
				else if (inout > 0)
				{
					count++;
				}
				else
				{
					// GD.Print("skip");
				}
			}
		}

		return count;
	}

	public static long LCM(long[] numbers)
	{
		return numbers.Aggregate(lcm);
	}
	public static long lcm(long a, long b)
	{
		return Math.Abs(a * b) / GCD(a, b);
	}
	public static long GCD(long a, long b)
	{
		return b == 0 ? a : GCD(b, a % b);
	}


}

public static class Extensions
{
	public static IEnumerable<Tuple<T,T>> GetCombinations<T>(this IEnumerable<T> source){
		for(int i = 0; i < source.Count(); i++){
			for(int j = i+1; j < source.Count(); j++){
				yield return new(source.ElementAt(i),source.ElementAt(j));
			}
		}
	}

	public static IEnumerable<Tuple<T,T>> Permutations<T>(this IEnumerable<T> source){
		for(int i = 0; i < source.Count(); i++){
			for(int j = 0; j < source.Count(); j++){
				yield return new(source.ElementAt(i),source.ElementAt(j));
			}
		}
	}


	public static V GetOrCreate<K,V>(this IDictionary<K,V> dict, K key, Func<V> gen){
		if(!dict.ContainsKey(key)){
			dict[key] = gen.Invoke();
		}
		return dict[key];
	}

	public static string Join<V>(this List<V> arr, string sep){
		List<String> strlist = arr.Aggregate(new List<string>(), (l, i) => {l.Add(i.ToString()); return l;});
		return strlist.ToArray().Join(sep);
	}

	public static string Join<V>(this HashSet<V> arr, string sep){
		List<String> strlist = arr.Aggregate(new List<string>(), (l, i) => {l.Add(i.ToString()); return l;});
		return strlist.ToArray().Join(sep);
	}

    public static IEnumerable<IEnumerable<T>> DifferentCombinations<T>(this IEnumerable<T> elements, int k)
    {
        return k == 0 ? new[] { Array.Empty<T>() } :
          elements.SelectMany((e, i) =>
            elements.Skip(i + 1).DifferentCombinations(k - 1).Select(c => (new[] {e}).Concat(c)));
    }
}

public class Direction
{
	public static readonly Direction UP = new("Up", 0, -1);
	public static readonly Direction DOWN = new("Down", 0, 1);
	public static readonly Direction LEFT = new("Left", -1, 0);
	public static readonly Direction RIGHT = new("Right", 1, 0);

	public static readonly Direction[] ALL = { Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.LEFT };


	public int DX
	{
		get;
	}

	public int DY
	{
		get;
	}

	public bool IsHorizontal()
	{
		return DY == 0;
	}

	public bool IsVertical()
	{
		return DX == 0;
	}

	public string Name
	{
		get;
	}

	public Direction(string name, int dx, int dy)
	{
		Name = name;
		DX = dx;
		DY = dy;
	}

	public Direction TurnCCW()
	{
		if (this == UP)
			return LEFT;
		if (this == LEFT)
			return DOWN;
		if (this == DOWN)
			return RIGHT;
		if (this == RIGHT)
			return UP;
		return null;
	}

	public Direction TurnCW()
	{
		if (this == UP)
			return RIGHT;
		if (this == RIGHT)
			return DOWN;
		if (this == DOWN)
			return LEFT;
		if (this == LEFT)
			return UP;
		return null;
	}

	public override string ToString()
	{
		return Name;
	}

	public int Bit(){
		return 1 << Array.IndexOf(ALL, this);
	}
};

public class LPos : IEquatable<LPos>
{
	public long X
	{
		get;
	}
	public long Y
	{
		get;
	}

	public LPos(long x, long y)
	{
		X = x;
		Y = y;
	}

	public LPos AfterMove(Direction d, long num = 1)
	{
		return new(X + (num * d.DX), Y + (num * d.DY));
	}

	public override string ToString()
	{
		return "[" + X + ", " + Y + "]";
	}

	public long Distance(LPos other)
	{
		return Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
	}
	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public bool Equals(LPos other)
	{
		return X == other.X && Y == other.Y;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as LPos);
	}
}

public class Pos : IEquatable<Pos>
{
	public int X
	{
		get;
	}
	public int Y
	{
		get;
	}
	public NeighborEnum Neighbors { get; }
	public NeighborEnumWithDir NeighborsWithDirs { get; }


	public Pos(int x, int y)
	{
		Neighbors = new(this);
		NeighborsWithDirs = new(this);
		X = x;
		Y = y;
	}

	public Pos(Pos other)
	{
		Neighbors = new(this);
		X = other.X;
		Y = other.Y;
	}

	public Pos AfterMove(Direction d, int num = 1)
	{
		return new(X + (num * d.DX), Y + (num * d.DY));
	}

	public override string ToString()
	{
		return "[" + X + ", " + Y + "]";
	}

	public int Distance(Pos other)
	{
		return Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
	}
	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public bool Equals(Pos other)
	{
		return X == other.X && Y == other.Y;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as Pos);
	}

	public class NeighborEnum : IEnumerable<Pos>
	{
		public IEnumerator<Pos> GetEnumerator()
		{
			return new Enumerator(Parent);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		Pos Parent { get; }

		public NeighborEnum(Pos parent)
		{
			Parent = parent;
		}

		private class Enumerator : IEnumerator<Pos>
		{
			int index = -1;
			public Pos Current { get; private set; }

			object IEnumerator.Current { get { return Current; } }

			Pos Parent { get; }

			public Enumerator(Pos parent)
			{
				Parent = parent;
			}

			public void Dispose()
			{

			}

			public bool MoveNext()
			{
				index++;
				if (index >= 4)
				{
					Current = null;
					return false;
				}
				Current = Parent.AfterMove(Direction.ALL[index]);
				return true;
			}

			public void Reset()
			{
				index = -1;
				Current = null;
			}

		}
	}
	public class NeighborEnumWithDir : IEnumerable<Tuple<Pos, Direction>>
	{
		public IEnumerator<Tuple<Pos, Direction>> GetEnumerator()
		{
			return new Enumerator(Parent);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		Pos Parent { get; }

		public NeighborEnumWithDir(Pos parent)
		{
			Parent = parent;
		}

		private class Enumerator : IEnumerator<Tuple<Pos, Direction>>
		{
			int index = -1;
			readonly Direction[] Directions = { Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.LEFT };
			public Tuple<Pos, Direction> Current { get; private set; }

			object IEnumerator.Current { get { return Current; } }

			Pos Parent { get; }

			public Enumerator(Pos parent)
			{
				Parent = parent;
			}

			public void Dispose()
			{

			}

			public bool MoveNext()
			{
				index++;
				if (index >= 4)
				{
					Current = null;
					return false;
				}
				Direction d = Directions[index];
				Current = new(Parent.AfterMove(d), d);
				return true;
			}

			public void Reset()
			{
				index = -1;
				Current = null;
			}

		}
	}
}

public class Map<T> : IEnumerable<Pos>
{
	public Pos UL
	{
		get;
	}
	public Pos UR
	{
		get;
	}
	public Pos LL
	{
		get;
	}
	public Pos LR
	{
		get;
	}

	public int Width
	{
		get { return MapData[0].Length; }
	}
	public int Height
	{
		get { return MapData.Length; }
	}
	readonly T[][] MapData;

	public Map(Map<T> other)
	{
		MapData = other.MapData.Select(a => a.ToArray()).ToArray();
		UL = new(other.UL);
		UR = new(other.UR);
		LL = new(other.LL);
		LR = new(other.LR);
	}

	public Map(int rows, int cols, T fillValue)
	{
		MapData = new T[rows][];
		for (int i = 0; i < rows; i++)
		{
			MapData[i] = new T[cols];
			Array.Fill(MapData[i], fillValue);
		}
		UL = new(0, 0);
		UR = new(cols - 1, 0);
		LL = new(0, rows - 1);
		LR = new(cols - 1, rows - 1);
	}

	public Map(int rows, int cols, Func<T> gen)
	{
		MapData = new T[rows][];
		for (int i = 0; i < rows; i++)
		{
			MapData[i] = new T[cols];
			for (int j = 0; j < cols; j++)
			{
				MapData[i][j] = gen();
			}
		}
		UL = new(0, 0);
		UR = new(cols - 1, 0);
		LL = new(0, rows - 1);
		LR = new(cols - 1, rows - 1);
	}

	public Map(T[][] data)
	{
		int rows = data.Length;
		int cols = data[0].Length;
		UL = new(0, 0);
		UR = new(cols - 1, 0);
		LL = new(0, rows - 1);
		LR = new(cols - 1, rows - 1);
		MapData = data;
	}

	public bool IsInMap(Pos pos)
	{
		if (pos.X < 0 || pos.X >= MapData[0].Length)
		{
			return false;
		}
		if (pos.Y < 0 || pos.Y >= MapData.Length)
		{
			return false;
		}
		return true;
	}

	public T ValueAt(Pos pos)
	{
		return MapData[pos.Y][pos.X];
	}

	public T ValueAt(int X, int Y)
	{
		return MapData[Y][X];
	}

	public T SetValueAt(Pos pos, T v)
	{
		T old = ValueAt(pos);
		MapData[pos.Y][pos.X] = v;
		return old;
	}

	public override string ToString()
	{
		string s = "";
		foreach (var row in MapData)
		{
			foreach (var col in row)
			{
				s += col;
			}
			s += "\n";
		}
		return s;
	}
	public string ToUpsidedownString()
	{
		string s = "";
		foreach (var row in MapData.Reverse())
		{
			foreach (var col in row)
			{
				s += col;
			}
			s += "\n";

		}
		return s;
	}

	public IEnumerator<Pos> GetEnumerator()
	{
		return new PosEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	class PosEnumerator : IEnumerator<Pos>
	{
		public Pos Current
		{
			get; private set;
		}

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		private readonly Map<T> Map;

		public PosEnumerator(Map<T> m) : base()
		{
			Map = m;
			Reset();
		}

		public void Dispose()
		{

		}

		public bool MoveNext()
		{
			if (Current == null)
			{
				Current = Map.UL;
				return true;
			}

			Pos ret = Current.AfterMove(Direction.RIGHT);
			if (!Map.IsInMap(ret))
			{
				ret = new(Map.UL.X, Current.Y + 1);
				if (!Map.IsInMap(ret))
				{
					Current = null;
					return false;
				}
			}
			Current = ret;
			return true;
		}

		public void Reset()
		{
			Current = null;
		}
	}

}


class Path : IComparable<Path>
{
	public Direction Dir
	{
		get;
	}
	public Pos Position
	{
		get;
	}
	public Pos Target
	{
		get;
	}
	public int NumInStraightLine
	{
		get;
	}
	public int MinStraightLine
	{
		get;
	}
	public int MaxStraightLine
	{
		get;
	}
	public int Cost
	{
		get;
	}


	public Path(Direction d, Pos p, Pos target, int cost, int numInStraightLine, int minStraightLine, int maxStraightLine)
	{
		Dir = d;
		Position = p;
		Target = target;
		Cost = cost;
		NumInStraightLine = numInStraightLine;
		MinStraightLine = minStraightLine;
		MaxStraightLine = maxStraightLine;
	}

	public int H
	{
		get
		{
			return Position.Distance(Target);
		}
	}

	public int G
	{
		get
		{
			return Cost + H;
		}
	}

	public List<Path> NextOptions(Map<int> map)
	{
		List<Path> ret = new();

		if ((MaxStraightLine == -1) || (NumInStraightLine < MaxStraightLine))
		{
			Pos newPos = Position.AfterMove(Dir);
			if (map.IsInMap(newPos))
			{
				int val = map.ValueAt(newPos);
				int newCost = Cost + val;
				Path newPath = new(Dir, newPos, Target, newCost, NumInStraightLine + 1, MinStraightLine, MaxStraightLine);
				ret.Add(newPath);
			}
		}

		Direction leftDir = Dir.TurnCCW();
		Direction rightDir = Dir.TurnCW();
		Pos leftPos = Position;
		Pos rightPos = Position;
		int leftCost = Cost;
		int rightCost = Cost;

		for (int i = 0; i < MinStraightLine; i++)
		{
			leftPos = leftPos.AfterMove(leftDir);
			if (!map.IsInMap(leftPos))
			{
				leftPos = null;
				break;
			}
			leftCost += map.ValueAt(leftPos);
		}

		if (leftPos != null)
		{
			Path newPath = new(leftDir, leftPos, Target, leftCost, MinStraightLine, MinStraightLine, MaxStraightLine);
			ret.Add(newPath);
		}

		for (int i = 0; i < MinStraightLine; i++)
		{
			rightPos = rightPos.AfterMove(rightDir);
			if (!map.IsInMap(rightPos))
			{
				rightPos = null;
				break;
			}
			rightCost += map.ValueAt(rightPos);
		}

		if (rightPos != null)
		{
			Path newPath = new(rightDir, rightPos, Target, rightCost, MinStraightLine, MinStraightLine, MaxStraightLine);
			ret.Add(newPath);
		}

		return ret;
	}

	public int CompareTo(Path other)
	{
		return G.CompareTo(other.G);
	}

	public override string ToString()
	{
		return "Path - " + Position + " Cost: " + Cost + " H: " + H + " G: " + G;
	}
}