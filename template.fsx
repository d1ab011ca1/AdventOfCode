// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#r "nuget: MathNet.Numerics.FSharp" // https://numerics.mathdotnet.com/api/
#r "nuget: FSharp.Collections.ParallelSeq"
#endif

open System
open FSharpHelpers

let parseInput (text: string) =
    let lines: string[] = text |> parseInputText

    // let charArrays = lines |> toCharArrays
    // let wordArrays = lines |> toWordArrays
    // let groups = lines |> toGroups "some group prefix"

    lines

let part1 (inputs: string[]) =
    printfn "%A" inputs
    0

let part2 (inputs: string[]) = 0

let sampleInputText1 =
    """
"""

[ sampleInputText1
  // getInput ()
  ]
|> Seq.iteri (fun inputNo inputText ->
    let input = inputText |> parseInput
    printfn "Input %d Part 1: %O" inputNo (part1 input) //
    printfn "Input %d Part 2: %O" inputNo (part2 input) //
    printfn "")
