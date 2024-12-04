using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AoCGodot;
public partial class AoC2023_Day8 : BaseChallengeScene
{
	[Export]
	StaticBody3D Root;
	[Export]
	Node3D Container;
	[Export]
	PackedScene NodeTemplate;

	[Export]
	Material StartMaterial;
	[Export]
	Material EndMaterial;
	[Export]
	Material BaseMaterial;

	[Export]
	ImmediateMesh ConnectionMesh;

	string instructions;
	Dictionary<string, Dictionary<char, string>> nodes = new();
	HashSet<string> ANodes = new();
	HashSet<string> ZNodes = new();

	Dictionary<string, TemplateNode> node3ds = new();

	public override void DoRun(string[] data)
	{
		ParseData(data);
		// DoPart1();
		// DoPart2();
		DoPart2_2();
	}
	private void DoPart1()
	{
		int steps = 0;

		string node = "AAA";

		while (node != "ZZZ")
		{
			node = nodes[node][instructions[steps % instructions.Length]];
			steps++;
		}

		resultsPanel.SetPart1Result(steps.ToString());
	}

	private void DoPart2()
	{
		IEnumerable<string> curNodes = new HashSet<string>(ANodes);
		long steps = 0;
		while (!curNodes.All(ZNodes.Contains))
		{
			curNodes = curNodes.Aggregate(new HashSet<string>(),
				(hs, n) =>
				{
					hs.Add(nodes[n][instructions[(int)(steps % instructions.Length)]]);
					return hs;
				});
			GD.Print(curNodes.ToArray().Join("/"));
			steps++;
		}
		resultsPanel.SetPart1Result(steps.ToString());
	}
	private void DoPart2_2()
	{
		Dictionary<string, long> loopsizes = new();
		foreach (string s in ANodes)
		{
			long steps = 0;
			int endinz = 0;
			long zat = 0;
			string node = s;
			string znode = "";
			long loopsize = 0;
			Dictionary<string, long> visited = new();
			do
			{
				int istep = (int)(steps % instructions.Length);
				steps++;

				char inst = instructions[istep];
				string newNode = nodes[node][inst];
				string newNode2 = newNode + istep.ToString();
				// GD.Print(s, " ", newNode, " ", steps);
				if (visited.ContainsKey(newNode2))
				{
					// GD.Print(s, " ", newNode, " ", visited[newNode2], " ", steps);
					loopsize = steps - visited[newNode2];
					loopsizes.Add(s, loopsize);
					break;
				}
				else
				{
					visited.Add(newNode2, steps);
				}
				if (newNode.EndsWith("Z"))
				{
					endinz++;
					zat = steps;
					znode = newNode;
				}
				node = newNode;
				// Thread.Sleep(100);
			} while (!node.EndsWith("A"));
			GD.Print("loopsize = ", loopsize, "  endinz = ", endinz, " zat = ", zat, " ", znode, " ", nodes[znode].Values.ToArray().Join("/"));

		}

		long max = LCM(loopsizes.Values.ToArray());

		resultsPanel.SetPart2Result(max.ToString());
	}
	RegEx NodeRegEx = RegEx.CreateFromString("(.*) = \\((.*), (.*)\\)");

	static long LCM(long[] numbers)
	{
		return numbers.Aggregate(lcm);
	}
	static long lcm(long a, long b)
	{
		return Math.Abs(a * b) / GCD(a, b);
	}
	static long GCD(long a, long b)
	{
		return b == 0 ? a : GCD(b, a % b);
	}

	private void ParseData(string[] data)
	{
		instructions = data[0];

		nodes.Clear();
		ANodes.Clear();
		ZNodes.Clear();
		node3ds.Clear();
		foreach (Node n in Container.GetChildren())
		{
			Container.RemoveChild(n);
			n.Free();
		}

		for (int i = 2; i < data.Length; i++)
		{
			RegExMatch m = NodeRegEx.Search(data[i]);
			nodes.Add(m.Strings[1], new()
			{
				['L'] = m.Strings[2],
				['R'] = m.Strings[3]
			});

			TemplateNode n = NodeTemplate.Instantiate<TemplateNode>();
			n.Name = m.Strings[1];
			Container.AddChild(n);
			n.Position = NodeToPosition(m.Strings[1]);
			node3ds.Add(n.Name, n);
			n.Visible = true;

			if (m.Strings[1].EndsWith('A'))
			{
				n.Freeze = true;
				n.Mesh.SetSurfaceOverrideMaterial(0, StartMaterial);
				ANodes.Add(m.Strings[1]);
			}
			else if (m.Strings[1].EndsWith('Z'))
			{
				n.Mesh.SetSurfaceOverrideMaterial(0, EndMaterial);
				ZNodes.Add(m.Strings[1]);
			}
			else
			{
				n.Mesh.SetSurfaceOverrideMaterial(0, BaseMaterial);
			}
		}

		foreach (TemplateNode n in node3ds.Values)
		{
			TemplateNode left = node3ds[nodes[n.Name]['L']];
			TemplateNode right = node3ds[nodes[n.Name]['R']];
			n.LeftNode = left;
			n.RightNode = right;
		}

		Container.PrintTree();
	}

	Vector3 NodeToPosition(string n)
	{
		// if(n.EndsWith('A')){
		// 	return new Vector3(ElemPos(n[0]), ElemPos(n[1]), ElemPos(n[2]));
		// }else if(n.EndsWith('Z')){
		// 	return new Vector3(ElemPos(n[0]), ElemPos(n[1]), ElemPos(n[2]));

		// }

		return new Vector3(ElemPos(n[0]), ElemPos(n[1]), ElemPos(n[2]));
		// return new Vector3(ElemPos(n[0]), ElemPos(n[1]), 0);
	}

	float ElemPos(char c)
	{
		if (Char.IsDigit(c))
		{
			return c - '0';
		}
		return c - 'A';
	}

	class LoopSize
	{
		public long Loopsize
		{
			get; set;
		}
		public long Zat
		{
			get; set;
		}

		public LoopSize(long loopsize, long zat)
		{
			Loopsize = loopsize;
			Zat = zat;
		}

		public void Increase()
		{
			Zat += Loopsize;
		}

		public void Multiply(long v)
		{
			Zat += v * Loopsize;
		}
	}

    public override void _PhysicsProcess(double delta)
    {

		if (node3ds.Count == 0)
		{
			return;
		}

		foreach (TemplateNode n in node3ds.Values)
		{
			if (!n.Name.ToString().EndsWith("A"))
			{
				// n.PrepareForce();
				n.ApplyForce(delta);
			}
		}
		// foreach (TemplateNode n in node3ds.Values)
		// {
		// 	if (!n.Name.ToString().EndsWith("A"))
		// 	{
		// 		n.ApplyForce(delta);
		// 	}
		// }
		ConnectionMesh.ClearSurfaces();
		ConnectionMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
		foreach (TemplateNode n in node3ds.Values)
		{
			ConnectionMesh.SurfaceSetColor(Colors.DarkBlue);
			ConnectionMesh.SurfaceAddVertex(n.Position);
			ConnectionMesh.SurfaceAddVertex(n.LeftNode.Position);
			ConnectionMesh.SurfaceSetColor(Colors.DarkGreen);
			ConnectionMesh.SurfaceAddVertex(n.Position);
			ConnectionMesh.SurfaceAddVertex(n.RightNode.Position);
		}
		ConnectionMesh.SurfaceEnd();
	}

}
