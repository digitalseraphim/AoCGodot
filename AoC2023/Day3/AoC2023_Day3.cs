using AoCGodot;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Schema;

public partial class AoC2023_Day3 : BaseChallengeScene
{
    [Export]
    TextEdit part1TE;
    [Export]
    TextEdit part2TE;

    public Highlighter highlighter1 = new();
    public Highlighter highlighter2 = new();

    public override void _Ready()
    {
        part1TE.SyntaxHighlighter = highlighter1;
        part2TE.SyntaxHighlighter = highlighter2;
    }

    public override void DoRun(string[] data)
    {
        int line = 0;
        part1TE.Clear();
        part2TE.Clear();
        highlighter1.Clear();
        highlighter2.Clear();

        foreach (String s in data)
        {
            part1TE.InsertLineAt(line, s);
            part2TE.InsertLineAt(line, s);
            line++;
        }

        DoPart1(data);
        DoPart2(data);
    }

    private static char GetCharAt(string[] data, int row, int col, int num_rows, int num_cols)
    {
        if (row < 0 || col < 0 || row >= num_rows || col >= num_cols)
        {
            return '.';
        }
        return data[row].ToCharArray()[col];
    }

    private void DoPart1(string[] data)
    {
        int num_rows = data.Length;
        int num_cols = data[0].Length;
        RegEx digits = RegEx.CreateFromString("\\d+");
        int total = 0;

        for (int r = 0; r < num_rows; r++)
        {
            Array<RegExMatch> ms = digits.SearchAll(data[r]);
            foreach (RegExMatch regExMatch in ms)
            {
                int c1 = regExMatch.GetStart();
                int c2 = regExMatch.GetEnd();
                bool found = false;
                for (int rr = r - 1; rr < r + 2; rr++)
                {
                    for (int cc = c1 - 1; cc < c2 + 1; cc++)
                    {
                        char ch = GetCharAt(data, rr, cc, num_rows, num_cols);
                        if (ch == '.' || Char.IsDigit(ch))
                        {
                            continue;
                        }
                        total += regExMatch.Strings[0].ToInt();
                        found = true;
                        break;
                    }
                    if (found)
                    {
                        break;
                    }
                }
                highlighter1.ChangeColor(r, c1, c2, found ? Colors.DarkGreen : Colors.DarkRed);
            }
        }
        resultsPanel.SetPart1Result(total.ToString());
    }

    private void DoPart2(string[] data)
    {
        int num_rows = data.Length;
        int num_cols = data[0].Length;
        int total = 0;

        for (int rr = 0; rr < num_rows; rr++)
        {
            for (int cc = 0; cc < num_cols; cc++)
            {
                if (GetCharAt(data, rr, cc, num_rows, num_cols) == '*')
                {
                    int num = -1;
                    int num_r = -1;
                    int num_c = -1;
                    int num_c2 = -1;
                    bool found = false;
                    for (int rrr = rr - 1; !found && rrr < rr + 2; rrr++)
                    {
                        for (int ccc = cc - 1; !found && ccc < cc + 2; ccc++)
                        {
                            if (Char.IsDigit(GetCharAt(data, rrr, ccc, num_rows, num_cols)))
                            {
                                int ccc2 = ccc;
                                while (Char.IsDigit(GetCharAt(data, rrr, ccc2 - 1, num_rows, num_cols)))
                                {
                                    ccc2--;
                                }
                                int len = 1;
                                while (Char.IsDigit(GetCharAt(data, rrr, ccc2 + len, num_rows, num_cols)))
                                {
                                    len++;
                                }
                                string substr = data[rrr].Substr(ccc2, len);
                                if (num == -1)
                                {
                                    num = substr.ToInt();
                                    num_r = rrr;
                                    num_c = ccc2;
                                    num_c2 = ccc2+len;
                                    //make sure we don't look at this number again
                                    ccc = num_c2;
                                }
                                else
                                {
                                    found = true;
                                    GD.Print(string.Format("num {0}*{1}", num, substr.ToInt()));
                                    total += num * substr.ToInt();
                                    highlighter2.ChangeColor(rrr, ccc2, ccc2+len, Colors.DarkGreen);
                                    highlighter2.ChangeColor(num_r, num_c, num_c2, Colors.DarkGreen);
                                    highlighter2.ChangeColor(rr, cc, cc+1, Colors.Gold);
                                }
                            }
                        }
                    }
                }
            }
        }
        resultsPanel.SetPart2Result(total.ToString());
    }
}
