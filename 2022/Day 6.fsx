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
mjqjpqmgbljsphdztnvjfqwrcgsmlb
"""

let inputText = downloadInput ()
// let inputText = sampleInputText1

let parseInput (text: string) = text.Trim()

let inputs = inputText |> parseInput
printfn "%A" inputs

let find len =
    inputs
    |> Seq.windowed len
    |> Seq.mapi (fun i w -> i + len, w)
    |> Seq.where (fun (_, w) -> w |> Seq.distinct |> Seq.length = len)
    |> Seq.head
    |> fst

let part1 () =
    let n = find 4
    printfn "Part 1: %A" n

let part2 () =
    let n = find 14
    printfn "Part 2: %A" n

part1 () // 1760
part2 () // 2974
