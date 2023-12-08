using Godot;
using System;
using System.IO;

namespace AoCGodot;

public partial class ChallengeDataPanel : Control
{
	[Export]
	int year;
	[Export]
	int day;
	[Export]
	Label ChallengeLabel;
	[Export]
	ButtonGroup DataButtonGroup;
	[Export]
	Button UseChallengeDataButton;
	[Export]
	Button DownloadButton;
	[Export]
	Label ChallengeDataInfoLabel;
	[Export]
	Button RunButton;
	[Export]
	TextEdit TestDataTE;

	string challengeData = null;

	[Signal]
	public delegate void RunCodeEventHandler(string[] data);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ChallengeLabel.Text = string.Format("AoC {0} day {1}", year, day);
		if (File.Exists(GetChallengeDataFileName()))
		{
			LoadChallengeData();
		}
		if (File.Exists(GetTestDataFileName()))
		{
			LoadTestData();
		}
	}

	void OnDownload()
	{
		String url = string.Format("https://adventofcode.com/{0}/day/{1}/input", year, day);
		StreamReader sessionSR = File.OpenText(GetSessionFileName());
		string session = sessionSR.ReadToEnd();
		sessionSR.Close();
		var httpRequest = new HttpRequest();
		AddChild(httpRequest);
		httpRequest.RequestCompleted += HttpRequestCompleted;

		string[] headers = {
			"User-Agent:github.com/digitalseraphim/AoCGodot",
			"cookie:session=" + session
			};
		Error error = httpRequest.Request(url, headers);
		if (error != Error.Ok)
		{
			GD.PushError("An error occurred in the HTTP request.");
		}
	}

	private string GetChallengeDataFileName()
	{
		return ProjectSettings.GlobalizePath(string.Format("user://AoC{0}/day/{1}/input", year, day));
	}

	private string GetTestDataFileName()
	{
		return ProjectSettings.GlobalizePath(string.Format("user://AoC{0}/day/{1}/test", year, day));
	}

	private string GetSessionFileName()
	{
		return ProjectSettings.GlobalizePath("user://session");
	}

	// Called when the HTTP request is completed.
	private void HttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		string dataFileName = GetChallengeDataFileName();
		GD.Print(string.Format("got {0} bytes", body.Length));
		Directory.GetParent(dataFileName).Create();
		File.WriteAllBytes(dataFileName, body);
		LoadChallengeData();
	}

	void LoadChallengeData()
	{
		challengeData = File.ReadAllText(GetChallengeDataFileName());
		DownloadButton.Visible = false;
		ChallengeDataInfoLabel.Text = string.Format("{0} lines", DataToLines(challengeData).Length);
	}

	void LoadTestData()
	{
		TestDataTE.Text = File.ReadAllText(GetTestDataFileName());
	}

	static string[] DataToLines(string data){
		return data.TrimEnd('\n').Split("\n");
	}

	void OnRun()
	{
		if (DataButtonGroup.GetPressedButton() == UseChallengeDataButton)
		{
			EmitSignal(SignalName.RunCode, DataToLines(challengeData));
		}
		else
		{
			string dataFileName = GetTestDataFileName();
			Directory.GetParent(dataFileName).Create();
			File.WriteAllText(dataFileName, TestDataTE.Text);

			EmitSignal(SignalName.RunCode, DataToLines(TestDataTE.Text));
		}
	}
}
