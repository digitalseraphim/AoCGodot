using AoCGodot;
using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoCGodot;
public partial class AoC2023_Day7 : BaseChallengeScene
{
	SortedSet<Hand> hands = new();
	SortedSet<Hand2> hands2 = new();

	public override void DoRun(string[] data)
	{
		ParseData(data);
		DoPart1();
		DoPart2();
	}
	private void DoPart1()
	{
		int total = 0;
		for (int i = 0; i < hands.Count; i++)
		{
			GD.Print(i, " ", hands.ElementAt(i));
			total += (i + 1) * hands.ElementAt(i).Bid;
		}
		resultsPanel.SetPart1Result(total.ToString());
	}

	private void DoPart2()
	{
		int total = 0;
		for (int i = 0; i < hands2.Count; i++)
		{
			GD.Print(i, " ", hands2.ElementAt(i));
			total += (i + 1) * hands2.ElementAt(i).Bid;
		}
		resultsPanel.SetPart2Result(total.ToString());
	}

	private void ParseData(string[] data)
	{
		hands.Clear();
		hands2.Clear();

		foreach (string s in data)
		{
			Hand h = Hand.FromString(s);
			hands.Add(h);
			hands2.Add(Hand2.FromString(s));
		}
	}


	public enum HandTypeEnum
	{
		HighCard,
		OnePair,
		TwoPair,
		ThreeOfAKind,
		FullHouse,
		FourOfAKind,
		FiveOfAKind
	};

	class Hand : IComparable<Hand>
	{
		public string Cards
		{
			get;
		}

		public int Bid
		{
			get;
		}

		public Hand(string Cards, int Bid)
		{
			this.Cards = Cards;
			this.Bid = Bid;
		}

		public static Hand FromString(string s)
		{
			string[] strings = s.Split(" ");
			return new(strings[0], int.Parse(strings[1]));
		}



		public HandTypeEnum HandType
		{
			get
			{
				int unique_cards = new HashSet<char>(Cards).Count;
				if (unique_cards == 5)
				{
					return HandTypeEnum.HighCard;
				}

				if (unique_cards == 1)
				{
					return HandTypeEnum.FiveOfAKind;
				}

				Dictionary<char, int> cardCount = new();
				foreach (char c in Cards)
				{
					cardCount[c] = cardCount.GetValueOrDefault(c, 0) + 1;
				}

				if (unique_cards == 2)
				{
					if (cardCount.ElementAt(0).Value == 4 || cardCount.ElementAt(1).Value == 4)
					{
						return HandTypeEnum.FourOfAKind;
					}
					else
					{
						return HandTypeEnum.FullHouse;
					}
				}

				if (unique_cards == 3)
				{
					if (cardCount.Any((e) => e.Value == 3))
					{
						return HandTypeEnum.ThreeOfAKind;
					}

					return HandTypeEnum.TwoPair;
				}

				return HandTypeEnum.OnePair;
			}
		}

		static string ranks = "23456789TJQKA";

		public int CompareTo(Hand other)
		{
			int type_comp = HandType.CompareTo(other.HandType);
			int i = 0;
			while (type_comp == 0 && i < 5)
			{
				type_comp = ranks.IndexOf(Cards[i]).CompareTo(ranks.IndexOf(other.Cards[i]));
				i++;
			}
			return type_comp;
		}

		public override string ToString()
		{
			return Cards + " " + HandType + " " + Bid;
		}
	}

	class Hand2 : IComparable<Hand2>
	{
		public string Cards
		{
			get;
		}

		public int Bid
		{
			get;
		}

		public Hand2(string Cards, int Bid)
		{
			this.Cards = Cards;
			this.Bid = Bid;
		}

		public static Hand2 FromString(string s)
		{
			string[] strings = s.Split(" ");
			return new(strings[0], int.Parse(strings[1]));
		}

		public HandTypeEnum HandType
		{
			get
			{
				int unique_cards = new HashSet<char>(Cards).Count;

				if (unique_cards == 5)
				{
					if (Cards.Contains('J'))
					{
						return HandTypeEnum.OnePair;
					}
					return HandTypeEnum.HighCard;
				}

				if (unique_cards == 1)
				{
					return HandTypeEnum.FiveOfAKind;
				}

				Dictionary<char, int> cardCount = new();
				foreach (char c in Cards)
				{
					cardCount[c] = cardCount.GetValueOrDefault(c, 0) + 1;
				}
				int j_count = cardCount.GetValueOrDefault('J',0);

				if (unique_cards == 2)
				{
					if (j_count > 0)
					{
						// J XXXX
						// JJ XXX
						// JJJ XX
						// JJJJ X
						return HandTypeEnum.FiveOfAKind;
					}
					else if (cardCount.ElementAt(0).Value == 4 || cardCount.ElementAt(1).Value == 4)
					{
						//XXXX Y
						//X YYYY
						return HandTypeEnum.FourOfAKind;
					}
					else
					{
						//XX YYY
						//XXX YY
						return HandTypeEnum.FullHouse;
					}
				}

				// three cards can be:
				// X Y ZZZ or
 				// X YY ZZ

				if (unique_cards == 3)
				{
					if (j_count > 0){
						int max = 0;
						foreach(var e in cardCount){
							if(e.Key != 'J'){
								max = Math.Max(max, e.Value);
							}
						}
						max += j_count;
						return (max == 3) ? HandTypeEnum.FullHouse : HandTypeEnum.FourOfAKind;
					}else if (cardCount.Any((e) => e.Value == 3))
					{
						return HandTypeEnum.ThreeOfAKind;
					}

					return HandTypeEnum.TwoPair;
				}

                // 4 unique
                return j_count switch
                {
                    1 => HandTypeEnum.ThreeOfAKind,// J XX Y Z
                    2 => HandTypeEnum.ThreeOfAKind,// JJ X Y Z
                    _ => HandTypeEnum.OnePair,// WW X Y Z
                };
            }
		}

		static string ranks = "J23456789TQKA";

		public int CompareTo(Hand2 other)
		{
			int type_comp = HandType.CompareTo(other.HandType);
			int i = 0;
			while (type_comp == 0 && i < 5)
			{
				type_comp = ranks.IndexOf(Cards[i]).CompareTo(ranks.IndexOf(other.Cards[i]));
				i++;
			}
			return type_comp;
		}

		public override string ToString()
		{
			return Cards + " " + HandType + " " + Bid;
		}
	}

}
