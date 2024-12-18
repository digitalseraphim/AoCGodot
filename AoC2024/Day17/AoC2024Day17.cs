using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day17 : BaseChallengeScene
{

	Machine machine;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private void DoPart1()
	{
		machine.Run();
		resultsPanel.SetPart1Result(machine.output.Join(","));
	}

	static long Val(List<int> l)
	{
		return l.Aggregate(0L, (a, v) => a << 3 | (uint)v);
	}

	private void DoPart2()
	{
		long RegA = 0;
		var revInst = machine.instructions.Reverse<int>();
		List<int> values = new();

		for (int i = 0; i < revInst.Count();)
		{
			int j = 0;
			if (values.Count == i + 1)
			{
				j = values.Last() + 1;
				values.RemoveAt(values.Count - 1);
			}
			for (; j < 8; j++)
			{
				var v = Val(values) << 3 | (uint)j;
				var m = new Machine(machine)
				{
					RegA = v
				};
				m.Run();

				if (m.instructions.Join(",").EndsWith(m.output.Join(",")))
				{
					values.Add(j);
					RegA = Val(values);
					break;
				}
			}
			if (j == 8)
			{
				i--;
			}
			else
			{
				i++;
			}
		}

		resultsPanel.SetPart2Result(RegA);
	}

	private void ParseData(string[] data)
	{
		machine = new(data);
	}

	class Machine
	{
		public long RegA;
		public long RegB;
		public long RegC;
		public int PC;
		public List<int> instructions;
		public List<int> output;
		public int iOutputIdx = 0;

		public Dictionary<int, Func<int, int, int>> opcodes;

		public Machine(string[] data)
		{
			RegA = Int64.Parse(data[0].Split(':', StringSplitOptions.TrimEntries)[1]);
			RegB = Int64.Parse(data[1].Split(':', StringSplitOptions.TrimEntries)[1]);
			RegC = Int64.Parse(data[2].Split(':', StringSplitOptions.TrimEntries)[1]);

			instructions = data[4].Split(":", StringSplitOptions.TrimEntries)[1].Split(',').Select(Int32.Parse).ToList();

			PC = 0;
			output = new();
			opcodes = new(){
				{0,ADV},
				{1,BXL},
				{2,BST},
				{3,JNZ},
				{4,BXC},
				{5,OUT},
				{6,BDV},
				{7,CDV}
			};
		}

		public Machine(Machine other)
		{
			RegA = other.RegA;
			RegB = other.RegB;
			RegC = other.RegC;
			instructions = other.instructions;
			PC = 0;
			output = new();
			opcodes = new(){
				{0,ADV},
				{1,BXL},
				{2,BST},
				{3,JNZ},
				{4,BXC},
				{5,OUT},
				{6,BDV},
				{7,CDV}
			};
		}

		public void Run()
		{
			while (Util.IsBetween(0, instructions.Count - 1, PC))
			{
				int opcode = instructions[PC];
				int operand = instructions[PC + 1];

				PC = opcodes[opcode](operand, PC);
			}
		}

		public int ADV(int operand, int PC)
		{
			long val = ComboOperand(operand);
			RegA = (long)(RegA / Math.Pow(2, val));
			return PC + 2;
		}

		public int BXL(int operand, int PC)
		{
			RegB ^= operand;
			return PC + 2;
		}

		public int BST(int operand, int PC)
		{
			RegB = Util.Mod(ComboOperand(operand), 8);
			return PC + 2;
		}

		public int JNZ(int operand, int PC)
		{
			if (RegA == 0)
			{
				return PC + 2;
			}
			return operand;
		}

		public int BXC(int _operand, int PC)
		{
			RegB ^= RegC;
			return PC + 2;
		}

		public int OUT(int operand, int PC)
		{
			int val = (int)Util.Mod(ComboOperand(operand), 8);
			output.Add(val);
			return PC + 2;
		}

		public int BDV(int operand, int PC)
		{
			long val = ComboOperand(operand);
			RegB = (long)(RegA / Math.Pow(2, val));
			return PC + 2;
		}

		public int CDV(int operand, int PC)
		{
			long val = ComboOperand(operand);
			RegC = (long)(RegA / Math.Pow(2, val));
			return PC + 2;
		}

		public long ComboOperand(int operand)
		{
			if (operand < 4)
			{
				return operand;
			}
			switch (operand)
			{
				case 4: return RegA;
				case 5: return RegB;
				case 6: return RegC;
				default: GD.Print($"Invalid combo Operand {operand}"); return -1;
			}
		}
	}
}
