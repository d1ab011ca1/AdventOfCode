// https://adventofcode.com/2024/day/11
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

type InputData = string list

let parseInput (text: string) : InputData =
    text.Trim() |> (fun s -> s |> String.split " ") |> Array.toList //|> dump

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if false then
        failwith "bad assumption"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
125 17
"""

let sample2 = sample1

let data = lazy (getInput () |> parseData)

let part1 blinks (data: InputData) =
    let mutable memos = Map.empty

    let expand stone =
        [ if stone = "0" then
              yield "1"
          elif stone.Length % 2 = 0 then
              yield stone.Substring(0, stone.Length / 2)
              let n = stone.Substring(stone.Length / 2).TrimStart('0')
              yield if n = "" then "0" else n
          else
              yield ((stone |> Int64.Parse) * 2024L).ToString() ]

    let rec loop blinks (stones) =
        if blinks = 0 then
            stones |> List.length |> int64
        else
            stones
            |> List.sumBy (fun stone ->
                match memos |> Map.tryFind (stone, blinks) with
                | Some len -> len
                | _ ->
                    let len = loop (blinks - 1) (expand stone)

                    if stone.Length <= 3 then
                        memos <- memos |> Map.add (stone, blinks) len

                    len)

    let res = data |> loop blinks
    // memos |> Map.count |> dumps "Memos" |> ignore
    res

let part2 (data: InputData) = part1 75 data

executePuzzle "Part 1 sample (6 blinks)" (fun () -> part1 6 sample1) 22L
executePuzzle "Part 1 sample" (fun () -> part1 25 sample1) 55312L
executePuzzle "Part 1 finale" (fun () -> part1 25 data.Value) 218079L

executePuzzle "Part 2 finale" (fun () -> part2 data.Value) 259755538429618L
