// https://adventofcode.com/2023/day/X
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type InputData = Input of string[]

let parseInput (text: string) : InputData =
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

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 0
executePuzzle "Part 1 finale" (fun () -> part1 data) 0

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 0
executePuzzle "Part 2 finale" (fun () -> part2 data) 0
