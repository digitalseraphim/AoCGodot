using AoCGodot;
using AoCGodot.Graph;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoCGodot;
public partial class AoC2023_Day25 : BaseChallengeScene
{
	Graph<UserData> Graph = null;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	private void DoPart1()
	{
		int count;

		Random r = new();

		while (true)
		{
			Graph<UserData> g = new(Graph);

			while(g.Vertices.Count > 2){
				int idx = r.Next(g.Edges.Count);
				Edge<UserData> e = g.Edges.ElementAt(idx);
				g.CollapseEdge(e);
			}

			if(g.Edges.Count == 3){
				count = g.Vertices.First().Value.User.Count * g.Vertices.Last().Value.User.Count;
				break;
			}else{
				GD.Print(g.Edges.Count);
			}

		}

		resultsPanel.SetPart1Result(count);
	}

	private void DoPart2()
	{
		resultsPanel.SetPart2Result("result");
	}

	/*
		jqt: rhn xhk nvd
		rsh: frs pzl lsr
		xhk: hfx
		cmg: qnr nvd lhk bvb
		rhn: xhk bvb hfx
		bvb: xhk hfx
		pzl: lsr hfx nvd
		qnr: nvd
		ntq: jqt hfx bvb xhk
		nvd: lhk
		lsr: lhk
		rzs: qnr cmg lsr rsh
		frs: qnr lhk lsr
	*/
	private void ParseData(string[] data)
	{
		Graph = new();

		foreach (string s in data)
		{
			string[] parts = s.Split(new[] { " ", ":" }, StringSplitOptions.RemoveEmptyEntries);
			Vertex<UserData> v = Graph.Vertices.GetOrCreate(parts[0], ()=>new(parts[0], new(parts[0])));

			foreach (string p in parts[1..])
			{
				Vertex<UserData> v2 = Graph.Vertices.GetOrCreate(p, ()=>new(p, new(p)));
				Edge<UserData> e = new(v,v2);
				Graph.EdgesByVert.GetOrCreate(v.Name, ()=>new()).Add(e);
				Graph.EdgesByVert.GetOrCreate(v2.Name, ()=>new()).Add(e);
				Graph.Edges.Add(e);
			}
		}

	}

	class UserData:BaseUserData<UserData>{
		public int Count {get{return Vertices.Count;}}
		public HashSet<string> Vertices = new();

		public UserData(IEnumerable<string> vertices){
			Vertices = new(vertices);
		}

		public UserData(string vertName){
			Vertices.Add(vertName);
		}

        public override UserData Combine(UserData other)
        {
            UserData newUD = new(Vertices.Union(other.Vertices));
            return newUD;
        }
    }

}
