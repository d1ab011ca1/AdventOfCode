// https://adventofcode.com/2024/day/X
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

#nowarn "57" // Experimental library feature (Array.Parallel), requires '--langversion:preview'.

open System
open System.Text.RegularExpressions
open FSharpHelpers

type InputData = string[]

let parseInput ((TextLines lines) as text) : InputData =
    lines
    |> Array.map (fun s ->
        // match s |> Regex.matchGroups @"^(.+),(.+)$" with
        // | None -> if not m.Success then failwithf "Bad input: %A" s
        // | Some gs -> (gs[1].Value, gs[2].Value)
        s)
    |> dump

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if false then
        failwith "bad assumption"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """

"""

let sample2 = sample1

let data =
    let rawData = getInput ()
    lazy (rawData |> parseData)

let part1 (data: InputData) =
    //
    0

let part2 (data: InputData) =
    //
    0

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 0
executePuzzle "Part 1 finale" (fun () -> part1 data.Value) 0

executePuzzle "Part 2 sample" (fun () -> part2 sample1) 0
executePuzzle "Part 2 finale" (fun () -> part2 data.Value) 0
