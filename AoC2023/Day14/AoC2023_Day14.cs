using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2023_Day14 : BaseChallengeScene
{
	char[][] map;
	char[][] map2;
	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	void TiltNorth(char[][] imap)
	{
		for (int r = 0; r < imap.Length; r++)
		{
			for (int c = 0; c < imap[0].Length; c++)
			{
				if (imap[r][c] == 'O')
				{
					int rr = r;
					while (rr > 0 && imap[rr - 1][c] == '.')
					{
						rr--;
					}
					imap[r][c] = '.';
					imap[rr][c] = 'O';
				}
			}
		}
	}

	void TiltSouth(char[][] imap)
	{
		for (int r = imap.Length - 1; r >= 0; r--)
		{
			for (int c = 0; c < imap[0].Length; c++)
			{
				if (imap[r][c] == 'O')
				{
					int rr = r;
					while (rr < imap.Length - 1 && imap[rr + 1][c] == '.')
					{
						rr++;
					}
					imap[r][c] = '.';
					imap[rr][c] = 'O';
				}
			}
		}
	}



	void TiltWest(char[][] imap)
	{
		for (int r = 0; r < imap.Length; r++)
		{
			for (int c = 0; c < imap[0].Length; c++)
			{
				if (imap[r][c] == 'O')
				{
					int cc = c;
					while (cc > 0 && imap[r][cc - 1] == '.')
					{
						cc--;
					}
					imap[r][c] = '.';
					imap[r][cc] = 'O';
				}
			}
		}
	}

	void TiltEast(char[][] imap)
	{
		for (int r = 0; r < imap.Length; r++)
		{
			for (int c = imap[0].Length - 1; c >= 0; c--)
			{
				if (imap[r][c] == 'O')
				{
					int cc = c;
					while (cc < imap.Length - 1 && imap[r][cc + 1] == '.')
					{
						cc++;
					}
					imap[r][c] = '.';
					imap[r][cc] = 'O';
				}
			}
		}
	}

	private void DoPart1()
	{
		TiltNorth(map);

		int load = CalcNorthLoad(map);

		resultsPanel.SetPart1Result(load.ToString());
	}

	int CalcNorthLoad(char[][] imap)
	{
		int load = 0;
		for (int r = 0; r < imap.Length; r++)
		{
			for (int c = 0; c < imap[0].Length; c++)
			{
				if (imap[r][c] == 'O')
				{
					load += imap.Length - r;
				}
			}
		}
		return load;
	}

	private string MapToString(char[][] imap)
	{
		String s = "";
		for (int r = 0; r < map.Length; r++)
		{
			s += new string(imap[r]) + "\n";
		}
		return s;
	}

	private void DoPart2()
	{
		Dictionary<string, int> seen = new();
		int i;
		string s = "";
		for (i = 0; i < 1000000000; i++)
		{
			TiltNorth(map2);
			TiltWest(map2);
			TiltSouth(map2);
			TiltEast(map2);
			s = MapToString(map2);
			GD.Print(i, " ", CalcNorthLoad(map2), " ", s.Hash());
			if (seen.ContainsKey(s))
			{
				break;
			}
			// if(i < 3){
			// 	GD.Print(MapToString(map2));
			// }
			seen.Add(s, i);
		}


		int first = seen[s];
		int loop_size = i - first;
		int extra = 1000000000 % loop_size;
		GD.Print("i = ", i);
		GD.Print("first = ", first);
		GD.Print("loop size = ", loop_size);
		GD.Print("rest = ", extra);
		int num_loops = 1000000000/loop_size;
		num_loops -= 10;
		i += num_loops * loop_size;
		GD.Print("i = ", i);

		for (; i < 999999999 ; i++)
		{
			TiltNorth(map2);
			TiltWest(map2);
			TiltSouth(map2);
			TiltEast(map2);
			s = MapToString(map2);
			GD.Print(i, " ", CalcNorthLoad(map2), " ", s.Hash());
		}


		int load = CalcNorthLoad(map2);

		resultsPanel.SetPart2Result(load.ToString());
	}

	private void ParseData(string[] data)
	{
		map = new char[data.Length][];
		map2 = new char[data.Length][];
		for (int i = 0; i < data.Length; i++)
		{
			map[i] = data[i].ToArray();
			map2[i] = data[i].ToArray();
		}
	}

}
