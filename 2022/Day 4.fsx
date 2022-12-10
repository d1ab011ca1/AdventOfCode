// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#r "nuget: MathNet.Numerics.FSharp" // https://numerics.mathdotnet.com/api/
#r "nuget: FSharp.Collections.ParallelSeq"
#endif

open System
open FSharpHelpers

let sampleInputText1 =
    """
2-4,6-8
2-3,4-5
5-7,7-9
2-8,3-7
6-6,4-6
2-6,4-8
"""

let inputText = getInput ()
// let inputText = sampleInputText1

let parseInput (text: string) =
    text
    |> parseInputText
    |> Seq.collect (fun line ->
        line.Split(',')
        |> Seq.map (fun s ->
            let p = s.Split('-')
            let f, l = int p[0], int p[1]
            Seq.init (l - f + 1) ((+) f) |> set)
        |> Seq.pairwise)
    |> Seq.toArray

let inputs = inputText |> parseInput
//printfn "%A" inputs

let part1 () =
    let numSubsets =
        inputs
        |> Seq.where (fun (a, b) -> a |> Set.isSubset b || b |> Set.isSubset a)
        |> Seq.length

    printfn "Part 1: %A" numSubsets

let part2 () =
    let numInterections =
        inputs
        |> Seq.where (fun (a, b) -> a |> Set.intersect b |> Set.isEmpty |> not)
        |> Seq.length

    printfn "Part 2: %A" numInterections

part1 () // 441
part2 () // 861
