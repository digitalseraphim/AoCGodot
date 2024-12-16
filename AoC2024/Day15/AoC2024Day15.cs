using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day15 : BaseChallengeScene
{
	Map<char> warehouse;
	Map<char> warehouse2;
	Dictionary<Pos, Box> Boxes;
	string moves;
	Pos start;
	Pos start2;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	
	private void DoPart1()
	{
		string dirs = "^>v<";
		Map<char> m = new(warehouse);
		Pos p = start;
		m.SetValueAt(p, '.');

		foreach(char move in moves){
			Direction d = Direction.ALL[dirs.IndexOf(move)];
			// Map<char> m2 = new(m);
			// m2.SetValueAt(p,'@');
			// GD.Print(m2);
			// GD.Print($"moving {d}");
			Pos p1 = p.AfterMove(d);
			Pos p2 = p.AfterMove(d, 2);
			if(m.ValueAt(p1) == '#'){
				continue;
			}
			if(m.ValueAt(p1) == 'O'){
				while(m.ValueAt(p2) == 'O'){
					p2 = p2.AfterMove(d);
				}
				if(m.ValueAt(p2) == '#'){
					continue;
				}

				m.SetValueAt(p1, '.');
				m.SetValueAt(p2, 'O');
			}
			p = p1;
		}

		int count = 0;
		foreach(Pos p3 in m){
			if(m.ValueAt(p3) == 'O'){
				count += (100 * p3.Y) + p3.X;
			}
		}

		resultsPanel.SetPart1Result(count);
	}

	private void DoPart2()
	{
		string dirs = "^>v<";
		string dirs2 = " ] [";
		Map<char> m = new(warehouse2);
		Pos p = start2;
		m.SetValueAt(p, '.');

		foreach(char move in moves){
			Direction d = Direction.ALL[dirs.IndexOf(move)];
			// Map<char> m2 = new(m);
			// m2.SetValueAt(p,'@');
			// GD.Print(m2);
			// GD.Print($"moving {d}");
			Pos p1 = p.AfterMove(d);
			Pos p2 = p.AfterMove(d, d.IsHorizontal()?2:3);
			char v1 = m.ValueAt(p1);
			char v2;

			if(m.ValueAt(p1) == '#'){
				continue;
			}

			if(Boxes.ContainsKey(p1)){
				HashSet<Box> toCheck = new();
				List<Box> toMove = new();
				// Box where we want to move
				Box box = Boxes[p1];
				// Where that box would push to
				Box box2 = box.AfterMove(d);
				bool wall = false;

				toCheck.Add(box2);
				toMove.Add(box);

				while(toCheck.Any()){
					Box check = toCheck.First();
					toCheck.Remove(check);

					if(m.ValueAt(check.Left) == '#' || m.ValueAt(check.Right) == '#'){
						wall = true;
						break;
					}

					// Potential boxes where it would move to

					if(d != Direction.RIGHT){
						Box box3 = Boxes.GetValueOrDefault(check.Left, null);

						if(box3 != null && !toMove.Contains(box3)){
							toCheck.Add(box3.AfterMove(d));
							toMove.Add(box3);
						}
					}
					
					if(d != Direction.LEFT){
						Box box4 = Boxes.GetValueOrDefault(check.Right, null);
						if(box4 != null && !toMove.Contains(box4)){
							toCheck.Add(box4.AfterMove(d));
							toMove.Add(box4);
						}
					}
				}
				
				if(wall){
					continue;
				}

				foreach(Box b in toMove){
					Boxes.Remove(b.Left);
					Boxes.Remove(b.Right);
					m.SetValueAt(b.Left, '.');
					m.SetValueAt(b.Right, '.');
				}
				foreach(Box b in toMove){
					Box bb = b.AfterMove(d);
					Boxes.Add(bb.Left, bb);
					Boxes.Add(bb.Right, bb);
					m.SetValueAt(bb.Left, '[');
					m.SetValueAt(bb.Right, ']');
				}
			}


			p = p1;
		}

		GD.Print(m.ToString());

		int count = 0;
		foreach(Pos p3 in m){
			if(m.ValueAt(p3) == '['){
				count += (100 * p3.Y) + p3.X;
			}
		}

		resultsPanel.SetPart2Result(count);
	}

	class Box{
		public Pos Left;
		public Pos Right;

		public Box(Pos l, Pos r){
			Left = l;
			Right = r;
		}

		public Box(int X, int Y){
			Left = new Pos(X, Y);
			Right = new Pos(X+1, Y);
		}

		public Box AfterMove(Direction d){
			return new(Left.AfterMove(d), Right.AfterMove(d));
		}
	}

	private void ParseData(string[] data){
		int idx = 0;
		for(; idx < data.Length; idx++){
			if(data[idx] == ""){
				break;
			}
		}

		warehouse = new(Util.ParseCharMap(data.AsSpan(0,idx)));
		moves = data.AsSpan(idx).Join("");

		foreach(Pos p in warehouse){
			if(warehouse.ValueAt(p) == '@'){
				start = p;
				break;
			}
		}
	
		warehouse2 = new(warehouse.Height, warehouse.Width*2, '.');
		Boxes = new();
		foreach(Pos p in warehouse){
			var c = warehouse.ValueAt(p);
			var c2 = '?';

			if("#.".Contains(c)){
				c2 = c;
			}else if(c == 'O'){
				c = '[';
				c2 = ']';
				Box b = new(p.X*2, p.Y);
				Boxes.Add(b.Left, b);
				Boxes.Add(b.Right, b);
			}else if(c == '@'){
				c2 = '.';
				start2 = new(p.X*2, p.Y);
			}

			warehouse2.SetValueAt(p.X*2, p.Y, c);
			warehouse2.SetValueAt((p.X*2)+1, p.Y, c2);
		}


	}

}
