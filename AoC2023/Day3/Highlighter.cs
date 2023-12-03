using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class Highlighter : SyntaxHighlighter
{
	public System.Collections.Generic.Dictionary<int, Dictionary> lineStyles = new();




	public override Dictionary _GetLineSyntaxHighlighting(int line)
	{
		return lineStyles.GetValueOrDefault(line, null);
	}

	public void Clear()
	{
		lineStyles.Clear();
	}

	public void ChangeColor(int line, int start, int end, Color color)
	{
		Variant oldValue;
		int idx;

		if (!lineStyles.TryGetValue(line, out Dictionary d))
		{
			d = new()
			{
				[0] = new Dictionary()
				{
					["color"] = GetTextEdit().GetThemeColor("font_color")
				}
			};
			lineStyles[line] = d;
		}

		// find previous color for start of range
		idx = start;
		while (!d.TryGetValue(idx, out oldValue))
		{
			idx--;
		}

		d[start] = new Dictionary()
		{
			["color"] = color
		};

		for (idx = start + 1; idx <= end; idx++)
		{
			// check for color change
			if (d.TryGetValue(idx, out Variant newOldValue))
			{
				d.Remove(idx);
				oldValue = newOldValue;
			}
		}

		d[end] = oldValue;

		// Need to sort the dictionary
		foreach(Variant key in d.Keys.OrderBy((v)=>v.As<int>()))
		{
			d.Remove(key, out Variant val);
			d[key] = val;
		}

		ClearHighlightingCache();
	}

	public void DebugPrint()
	{
		GD.Print("highlighter:\n");
		foreach (KeyValuePair<int, Dictionary> kvp in lineStyles)
		{
			GD.Print(string.Format("[{0}]:", kvp.Key));
			foreach (KeyValuePair<Variant, Variant> kvp2 in kvp.Value)
			{
				GD.Print(string.Format("   [{0}] : {1}", kvp2.Key, kvp2.Value));
			}
		}
	}
}
