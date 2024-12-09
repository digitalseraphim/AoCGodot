using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2024Day9 : BaseChallengeScene
{
	List<Tuple<int,int>> disk_map;

	class File{
		public readonly int Id;
		public readonly int Blocks;
		public int Free;
		
		public File(int id, int blocks, int free){
			Id = id;
			Blocks = blocks;
			Free = free;
		}
	}

	List<File> files;
	Dictionary<int, File> file_map;

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}

	private void DoPart1()
	{
		int start_idx = 0;
		int end_idx = disk_map.Count-1;
		int i = 0;
		long checksum = 0;
		int num_end_blocks = disk_map[end_idx].Item1;

		while(true){
			// handle the filled part of this block
			for(int j = 0; j < disk_map[start_idx].Item1; j++, i++){
				GD.Print(start_idx);
				checksum += i*start_idx;
			}
			
			// empty space
			for(int k = 0; k < disk_map[start_idx].Item2; k++, i++){
				GD.Print(end_idx);
				checksum += i * end_idx;
				num_end_blocks --;
				if(num_end_blocks == 0){
					end_idx --;
					num_end_blocks = disk_map[end_idx].Item1;
				}
			}
			start_idx ++;

			if(end_idx == start_idx){
				for(int j = 0; j < num_end_blocks; j++, i++){
					GD.Print(end_idx);
					checksum += i*end_idx;
				}
				break;
			}
		}

		resultsPanel.SetPart1Result(checksum);
	}


	private void DoPart2()
	{
		for(int idx = files.Last().Id; idx >= 0; idx --){
			GD.Print($"idx = {idx}");
			File to_move = file_map[idx];
			int idx2 = files.IndexOf(to_move);

			for(int ii = 0; ii < idx2; ii++){
				File f = files[ii];
				if(f.Free >= to_move.Blocks){
					GD.Print($"moving after {ii}");
					files[idx2-1].Free += to_move.Blocks + to_move.Free;
					to_move.Free = f.Free - to_move.Blocks;
					f.Free = 0;
					files.Remove(to_move);
					files.Insert(ii+1, to_move);
					break;
				}
			}
		}

		int i = 0;
		long checksum = 0;

		foreach(File f in files){
			for(int j = 0; j < f.Blocks; j++, i++){
				checksum += i*f.Id;
			}
			i+=f.Free;
		}

		resultsPanel.SetPart2Result(checksum);
	}

	private void PrintFiles(){
		string output = "";
		foreach(File f in files){
			for(int i = 0; i < f.Blocks; i++){
				output += $"{f.Id}";
			}
			for(int i = 0; i < f.Free; i++){
				output += ".";
			}
		}
		GD.Print($"{output}");
	}

	private void ParseData(string[] data){
		disk_map = data[0].Select((x)=>x-'0')
						  .Chunk(2)
						  .Select((x)=>new Tuple<int,int>(x[0],x.ElementAtOrDefault(1)))
						  .ToList();
		files = disk_map.Select((x,i) => new File(i, x.Item1, x.Item2)).ToList();

		file_map = files.ToDictionary((x)=>x.Id);


		GD.Print($"files = {files.Count} {files[0].Free} {file_map[0].Free}");
		int c = files[0].Free;
		files[0].Free = 42;
		GD.Print($"42? {files[0].Free} {file_map[0].Free}");
		File f = files[0];
		f.Free = 69;
		GD.Print($"69? {files[0].Free} {file_map[0].Free}");
		files[0].Free = c;
		GD.Print($"back to normal? {files[0].Free} {file_map[0].Free}");

	}

}
