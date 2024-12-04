using Godot;
using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;

public partial class AocWebHandler
{
	DateTime lastAPICall;

	private string GetSessionFileName()
	{
		return ProjectSettings.GlobalizePath("user://session");
	}

	public string GetSessionData(){
		string sessionFileName = GetSessionFileName();
		if(File.Exists(sessionFileName)){
			return File.ReadAllText(sessionFileName);
		}
		return null;
	}

	public void SetSessionData(string data){
		File.WriteAllText(GetSessionFileName(), data);
	}

	void Download(string url, Node requestor, HttpRequest.RequestCompletedEventHandler HttpRequestCompleted)
	{
		string session = GetSessionData();
		var httpRequest = new HttpRequest();
		requestor.AddChild(httpRequest);
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
}
