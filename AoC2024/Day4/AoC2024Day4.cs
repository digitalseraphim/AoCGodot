using AoCGodot;
using Godot;
using System;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day4 : BaseChallengeScene
{
	string[] lines;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private bool Check(int r, int c, int dr, int dc){
		string test = "MAS";
		GD.Print($"Check({r},{c},{dr}{dc})");
		for(int i = 0; i < 3; i++){
			int rr = r + dr*i;
			int cc = c + dc*i;
			if (rr >= 0 && cc >= 0 && rr < lines.Length && cc < lines[0].Length){
				if(lines[rr][cc] != test[i]){
					GD.Print("false");
					return false;
				}
			}else{
				return false;
			}
		}
		GD.Print("true");
		return true;
	}


	private void DoPart1()
	{
		int count = 0;

		for(int l = 0; l < lines.Length; l++){
			var line = lines[l];
			for(int ch = 0; ch < line.Length; ch++){
				if(line[ch] == 'X'){
					for(int r=-1; r<=1; r++){
						for(int c=-1; c<=1; c++){
							if(Check(l+r, ch+c, r, c)){
								count ++;
							}
						}
					}
				}
			}
		}

		resultsPanel.SetPart1Result(count);
	}

	private void DoPart2()
	{
		int count = 0;

		for(int l = 0; l < lines.Length; l++){
			var line = lines[l];
			for(int ch = 0; ch < line.Length; ch++){
				if(line[ch] == 'A'){
					if(
						(Check(l-1, ch-1, 1, 1) || Check(l+1, ch+1, -1,-1)) &&
						(Check(l+1, ch-1, -1, 1) || Check(l-1, ch+1, 1,-1)) 
					){
						count ++;
					}
				}
			}
		}

		resultsPanel.SetPart2Result(count);
	}

	private void ParseData(string[] data){
		lines = data;	
	}

}
