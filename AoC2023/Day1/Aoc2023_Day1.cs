using AoCGodot;
using Godot;
using System;
using System.Threading;

public partial class AoC2023_Day1 : BaseChallengeScene
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

		static readonly string NUM = "zero|one|two|three|four|five|six|seven|eight|nine|[0-9]";
	static readonly string[] nums = NUM.Split("|");

	public override void DoRun(string[] data)
	{
		Thread t1 = new(new ThreadStart(() => DoPart1(data)));
		Thread t2 = new(new ThreadStart(() => DoPart2(data)));
		GD.Print("AOCD1 Run");
		progressBar1.MaxValue = data.Length-1;
		progressBar2.MaxValue = data.Length-1;
		progressBar1.Value = 0;
		progressBar2.Value = 0;

		tree1.Clear();
		root1 = tree1.CreateItem();
		tree2.Clear();
		root2 = tree2.CreateItem();

		t1.Start();
		t2.Start();
	}

	private void DoPart1(string[] data)
	{
		RegEx regEx = RegEx.CreateFromString("^(?=[^0-9]*([0-9])).*([0-9])[^0-9]*$");
		int result = DoWork(data, regEx, 1);
		resultsPanel.SetResultDeferred(1, result.ToString());
	}

	private void DoPart2(string[] data)
	{
		RegEx regEx = RegEx.CreateFromString(string.Format("^(?=[^0-9]*?({0})).*({0})[^0-9]*$", NUM));
		int result = DoWork(data, regEx, 2);
		resultsPanel.SetResultDeferred(2, result.ToString());
	}

	private void AddDebugLine(int part, string s1, string s2, string s3, string s4, string s5)
	{
		CallDeferred("DeferredAppend", part, s1, s2, s3, s4, s5);
	}

	private void DeferredAppend(int part, string s1, string s2, string s3, string s4, string s5){
		TreeItem t;
		if(part == 1){
			t = tree1.CreateItem(root1);
		}else{
			t = tree2.CreateItem(root2);
		}
		t.SetText(0,s1);
		t.SetText(1,s2);
		t.SetText(2,s3);
		t.SetText(3,s4);
		t.SetText(4,s5);

	}
	private void AddDebugLine(int part, string s1)
	{
		CallDeferred("DeferredAppend", part, s1);
	}

	private void DeferredAppend(int part, string s1){
		TreeItem t;
		if(part == 1){
			t = tree1.CreateItem(root1);
		}else{
			t = tree2.CreateItem(root2);
		}
		t.SetText(0,s1);
		t.SetTextOverrunBehavior(0, TextServer.OverrunBehavior.NoTrimming);
	}

	private void IncreaseProgress(int part){
		CallDeferred("DeferredIncreaseProgress", part);
	}

	private void DeferredIncreaseProgress(int part){
		(part == 1?progressBar1:progressBar2).Value ++;
	}

	private int DoWork(string[] data, RegEx regEx, int part)
	{
		int total = 0;
		foreach (string s in data)
		{
			RegExMatch m = regEx.Search(s);
			if (m == null)
			{
				AddDebugLine(part, string.Format("regex didn't match line {0}\n", s));
				continue;
			}
			string num1s = m.GetString(1);
			string num2s = m.GetString(2);
			int num1 = -2;
			int num2 = -2;
			if(!int.TryParse(num1s, out num1))
			{
				num1 = Array.IndexOf(nums, num1s);
			}
			if(!int.TryParse(num2s, out num2))
			{
				num2 = Array.IndexOf(nums, num2s);
			}
			IncreaseProgress(part);
			AddDebugLine(part, s, num1s, num2s, num1.ToString(), num2.ToString());
			total += (num1 * 10) + num2;
			// Thread.Sleep(10);
		}
		return total;
	}
}
