using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2023_Day20 : BaseChallengeScene
{
	string[] OrigData;

	public override void DoRun(string[] data)
	{
		OrigData = data;
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	private void DoPart1()
	{
		SignalTracker signalTracker = new();

		for (int i = 0; i < 1000; i++)
		{
			List<Signal> toSend = new(){
				new(null, Modules["broadcaster"], false)
			};

			while (toSend.Count > 0)
			{
				List<Signal> nextToSend = new();

				foreach (var ping in toSend)
				{
					signalTracker.TrackSignal(ping.High);
					// GD.Print(string.Format("Sending {0} pulse from {1} to {2}",
					// 	ping.High, (ping.Sender == null)?"button":ping.Sender.Name, ping.Reciever.Name));
					ping.Reciever.ProcessPulse(ping.Sender, ping.High, nextToSend);
				}

				toSend = nextToSend;
			}
		}

		long value = signalTracker.HighSignals * signalTracker.LowSignals;

		resultsPanel.SetPart1Result(value.ToString());
	}

	class ButtonCount : IComparable<ButtonCount>
	{
		public Module Module
		{
			get; private set;
		}
		public bool NeededOutput
		{
			get; set;
		}
		public long Count
		{
			get; set;
		}

		public ButtonCount(Module module, bool neededOutput, long count)
		{
			Module = module;
			NeededOutput = neededOutput;
			Count = count;
		}

		public int CompareTo(ButtonCount other)
		{
			return Count.CompareTo(other.Count);
		}
	}

	private void DoPart2(){
		//reset
		ParseData(OrigData);
		

		for (long i = 0; ; i++)
		{
			List<Signal> toSend = new(){
				new(null, Modules["broadcaster"], false)
			};

			while (toSend.Count > 0)
			{
				List<Signal> nextToSend = new();

				foreach (var ping in toSend)
				{
					if(ping.Reciever.Name == "rx" && !ping.High){
						resultsPanel.SetPart1Result(i+1);
						return;
					}
					ping.Reciever.ProcessPulse(ping.Sender, ping.High, nextToSend);
				}

				toSend = nextToSend;
			}
		}
	}


	private void DoPart2_other(){
		HashSet<Module> used = new();
		OutputModule rx = (OutputModule)Modules["rx"];
		Queue<Module> toProcess = new();

		used.Add(rx);
		toProcess.Enqueue(rx);

		while(toProcess.Any()){
			Module m = toProcess.Dequeue();

			foreach(var other in Modules){
				if(other.Value.Outputs.Contains(m)){
					if(!used.Contains(other.Value)){
						used.Add(other.Value);
						toProcess.Enqueue(other.Value);
					}
				}
			}
		}

		if(used.Count != Modules.Count){
			GD.Print($"Reduced set of modules from {Modules.Count} to {used.Count}");
		}

	}

	private void DoPart2Old()
	{
		long value = 0;
		OutputModule rx = (OutputModule)Modules["rx"];

		List<ButtonCount> toProcess = new(){
			new(rx, false, 0)
		};

		while (toProcess.Count > 0)
		{
			ButtonCount bc = toProcess.First();
			toProcess.RemoveAt(0);
			if (bc.Module.Name == "broadcaster")
			{
				value = bc.Count;
				resultsPanel.SetPart2Result(value.ToString());
				return;
			}


			if(bc.Module is FlipFlop){

			}
			else if(bc.Module is Conjunction)
			{

			}

		}


	}

	readonly Dictionary<string, Module> Modules = new();

	private void ParseData(string[] data)
	{
		Modules.Clear();
		Dictionary<Module, string[]> outputs = new();

		foreach (string s in data)
		{
			string[] parts = s.Split(new[] { "->", "," }, StringSplitOptions.TrimEntries);
			Module m;

			if (parts[0][0] == '%')
			{
				m = new FlipFlop(parts[0][1..]);
			}
			else if (parts[0][0] == '&')
			{
				m = new Conjunction(parts[0][1..]);
			}
			else
			{
				m = new Broadcaster();
			}

			Modules.Add(m.Name, m);
			outputs.Add(m, parts[1..]);
		}

		foreach (var op in outputs)
		{
			foreach (string s in op.Value)
			{
				Module m;

				if (Modules.ContainsKey(s))
				{
					m = Modules[s];
				}
				else
				{
					m = new OutputModule(s);
					Modules[s] = m;
				}

				op.Key.AddOutput(m);
				m.AddInput(op.Key);
			}
		}
	}

	class SignalTracker
	{
		public long HighSignals
		{
			get; private set;
		}

		public long LowSignals
		{
			get; private set;
		}

		public SignalTracker()
		{
			HighSignals = 0;
			LowSignals = 0;
		}

		public void TrackSignal(bool high)
		{
			if (high)
			{
				HighSignals++;
			}
			else
			{
				LowSignals++;
			}
		}
	}

	abstract class Module
	{
		public string Name
		{
			get;
		}

		public readonly HashSet<Module> Outputs = new();
		public readonly HashSet<Module> Inputs = new();

		public Module(string name)
		{
			Name = name;
		}

		public virtual void AddOutput(Module m)
		{
			Outputs.Add(m);
		}

		public virtual void AddInput(Module m)
		{
			Inputs.Add(m);
		}

		public abstract void ProcessPulse(Module sender, bool high, List<Signal> nextToSend);

		public void SendPulse(bool high, List<Signal> nextToSend)
		{
			foreach (Module o in Outputs)
			{
				nextToSend.Add(new(this, o, high));
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}

	class OutputModule : Module
	{
		public bool GotLowSignal
		{
			get; private set;
		}

		public OutputModule(string name) : base(name)
		{
			GotLowSignal = false;
		}

		public override void ProcessPulse(Module sender, bool high, List<Signal> nextToSend)
		{
			if (!high)
			{
				GotLowSignal = true;
			}
		}
	}

	class FlipFlop : Module
	{
		public bool State
		{
			get; private set;
		}

		public FlipFlop(string name) : base(name)
		{
			State = false;
		}

		public override void ProcessPulse(Module sender, bool high, List<Signal> nextToSend)
		{
			if (!high)
			{
				State = !State;
				SendPulse(State, nextToSend);
			}
		}

		public override string ToString()
		{
			return Name + " - FF";
		}
	}

	class Conjunction : Module
	{
		readonly Dictionary<Module, bool> InputValues = new();
		public Conjunction(string name) : base(name)
		{

		}

		public override void ProcessPulse(Module sender, bool high, List<Signal> nextToSend)
		{
			InputValues[sender] = high;

			SendPulse(!InputValues.All((p) => p.Value), nextToSend);
		}

		public override void AddOutput(Module m)
		{
			base.AddOutput(m);
		}

		public override void AddInput(Module m)
		{
			base.AddInput(m);
			InputValues.Add(m, false);
		}

		public override string ToString()
		{
			return Name + " - C";
		}
	}

	class Broadcaster : Module
	{
		public Broadcaster() : base("broadcaster")
		{

		}

		public override void ProcessPulse(Module sender, bool high, List<Signal> nextToSend)
		{
			SendPulse(high, nextToSend);
		}

	}

	class Signal
	{
		public Module Sender { get; }
		public Module Reciever { get; }
		public bool High { get; }

		public Signal(Module sender, Module reciever, bool high)
		{
			Sender = sender;
			Reciever = reciever;
			High = high;
		}
	}
}
