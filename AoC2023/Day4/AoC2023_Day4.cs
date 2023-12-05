using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoCGodot;
public partial class AoC2023_Day4 : BaseChallengeScene
{
    public override void DoRun(string[] data)
    {
        List<Tuple<HashSet<string>, HashSet<string>>> cards = new();
        int myCount = -1;
        int winCount = -1;
        foreach (string card in data)
        {
            HashSet<string> myNums = new();
            HashSet<string> winNums = new();
            HashSet<string> tmp = winNums;
            string[] numbers = card.Split(": ")[1].Split(" ");
            foreach (string num in numbers)
            {
                string tnum = num.Trim();
                if (tnum == "|")
                {
                    tmp = myNums;
                    continue;
                }
                else if (tnum.Length != 0)
                {
                    tmp.Add(tnum);
                }
            }
            if (myCount == -1)
            {
                myCount = myNums.Count;
            }
            if (myNums.Count != myCount)
            {
                GD.Print("Got fewer than expected for myNums: ", myCount, " ", numbers.Join("|"));
                GD.Print("myNums: ", myNums.ToArray().Join("|"));
            }
            if (winCount == -1)
            {
                winCount = winNums.Count;
            }
            if (winNums.Count != winCount)
            {
                GD.Print("Got fewer than expected for winNums", winCount, " ", numbers.Join("|"));
                GD.Print("winNums: ", winNums.ToArray().Join("|"));
            }
            cards.Add(new(myNums, winNums));
        }
        DoPart1(cards);
        DoPart2(cards);
    }

    private void DoPart1(List<Tuple<HashSet<string>, HashSet<string>>> cards)
    {
        int total = 0;

        foreach (var card in cards)
        {
            HashSet<string> myNums = card.Item1;
            HashSet<string> winNums = card.Item2;
            int numMatching = winNums.Intersect(myNums).Count();
            if (numMatching > 0)
            {
                int score = 1 << numMatching - 1;
                total += score;
            }
        }
        resultsPanel.SetPart1Result(total.ToString());
    }

    private void DoPart2(List<Tuple<HashSet<string>, HashSet<string>>> cards)
    {
        Dictionary<int, int> numCards = new(){
            [1] = 1
        };
        int cardNum = 1;
        foreach (var card in cards)
        {
            HashSet<string> myNums = card.Item1;
            HashSet<string> winNums = card.Item2;
            int numMatching = winNums.Intersect(myNums).Count();
            if (numMatching > 0)
            {
                int numThisCard = numCards.GetValueOrDefault(cardNum, 1);
                for (int i = 0; i < numMatching; i++)
                {
                    int nextCardNum = cardNum + i + 1;
                    int oldValue = numCards.GetValueOrDefault(nextCardNum, 1);
                    numCards[nextCardNum] = oldValue + numThisCard;
                }
            }
            cardNum++;
        }

        int total = 0;
        for (int knum=1; knum < cardNum;knum++)
        {
            GD.Print(string.Format("have {0} of {1}", numCards.GetValueOrDefault(knum,1), knum));
            total += numCards.GetValueOrDefault(knum,1);
        }

        resultsPanel.SetPart2Result(total.ToString());
    }

}
