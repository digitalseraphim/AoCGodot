using Godot;
using System;
using System.Linq;

public partial class Aoc2023_Day1 : Node2D
{
	[Export]
	Label part1Ans;
	[Export]
	TextEdit debug;
	void DoRun(string[] data)
	{
		int total = 0;
		// RegEx regEx = RegEx.CreateFromString("^(?=[^0-9]*([0-9])).*([0-9])[^0-9]*$");
		string num = "zero|one|two|three|four|five|six|seven|eight|nine|[0-9]";
		string[] nums = num.Split("|");
		RegEx regEx = RegEx.CreateFromString(string.Format("^(?=[^0-9]*?({0})).*({0})[^0-9]*$", num));
		foreach(string s in data){
			RegExMatch m = regEx.Search(s);
			if(m == null){
				debug.Text += string.Format("regex didn't match line {0}\n", s);
				continue;
			}
			string num1s = m.GetString(1);
			string num2s = m.GetString(2);
			int num1 = -2;
			int num2 = -2;
			try{
				num1 = int.Parse(num1s);
			}catch(Exception){
				num1 = Array.IndexOf(nums, num1s);
			}
			try{
				num2 = int.Parse(num2s);
			}catch(Exception){
				num2 = Array.IndexOf(nums, num2s);
			}
			debug.Text += string.Format("{0}  |  {1}  |  {2}  |  {3}  |  {4}\n", s, num1s, num2s, num1, num2);
			total += (num1*10)+num2;
		}
		part1Ans.Text = string.Format("{0}", total);
	}
}
