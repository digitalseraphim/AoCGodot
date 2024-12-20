using Godot;
using System;
using System.Collections.Generic;
using System.IO;
public partial class MainScene : MarginContainer
{
	[Export]
	ItemList yearList;

	[Export]
	GridContainer grid;

	Dictionary<int, Dictionary<int, string>> scenes = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for(int year = 2015; year <= 2024; year++){
			bool foundYear = false;

			for(int day = 1; day <= 25; day ++){
				string resString = string.Format("res://AoC{0}/Day{1}/AoC{0}_Day{1}.tscn", year, day);
				if(ResourceLoader.Exists(resString))
				{
					if(!foundYear){
						yearList.AddItem(year.ToString());
						foundYear = true;
						scenes[year] = new();
					}

					scenes[year][day] = resString;
				}
			}
		}
		
		yearList.Select(yearList.ItemCount-1, true);
		yearList.EmitSignal(ItemList.SignalName.ItemSelected, yearList.ItemCount-1);
	}

	void OnYearSelected(int idx){
		int year = yearList.GetItemText(idx).ToInt();
		Dictionary<int, string> yearNodes = scenes[year];
		for(int day = 1; day <= 25; day++){
			Button button = grid.GetChild<Button>(day-1);
			button.Disabled = !yearNodes.ContainsKey(day);
		}
	}

	void OnButtonPressed(int day){
		int year = yearList.GetItemText(yearList.GetSelectedItems()[0]).ToInt();
		Console.WriteLine("Year = " + year);
		if(scenes.ContainsKey(year) && scenes[year].ContainsKey(day)){
			GetTree().ChangeSceneToFile(scenes[year][day]);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
