// https://adventofcode.com/2023/day/4
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type Card = (int Set * int Set)
type CardArray = Card array

let parseInput (text: string) : CardArray =
    text
    |> String.splitAndTrim "\n"
    |> Array.map (fun s ->
        match s.Split([| ':'; '|' |]) with
        | [| _; a; b |] ->
            let a = a |> String.splitAndTrim " " |> Seq.map Int32.Parse |> Set.ofSeq
            let b = b |> String.splitAndTrim " " |> Seq.map Int32.Parse |> Set.ofSeq
            (a, b)
        | _ -> failwithf "Unexpected input: %s" s)
// |> tee (printfn "%A")

let sample1 =
    parseInput
        """
Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
"""

let sample2 = sample1

let numberOfMatches (winningNumbers: int Set, myNumbers: int Set) =
    Set.intersect (winningNumbers) (myNumbers) |> Set.count

let calcScore card =
    match numberOfMatches card with
    | 0 -> 0
    | cnt -> 1 <<< (cnt - 1)

let part1 (cards: CardArray) =
    cards |> Seq.sumBy (fun card -> calcScore card)

type Part2 =
    { NumberOfMatches: int
      mutable Copies: int }

let part2 (cards: CardArray) =
    let input =
        cards
        |> Array.map (fun card ->
            { NumberOfMatches = numberOfMatches card
              Copies = 0 })

    for i = 0 to input.Length - 1 do
        let cur = input[i]

        for n = 1 to cur.NumberOfMatches do
            let next = input[i + n]
            next.Copies <- next.Copies + 1 + cur.Copies

    input |> Seq.sumBy (fun c -> c.Copies + 1)

let data = getInput () |> parseInput

part1 sample1 |> testEqual "Part 1 sample" 13
part1 data |> tee (printfn "Part 1: %A") |> testEqual "Part 1" 25010

part2 sample2 |> testEqual "Part 2 sample" 30
part2 data |> tee (printfn "Part 2: %A") |> testEqual "Part 2" 9924412
