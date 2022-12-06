// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#r "nuget: MathNet.Numerics.FSharp" // https://numerics.mathdotnet.com/api/
#r "nuget: FSharp.Collections.ParallelSeq"
#endif

open System
open FSharpHelpers

let sampleInputText1 =
    """1000
    2000
    3000

    4000

    5000
    6000

    7000
    8000
    9000

    10000"""

let cookie = IO.File.ReadAllText("cookie.txt")
let inputText = downloadInput cookie
// let inputText = sampleInputText1

let parseInput (text: string) =
    let lines: string[] = text |> String.splitO "\n" StringSplitOptions.TrimEntries

    let mutable groups = List.empty
    let mutable group = List.empty

    for line in lines do
        match line with
        | "" ->
            groups <- (group |> Seq.rev |> Seq.toArray) :: groups
            group <- List.empty
        | _ -> group <- int line :: group

    (group |> Seq.rev |> Seq.toArray) :: groups |> Seq.rev |> Seq.toArray

let inputs = inputText |> parseInput
printfn "%A" inputs

let part1 () =
    let max = inputs |> Seq.map (Seq.fold (+) 0) |> Seq.max
    printfn "Part 1: %A" max

let part2 () =
    let max =
        inputs
        |> Seq.map (Seq.fold (+) 0)
        |> Seq.sortDescending
        |> Seq.take 3
        |> Seq.fold (+) 0

    printfn "Part 2: %A" max

part1 () // 72511
part2 () // 212117
