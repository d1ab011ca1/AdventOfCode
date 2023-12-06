// https://adventofcode.com/2023/day/X
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type InputData = Input of Data: string array

let parseInput (text: string) =
    text
    |> String.splitAndTrim "\n"
    |> Array.map (fun s ->
        match s with
        | "pattern" -> s
        | _ -> failwithf "Unexpected input: %s" s)

    |> Input
    |> tee (printfn "%A")

let sample1 =
    parseInput
        """
"""

let sample2 = sample1

let part1 (Input data) = 0

let part2 (Input data) = 0

let data = getInput () |> parseInput

part1 sample1 |> testEqual "Part 1 sample" 0
part1 data |> tee (printfn "Part 1: %A") |> testEqual "Part 1" 0

part2 sample2 |> testEqual "Part 2 sample" 0
part2 data |> tee (printfn "Part 2: %A") |> testEqual "Part 2" 0
