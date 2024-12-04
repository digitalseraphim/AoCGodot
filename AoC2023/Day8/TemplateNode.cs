using Godot;
using System;
using System.Collections.Generic;

namespace AoCGodot;

public partial class TemplateNode : RigidBody3D
{
	[Export]
	public MeshInstance3D Mesh;

	public TemplateNode LeftNode
	{
		get; set;
	}
	public TemplateNode RightNode
	{
		get; set;
	}

	HashSet<TemplateNode> NearbyNodes = new();
	Vector3 Forces = new(0, 0, 0);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnBodyEnter(Node3D other)
	{
		if (other == LeftNode || other == RightNode || IsAncestorOf(other))
		{
			return;
		}
		NearbyNodes.Add(other as TemplateNode);
		GD.Print("enter - ", NearbyNodes.Count);
	}

	public void OnBodyLeave(Node3D other)
	{
		NearbyNodes.Remove(other as TemplateNode);
		// GD.Print("leave - ", NearbyNodes.Count);
	}

	public void PrepareForce()
	{
		Forces = new();

		Forces += (LeftNode.Position - Position).Normalized() * Position.DistanceSquaredTo(LeftNode.Position);
		Forces += (RightNode.Position - Position).Normalized() * Position.DistanceSquaredTo(RightNode.Position);

		// foreach (Node3D other in NearbyNodes)
		// {
		// 	Forces += (Position - other.Position).Normalized();// * Position.DistanceSquaredTo(other.Position);
		// }
		Forces = Forces.Normalized();
	}

	public void ApplyForce2(double delta)
	{
	}
	public void ApplyForce(double delta)
	{
		// Position += Forces * (float)delta;
		Vector3 f = LeftNode.Position - Position;
		f = f.Normalized() * f.LengthSquared();
		f /= 10f;
		// if (f.Length() > 900f)
		{
			ApplyCentralForce(f);
			LeftNode.ApplyCentralForce(-f);
		}

		f = RightNode.Position - Position;
		f = f.Normalized() * f.LengthSquared();
		f /= 10f;
		// if (f.Length() > 900f)
		{
			ApplyCentralForce(f);
			RightNode.ApplyCentralForce(-f);
		}

		foreach (TemplateNode other in NearbyNodes)
		{
			f = Position - other.Position;
			f = f.Normalized() * (300 - f.LengthSquared());
			f /= 10f;
			// if (f.Length() > 900f)
			{
				ApplyCentralForce(2 * f);
				other.ApplyCentralForce(-2 * f);
			}
		}
	}
}
