using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2023_Day10 : BaseChallengeScene
{
	public override void DoRun(string[] data)
	{
		ParseData(data);
		// DoPart1();
		// DoPart2();
	}
	private void DoPart1()
	{
		resultsPanel.SetPart1Result("result");
	}

	private void DoPart2()
	{
		resultsPanel.SetPart2Result("result");
	}


	// string top = "7|F";
	// string bottom = "L|J";
	// string left = "L-F";
	// string right ="J-7";

	private void ParseData(string[] data)
	{
		int width = data[0].Length;
		int height = data.Length;

		string all = data.Join("");
		int location = all.IndexOf("S");
		string direction = "";
		string start_dir = "";
		int steps = 0;
		char[] buf = new char[all.Count()];
		Array.Fill(buf, '.');

		int start_loc = location;

		Dictionary<string, Dictionary<char, Tuple<int, string>>> directions = new()
		{
			["top"] = new()
			{
				['7'] = new(-1, "left"),
				['|'] = new(-width, "top"),
				['F'] = new(1, "right")
			},
			["bottom"] = new()
			{
				['J'] = new(-1, "left"),
				['|'] = new(width, "bottom"),
				['L'] = new(1, "right")
			},
			["left"] = new()
			{
				['L'] = new(-width, "top"),
				['-'] = new(-1, "left"),
				['F'] = new(width, "bottom")
			},
			["right"] = new()
			{
				['J'] = new(-width, "top"),
				['-'] = new(1, "right"),
				['7'] = new(width, "bottom")
			}
		};

		foreach (String s in data)
		{
			GD.Print(s);
		}

		foreach (string d in directions.Keys)
		{
			GD.Print("direction ", d);
			Tuple<int, string> t = directions[d].GetValueOrDefault('|', directions[d].GetValueOrDefault('-', null));
			GD.Print("tuple ", t.Item1, " ", t.Item2);
			int newLoc = location + t.Item1;
			GD.Print("newLoc = ", newLoc);

			if (newLoc > 0 && newLoc < all.Count())
			{
				if (directions[d].ContainsKey(all[newLoc]))
				{
					GD.Print("yes");
					direction = d;
					location = newLoc;
					steps++;
					break;
				}
			}
		}
		start_dir = direction;

		GD.Print("starting loc ", location);
		GD.Print("starting dir ", direction);

		do
		{
			Tuple<int, string> t = directions[direction][all[location]];
			buf[location] = all[location];
			location += t.Item1;
			direction = t.Item2;
			steps++;
		} while (all[location] != 'S');

		char startchar = ' ';

		if (start_dir == direction)
		{
			startchar = (direction == "top" || direction == "bottom") ? '|' : '-';
		}
		else
		{
			if (start_dir == "top")
			{
				startchar = (direction == "left") ? 'L' : 'J';
			}
			else if (start_dir == "bottom")
			{
				startchar = (direction == "left") ? 'F' : '7';
			}
			else if (start_dir == "left")
			{
				startchar = (direction == "top") ? '7' : 'J';
			}
			else if (start_dir == "right")
			{
				startchar = (direction == "top") ? 'F' : 'L';
			}
		}

		buf[start_loc] = startchar;
		foreach (char[] s in buf.Chunk(width))
		{
			GD.Print(new string(s));
		}

		resultsPanel.SetPart1Result((steps / 2).ToString());

		int count = 0;
		int inout = 0;
		int pipe = 1;

		for (int loc = 0; loc < buf.Length; loc++)
		{
			char c2 = buf[loc];

			GD.Print(c2, " - " + inout);
			if (c2 == '-')
			{
				GD.Print("skip -");
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
					loc++;
					c2 = buf[loc];
				} while (!"J7".Contains(c2));
				if(c2 == 'J'){
					inout += pipe;
					pipe *= -1;
				}
			}
			else if ("L".Contains(c2))
			{
				do
				{
					loc++;
					c2 = buf[loc];
				} while (!"J7".Contains(c2));
				if(c2 == '7'){
					inout += pipe;
					pipe *= -1;
				}
			}
			else if (inout > 0)
			{
				buf[loc] = '#';
				GD.Print("+");
				count++;
			}
			else
			{
				GD.Print("skip");
			}
		}

		foreach (char[] s in buf.Chunk(width))
		{
			GD.Print(new string(s));
		}

		resultsPanel.SetPart2Result(count.ToString());

	}

}
