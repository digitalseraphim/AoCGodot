using Godot;
using System;
using System.IO;

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
	Button LoadButton;
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
		if(File.Exists(GetChallengeDataFileName())){
			LoadButton.Disabled = false;
			OnLoad();
		}
		if(File.Exists(GetTestDataFileName())){
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

		// Perform a GET request. The URL below returns JSON as of writing.
		string[] headers = {
			"User-Agent:github.com/digitalseraphim",
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
		challengeData = body.GetStringFromAscii();
		Directory.GetParent(dataFileName).Create();
		FileStream f = File.OpenWrite(dataFileName);
		f.Write(body);
		f.Close();
	}

	void OnLoad()
	{
		StreamReader f = File.OpenText(GetChallengeDataFileName());
		challengeData = f.ReadToEnd();
		f.Close();
	}

	void LoadTestData()
	{
		StreamReader f = File.OpenText(GetTestDataFileName());
		TestDataTE.Text = f.ReadToEnd();
		f.Close();
	}

	void OnRun()
	{
		if (DataButtonGroup.GetPressedButton() == UseChallengeDataButton)
		{
			EmitSignal(SignalName.RunCode, challengeData.Split("\n"));
		}
		else
		{
			string dataFileName = GetTestDataFileName();
			Directory.GetParent(dataFileName).Create();
			FileStream f = File.OpenWrite(dataFileName);
			f.Write(TestDataTE.Text.ToAsciiBuffer());
			f.Close();

			EmitSignal(SignalName.RunCode, TestDataTE.Text.Split("\n"));
		}
	}
}
