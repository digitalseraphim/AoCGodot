

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace AoCGodot.Graph;


public class BaseUserData<T> where T: BaseUserData<T> {
    public virtual T Combine(T other){
        return other;
    }
}

class Vertex<T> where T:BaseUserData<T>{
    public string Name { get; }
    public T User {get;set;} = default;

    public Vertex(String name, T user){
        Name = name;
        User = user;
    }

    public override string ToString()
    {
        return Name;
    }
}

class Edge<T> where T:BaseUserData<T>
{
    public Vertex<T> Vert1 { get; set; }
    public Vertex<T> Vert2 { get; set; }

    public Edge(Vertex<T> v1, Vertex<T> v2){
        Vert1 = v1;
        Vert2 = v2;
    }

    public Edge(Edge<T> other){
        Vert1 = other.Vert1;
        Vert2 = other.Vert2;
    }

    public void ReplaceVert(Vertex<T> oldV, Vertex<T> newV){
        // GD.Print("before replace - ", Vert1, " ", Vert2, " ", oldV, " ", newV);
        if(Vert1 == oldV){
            // GD.Print("A");
            Vert1 = newV;
        }else if(Vert2 == oldV){
            // GD.Print("B");
            Vert2 = newV;
        }
        // GD.Print("after replace - ", Vert1, " ", Vert2, " ", oldV, " ", newV);
    }

    public bool Contains(Vertex<T> vertex){
        return Vert1 == vertex || Vert2 == vertex;
    }

    public override string ToString()
    {
        return "Edge: " + Vert1 + " - " + Vert2;
    }
}


class Graph<T> where T:BaseUserData<T>
{
    public List<Edge<T>> Edges { get; } = new();
    public Dictionary<string, Vertex<T>> Vertices { get; } = new();

    public Dictionary<string, List<Edge<T>>> EdgesByVert {get;} = new();

    public Graph(){}

    public Graph(Graph<T> other){
        Edges = other.Edges.Aggregate(new List<Edge<T>>(), (hs, e) => {hs.Add(new(e)); return hs;});
        Vertices = new(other.Vertices);
        foreach(Edge<T> e in Edges){
            EdgesByVert.GetOrCreate(e.Vert1.Name, ()=>new()).Add(e);
            EdgesByVert.GetOrCreate(e.Vert2.Name, ()=>new()).Add(e);
        }
    }

    public void CollapseEdge(Edge<T> e){
        Vertex<T> v1 = e.Vert1;
        Vertex<T> v2 = e.Vert2;
        Vertex<T> newV = new((e.Vert1.Name+"_"+e.Vert2.Name).Hash().ToString(), v1.User.Combine(v2.User));

        // GD.Print(v1.Name + " " + v2.Name + " " + newV.Name);

        Edges.Remove(e);

        List<Edge<T>> toRemove = new();

        foreach(Edge<T> e2 in EdgesByVert[v1.Name]){
            if(e2.Contains(v2))
            {
                toRemove.Add(e2);
                continue;
            }
            e2.ReplaceVert(v1, newV);
            EdgesByVert.GetOrCreate(newV.Name, ()=>new()).Add(e2);
        }

        foreach(Edge<T> e2 in EdgesByVert[v2.Name]){
            if(e2.Contains(v1))
            {
                toRemove.Add(e2);
                continue;
            }
            e2.ReplaceVert(v2, newV);
            EdgesByVert.GetOrCreate(newV.Name, ()=>new()).Add(e2);
        }

        foreach(Edge<T> e2 in toRemove){
            Edges.Remove(e2);
        }

        bool b;
        b = Vertices.Remove(v1.Name);
        EdgesByVert.Remove(v1.Name);
        // GD.Print("Remove ", v1.Name, " ", b);

        b = Vertices.Remove(v2.Name);
        EdgesByVert.Remove(v1.Name);
        // GD.Print("Remove ", v2.Name, " ", b);

        Vertices.Add(newV.Name, newV);
    }
}