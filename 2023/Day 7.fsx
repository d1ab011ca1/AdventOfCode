// https://adventofcode.com/2023/day/7
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type CardFrequency = Map<char, int>

type HandBid =
    { Hand: string
      Bid: int
      Frequencies: CardFrequency }

type InputData = Input of HandBid[]

let getCardFreq (hand: string) =
    hand
    |> Seq.fold
        (fun frequencies card ->
            frequencies
            |> Map.change card (function
                | Some cnt -> Some(cnt + 1)
                | _ -> Some 1))
        Map.empty

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Array.map (fun s ->
        match s |> String.split " " with
        | [| hand; bid |] ->
            { Hand = hand
              Bid = bid |> Int32.Parse
              Frequencies = hand |> getCardFreq }
        | _ -> failwithf "Unexpected input: %s" s)
    |> Input
// |> tee (printfn "%A")

let sample1 =
    parseInput
        """
32T3K 765
T55J5 684
KK677 28
KTJJT 220
QQQJA 483
"""

let sample2 = sample1

type RankKind =
    | FiveKind
    | FourKind
    | FullHouse
    | ThreeKind
    | TwoPair
    | OnePair
    | HighCard

let calcWinnings (handRank: HandBid -> RankKind) (cardRank: string) data =
    data
    |> Seq.sortWith (fun h1 h2 ->
        // sort descending!
        match compare (handRank h2) (handRank h1) with
        | 0 ->
            (h1.Hand, h2.Hand)
            ||> Seq.compareWith (fun c1 c2 -> cardRank.IndexOf(c2) - cardRank.IndexOf(c1))
        | n -> n)
    |> Seq.mapi (fun i h -> h.Bid * (i + 1))
    |> Seq.sum

let part1 (Input data) =
    let cardRank = "AKQJT98765432"

    let handRank hand =
        match hand.Frequencies |> Map.values |> Seq.sort |> Seq.toList with
        | [ 5 ] -> FiveKind
        | [ 1; 4 ] -> FourKind
        | [ 2; 3 ] -> FullHouse
        | [ 1; 1; 3 ] -> ThreeKind
        | [ 1; 2; 2 ] -> TwoPair
        | [ 1; 1; 1; 2 ] -> OnePair
        | _ -> HighCard

    data |> calcWinnings handRank cardRank

let part2 (Input data) =
    // Jokers are wild and the lowest card
    let cardRank = "AKQT98765432J"

    let handRank hand =
        let jokers = hand.Frequencies |> Map.tryFind 'J' |> Option.defaultValue 0
        let frequencies = hand.Frequencies |> Map.remove 'J'

        match jokers, frequencies |> Map.values |> Seq.sort |> Seq.toList with
        | 0, [ 5 ] -> FiveKind
        | 0, [ 1; 4 ] -> FourKind
        | 0, [ 2; 3 ] -> FullHouse
        | 0, [ 1; 1; 3 ] -> ThreeKind
        | 0, [ 1; 2; 2 ] -> TwoPair
        | 0, [ 1; 1; 1; 2 ] -> OnePair
        | 0, _ -> HighCard
        | 1, [ 4 ] -> FiveKind
        | 1, [ 1; 3 ] -> FourKind
        | 1, [ 2; 2 ] -> FullHouse
        | 1, [ 1; 1; 2 ] -> ThreeKind
        | 1, [ 1; 1; 1; 1 ] -> OnePair
        | 1, _ -> OnePair
        | 2, [ 3 ] -> FiveKind
        | 2, [ 1; 2 ] -> FourKind
        | 2, [ 1; 1; 1 ] -> ThreeKind
        | 2, _ -> ThreeKind
        | 3, [ 2 ] -> FiveKind
        | 3, [ 1; 1 ] -> FourKind
        | 3, _ -> FourKind
        | _ -> FiveKind

    data |> calcWinnings handRank cardRank

let data = getInput () |> parseInput

part1 sample1 |> testEqual "Part 1 sample" 6440
part1 data |> tee (printfn "Part 1: %A") |> testEqual "Part 1" 250232501

part2 sample2 |> testEqual "Part 2 sample" 5905
part2 data |> tee (printfn "Part 2: %A") |> testEqual "Part 2" 249138943
