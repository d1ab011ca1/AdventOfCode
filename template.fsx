// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#r "nuget: MathNet.Numerics.FSharp" // https://numerics.mathdotnet.com/api/
#r "nuget: FSharp.Collections.ParallelSeq"
#endif

open System
open FSharpHelpers
// open MathNet.Numerics
// open FSharp.Collections.ParallelSeq

let sampleInputText1 = """
"""

let cookie = IO.File.ReadAllLines ("cookie.txt") |> Array.head
// let inputText = downloadInput cookie
let inputText = sampleInputText1

let parseInput (text: string) =
    let lines: string[] = text |> parseInputText

    // let charArrays = lines |> toCharArrays
    // let wordArrays = lines |> toWordArrays
    // let groups = lines |> toGroups "some group prefix"

    lines

let inputs = inputText |> parseInput
printfn "%A" inputs

let part1 () = printfn "Part 1: "

let part2 () = printfn "Part 2: "

part1 () //
part2 () //
