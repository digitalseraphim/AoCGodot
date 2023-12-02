using Godot;

namespace AoCGodot;

public partial class BaseChallengeScene : Control
{
	[Export]
	protected ResultsPanel resultsPanel;
	[Export]
	protected ChallengeDataPanel challengeDataPanel;
	public virtual void DoRun(string[] data){
		GD.Print("base Run");
	}

	public void DoBack(){
		GetTree().ChangeSceneToFile("res://MainScene/MainScene.tscn");
	}
}
