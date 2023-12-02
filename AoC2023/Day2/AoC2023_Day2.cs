using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Schema;

public partial class AoC2023_Day2 : BaseChallengeScene
{
	[Export]
	ProgressBar progressBar1;
	[Export]
	ProgressBar progressBar2;

	[Export]
	Tree tree1;
	[Export]
	Tree tree2;
	TreeItem root1 = null;
	TreeItem root2 = null;

	[Export]
	int numRed;
	[Export]
	int numGreen;
	[Export]
	int numBlue;

	public override void DoRun(string[] data){
		progressBar1.MaxValue = data.Length-1;
		progressBar2.MaxValue = data.Length-1;
		progressBar1.Value = 0;
		progressBar2.Value = 0;

		tree1.Clear();
		root1 = tree1.CreateItem();
		tree2.Clear();
		root2 = tree2.CreateItem();

		DoPart1(data);
		DoPart2(data);
	}

    public void DoPart1(string[] data)
    {
		Dictionary<string,int> maxDice = new(){
			["red"] = numRed,
			["green"] = numGreen,
			["blue"] = numBlue
		};
		int total = 0;

		// Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
		RegEx regEx = RegEx.CreateFromString("Game (\\d*): ((?:[^;]*;?)*)");
        foreach(string game in data){
			RegExMatch m = regEx.Search(game);
			int gameNum = m.Strings[1].ToInt();
			string[] pulls = m.Strings[2].Split(";");
			if(TestGame(gameNum, pulls, maxDice)){
				total += gameNum;

			}
		}

		resultsPanel.SetResultDeferred(1, total.ToString());
	}

	Dictionary<string,int> colorToCol = new(){
		["red"] = 1,
		["green"] = 2,
		["blue"] = 3
	};

	private bool TestGame(int gameNum, string[] pulls, Dictionary<string, int> maxDice){
		RegEx dieRegEx = RegEx.CreateFromString("(\\d*) (.*)");
		TreeItem gameItem = tree1.CreateItem(root1);
		gameItem.SetText(0, "Game " + gameNum.ToString());
		int pullNum = 0;
		bool retval = true;
		foreach(string pull in pulls){
			TreeItem pullItem = tree1.CreateItem(gameItem);
			pullItem.SetText(0, "Pull " + pullNum++);

			string[] dice = pull.Trim().Split(",");
			foreach(string die in dice){
				RegExMatch m2 = dieRegEx.Search(die.Trim());
				int num = m2.Strings[1].ToInt();
				string color = m2.Strings[2];
				pullItem.SetText(colorToCol[color], num.ToString());
				if(num > maxDice[color]){
					pullItem.SetCustomBgColor(colorToCol[color], Colors.DarkRed);
					retval = false;
				}
			}
		}
		if(retval){
			gameItem.SetCustomBgColor(0, Colors.DarkGreen);
		}
		return retval;
    }


    public void DoPart2(string[] data)
    {
		int total = 0;

		// Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
		RegEx regEx = RegEx.CreateFromString("Game (\\d*): ((?:[^;]*;?)*)");
        foreach(string game in data){
			RegExMatch m = regEx.Search(game);
			int gameNum = m.Strings[1].ToInt();
			string[] pulls = m.Strings[2].Split(";");
			int score = ComputeGame(gameNum, pulls);
			total += score;
		}

		resultsPanel.SetResultDeferred(2, total.ToString());
	}

	private int ComputeGame(int gameNum, string[] pulls)
	{
		TreeItem gameItem = tree2.CreateItem(root2);
		gameItem.SetText(0, "Game " + gameNum.ToString());

		Dictionary<string, int> maxDice = new()
		{
			["red"] = 0,
			["green"] = 0,
			["blue"] = 0
		};

		RegEx dieRegEx = RegEx.CreateFromString("(\\d*) (.*)");
		int pullNum = 0;
		foreach(string pull in pulls){
			TreeItem pullItem = tree2.CreateItem(gameItem);
			pullItem.SetText(0, "Pull " + pullNum++);
			string[] dice = pull.Trim().Split(",");
			foreach(string die in dice){
				RegExMatch m2 = dieRegEx.Search(die.Trim());

				int num = m2.Strings[1].ToInt();
				string color = m2.Strings[2];
				pullItem.SetText(colorToCol[color], num.ToString());
				if(num > maxDice[color]){
					pullItem.SetCustomBgColor(colorToCol[color], Colors.DarkGreen);
					maxDice[color] = num;
				}
			}
		}
		gameItem.SetText(1, maxDice["red"].ToString());
		gameItem.SetText(2, maxDice["green"].ToString());
		gameItem.SetText(3, maxDice["blue"].ToString());
		int score =  maxDice["red"] * maxDice["green"] * maxDice["blue"];
		gameItem.SetText(4, score.ToString());
		return score;
	}

}
