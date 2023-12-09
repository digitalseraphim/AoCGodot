using AoCGodot;
using Godot;
using System;

namespace AoCGodot;
public partial class _CLASS_ : BaseChallengeScene
{
    public override void DoRun(string[] data)
    {
        ParseData(data);
        DoPart1();
        DoPart2();
    }
	private void DoPart1()
	{
		resultsPanel.SetPart1Result("result");
	}

	private void DoPart2()
	{
		resultsPanel.SetPart2Result("result");
	}

	private void ParseData(string[] data){
	}

}
